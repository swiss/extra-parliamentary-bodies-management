using System.Globalization;
using Bk.APG.Business.Dtos;
using Bk.APG.Business.Mapper;
using Bk.APG.Business.Models;
using Bk.APG.Business.Repositories;
using Bk.APG.Common.Resources;
using Bk.APG.CrossCutting;
using Bk.APG.CrossCutting.Exception;
using Microsoft.Extensions.Logging;
using Swiss.FCh.DocumentService.Client.Models;

namespace Bk.APG.Business.Services;

public class GeneralElectionCommitteeService : IGeneralElectionCommitteeService
{
    private readonly IGeneralElectionCommitteeRepository _generalElectionCommitteeRepository;
    private readonly IAuthorizationService _authorizationService;
    private readonly ICultureService _cultureService;
    private readonly ICommitteeService _committeeService;
    private readonly IGeneralMeasureRepository _generalMeasureRepository;
    private readonly IWorklistTaskRepository _worklistTaskRepository;
    private readonly IMasterDataRepository _masterDataRepository;
    private readonly Swiss.FCh.DocumentService.Client.IDocumentService _documentService;
    private readonly ILogger<GeneralElectionCommitteeService> _logger;

    public GeneralElectionCommitteeService(
        IGeneralElectionCommitteeRepository generalElectionCommitteeRepository,
        IAuthorizationService authorizationService,
        ICultureService cultureService,
        ICommitteeService committeeService,
        IGeneralMeasureRepository generalMeasureRepository,
        IWorklistTaskRepository worklistTaskRepository,
        IMasterDataRepository masterDataRepository,
        Swiss.FCh.DocumentService.Client.IDocumentService documentService,
        ILogger<GeneralElectionCommitteeService> logger
    )
    {
        _generalElectionCommitteeRepository = generalElectionCommitteeRepository;
        _authorizationService = authorizationService;
        _cultureService = cultureService;
        _committeeService = committeeService;
        _generalMeasureRepository = generalMeasureRepository;
        _worklistTaskRepository = worklistTaskRepository;
        _masterDataRepository = masterDataRepository;
        _documentService = documentService;
        _logger = logger;
    }

    public async Task InvalidateMembershipCandidateList(Guid committeeId)
    {
        var generalElectionCommittee = await _generalElectionCommitteeRepository.GetByCommitteeIdForUpdate(committeeId);
        generalElectionCommittee.IsValidated = false;
        generalElectionCommittee.CandidateListStateId = CandidateListState.Draft;

        var taskApproveByDepartment = (await _worklistTaskRepository.GetAllByGeneralElectionCommitteeId(generalElectionCommittee.Id)).FirstOrDefault(y => y.AssignedTo?.Role == Role.Department
            && y.WorklistTaskTypeId == WorklistTaskType.CandidateListApprove);
        var taskListForSecretariat = (await _worklistTaskRepository.GetAllByGeneralElectionCommitteeId(generalElectionCommittee.Id)).Where(y => y.AssignedTo?.Role == Role.Secretariat
            && (y.WorklistTaskTypeId == WorklistTaskType.GeneralElectionMissingJustifications ||
            y.WorklistTaskTypeId == WorklistTaskType.GeneralElectionMissingSecretariat ||
            y.WorklistTaskTypeId == WorklistTaskType.GeneralElectionPersonBaseData ||
            y.WorklistTaskTypeId == WorklistTaskType.GeneralElectionPersonInterests ||
            y.WorklistTaskTypeId == WorklistTaskType.GeneralElectionMissingDataProtectionOfficer ||
            y.WorklistTaskTypeId == WorklistTaskType.GeneralElectionMembershipValidation));

        if (taskApproveByDepartment is not null)
        {
            taskApproveByDepartment.WorklistTaskStateId = WorklistTaskState.Active;
            taskApproveByDepartment.Modified = DateTime.UtcNow;
            taskApproveByDepartment.ModifiedBy = _authorizationService.GetCurrentUserName();
        }

        foreach (var task in taskListForSecretariat)
        {
            task.WorklistTaskStateId = WorklistTaskState.Inactive;
            task.Modified = DateTime.UtcNow;
            task.ModifiedBy = _authorizationService.GetCurrentUserName();
        }

        // Invalidate BRA Ready tasks

        var taskListForProposalAdminOrDepartmentOrOffice = (await _worklistTaskRepository.GetAllByGeneralElectionCommitteeId(generalElectionCommittee.Id)).Where(y =>
           (y.AssignedTo?.Role == Role.Admin || y.AssignedTo?.Role == Role.Department || y.AssignedTo?.Role == Role.Office || y.AssignedTo?.Role == Role.Secretariat)
           && y.WorklistTaskTypeId == WorklistTaskType.ReadyForFederalCouncilProposal);

        foreach (var task in taskListForProposalAdminOrDepartmentOrOffice)
        {
            task.WorklistTaskStateId = WorklistTaskState.Inactive;
            task.Modified = DateTime.UtcNow;
            task.ModifiedBy = _authorizationService.GetCurrentUserName();
        }
        await _worklistTaskRepository.CommitChanges();
    }

