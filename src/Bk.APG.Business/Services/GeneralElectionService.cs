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
    private readonly IGeneralElectionCommitteeService _generalElectionCommitteeService;
    private readonly IGeneralElectionCommitteeRepository _generalElectionCommitteeRepository;
    private readonly ICommitteeRepository _committeeRepository;
    private readonly IMembershipRepository _membershipRepository;
    private readonly IMembershipCandidateRepository _membershipCandidateRepository;
    private readonly IMembershipCandidateLogMessageRepository _membershipCandidateLogMessageRepository;
    private readonly IWorklistTaskRepository _worklistTaskRepository;
    private readonly ILogger<GeneralElectionService> _logger;

    public GeneralElectionService(
        IAuthorizationService authorizationService,
        IWorklistTaskService worklistTaskService,
        IEiamAssignmentService eiamAssignmentService,
        IMasterDataRepository masterDataRepository,
        ITermOfOfficeDateService termOfOfficeDateService,
        IGeneralElectionCommitteeService generalElectionCommitteeService,
        IGeneralElectionCommitteeRepository generalElectionCommitteeRepository,
        ICommitteeRepository committeeRepository,
        IMembershipRepository membershipRepository,
        IMembershipCandidateRepository membershipCandidateRepository,
        IMembershipCandidateLogMessageRepository membershipCandidateLogMessageRepository,
        IWorklistTaskRepository worklistTaskRepository,
        ILogger<GeneralElectionService> logger)
    {
        _authorizationService = authorizationService;
        _worklistTaskService = worklistTaskService;
        _eiamAssignmentService = eiamAssignmentService;
        _masterDataRepository = masterDataRepository;
        _termOfOfficeDateService = termOfOfficeDateService;
        _generalElectionCommitteeService = generalElectionCommitteeService;
        _generalElectionCommitteeRepository = generalElectionCommitteeRepository;
        _committeeRepository = committeeRepository;
        _membershipRepository = membershipRepository;
        _membershipCandidateRepository = membershipCandidateRepository;
        _membershipCandidateLogMessageRepository = membershipCandidateLogMessageRepository;
        _worklistTaskRepository = worklistTaskRepository;
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
        ArgumentNullException.ThrowIfNull(worklistTaskCreateDto);

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
                generalElectionCommittee.CommitteeType = null;
                generalElectionCommittee.Department = null;

                var createdGeneralElectionCommittee = await _generalElectionCommitteeRepository.Create(generalElectionCommittee);

                var activeMemberships = await _membershipRepository.GetAllActiveMembershipsForCommittee(committee.Id);

                foreach (var membership in activeMemberships)
                {
                    if (!CheckMembership(membership))
                    {
                        continue;
                    }

                    var membershipCandidate = GeneralElectionMapper.FromMembershipAndPersonToMembershipCandidate(membership, createdGeneralElectionCommittee.Id, _authorizationService.GetCurrentUserName(), termOfOfficeBeginDate, termOfOfficeEndDate);

                    var newEndDate = await CheckMembershipCandidateSpecialCases(membershipCandidate, createdGeneralElectionCommittee, membership.Person!.FederalDuty, termOfOfficeEndDate);

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
        ArgumentNullException.ThrowIfNull(person);

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

    public async Task CreateNewMembershipCandidate(Membership membership, bool isSelected = false)
    {
        ArgumentNullException.ThrowIfNull(membership);

        _logger.LogInformation("Create new membership candidate for general election committee {CommitteeId}", membership.CommitteeId);

        var generalElectionCommittee = await _generalElectionCommitteeRepository.GetByCommitteeId(membership.CommitteeId);
        var termOfOfficeDate = generalElectionCommittee.TermOfOfficeDate;

        var membershipCandidate = GeneralElectionMapper.FromMembershipAndPersonToMembershipCandidate(membership, generalElectionCommittee.Id, _authorizationService.GetCurrentUserName(), termOfOfficeDate!.BeginDate, (DateOnly)termOfOfficeDate.EndDate!);

        // Check for maximum duration
        var newEndDate = await CheckMembershipCandidateSpecialCases(membershipCandidate, generalElectionCommittee, membership.Person!.FederalDuty, (DateOnly)termOfOfficeDate.EndDate!);

        if (termOfOfficeDate.BeginDate != newEndDate)
        {
            membershipCandidate.EndDate = newEndDate;
            membershipCandidate.IsSelected = isSelected;
            await _membershipCandidateRepository.Create(membershipCandidate);
            _logger.LogInformation("New membership candidate created {MembershipCandidateId}", membershipCandidate.Id);
        }
        else
        {
            // Here we write the new log table, saying that Member XX has not been transferred to new period
            var personInfo = $"PersonId {membershipCandidate.PersonId}, {membershipCandidate.GivenName} {membershipCandidate.Surname}, {membershipCandidate.BirthYear}";
            var errorText = BusinessTexts.GeneralElectionMaximumDurationExceeded + personInfo;

            var dto = MembershipCandidateLogMessageMapper.ToMembershipCandidateLogMessage(generalElectionCommittee.Id, (Guid)membershipCandidate.PersonId!, errorText, _authorizationService.GetCurrentUserName());
            await _membershipCandidateLogMessageRepository.Create(dto);
            _logger.LogInformation("Membership candidate for person {PersonId} not created because maximum duration exceeded", membershipCandidate.PersonId);
        }
    }

    private async Task<DateOnly> CheckMembershipCandidateSpecialCases(
        MembershipCandidate membershipCandidate,
        GeneralElectionCommittee committee,
        bool isPersonInFederalDuty,
        DateOnly termOfOfficeEndDate)
    {
        var correctEndDate = membershipCandidate.EndDate;

        if ((committee.CommitteeTypeId == CommitteeType.AuthoritiesCommissionGuid || committee.CommitteeTypeId == CommitteeType.AdministrationCommissionGuid) &&
            (!membershipCandidate.InCorrelationWithFederalDuty || (membershipCandidate.InCorrelationWithFederalDuty && isPersonInFederalDuty)))
        {
            // if all these conditions match, we have to check for the maximum duration of 16 years. So if current duration is smaller than 12 years, membership will continue.
            // If bigger than 12 years, the end date has to be set accordingly.
            var personMemberships = await _membershipRepository.GetAllMembershipsForCommitteeAndPerson(committee.CommitteeId, (Guid)membershipCandidate.PersonId!);

            var totalDays = 0;

            foreach (var membership in personMemberships)
            {
                totalDays += membership.EndDate.DayNumber - membership.BeginDate.DayNumber;
            }

            var currentYears = (int)Math.Round((double)totalDays / 365);

            if (currentYears is >= 13 and < 16)
            {
                // 16 -13 = 3 / 4 - 3
                var allowedYears = 16 - currentYears;

                correctEndDate = termOfOfficeEndDate.AddYears((4 - allowedYears) * -1);
            }
            else if (currentYears is >= 16)
            {
                correctEndDate = membershipCandidate.BeginDate;
            }
        }

        return correctEndDate;
    }

    public async Task<bool> PrepareEndGeneralElection(WorklistTaskCreateDto worklistTaskCreateDto)
    {
        ArgumentNullException.ThrowIfNull(worklistTaskCreateDto);

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
            // we immediately end the GE when the task is executed...
            nextTermOfOffice.PlannedPublicationDate = worklistTaskCreateDto.DueDate;
            nextTermOfOffice.IsGeneralElection = false;
            await _termOfOfficeDateService.Update(nextTermOfOffice);

            // there is only one of this type..
            var geStartTasks = await _worklistTaskRepository.GetByWorklistTaskTypeId(WorklistTaskType.GeneralElectionStart);
            var parentTask = geStartTasks.FirstOrDefault();

            if (parentTask != null)
            {
                var generalElectionEndWorklistTask = WorklistTaskMapper.CreateGeneralElectionEndWorklistTaskDto(parentTask.Id, nextTermOfOffice.Id, worklistTaskCreateDto.DueDate, worklistTaskCreateDto.Description);
                await _worklistTaskService.CreateWorklistTaskByAdmin(generalElectionEndWorklistTask);
            }
        }

        return true;
    }

    public async Task<bool> EndGeneralElection()
    {
        _logger.LogInformation("Ending General Election by BackgroundService");

        var nextTermOfOffice = await _termOfOfficeDateService.GetNextTermOfOfficeDate();

        if (nextTermOfOffice != null)
        {
            nextTermOfOffice.PlannedPublicationDate = null;
            nextTermOfOffice.PublicationDate = DateTime.UtcNow;
            nextTermOfOffice.IsGeneralElection = false;

            await _termOfOfficeDateService.Update(nextTermOfOffice);

            var committees = await _generalElectionCommitteeRepository.GetAll();

            var finishedCommittees = committees.Where(c => c.IsValidated).ToList();

            foreach (var committee in finishedCommittees)
            {
                // we only want to transfer the ones, which are selected
                committee.MembershipCandidates = committee.MembershipCandidates.Where(m => m.IsSelected && m.PersonId != null).ToList();

                await _generalElectionCommitteeService.EndGeneralElectionForCommittee(committee);
            }

            await _worklistTaskRepository.SetAllWorklistTasksToIsDeleted();
        }

        return true;
    }

    private static bool CheckMembership(Membership membership)
    {
        // any form of retirement will be ignored
        if (membership.ElectionTypeId == ElectionType.MaximumMembershipDurationGuid || membership.ElectionTypeId == ElectionType.MembershipEndedBecauseOfDeathGuid ||
            membership.ElectionTypeId == ElectionType.OtherRetirementReasonGuid || membership.ElectionTypeId == ElectionType.RetirementGuid)
        {
            return false;
        }

        // memberships with "Andere" as election office don't need to be handled in GEW
        if (membership.HasOtherElectionOffice)
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
