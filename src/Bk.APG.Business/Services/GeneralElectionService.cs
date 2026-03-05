using Bk.APG.Business.Dtos;
using Bk.APG.Business.Mapper;
using Bk.APG.Business.Models;
using Bk.APG.Business.Repositories;
using Bk.APG.Common.Resources;
using Bk.APG.CrossCutting.Exception;
using Microsoft.Extensions.Logging;

namespace Bk.APG.Business.Services;

public class GeneralElectionService : IGeneralElectionService
{
    private readonly IAuthorizationService _authorizationService;
    private readonly IWorklistTaskService _worklistTaskService;
    private readonly IEiamAssignmentService _eiamAssignmentService;
    private readonly IMasterDataRepository _masterDataRepository;
    private readonly ITermOfOfficeDateService _termOfOfficeDateService;
    private readonly IGeneralElectionHelperService _generalElectionHelperService;
    private readonly IGeneralElectionCommitteeRepository _generalElectionCommitteeRepository;
    private readonly ICommitteeRepository _committeeRepository;
    private readonly IMembershipRepository _membershipRepository;
    private readonly IMembershipCandidateRepository _membershipCandidateRepository;
    private readonly IMembershipCandidateLogMessageRepository _membershipCandidateLogMessageRepository;
    private readonly ILogger<GeneralElectionService> _logger;

    public GeneralElectionService(
        IAuthorizationService authorizationService,
        IWorklistTaskService worklistTaskService,
        IEiamAssignmentService eiamAssignmentService,
        IMasterDataRepository masterDataRepository,
        ITermOfOfficeDateService termOfOfficeDateService,
        IGeneralElectionHelperService generalElectionHelperService,
        IGeneralElectionCommitteeRepository generalElectionCommitteeRepository,
        ICommitteeRepository committeeRepository,
        IMembershipRepository membershipRepository,
        IMembershipCandidateRepository membershipCandidateRepository,
        IMembershipCandidateLogMessageRepository membershipCandidateLogMessageRepository,
        ILogger<GeneralElectionService> logger)
    {
        _authorizationService = authorizationService;
        _worklistTaskService = worklistTaskService;
        _eiamAssignmentService = eiamAssignmentService;
        _masterDataRepository = masterDataRepository;
        _termOfOfficeDateService = termOfOfficeDateService;
        _generalElectionHelperService = generalElectionHelperService;
        _generalElectionCommitteeRepository = generalElectionCommitteeRepository;
        _committeeRepository = committeeRepository;
        _membershipRepository = membershipRepository;
        _membershipCandidateRepository = membershipCandidateRepository;
        _membershipCandidateLogMessageRepository = membershipCandidateLogMessageRepository;
        _logger = logger;
    }

    public async Task<bool> IsGeneralElectionToggleAvailable()
    {
        bool result;

        var isGeneralElectionInProgress = await _termOfOfficeDateService.CheckForRunningGeneralElection();

        if (!isGeneralElectionInProgress)
        {
            result = false;
        }
        else if (_authorizationService.IsAdmin || _authorizationService.IsObserver)
        {
            result = true;
        }
        else
        {
            var committees = await _authorizationService.LoadCommittees();

            result = committees.Any(c => c.IsInGeneralElection);
        }

        return result;
    }

    public async Task<bool> PrepareGeneralElection(WorklistTaskCreateDto worklistTaskCreateDto)
    {
        _logger.LogInformation("Prepare general election started by user {User}", _authorizationService.GetCurrentUserName());

        var running = await _termOfOfficeDateService.CheckForRunningGeneralElection();

        if (running)
        {
            const string message = "There is already a general election in progress, cannot start a new one! Check the data in TermOfOfficeData table.";
            _logger.LogError(message);
            throw new BusinessValidationException(message);
        }

        var nextTermOfOffice = await _termOfOfficeDateService.GetNextTermOfOfficeDate();

        nextTermOfOffice.IsGeneralElection = true;
        await _termOfOfficeDateService.Update(nextTermOfOffice);

        var generalElectionResult = await StartGeneralElection(nextTermOfOffice.Id, nextTermOfOffice.BeginDate, (DateOnly)nextTermOfOffice.EndDate!, worklistTaskCreateDto.DueDate, worklistTaskCreateDto.Description ?? string.Empty);

        return generalElectionResult;
    }