    public async Task SetFederalCouncilProposalToDirty(Guid committeeId)
    {
        var generalElectionCommittee = await _generalElectionCommitteeRepository.GetByCommitteeIdForUpdate(committeeId);
        generalElectionCommittee.IsFederalCouncilProposalDirty = true;

        var taskListForProposalSecretariat = (await _worklistTaskRepository.GetAllByGeneralElectionCommitteeId(generalElectionCommittee.Id)).Where(y => y.WorklistTaskStateId == WorklistTaskState.Completed
            && (y.AssignedTo?.Role == Role.Secretariat)
            && y.WorklistTaskTypeId == WorklistTaskType.ReadyForFederalCouncilProposal);

        foreach (var task in taskListForProposalSecretariat)
        {
            task.WorklistTaskStateId = WorklistTaskState.Active;
            task.Modified = DateTime.UtcNow;
            task.ModifiedBy = _authorizationService.GetCurrentUserName();
        }

        var taskListForProposalAdminOrDepartmentOrOffice = (await _worklistTaskRepository.GetAllByGeneralElectionCommitteeId(generalElectionCommittee.Id)).Where(y =>
            (y.AssignedTo?.Role == Role.Admin || y.AssignedTo?.Role == Role.Department || y.AssignedTo?.Role == Role.Office)
            && y.WorklistTaskTypeId == WorklistTaskType.ReadyForFederalCouncilProposal);

        foreach (var task in taskListForProposalAdminOrDepartmentOrOffice)
        {
            task.WorklistTaskStateId = WorklistTaskState.Inactive;
            task.Modified = DateTime.UtcNow;
            task.ModifiedBy = _authorizationService.GetCurrentUserName();
        }
        await _worklistTaskRepository.CommitChanges();
    }

