using System.Transactions;
using Bk.APG.Business.Dtos;
using Bk.APG.Business.Extensions;
using Bk.APG.Business.Mapper;
using Bk.APG.Business.Models;
using Bk.APG.Business.Repositories;
using Bk.APG.Common.Resources;
using Bk.APG.CrossCutting.Exception;
using Microsoft.Extensions.Logging;

namespace Bk.APG.Business.Services;

public class MembershipCandidateService : IMembershipCandidateService
{
    private readonly IMembershipCandidateRepository _membershipCandidateRepository;
    private readonly IAuthorizationService _authorizationService;
    private readonly IGeneralElectionCommitteeRepository _generalElectionCommitteeRepository;
    private readonly IWorklistTaskRepository _worklistTaskRepository;
    private readonly IEiamAssignmentRepository _eiamAssignmentRepository;
    private readonly IPersonService _personService;
    private readonly ILogger<MembershipCandidateService> _logger;

    public MembershipCandidateService(
        IMembershipCandidateRepository membershipCandidateRepository,
        IAuthorizationService authorizationService,
        IGeneralElectionCommitteeRepository generalElectionCommitteeRepository,
        IWorklistTaskRepository worklistTaskRepository,
        IEiamAssignmentRepository eiamAssignmentRepository,
        IPersonService personService,
        ILogger<MembershipCandidateService> logger)
    {
        _membershipCandidateRepository = membershipCandidateRepository;
        _authorizationService = authorizationService;
        _generalElectionCommitteeRepository = generalElectionCommitteeRepository;
        _worklistTaskRepository = worklistTaskRepository;
        _eiamAssignmentRepository = eiamAssignmentRepository;
        _personService = personService;
        _logger = logger;
    }

    public async Task<MembershipCandidateDetailDto?> GetDuplicateMembershipCandidateForList(Guid committeeId, string surname, string givenName, int birthYear, Guid genderId, Guid languageId)
    {
        var generalElectionCommittee = await _generalElectionCommitteeRepository.GetByCommitteeId(committeeId);

        if (generalElectionCommittee.MembershipCandidates.Count == 0)
        {
            return null;
        }

        var duplicateCandidate = generalElectionCommittee.MembershipCandidates
            .FirstOrDefault(candidate => (candidate.Person is not null && PersonService.NamesAreEqual(candidate.Person.Surname, surname) && PersonService.NamesAreEqual(candidate.Person.GivenName, givenName) && candidate.Person.BirthYear == birthYear)
                || (PersonService.NamesAreEqual(candidate.Surname, surname) && PersonService.NamesAreEqual(candidate.GivenName, givenName) && candidate.GenderId == genderId && candidate.BirthYear == birthYear));

        return duplicateCandidate is not null ? MembershipCandidateMapper.ToMembershipCandidateDetailDto(duplicateCandidate) : null;
    }

    public async Task<CandidateListValidationResultDto> ValidateCandidateList(Guid committeeId, IEnumerable<Guid> selectedCandidateIds, bool duplicateCheckDone)
    {
        _logger.LogInformation("Validate candidate list for general election committee {CommitteeId}", committeeId);

        var validationResult = new CandidateListValidationResultDto();
        var generalElectionCommittee = await _generalElectionCommitteeRepository.GetByCommitteeIdForUpdate(committeeId);
        var candidateIds = selectedCandidateIds.ToList();

        ValidateCandidateCount(candidateIds.Count, generalElectionCommittee, validationResult);

        if (validationResult.IsValid)
        {
            UpdateCandidateSelection(generalElectionCommittee.MembershipCandidates, candidateIds);
            var successfulDuplicateCheck = await ProcessCandidateDuplicatesAndCreatePersons(generalElectionCommittee, validationResult, duplicateCheckDone);

            if (successfulDuplicateCheck)
            {
                if (generalElectionCommittee.CandidateListStateId != CandidateListState.Completed)
                {
                    await CompleteCandidateList(generalElectionCommittee);
                    await CreateReadyForFederalCouncilProposalTasks(generalElectionCommittee);
                }

                validationResult.AreJustificationsMissing = await CheckJustificationsMissingAndCreateTasks(generalElectionCommittee);
                await CheckCandidatePersonsAndCreateTasks(generalElectionCommittee, validationResult);

                await CheckCandidateMembershipsAndCreateTasks(generalElectionCommittee, validationResult);
            }

            validationResult.AreContactPointsMissing = await CheckContactPointsAndCreateTasks(generalElectionCommittee);

            await _generalElectionCommitteeRepository.CommitChanges();

            _logger.LogInformation("Validated candidate list for general election committee {CommitteeId}", committeeId);
        }
        else
        {
            _logger.LogInformation("Candidate list for general election committee {CommitteeId} is not valid", committeeId);
        }

        return validationResult;
    }