    public async Task<bool> StartGeneralElection(Guid termOfOfficeDateId, DateOnly termOfOfficeBeginDate, DateOnly termOfOfficeEndDate, DateOnly dueDate, string description)
    {
        _logger.LogInformation("Start general election process for TermOfOfficeDateId {TermOfOfficeDateId}", termOfOfficeDateId);

        // this function can only be executed from Admin role, so we will always get 3 zero guids here!
        var (departmentId, officeId, committeeId) = await _eiamAssignmentService.GetPermittedIds();

        // Delete complete content of the GE tables. Check, if all dependent data is deleted as well here!
        await _generalElectionCommitteeRepository.DeleteAll();

        // Generate data for all the committees in GE, including memberships etc.
        var committees = await _committeeRepository.GetAllForGeneralElection(departmentId, officeId, committeeId);

        try
        {
            foreach (var committee in committees)
            {
                _logger.LogInformation("Generate general election data for committee {CommitteeId}", committee.Id);

                var generalElectionCommittee = GeneralElectionMapper.FromCommitteeToGeneralElectionCommittee(committee, _authorizationService.GetCurrentUserName());

                var createdGeneralElectionCommittee = await _generalElectionCommitteeRepository.Create(generalElectionCommittee);

                var activeMemberships = await _membershipRepository.GetAllActiveMembershipsForCommittee(committee.Id);

                foreach (var membership in activeMemberships)
                {
                    if (!CheckMembership(membership, termOfOfficeBeginDate))
                    {
                        continue;
                    }

                    var membershipCandidate = GeneralElectionMapper.FromMembershipAndPersonToMembershipCandidate(membership, createdGeneralElectionCommittee.Id, _authorizationService.GetCurrentUserName(), termOfOfficeBeginDate, termOfOfficeEndDate);

                    var newEndDate = await _generalElectionHelperService.CheckMembershipCandidateSpecialCases(membershipCandidate, createdGeneralElectionCommittee, membership.Person!.FederalDuty, termOfOfficeEndDate);

                    if (termOfOfficeBeginDate != newEndDate)
                    {
                        membershipCandidate.EndDate = newEndDate;
                        await _membershipCandidateRepository.Create(membershipCandidate);
                    }
                    else
                    {
                        // Here we write the new log table, saying that Member XX has not been transferred to new period
                        var personInfo = $"PersonId {membershipCandidate.PersonId}, {membershipCandidate.GivenName} {membershipCandidate.Surname}, {membershipCandidate.BirthYear}";
                        var errorText = BusinessTexts.GeneralElectionMaximumDurationExceeded + personInfo;

                        var dto = MembershipCandidateLogMessageMapper.ToMembershipCandidateLogMessage(createdGeneralElectionCommittee.Id, (Guid)membershipCandidate.PersonId!, errorText, _authorizationService.GetCurrentUserName());
                        await _membershipCandidateLogMessageRepository.Create(dto);
                    }
                }
            }

            var generalElectionWorklistTask = WorklistTaskMapper.CreateGeneralElectionMainWorklistTaskDto(termOfOfficeDateId, dueDate, description);

            var parentTask = await _worklistTaskService.CreateWorklistTaskByAdmin(generalElectionWorklistTask);

            var departments = await _masterDataRepository.GetDepartments();
            var generalMeasureTaskDueDate = DateOnly.FromDateTime(DateTime.Now.AddMonths(3));

            foreach (var department in departments)
            {
                var generalElectionDepartmentWorklistTask = WorklistTaskMapper.CreateGeneralElectionDepartmentWorklistTaskDto(parentTask.Id, parentTask.Description, department.Id, department.EiamAssignmentId, termOfOfficeDateId);
                await _worklistTaskService.CreateWorklistTaskByAdmin(generalElectionDepartmentWorklistTask);

                var generalMeasureDepartmentCheckTask = WorklistTaskMapper.CreateGeneralMeasureDepartmentCheckWorklistTaskDto(parentTask.Id, department.Id, department.EiamAssignmentId, termOfOfficeDateId, generalMeasureTaskDueDate);
                await _worklistTaskService.CreateWorklistTaskByAdmin(generalMeasureDepartmentCheckTask);

                var generalMeasureAdminValidationTask = WorklistTaskMapper.CreateGeneralMeasureAdminValidationWorklistTaskDto(parentTask.Id, department.Id, termOfOfficeDateId, generalMeasureTaskDueDate);
                await _worklistTaskService.CreateWorklistTaskByAdmin(generalMeasureAdminValidationTask);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during general election data preparation for TermOfOfficeDateId {TermOfOfficeDateId}", termOfOfficeDateId);

            // If something fails, we delete complete content of the GE tables. All dependent data is deleted as well here!
            await _generalElectionCommitteeRepository.DeleteAll();
            // delete als GE tasks by deleting the parent
            await _worklistTaskService.RemoveAllGeneralElectionTasks(termOfOfficeDateId);
            // set GE to not in progress?
            await ResetGeneralElectionInTermOfOfficeDate();
            throw;
        }

        _logger.LogInformation("General election started for TermOfOfficeDateId {TermOfOfficeDateId}", termOfOfficeDateId);

        return true;
    }

    public async Task UpdateCandidatesFromPerson(Person person)
    {
        _logger.LogInformation("Update candidates from person {PersonId}", person.Id);

        var candidates = await _membershipCandidateRepository.GetByPersonIdForUpdate(person.Id);

        foreach (var candidate in candidates)
        {
            candidate.Surname = person.Surname;
            candidate.GivenName = person.GivenName;
            candidate.BirthYear = person.BirthYear;
            candidate.LanguageId = person.LanguageId;
            candidate.GenderId = person.GenderId;
            candidate.Modified = person.Modified;
            candidate.ModifiedBy = person.ModifiedBy;

            await _membershipCandidateRepository.CommitChanges();
        }
    }

    //public async Task MirrorOrDeleteMembershipForGeneralElection(Membership membership, bool deleteCandidate)
    //{
    //    _logger.LogInformation("Mirror membership {MembershipId} for general election", membership.Id);

    //    var membershipCandidate = await _membershipCandidateRepository.GetByMembershipIdForUpdate(membership.Id);

    //    if (membershipCandidate != null)
    //    {
    //        if (membershipCandidate.GeneralElectionCommittee?.IsValidated == true)
    //        {
    //            _logger.LogInformation("Membership candidate list already validated, skip mirror entries");
    //            return;
    //        }

    //        if (deleteCandidate)
    //        {
    //            // if the end date has been shortened or the election type is an ending one, we can delete the membershipCandidate
    //            await _membershipCandidateRepository.Delete(membershipCandidate);
    //            _logger.LogInformation("Membership candidate {MembershipCandidateId} deleted because of end date change in present data", membershipCandidate.Id);
    //        }
    //        else
    //        {
    //            var updatedMembership = GeneralElectionMapper.ToMembershipCandidateMirrorDto(membership);

    //            membershipCandidate.MaximumEmploymentLevel = updatedMembership.MaximumEmploymentLevel;
    //            membershipCandidate.ElectionTypeId = updatedMembership.ElectionTypeId;
    //            membershipCandidate.FunctionId = updatedMembership.FunctionId;
    //            membershipCandidate.ElectionOfficeId = updatedMembership.ElectionOfficeId;
    //            membershipCandidate.MembershipAdditionId = updatedMembership.MembershipAdditionId;
    //            membershipCandidate.Remarks = updatedMembership.Remarks;
    //            membershipCandidate.RemarksStatus = updatedMembership.RemarksStatus;
    //            membershipCandidate.InCorrelationWithFederalDuty = updatedMembership.InCorrelationWithFederalDuty;
    //            membershipCandidate.Modified = updatedMembership.Modified;
    //            membershipCandidate.ModifiedBy = updatedMembership.ModifiedBy;
    //            membershipCandidate.InCorrelationWithFederalDuty = membership.InCorrelationWithFederalDuty;
    //            membershipCandidate.JustificationLongerDuty = membership.JustificationLongerDuty;
    //            membershipCandidate.JustificationShorterDuty = membership.JustificationShorterDuty;
    //            membershipCandidate.JustificationMemberInFederalAssembly = membership.JustificationMemberInFederalAssembly;
    //            membershipCandidate.JustificationMemberInFederalDuty = membership.JustificationMemberInFederalDuty;
    //            membershipCandidate.RequirementsProfile = membership.RequirementsProfile;

    //            await _membershipCandidateRepository.CommitChanges();

    //            _logger.LogInformation("Updated membership candidate {MembershipCandidateId} with data from current membership", membershipCandidate.Id);
    //        }
    //    }
    //}

    public async Task<bool> PrepareEndGeneralElection(WorklistTaskCreateDto worklistTaskCreateDto)
    {
        _logger.LogInformation("Prepare the end of general election started by user {User}", _authorizationService.GetCurrentUserName());

        var running = await _termOfOfficeDateService.CheckForRunningGeneralElection();

        if (!running)
        {
            const string message = "There is no general election in progress, cannot end the general election! Check the data in TermOfOfficeData table.";
            _logger.LogError(message);
            throw new BusinessValidationException(message);
        }

        var nextTermOfOffice = await _termOfOfficeDateService.GetNextTermOfOfficeDate();

        if (nextTermOfOffice != null && nextTermOfOffice.IsGeneralElection == true)
        {

            nextTermOfOffice.PlannedPublicationDate = worklistTaskCreateDto.DueDate;
            await _termOfOfficeDateService.Update(nextTermOfOffice);
        }

        return true;
    }

    public async Task<bool> EndGeneralElection()
    {
        _logger.LogInformation("Ending General Election by BackgroundService");

        var running = await _termOfOfficeDateService.CheckForRunningGeneralElection();

        if (!running)
        {
            const string message = "There is no general election in progress, cannot end the general election! Check the data in TermOfOfficeData table.";
            _logger.LogError(message);
            throw new BusinessValidationException(message);
        }

        var nextTermOfOffice = await _termOfOfficeDateService.GetNextTermOfOfficeDate();

        if (nextTermOfOffice != null && nextTermOfOffice.IsGeneralElection == true)
        {
            var committees = await _generalElectionCommitteeRepository.GetAll();

            var finishedCommittees = committees.Where(c => c.IsValidated).ToList();

            foreach (var committee in finishedCommittees)
            {
                // todo, finish it!
                //await _generalElectionCommitteeService.EndGeneralElectionForCommittee(committee);
            }

            nextTermOfOffice.PlannedPublicationDate = null;
            nextTermOfOffice.IsGeneralElection = false;

            await _termOfOfficeDateService.Update(nextTermOfOffice);
        }

        return true;
    }

    private static bool CheckMembership(Membership membership, DateOnly termOfOfficeStartDate)
    {
        // any form of retirement will be ignored
        if (membership.ElectionTypeId == ElectionType.MaximumMembershipDurationGuid || membership.ElectionTypeId == ElectionType.MembershipEndedBecauseOfDeathGuid ||
            membership.ElectionTypeId == ElectionType.OtherRetirementReasonGuid || membership.ElectionTypeId == ElectionType.RetirementGuid)
        {
            return false;
        }

        return true;
    }

    private async Task ResetGeneralElectionInTermOfOfficeDate()
    {
        var nextTermOfOffice = await _termOfOfficeDateService.GetNextTermOfOfficeDate();

        nextTermOfOffice.IsGeneralElection = false;

        await _termOfOfficeDateService.Update(nextTermOfOffice);
    }
}