    public async Task<GeneralElectionCommitteeDetailDto> GetGeneralElectionCommittee(Guid committeeId)
    {
        var generalElectionCommittee = await _generalElectionCommitteeRepository.GetByCommitteeId(committeeId);

        var dto = GeneralElectionCommitteeMapper.ToGeneralElectionCommitteeDetailDto(generalElectionCommittee);

        dto.CanEdit = await _authorizationService.HasAccessToCommittee(generalElectionCommittee.Committee!);

        var generalGenderMeasure = await _generalMeasureRepository.GetGeneralGenderMeasure(generalElectionCommittee.DepartmentId);
        dto.GeneralGenderMeasure = generalGenderMeasure?.Description;

        var generalLanguageMeasure = await _generalMeasureRepository.GetGeneralLanguageMeasure(generalElectionCommittee.DepartmentId);
        dto.GeneralLanguageMeasure = generalLanguageMeasure?.Description;

        var currentEiamAssignment = await _authorizationService.GetCurrentEiamAssignment();
        var generalElectionCommitteeTasks = (await _worklistTaskRepository.GetAllByGeneralElectionCommitteeId(generalElectionCommittee.Id)).ToList();
        var candidateListTasks = generalElectionCommitteeTasks
            .Where(x => x.WorklistTaskTypeId == WorklistTaskType.CandidateListCreate || x.WorklistTaskTypeId == WorklistTaskType.CandidateListForward || x.WorklistTaskTypeId == WorklistTaskType.CandidateListApprove).ToList();
        var activeCandidateListTask = candidateListTasks.FirstOrDefault(x => x.WorklistTaskStateId == WorklistTaskState.Active);
        var activeReadyForProposalTask = generalElectionCommitteeTasks
            .FirstOrDefault(x => x.WorklistTaskTypeId == WorklistTaskType.ReadyForFederalCouncilProposal && x.WorklistTaskStateId == WorklistTaskState.Active);
        var wasGeneralElectionStartedForCommittee = candidateListTasks.Count != 0;

        var canForward = activeCandidateListTask?.AssignedToId == currentEiamAssignment.Id;
        var isCandidateListValidatedOrReadyForFederalCouncil = generalElectionCommittee.CandidateListStateId == CandidateListState.Validated
            || generalElectionCommittee.CandidateListStateId == CandidateListState.ReadyForFederalCouncilProposalForwarded
            || generalElectionCommittee.CandidateListStateId == CandidateListState.ReadyForFederalCouncilProposalFinalized;
        var isFederalCouncilProposalForwarded = generalElectionCommittee.CandidateListStateId == CandidateListState.ReadyForFederalCouncilProposalForwarded;

        var hasActiveMembershipValidationTasks = generalElectionCommitteeTasks.Any(y => y.AssignedTo?.Role == Role.Secretariat && y.WorklistTaskStateId == WorklistTaskState.Active
            && (y.WorklistTaskTypeId == WorklistTaskType.GeneralElectionMissingJustifications ||
            y.WorklistTaskTypeId == WorklistTaskType.GeneralElectionMissingSecretariat ||
            y.WorklistTaskTypeId == WorklistTaskType.GeneralElectionPersonBaseData ||
            y.WorklistTaskTypeId == WorklistTaskType.GeneralElectionPersonInterests ||
            y.WorklistTaskTypeId == WorklistTaskType.GeneralElectionMissingDataProtectionOfficer ||
            y.WorklistTaskTypeId == WorklistTaskType.GeneralElectionMembershipValidation));

        var isFederalCouncilProposalFinalized = generalElectionCommittee.CandidateListStateId == CandidateListState.ReadyForFederalCouncilProposalFinalized;
        var isReadyForProposalForCurrentRole = generalElectionCommittee.CandidateListStateId == CandidateListState.ReadyForFederalCouncilProposalForwarded &&
            generalElectionCommitteeTasks.FirstOrDefault(y => y.WorklistTaskTypeId == WorklistTaskType.ReadyForFederalCouncilProposal && y.AssignedToId == currentEiamAssignment.Id && y.WorklistTaskStateId == WorklistTaskState.Completed) is not null;
        var canValidate = (currentEiamAssignment.Role == Role.Department && !isReadyForProposalForCurrentRole && !isFederalCouncilProposalFinalized) || currentEiamAssignment.Role == Role.Admin;
        var canForwardReadyForProposal = activeReadyForProposalTask?.AssignedToId == currentEiamAssignment.Id;
        var canFinalizeReadyForProposal = currentEiamAssignment.Role == Role.Admin && !isFederalCouncilProposalFinalized && isFederalCouncilProposalForwarded;

        dto.AssignedTo = activeCandidateListTask?.AssignedTo!.GetText();
        dto.WasGeneralElectionStartedForCommittee = wasGeneralElectionStartedForCommittee;
        dto.CanSaveCandidateList = wasGeneralElectionStartedForCommittee && (canValidate || canForward) && !isCandidateListValidatedOrReadyForFederalCouncil;
        dto.CanValidateCandidateList = wasGeneralElectionStartedForCommittee && canValidate;
        dto.CanForwardCandidateList = wasGeneralElectionStartedForCommittee && canForward;
        dto.IsCandidateListValidated = isCandidateListValidatedOrReadyForFederalCouncil;
        dto.ReadyForProposalAssignedTo = activeReadyForProposalTask?.AssignedTo!.GetText();
        dto.CanForwardReadyForProposal = canForwardReadyForProposal;
        dto.CanFinalizeReadyForProposal = canFinalizeReadyForProposal;
        dto.IsReadyForProposalForCurrentRole = isReadyForProposalForCurrentRole;
        dto.IsReadyForProposalFinalized = isFederalCouncilProposalFinalized;
        dto.HasActiveMembershipValidationTasks = hasActiveMembershipValidationTasks;

        return dto;
    }