    private async Task CheckCandidateMembershipsAndCreateTasks(GeneralElectionCommittee generalElectionCommittee, CandidateListValidationResultDto validationResult)
    {
        if (validationResult.PersonsWithMissingInterests.Count != 0 || validationResult.PersonsWithMissingBaseData.Count != 0)
        {
            return;
        }

        var worklistTasks = (await _worklistTaskRepository.GetAllByGeneralElectionCommitteeId(generalElectionCommittee.Id))
            .Where(x => x.WorklistTaskTypeId == WorklistTaskType.GeneralElectionMembershipValidation).ToList();

        foreach (var membershipCandidate in generalElectionCommittee.MembershipCandidates)
        {
            var membershipTask = worklistTasks.FirstOrDefault(x => x.MembershipCandidateId == membershipCandidate.Id && x.WorklistTaskTypeId == WorklistTaskType.GeneralElectionMembershipValidation);

            if (!membershipCandidate.IsSelected)
            {
                if (membershipTask is not null && membershipTask.WorklistTaskStateId != WorklistTaskState.Completed)
                {
                    membershipTask.WorklistTaskStateId = WorklistTaskState.Completed;
                    membershipTask.Modified = DateTime.UtcNow;
                    membershipTask.ModifiedBy = "System";
                }

                continue;
            }

            if (membershipCandidate.HasMembershipValidationIssues)
            {
                if (membershipCandidate.Person is not null)
                {
                    validationResult.PersonsWithMembershipValidationIssues.Add(PersonMapper.ToPersonMinimalDto(membershipCandidate.Person));
                }

                if (membershipTask is null)
                {
                    _logger.LogInformation("Creating membership validation task for committee {CommitteeId} and membership candidate {MembershipCandidateId}", generalElectionCommittee.CommitteeId, membershipCandidate.Id);

                    await _worklistTaskRepository.Create(new WorklistTask
                    {
                        AssignedToId = generalElectionCommittee.Committee!.EiamAssignmentId!.Value,
                        AssignedById = EiamAssignment.ApgId,
                        DueDate = generalElectionCommittee.SecretariatReadyForProposalDueDate!.Value.AddDays(-14),
                        Description = generalElectionCommittee.GetDescription(),
                        WorklistTaskTypeId = WorklistTaskType.GeneralElectionMembershipValidation,
                        WorklistTaskStateId = WorklistTaskState.Active,
                        GeneralElectionCommitteeId = generalElectionCommittee.Id,
                        DepartmentId = generalElectionCommittee.DepartmentId,
                        OfficeId = generalElectionCommittee.OfficeId,
                        CommitteeId = generalElectionCommittee.CommitteeId,
                        MembershipCandidateId = membershipCandidate.Id,
                        PersonId = membershipCandidate.PersonId,
                        Created = DateTime.UtcNow,
                        CreatedBy = "System",
                        Modified = DateTime.UtcNow,
                        ModifiedBy = "System"
                    });
                }
                else if (membershipTask.WorklistTaskStateId != WorklistTaskState.Active)
                {
                    membershipTask.WorklistTaskStateId = WorklistTaskState.Active;
                    membershipTask.Modified = DateTime.UtcNow;
                    membershipTask.ModifiedBy = "System";
                }
            }
            else
            {
                if (membershipTask is null || membershipTask.WorklistTaskStateId == WorklistTaskState.Completed)
                {
                    return;
                }

                membershipTask.WorklistTaskStateId = WorklistTaskState.Completed;
                membershipTask.Modified = DateTime.UtcNow;
                membershipTask.ModifiedBy = "System";
            }
        }
    }