    public async Task<GeneralElectionCommitteeJustificationUpdateDto> GetGeneralElectionCommitteeJustificationForUpdate(Guid committeeId)
    {
        var generalElectionCommittee = await _generalElectionCommitteeRepository.GetByCommitteeId(committeeId);

        return GeneralElectionCommitteeMapper.ToGeneralElectionCommitteeJustificationUpdateDto(generalElectionCommittee);
    }

    public async Task<IEnumerable<GeneralElectionCommitteeListDto>> GetGeneralElectionCommitteeListForRecipientExport(GeneralElectionCommitteeExportFilterParametersDto? filter)
    {
        var filterParameters = GeneralElectionCommitteeMapper.ToGeneralElectionCommitteeExportFilterParameters(filter);
        var allElectionTypes = await _masterDataRepository.GetElectionTypes();

        var electionTypeList = allElectionTypes.Select(e => e.Id).ToList();
        electionTypeList.Remove(ElectionType.MembershipEndedBecauseOfDeathGuid);
        electionTypeList.Remove(ElectionType.PermanentGuid);

        var electionTypeListPresent = electionTypeList.ToList();
        electionTypeListPresent.Remove(ElectionType.NewElectionGuid);
        electionTypeListPresent.Remove(ElectionType.ReElectionGuid);

        var electionTypeListFuture = electionTypeList.ToList();
        electionTypeListFuture.Remove(ElectionType.MaximumMembershipDurationGuid);
        electionTypeListFuture.Remove(ElectionType.OtherRetirementReasonGuid);
        electionTypeListFuture.Remove(ElectionType.RetirementGuid);

        if (_authorizationService is { IsAdmin: false, IsObserver: false })
        {
            var assignedCommittees = await _authorizationService.LoadCommittees();
            var committeeIds = assignedCommittees.Select(x => x.Id).ToList();

            if (committeeIds.Count == 0)
            {
                return new List<GeneralElectionCommitteeListDto>();
            }

            filterParameters.CommitteeIds = committeeIds;
        }

        var committees = await _generalElectionCommitteeRepository.GetAllForFormLetterPreview(filterParameters, electionTypeListFuture);

        var allCommittees = committees.Select(committee => GeneralElectionCommitteeMapper.ToGeneralElectionCommitteeListDto(committee, _cultureService.GetCurrentUiCulture())).ToList();

        // here we have to select the present committees as well, when filterDto ElectionType contains one of the 3 retire types or none are selected at all..
        if (filterParameters.ElectionTypeIds == null || (filterParameters.ElectionTypeIds != null && (filterParameters.ElectionTypeIds.Contains(ElectionType.MaximumMembershipDurationGuid) ||
            filterParameters.ElectionTypeIds.Contains(ElectionType.RetirementGuid) || filterParameters.ElectionTypeIds.Contains(ElectionType.OtherRetirementReasonGuid))))
        {
            var currentCommittees = await _committeeService.GetCommitteesWithRetiredMembers(filter, electionTypeListPresent);

            var existingCommittees = currentCommittees.Select(c => GeneralElectionMapper.FromCommitteeToGeneralElectionCommittee(c, "pp"));

            var presentCommittees = existingCommittees.Select(committee => GeneralElectionCommitteeMapper.ToGeneralElectionCommitteeListDto(committee, _cultureService.GetCurrentUiCulture()));

            var existingNames = new HashSet<string>(allCommittees.Select(x => x.Description));

            var itemsToAdd = presentCommittees.Where(x => !existingNames.Contains(x.Description));

            allCommittees.AddRange(itemsToAdd);
        }

        return allCommittees;
    }

    public async Task<PagedResultDto<GeneralElectionCommitteeListDto>> GetGeneralElectionCommitteeList(PagingParametersDto paging, GeneralElectionCommitteeFilterParametersDto? filter, string? sort, SortDirection? sortDirection)
    {
        ArgumentNullException.ThrowIfNull(paging);

        var filterParameters = GeneralElectionCommitteeMapper.ToGeneralElectionCommitteeFilterParameters(filter);

        if (_authorizationService is { IsAdmin: false, IsObserver: false })
        {
            var assignedCommittees = await _authorizationService.LoadCommittees();
            var committeeIds = assignedCommittees.Select(x => x.Id).ToList();

            if (committeeIds.Count == 0)
            {
                return new PagedResultDto<GeneralElectionCommitteeListDto>
                {
                    Index = paging.PageIndex ?? 0,
                    Total = 0,
                    Items = []
                };
            }

            filterParameters.CommitteeIds = committeeIds;
        }

        var pagingParameters = PagingMapper.ToPagingParameters(paging);
        var committees = await _generalElectionCommitteeRepository.GetAll(pagingParameters, filterParameters, sort, sortDirection);
        return new PagedResultDto<GeneralElectionCommitteeListDto>
        {
            Index = committees.Index,
            Total = committees.Total,
            Items = committees.Items.Select(committee => GeneralElectionCommitteeMapper.ToGeneralElectionCommitteeListDto(committee, _cultureService.GetCurrentUiCulture()))
        };
    }

    public async Task<GeneralElectionCommitteeUpdateDto> GetGeneralElectionCommitteeForUpdate(Guid committeeId)
    {
        var generalElectionCommitteeUpdate = await _generalElectionCommitteeRepository.GetByCommitteeIdForUpdate(committeeId);
        var dto = GeneralElectionCommitteeMapper.ToGeneralElectionCommitteeUpdateDto(generalElectionCommitteeUpdate);
        dto.CanEditAll =
            _authorizationService.IsAdmin ||
            (generalElectionCommitteeUpdate.Committee!.IsActive && _authorizationService.IsDepartment
                && generalElectionCommitteeUpdate.Committee!.DepartmentId == (await _authorizationService.GetDepartment())?.Id);
        dto.CanEditDepartment = _authorizationService.IsAdmin;
        dto.CanEditLegalbase = await _authorizationService.HasAccessToCommittee(generalElectionCommitteeUpdate.Committee!);

        return dto;
    }