    private async Task<bool> CheckContactPointsAndCreateTasks(GeneralElectionCommittee generalElectionCommittee)
    {
        var needsSecretariat = generalElectionCommittee.Committee!.NeedsAttentionSecretariat;
        var needsDataProtectionOfficer = generalElectionCommittee.Committee.NeedsAttentionDataProtectionOfficer;
        var areContactPointsMissing = needsSecretariat || needsDataProtectionOfficer;
        var worklistTasks = (await _worklistTaskRepository.GetAllByGeneralElectionCommitteeId(generalElectionCommittee.Id)).ToList();
        var missingSecretariatTask = worklistTasks.FirstOrDefault(x => x.WorklistTaskTypeId == WorklistTaskType.GeneralElectionMissingSecretariat);
        var missingDataProtectionOfficerTask = worklistTasks.FirstOrDefault(x => x.WorklistTaskTypeId == WorklistTaskType.GeneralElectionMissingDataProtectionOfficer);

        if (needsSecretariat)
        {
            if (missingSecretariatTask is null)
            {
                _logger.LogInformation("Creating missing secretariat task for committee {CommitteeId}", generalElectionCommittee.CommitteeId);

                await _worklistTaskRepository.Create(new WorklistTask
                {
                    AssignedToId = generalElectionCommittee.Committee.EiamAssignmentId!.Value,
                    AssignedById = EiamAssignment.ApgId,
                    DueDate = generalElectionCommittee.SecretariatReadyForProposalDueDate!.Value.AddDays(-14),
                    Description = BusinessTexts.GeneralElection_MissingSecretariat_TaskDescription,
                    WorklistTaskTypeId = WorklistTaskType.GeneralElectionMissingSecretariat,
                    WorklistTaskStateId = WorklistTaskState.Active,
                    GeneralElectionCommitteeId = generalElectionCommittee.Id,
                    DepartmentId = generalElectionCommittee.DepartmentId,
                    OfficeId = generalElectionCommittee.OfficeId,
                    CommitteeId = generalElectionCommittee.CommitteeId,
                    Created = DateTime.UtcNow,
                    CreatedBy = "System",
                    Modified = DateTime.UtcNow,
                    ModifiedBy = "System"
                });
            }
            else if (missingSecretariatTask.WorklistTaskStateId != WorklistTaskState.Active)
            {
                missingSecretariatTask.WorklistTaskStateId = WorklistTaskState.Active;
                missingSecretariatTask.Modified = DateTime.UtcNow;
                missingSecretariatTask.ModifiedBy = "System";
            }
        }
        else
        {
            if (missingSecretariatTask is not null && missingSecretariatTask.WorklistTaskStateId != WorklistTaskState.Completed)
            {
                missingSecretariatTask.WorklistTaskStateId = WorklistTaskState.Completed;
                missingSecretariatTask.Modified = DateTime.UtcNow;
                missingSecretariatTask.ModifiedBy = "System";
            }
        }

        if (needsDataProtectionOfficer)
        {
            if (missingDataProtectionOfficerTask is null)
            {
                _logger.LogInformation("Creating missing data protection officer task for committee {CommitteeId}", generalElectionCommittee.CommitteeId);

                await _worklistTaskRepository.Create(new WorklistTask
                {
                    AssignedToId = generalElectionCommittee.Committee.EiamAssignmentId!.Value,
                    AssignedById = EiamAssignment.ApgId,
                    DueDate = generalElectionCommittee.SecretariatReadyForProposalDueDate!.Value.AddDays(-14),
                    Description = BusinessTexts.GeneralElection_MissingDataProtectionOfficer_TaskDescription,
                    WorklistTaskTypeId = WorklistTaskType.GeneralElectionMissingDataProtectionOfficer,
                    WorklistTaskStateId = WorklistTaskState.Active,
                    GeneralElectionCommitteeId = generalElectionCommittee.Id,
                    DepartmentId = generalElectionCommittee.DepartmentId,
                    OfficeId = generalElectionCommittee.OfficeId,
                    CommitteeId = generalElectionCommittee.CommitteeId,
                    Created = DateTime.UtcNow,
                    CreatedBy = "System",
                    Modified = DateTime.UtcNow,
                    ModifiedBy = "System"
                });
            }
            else if (missingDataProtectionOfficerTask.WorklistTaskStateId != WorklistTaskState.Active)
            {
                missingDataProtectionOfficerTask.WorklistTaskStateId = WorklistTaskState.Active;
                missingDataProtectionOfficerTask.Modified = DateTime.UtcNow;
                missingDataProtectionOfficerTask.ModifiedBy = "System";
            }
        }
        else
        {
            if (missingDataProtectionOfficerTask is not null && missingDataProtectionOfficerTask.WorklistTaskStateId != WorklistTaskState.Completed)
            {
                missingDataProtectionOfficerTask.WorklistTaskStateId = WorklistTaskState.Completed;
                missingDataProtectionOfficerTask.Modified = DateTime.UtcNow;
                missingDataProtectionOfficerTask.ModifiedBy = "System";
            }
        }

        return areContactPointsMissing;
    }