    public async Task<GeneralElectionCommitteeDetailDto> UpdateGeneralElectionCommittee(Guid committeeId, GeneralElectionCommitteeUpdateDto updateDto)
    {
        ArgumentNullException.ThrowIfNull(updateDto);

        _logger.LogInformation("Update general election committee {CommitteeId}", committeeId);

        var existingCommittee = await _generalElectionCommitteeRepository.GetByCommitteeIdForUpdate(committeeId, updateDto.RowVersion);

        await CheckAuthorizationForUpdate(existingCommittee.Committee!);

        existingCommittee.EndDate = updateDto.EndDate;
        existingCommittee.Modified = DateTime.UtcNow;
        existingCommittee.ModifiedBy = _authorizationService.GetCurrentUserName();

        existingCommittee.DescriptionGerman = updateDto.DescriptionGerman;
        existingCommittee.DescriptionFrench = updateDto.DescriptionFrench;
        existingCommittee.DescriptionItalian = updateDto.DescriptionItalian;
        existingCommittee.DescriptionRomansh = updateDto.DescriptionRomansh;

        existingCommittee.CommitteeLevelId = updateDto.LevelId;
        existingCommittee.OfficeId = updateDto.OfficeId;
        existingCommittee.DepartmentId = updateDto.DepartmentId;
        existingCommittee.CommitteeTypeId = updateDto.CommitteeTypeId;

        existingCommittee.FederalLawEstablishment = updateDto.FederalLawEstablishment;
        existingCommittee.FederalInstitution = updateDto.FederalInstitution;
        existingCommittee.SupervisionDuty = updateDto.SupervisionDuty;
        existingCommittee.MarketOrientated = updateDto.MarketOrientated;

        existingCommittee.LegalFormId = updateDto.LegalFormId;
        existingCommittee.LegalBase = updateDto.LegalBase;

        existingCommittee.TermOfOfficeId = updateDto.TermOfOfficeId;
        existingCommittee.MinimalMembers = updateDto.MinimalMembers;
        existingCommittee.MaximalMembers = updateDto.MaximalMembers;
        existingCommittee.AdditionalAuthorityMembers = updateDto.AdditionalAuthorityMembers;
        existingCommittee.LinkAuthorityWebsite = updateDto.AdditionalAuthorityMembers ? updateDto.LinkAuthorityWebsite : null;
        existingCommittee.LinkHomepageGerman = updateDto.LinkHomepageGerman;
        existingCommittee.LinkHomepageFrench = updateDto.LinkHomepageFrench;
        existingCommittee.LinkHomepageItalian = updateDto.LinkHomepageItalian;
        existingCommittee.LinkHomepageRomansh = updateDto.LinkHomepageRomansh;

        existingCommittee.Modified = DateTime.UtcNow;
        existingCommittee.ModifiedBy = _authorizationService.GetCurrentUserName();

        await _generalElectionCommitteeRepository.CommitChanges();

        _logger.LogInformation("Updated general election committee {CommitteeId}", committeeId);

        var changedGeneralElectionCommittee = await _generalElectionCommitteeRepository.GetByCommitteeId(committeeId);

        return GeneralElectionCommitteeMapper.ToGeneralElectionCommitteeDetailDto(changedGeneralElectionCommittee);
    }

    public async Task<GeneralElectionCommitteeJustificationUpdateDto> UpdateGeneralElectionCommitteeJustifications(Guid committeeId, GeneralElectionCommitteeJustificationUpdateDto updateDto)
    {
        ArgumentNullException.ThrowIfNull(updateDto);

        _logger.LogInformation("Update justifications for general election committee {CommitteeId}", committeeId);

        var existingGeneralElectionCommittee = await _generalElectionCommitteeRepository.GetByIdForUpdate(committeeId, updateDto.RowVersion);

        await CheckAuthorizationForUpdate(existingGeneralElectionCommittee.Committee!);

        existingGeneralElectionCommittee.JustificationMembers = updateDto.JustificationMembers;
        existingGeneralElectionCommittee.JustificationGenders = updateDto.JustificationGenders;
        existingGeneralElectionCommittee.MeasuresGenders = updateDto.MeasuresGenders;
        existingGeneralElectionCommittee.JustificationLanguages = updateDto.JustificationLanguages;
        existingGeneralElectionCommittee.MeasuresLanguages = updateDto.MeasuresLanguages;
        existingGeneralElectionCommittee.SelectionProcedure = updateDto.SelectionProcedure;

        existingGeneralElectionCommittee.Modified = DateTime.UtcNow;
        existingGeneralElectionCommittee.ModifiedBy = _authorizationService.GetCurrentUserName();

        var missingJustificationTask = (await _worklistTaskRepository.GetAllByGeneralElectionCommitteeId(existingGeneralElectionCommittee.Id))
            .FirstOrDefault(x => x.WorklistTaskTypeId == WorklistTaskType.GeneralElectionMissingJustifications);

        if (missingJustificationTask is not null && missingJustificationTask.WorklistTaskStateId != WorklistTaskState.Completed)
        {
            missingJustificationTask.WorklistTaskStateId = WorklistTaskState.Completed;
            missingJustificationTask.Modified = DateTime.UtcNow;
            missingJustificationTask.ModifiedBy = _authorizationService.GetCurrentUserName();
        }

        await _generalElectionCommitteeRepository.CommitChanges();

        _logger.LogInformation("Updated justifications for general election committee {CommitteeId}", committeeId);

        return GeneralElectionCommitteeMapper.ToGeneralElectionCommitteeJustificationUpdateDto(existingGeneralElectionCommittee);
    }

    public async Task<GeneralElectionCommitteeUpdateDto> UpdateGeneralElectionCommitteeVacancies(Guid id, int vacancies)
    {
        _logger.LogInformation("Update vacancies for general election committee {CommitteeId}", id);

        var existingCommittee = await _generalElectionCommitteeRepository.GetByCommitteeIdForUpdate(id);

        await CheckAuthorizationForUpdate(existingCommittee.Committee!);

        existingCommittee.VacanciesGeneralElection = vacancies;
        existingCommittee.Modified = DateTime.UtcNow;
        existingCommittee.ModifiedBy = _authorizationService.GetCurrentUserName();

        await _generalElectionCommitteeRepository.CommitChanges();

        _logger.LogInformation("Updated vacancies for general election committee {CommitteeId}", id);

        return GeneralElectionCommitteeMapper.ToGeneralElectionCommitteeUpdateDto(existingCommittee);
    }

    public async Task<IEnumerable<GeneralElectionCommitteeListDto>> GetAllUnfinishedCommittees()
    {
        var committees = await _generalElectionCommitteeRepository.GetAll();
        var unfinishedCommittees = committees.Where(c => c.CandidateListStateId != CandidateListState.ReadyForFederalCouncilProposalFinalized);

        var list = unfinishedCommittees.Select(committee => GeneralElectionCommitteeMapper.ToGeneralElectionCommitteeListDto(committee, _cultureService.GetCurrentUiCulture()));
        list = list.OrderBy(l => l.Description);

        return list;
    }

    public async Task<(string fileName, Stream content)> GenerateCandidateListExport(Guid id, IEnumerable<Guid> membershipCandidateIds)
    {
        _logger.LogInformation("Generate candidate list export for general election committee {CommitteeId}", id);

        string[] headers =
        [
            BusinessTexts.CandidateList_Committee,
            BusinessTexts.CandidateList_Office,
            BusinessTexts.CandidateList_Title,
            BusinessTexts.CandidateList_Surname,
            BusinessTexts.CandidateList_GivenName,
            BusinessTexts.CandidateList_BirthYear,
            BusinessTexts.CandidateList_Gender,
            BusinessTexts.CandidateList_Language,
            BusinessTexts.CandidateList_RemarkStatus,
            BusinessTexts.CandidateList_Function,
            BusinessTexts.CandidateList_BeginDate,
            BusinessTexts.CandidateList_EndDate,
            BusinessTexts.CandidateList_ElectionType,
            BusinessTexts.CandidateList_MembershipAddition,
            BusinessTexts.CandidateList_Remarks,
            BusinessTexts.CandidateList_Occupation,
            BusinessTexts.CandidateList_City,
            BusinessTexts.CandidateList_Phone,
            BusinessTexts.CandidateList_Email,
            BusinessTexts.CandidateList_Interests
        ];

        var bodyCells = await GetCandidateListData(id, membershipCandidateIds);
        var spreadsheet = new Spreadsheet
        {
            HeaderCells = headers.Select(header => new Cell { Text = header, Format = CellFormat.Bold }).ToList(),
            BodyCells = bodyCells,
        };

        var exportStream = await _documentService.CreateExcel(spreadsheet);

        return (GenerateFileName(DateTime.Now, BusinessTexts.CandidateList), exportStream);
    }

    public async Task<bool> EndGeneralElectionForCommittee(GeneralElectionCommittee committee)
    {
        ArgumentNullException.ThrowIfNull(committee);

        // Writes back all the changes from General Election to the current data
        var mappedCommittee = GeneralElectionMapper.FromGeneralElectionCommitteeToCommittee(committee);
        var updateCommittee = CommitteeMapper.ToCommitteeUpdateDto(mappedCommittee);

        await _committeeService.UpdateCommitteeAfterGeneralElection(updateCommittee.Id, updateCommittee, committee.MembershipCandidates.ToList());

        return true;
    }