    public static void ValidateCandidateCount(int candidateCount, GeneralElectionCommittee generalElectionCommittee, CandidateListValidationResultDto validationResult)
    {
        if (candidateCount < generalElectionCommittee.MinimalMembers)
        {
            validationResult.Errors.Add(string.Format(BusinessTexts.CandidateListValidationError_MinimumMembers, generalElectionCommittee.MinimalMembers));
        }

        if (generalElectionCommittee.MaximalMembers is not null && generalElectionCommittee.MaximalMembers != 0 && candidateCount > generalElectionCommittee.MaximalMembers)
        {
            validationResult.Errors.Add(string.Format(BusinessTexts.CandidateListValidationError_MaximumMembers, generalElectionCommittee.MaximalMembers));
        }
    }

    private async Task CompleteCandidateList(GeneralElectionCommittee generalElectionCommittee)
    {
        var candidateListTasks = (await _worklistTaskRepository.GetAllByGeneralElectionCommitteeId(generalElectionCommittee.Id))
            .Where(x => x.WorklistTaskTypeId == WorklistTaskType.CandidateListCreate ||
                x.WorklistTaskTypeId == WorklistTaskType.CandidateListForward ||
                x.WorklistTaskTypeId == WorklistTaskType.CandidateListApprove).ToList();

        foreach (var candidateListTask in candidateListTasks)
        {
            candidateListTask.WorklistTaskStateId = WorklistTaskState.Completed;
        }

        generalElectionCommittee.CandidateListStateId = CandidateListState.Completed;
        generalElectionCommittee.IsValidated = true;
    }

    private async Task CheckCandidatePersonsAndCreateTasks(GeneralElectionCommittee generalElectionCommittee, CandidateListValidationResultDto validationResult)
    {
        foreach (var membershipCandidate in generalElectionCommittee.MembershipCandidates)
        {
            if (membershipCandidate.Person is null)
            {
                return;
            }

            var personTasks = (await _worklistTaskRepository.GetAllByPersonId(membershipCandidate.Person.Id)).ToList();

            if (membershipCandidate.IsSelected)
            {
                if (membershipCandidate.NeedsAttentionInterests)
                {
                    validationResult.PersonsWithMissingInterests.Add(PersonMapper.ToPersonMinimalDto(membershipCandidate.Person));

                    await CreateOrActivatePersonTask(generalElectionCommittee, personTasks, membershipCandidate, WorklistTaskType.GeneralElectionPersonInterests, 7);
                }

                if (membershipCandidate.NeedsAttentionBasicDataOrOccupation)
                {
                    validationResult.PersonsWithMissingBaseData.Add(PersonMapper.ToPersonMinimalDto(membershipCandidate.Person));

                    await CreateOrActivatePersonTask(generalElectionCommittee, personTasks, membershipCandidate, WorklistTaskType.GeneralElectionPersonBaseData, 14);
                }
            }
            else
            {
                foreach (var task in personTasks.Where(x => x.WorklistTaskTypeId == WorklistTaskType.GeneralElectionPersonInterests || x.WorklistTaskTypeId == WorklistTaskType.GeneralElectionPersonBaseData))
                {
                    if (task.WorklistTaskStateId != WorklistTaskState.Completed)
                    {
                        task.WorklistTaskStateId = WorklistTaskState.Completed;
                        task.Modified = DateTime.UtcNow;
                        task.ModifiedBy = "System";
                    }
                }
            }
        }
    }

    private async Task CreateOrActivatePersonTask(GeneralElectionCommittee generalElectionCommittee, List<WorklistTask> personTasks,
        MembershipCandidate membershipCandidate, Guid worklistTaskTypeId, int daysBeforeProposalDueDate)
    {
        var interestsTask = personTasks.FirstOrDefault(x => x.WorklistTaskTypeId == worklistTaskTypeId);
        if (interestsTask is null)
        {
            _logger.LogInformation("Creating task {TaskTypeId} for person {PersonId}", worklistTaskTypeId, membershipCandidate.PersonId);

            await _worklistTaskRepository.Create(new WorklistTask
            {
                AssignedToId = generalElectionCommittee.Committee!.EiamAssignmentId!.Value,
                AssignedById = EiamAssignment.ApgId,
                DueDate = generalElectionCommittee.SecretariatReadyForProposalDueDate!.Value.AddDays(-daysBeforeProposalDueDate),
                Description = BusinessTexts.GeneralElection_PersonData_TaskDescription,
                WorklistTaskTypeId = worklistTaskTypeId,
                WorklistTaskStateId = WorklistTaskState.Active,
                GeneralElectionCommitteeId = generalElectionCommittee.Id,
                DepartmentId = generalElectionCommittee.DepartmentId,
                OfficeId = generalElectionCommittee.OfficeId,
                CommitteeId = generalElectionCommittee.CommitteeId,
                PersonId = membershipCandidate.PersonId,
                Created = DateTime.UtcNow,
                CreatedBy = "System",
                Modified = DateTime.UtcNow,
                ModifiedBy = "System"
            });
        }
        else
        {
            if (interestsTask.WorklistTaskStateId != WorklistTaskState.Active)
            {
                interestsTask.WorklistTaskStateId = WorklistTaskState.Active;
                interestsTask.Modified = DateTime.UtcNow;
                interestsTask.ModifiedBy = "System";
            }
        }
    }

    private async Task<bool> CheckJustificationsMissingAndCreateTasks(GeneralElectionCommittee generalElectionCommittee)
    {
        var areJustificationsMissing = generalElectionCommittee.JustificationsNeedAttention;
        var missingJustificationTask = (await _worklistTaskRepository.GetAllByGeneralElectionCommitteeId(generalElectionCommittee.Id))
            .FirstOrDefault(x => x.WorklistTaskTypeId == WorklistTaskType.GeneralElectionMissingJustifications);

        if (areJustificationsMissing)
        {
            if (missingJustificationTask is null)
            {
                _logger.LogInformation("Creating missing justification task for general election committee {CommitteeId}", generalElectionCommittee.Id);

                await _worklistTaskRepository.Create(new WorklistTask
                {
                    AssignedToId = generalElectionCommittee.Committee!.EiamAssignmentId!.Value,
                    AssignedById = EiamAssignment.ApgId,
                    DueDate = generalElectionCommittee.SecretariatReadyForProposalDueDate!.Value.AddDays(-7),
                    Description = BusinessTexts.GeneralElection_MissingJustifications_TaskDescription,
                    WorklistTaskTypeId = WorklistTaskType.GeneralElectionMissingJustifications,
                    WorklistTaskStateId = WorklistTaskState.Active,
                    GeneralElectionCommitteeId = generalElectionCommittee.Id,
                    DepartmentId = generalElectionCommittee.DepartmentId,
                    OfficeId = generalElectionCommittee.OfficeId,
                    CommitteeId = generalElectionCommittee.CommitteeId,
                    Created = DateTime.UtcNow,
                    CreatedBy = "System",
                    Modified = DateTime.UtcNow,
                    ModifiedBy = "System"
                });
            }
            else
            {
                if (missingJustificationTask.WorklistTaskStateId != WorklistTaskState.Active)
                {
                    missingJustificationTask.WorklistTaskStateId = WorklistTaskState.Active;
                }
            }
        }
        else
        {
            if (missingJustificationTask is not null && missingJustificationTask.WorklistTaskStateId != WorklistTaskState.Completed)
            {
                missingJustificationTask.WorklistTaskStateId = WorklistTaskState.Completed;
            }
        }

        return areJustificationsMissing;
    }

    private async Task CreateReadyForFederalCouncilProposalTasks(GeneralElectionCommittee generalElectionCommittee)
    {
        var gewStartTask = (await _worklistTaskRepository.GetByWorklistTaskTypeId(WorklistTaskType.GeneralElectionStart)).OrderByDescending(x => x.Created).First();

        var newTasks = new List<WorklistTask>();
        if (generalElectionCommittee.Department!.IsBigDepartment)
        {
            newTasks.Add(GenerateProposalTask(generalElectionCommittee, generalElectionCommittee.Office!.EiamAssignmentId!.Value, generalElectionCommittee.OfficeReadyForProposalDueDate!.Value));
        }

        newTasks.Add(GenerateProposalTask(generalElectionCommittee, generalElectionCommittee.Committee!.EiamAssignmentId!.Value, generalElectionCommittee.SecretariatReadyForProposalDueDate!.Value));
        newTasks.Add(GenerateProposalTask(generalElectionCommittee, generalElectionCommittee.Department.EiamAssignmentId, gewStartTask.DueDate));
        newTasks.Add(GenerateProposalTask(generalElectionCommittee, EiamAssignment.AdminId, DateOnly.FromDateTime(DateTime.UtcNow.AddDays(14))));

        await _worklistTaskRepository.CreateRange(newTasks);
    }