    private async Task<IList<IList<Cell>>> GetCandidateListData(Guid id, IEnumerable<Guid> membershipCandidateIds)
    {
        var committee = await _generalElectionCommitteeRepository.GetForCandidateListExport(id, membershipCandidateIds);

        var bodyCells = committee
            .MembershipCandidates
            .OrderBy(y => y.Person?.Surname ?? y.Surname)
            .ThenBy(y => y.Person?.GivenName ?? y.GivenName)
            .Select(candidate => new List<Cell>
            {
                new() { Text = committee.GetDescription() }, // Gremium
                new() { Text = committee.Office?.GetDescription() }, // Verwaltungsstelle
                new() { Text = candidate.Person is not null ? candidate.Person!.Title : string.Empty }, // Title
                new() { Text = candidate.Person is not null ? candidate.Person!.Surname : candidate.Surname }, // Name
                new() { Text = candidate.Person is not null ? candidate.Person!.GivenName : candidate.GivenName }, // Vorname
                NumberCell(candidate.Person?.BirthYear ?? candidate.BirthYear), // Jahrgang
                new() { Text = candidate.Person?.Gender?.GetText() ?? candidate.Gender?.GetText() ?? string.Empty }, // Geschlecht
                new() { Text = candidate.Person?.Language?.GetText() ?? candidate.Language?.GetText() ?? string.Empty }, // Sprache
                new() { Text = candidate.RemarksStatus }, // Vertretung
                new() { Text = candidate.Function?.GetText() }, // Funktion
                DateCell(candidate.BeginDate), // Startdatum
                DateCell(candidate.EndDate), // Enddatum
                new() { Text = candidate.ElectionType?.GetText() }, // Status
                new() { Text = candidate.MembershipAddition?.GetText() }, // Mitgliedzusatz
                new() { Text = candidate.Remarks }, // Bemerkungen
                new() { Text = string.Join(";", candidate.Person?.Occupations?.Select(y => y.GetText()) ?? Enumerable.Empty<string>()) }, // Beruf
                new() { Text = candidate.Person?.CorrespondenceAddress?.City ?? string.Empty }, // Ort
                new() { Text = candidate.Person?.CorrespondenceAddress?.Phone ?? string.Empty }, // Telefon
                new() { Text = candidate.Person?.CorrespondenceAddress?.Email ?? string.Empty }, // E-Mail
                new() { Text = string.Join(";", candidate.Person?.Interests?.Where(y => !string.IsNullOrWhiteSpace(y.InterestText)).Select(y => y.InterestText) ?? Enumerable.Empty<string>()) } // Interessenbindungen
            } as IList<Cell>).ToList();

        return bodyCells;
    }

    private static Cell NumberCell(decimal value, string format = "0")
    {
        return new Cell
        {
            Text = value.ToString(CultureInfo.InvariantCulture),
            FormatType = CellFormatTypes.Number,
            Format = format
        };
    }

    private static Cell DateCell(DateOnly? value)
    {
        return new Cell
        {
            Text = value is not null ? value.Value.ToString("O", CultureInfo.InvariantCulture) : string.Empty,
            FormatType = CellFormatTypes.Date,
            Format = "dd.MM.yyyy"
        };
    }

    private async Task CheckAuthorizationForUpdate(Committee committee)
    {
        if (!(_authorizationService.IsAdmin || (_authorizationService.IsDepartment && (await _authorizationService.GetDepartment())?.Id == committee.DepartmentId) ||
                ((_authorizationService.IsOffice || _authorizationService.IsSecretariat) && await _authorizationService.IsCommitteeAssigned(committee.Id))))
        {
            _logger.LogError("User is not allowed to edit general election committee {CommitteeId}", committee.Id);

            throw new AuthorizationException($"User is not allowed to edit general election committee with id: {committee.Id}");
        }

        if (!committee.IsActive && !_authorizationService.IsAdmin)
        {
            _logger.LogError("General election committee {CommitteeId} can be updated by admin role only", committee.Id);

            throw new AuthorizationException($"General election committee with id: {committee.Id} can be updated by admin role only");
        }
    }

    private static string GenerateFileName(DateTime exportDate, string exportType)
    {
        return $"{exportDate:yyyyMMdd}_{exportType}.xlsx";
    }
}