    private static WorklistTask GenerateProposalTask(GeneralElectionCommittee generalElectionCommittee, Guid assignedToId, DateOnly dueDate)
    {
        return new WorklistTask
        {
            AssignedToId = assignedToId,
            AssignedById = EiamAssignment.ApgId,
            DueDate = dueDate,
            Description = generalElectionCommittee.GetDescription(),
            WorklistTaskTypeId = WorklistTaskType.ReadyForFederalCouncilProposal,
            WorklistTaskStateId = WorklistTaskState.Inactive,
            GeneralElectionCommitteeId = generalElectionCommittee.Id,
            DepartmentId = generalElectionCommittee.DepartmentId,
            OfficeId = generalElectionCommittee.OfficeId,
            CommitteeId = generalElectionCommittee.CommitteeId,
            Created = DateTime.UtcNow,
            CreatedBy = "System",
            Modified = DateTime.UtcNow,
            ModifiedBy = "System"
        };
    }

    private async Task<bool> ProcessCandidateDuplicatesAndCreatePersons(GeneralElectionCommittee generalElectionCommittee,
        CandidateListValidationResultDto validationResult, bool duplicateCheckDone)
    {
        var result = false;
        if (!duplicateCheckDone)
        {
            foreach (var candidate in generalElectionCommittee.MembershipCandidates.Where(p => p.IsSelected && p.Person is null))
            {
                var dto = await _personService.GetDuplicatePersonForGeneralElection(candidate);
                validationResult.DuplicateCheckResults.Add(dto);
            }
        }

        validationResult.ExistingPersons.AddRange(generalElectionCommittee.MembershipCandidates.Where(p => p.IsSelected && p.Person is not null).Select(mc => PersonMapper.ToPersonDetailDto(mc.Person!, false)));

        if (validationResult.DuplicateCheckResults.All(y => y.DuplicateReason == DuplicateReason.NoDuplicateFound))
        {
            foreach (var candidate in generalElectionCommittee.MembershipCandidates.Where(p => p.IsSelected && p.Person is null))
            {
                var newPerson = await _personService.CreatePersonInGeneralElection(candidate);
                candidate.Person = newPerson;
                candidate.PersonId = newPerson.Id;
                validationResult.CreatedPersons.Add(PersonMapper.ToPersonDetailDto(newPerson, false));
            }

            result = true;
        }

        return result;
    }

    public async Task SaveCandidateList(Guid committeeId, IEnumerable<Guid> candidateIds)
    {
        _logger.LogInformation("Save candidate list for general election committee {CommitteeId}", committeeId);

        var generalElectionCommittee = await _generalElectionCommitteeRepository.GetByCommitteeIdForUpdate(committeeId);

        UpdateCandidateSelection(generalElectionCommittee.MembershipCandidates, candidateIds);

        await _generalElectionCommitteeRepository.CommitChanges();
        _logger.LogInformation("Saved candidate list for general election committee {CommitteeId}", committeeId);
    }

    public async Task ForwardCandidateList(Guid committeeId, CandidateListForwardDto forwardDto)
    {
        _logger.LogInformation("Forward candidate list for general election committee {CommitteeId} to assignment {ForwardToId}", committeeId, forwardDto.ForwardToId);

        using var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);

        var generalElectionCommittee = await _generalElectionCommitteeRepository.GetByCommitteeIdForUpdate(committeeId);
        var forwardTo = await _eiamAssignmentRepository.GetById(forwardDto.ForwardToId);

        var candidateListTasks = (await _worklistTaskRepository.GetAllByGeneralElectionCommitteeId(generalElectionCommittee.Id)).ToList();
        var departmentTask = candidateListTasks.First(x => x.WorklistTaskTypeId == WorklistTaskType.CandidateListApprove);
        var officeTask = candidateListTasks.FirstOrDefault(x => x.WorklistTaskTypeId == WorklistTaskType.CandidateListForward);
        var secretariatTask = candidateListTasks.First(x => x.WorklistTaskTypeId == WorklistTaskType.CandidateListCreate);

        switch (forwardTo.Role)
        {
            case Role.Department:
                departmentTask.WorklistTaskStateId = WorklistTaskState.Active;
                departmentTask.Description = forwardDto.Description;

                if (officeTask is not null)
                {
                    officeTask.WorklistTaskStateId = WorklistTaskState.Completed;
                }
                else
                {
                    secretariatTask.WorklistTaskStateId = WorklistTaskState.Completed;
                }

                break;
            case Role.Office:
                if (officeTask is null)
                {
                    throw new NotSupportedException($"Forward candidate list to office is not supported for the committee {committeeId}");
                }

                officeTask.WorklistTaskStateId = WorklistTaskState.Active;
                officeTask.Description = forwardDto.Description;

                if (secretariatTask.WorklistTaskStateId == WorklistTaskState.Active)
                {
                    secretariatTask.WorklistTaskStateId = WorklistTaskState.Completed;
                }
                else
                {
                    departmentTask.WorklistTaskStateId = WorklistTaskState.Inactive;
                }

                break;
            case Role.Secretariat:
                secretariatTask.WorklistTaskStateId = WorklistTaskState.Active;
                secretariatTask.Description = forwardDto.Description;

                if (officeTask is not null)
                {
                    officeTask.WorklistTaskStateId = WorklistTaskState.Inactive;
                }
                else
                {
                    departmentTask.WorklistTaskStateId = WorklistTaskState.Inactive;
                }

                break;
            case Role.Admin:
            case Role.Observer:
            default:
                throw new ArgumentOutOfRangeException($"Forward candidate list to role {forwardTo.Role} is not supported");
        }

        UpdateCandidateSelection(generalElectionCommittee.MembershipCandidates, forwardDto.CandidateIds);

        await _generalElectionCommitteeRepository.CommitChanges();
        await _worklistTaskRepository.CommitChanges();

        scope.Complete();

        _logger.LogInformation("Forwarded candidate list for general election committee {CommitteeId} to assignment {ForwardToId}", committeeId, forwardDto.ForwardToId);
    }

    public async Task PartialUpdateMembershipCandidate(Guid id, MembershipCandidatePartialUpdateDto membershipCandidatePartialUpdate)
    {
        _logger.LogInformation("Partial update membership candidate {MembershipCandidateId}", id);

        var membershipCandidate = await _membershipCandidateRepository.GetByIdForUpdate(id);

        membershipCandidate.FunctionId = membershipCandidatePartialUpdate.FunctionId;
        membershipCandidate.Remarks = membershipCandidatePartialUpdate.Remarks;
        membershipCandidate.RemarksStatus = membershipCandidatePartialUpdate.RemarksStatus;

        membershipCandidate.Modified = DateTime.UtcNow;
        membershipCandidate.ModifiedBy = _authorizationService.GetCurrentUserName();

        await _membershipCandidateRepository.CommitChanges();
        _logger.LogInformation("Partial updated membership candidate {MembershipCandidateId}", id);
    }

    public async Task<MembershipListDto> GetMembers(Guid committeeId)
    {
        var generalElectionCommittee = await _generalElectionCommitteeRepository.GetByCommitteeId(committeeId);

        var activeMemberships = generalElectionCommittee.CandidateListStateId != CandidateListState.Completed
            ? []
            : generalElectionCommittee.MembershipCandidates
                .Where(m => m is { IsDeleted: false, IsSelected: true })
                .Select(GeneralElectionMapper.ToCommitteeMemberDto);
        var inactiveMemberships = new List<CommitteeMemberDto>();

        return new MembershipListDto
        {
            CommitteeQuotas = generalElectionCommittee.GetQuotas(),
            ActiveMemberships = activeMemberships,
            InactiveMemberships = inactiveMemberships
        };
    }

    public async Task UpdateMembershipCandidate(Guid id, MembershipCandidateUpdateDto membershipCandidateUpdate)
    {
        _logger.LogInformation("Update membership candidate {MembershipCandidateId}", id);

        var membershipCandidate = await _membershipCandidateRepository.GetByIdForUpdate(id);

        if (membershipCandidateUpdate.PersonId is null)
        {
            membershipCandidate.GivenName = membershipCandidateUpdate.GivenName!;
            membershipCandidate.Surname = membershipCandidateUpdate.Surname!;
            membershipCandidate.BirthYear = membershipCandidateUpdate.BirthYear.GetValueOrDefault();
            membershipCandidate.GenderId = membershipCandidateUpdate.GenderId!.Value;
            membershipCandidate.LanguageId = membershipCandidateUpdate.LanguageId!.Value;
        }

        membershipCandidate.ElectionOfficeId = membershipCandidateUpdate.ElectionOfficeId;
        membershipCandidate.FunctionId = membershipCandidateUpdate.FunctionId;
        membershipCandidate.MaximumEmploymentLevel = membershipCandidateUpdate.MaximumEmploymentLevel;
        membershipCandidate.MembershipAdditionId = membershipCandidateUpdate.MembershipAdditionId;
        membershipCandidate.EndDate = membershipCandidateUpdate.EndDate;
        membershipCandidate.JustificationLongerDuty = membershipCandidateUpdate.JustificationLongerDuty;
        membershipCandidate.JustificationShorterDuty = membershipCandidateUpdate.JustificationShorterDuty;
        membershipCandidate.JustificationMemberInFederalAssembly = membershipCandidateUpdate.JustificationMemberInFederalAssembly;
        membershipCandidate.JustificationMemberInFederalDuty = membershipCandidateUpdate.JustificationMemberInFederalDuty;
        membershipCandidate.RequirementsProfile = membershipCandidateUpdate.RequirementsProfile;
        membershipCandidate.InCorrelationWithFederalDuty = membershipCandidateUpdate.InCorrelationWithFederalDuty;

        membershipCandidate.Modified = DateTime.UtcNow;
        membershipCandidate.ModifiedBy = _authorizationService.GetCurrentUserName();

        await _membershipCandidateRepository.CommitChanges();
        _logger.LogInformation("Updated membership candidate {MembershipCandidateId}", id);
    }

    public async Task<MembershipCandidateUpdateDto> GetMembershipCandidateForUpdate(Guid id)
    {
        var membershipCandidate = await _membershipCandidateRepository.GetByIdForUpdate(id);

        return MembershipCandidateMapper.ToMembershipCandidateUpdateDto(membershipCandidate);
    }

    public async Task<MembershipCandidateDetailDto> CreateMembershipCandidate(MembershipCandidateCreateDto membershipCandidateCreate)
    {
        _logger.LogInformation("Create membership candidate for committee {CommitteeId}", membershipCandidateCreate.CommitteeId);

        var generalElectionCommittee = await _generalElectionCommitteeRepository.GetByCommitteeId(membershipCandidateCreate.CommitteeId);

        var membershipCandidate = new MembershipCandidate
        {
            GeneralElectionCommitteeId = generalElectionCommittee.Id,
            PersonId = membershipCandidateCreate.PersonId,
            Surname = membershipCandidateCreate.Surname,
            GivenName = membershipCandidateCreate.GivenName,
            BirthYear = membershipCandidateCreate.BirthYear,
            GenderId = membershipCandidateCreate.GenderId,
            LanguageId = membershipCandidateCreate.LanguageId,
            FunctionId = membershipCandidateCreate.FunctionId,
            ElectionTypeId = ElectionType.NewElectionGuid,
            ElectionOfficeId = ElectionOffice.DepartmentAsGuid,
            BeginDate = generalElectionCommittee.TermOfOfficeDate!.BeginDate,
            EndDate = generalElectionCommittee.TermOfOfficeDate!.EndDate.GetValueOrDefault(),
            InCorrelationWithFederalDuty = false,
            Remarks = membershipCandidateCreate.Remarks,
            RemarksStatus = membershipCandidateCreate.RemarksStatus,
            Created = DateTime.UtcNow,
            CreatedBy = _authorizationService.GetCurrentUserName(),
            Modified = DateTime.UtcNow,
            ModifiedBy = _authorizationService.GetCurrentUserName(),
            IsDeleted = false,
            IsSelected = false,
        };

        var createdMembershipCandidate = await _membershipCandidateRepository.Create(membershipCandidate);
        _logger.LogInformation("Created membership candidate {MembershipCandidateId}", createdMembershipCandidate.Id);

        return await GetMembershipCandidateDetail(createdMembershipCandidate.Id);
    }

    private async Task<MembershipCandidateDetailDto> GetMembershipCandidateDetail(Guid id)
    {
        var membershipCandidate = await _membershipCandidateRepository.GetById(id);

        return MembershipCandidateMapper.ToMembershipCandidateDetailDto(membershipCandidate);
    }

    public async Task DeleteMembershipCandidate(Guid id)
    {
        _logger.LogInformation("Delete membership candidate {MembershipCandidateId}", id);

        var membershipCandidate = await _membershipCandidateRepository.GetByIdForUpdate(id);

        if (membershipCandidate.PersonId.HasValue && membershipCandidate.ElectionTypeId != ElectionType.NewElectionGuid)
        {
            const string message = "Only membership candidates without a linked person or with election type 'New Election' can be deleted.";
            _logger.LogError(message);
            throw new BusinessValidationException(message);
        }

        await _membershipCandidateRepository.Delete(membershipCandidate);
        _logger.LogInformation("Deleted membership candidate {MembershipCandidateId}", id);
    }

    private static void UpdateCandidateSelection(IEnumerable<MembershipCandidate> candidates, IEnumerable<Guid> selectedCandidateIds)
    {
        var selectedIds = selectedCandidateIds.ToHashSet();
        foreach (var candidate in candidates)
        {
            candidate.IsSelected = selectedIds.Contains(candidate.Id);
        }
    }
}
