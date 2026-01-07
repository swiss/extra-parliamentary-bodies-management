using Bk.APG.Business.Dtos;
using Bk.APG.Business.Mapper;
using Bk.APG.Business.Models;
using Bk.APG.Business.Repositories;
using Bk.APG.CrossCutting;
using Bk.APG.CrossCutting.Exception;
using Microsoft.Extensions.Logging;

namespace Bk.APG.Business.Services;

public class GeneralElectionCommitteeService : IGeneralElectionCommitteeService
{
    private readonly IGeneralElectionCommitteeRepository _generalElectionCommitteeRepository;
    private readonly IAuthorizationService _authorizationService;
    private readonly ICultureService _cultureService;
    private readonly IGeneralMeasureRepository _generalMeasureRepository;
    private readonly IWorklistTaskRepository _worklistTaskRepository;
    private readonly ILogger<GeneralElectionCommitteeService> _logger;

    public GeneralElectionCommitteeService(
        IGeneralElectionCommitteeRepository generalElectionCommitteeRepository,
        IAuthorizationService authorizationService,
        ICultureService cultureService,
        IGeneralMeasureRepository generalMeasureRepository,
        IWorklistTaskRepository worklistTaskRepository,
        ILogger<GeneralElectionCommitteeService> logger
    )
    {
        _generalElectionCommitteeRepository = generalElectionCommitteeRepository;
        _authorizationService = authorizationService;
        _cultureService = cultureService;
        _generalMeasureRepository = generalMeasureRepository;
        _worklistTaskRepository = worklistTaskRepository;
        _logger = logger;
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
        var activeCandidateListTask = (await _worklistTaskRepository.GetAllByGeneralElectionCommitteeId(generalElectionCommittee.Id))
            .FirstOrDefault(x => x.WorklistTaskStateId == WorklistTaskState.Active);

        var canForward = activeCandidateListTask?.AssignedToId == currentEiamAssignment.Id;
        var isCompleted = generalElectionCommittee.CandidateListStateId == CandidateListState.Completed;
        var canValidate = currentEiamAssignment.Role is Role.Department or Role.Admin;

        dto.AssignedTo = activeCandidateListTask?.AssignedTo!.GetText();
        dto.CanSaveCandidateList = (canValidate || canForward) && !isCompleted;
        dto.CanValidateCandidateList = canValidate;
        dto.CanForwardCandidateList = canForward;
        dto.IsCandidateListCompleted = isCompleted;

        return dto;
    }

    public async Task<GeneralElectionCommitteeJustificationUpdateDto> GetGeneralElectionCommitteeJustificationForUpdate(Guid committeeId)
    {
        var generalElectionCommittee = await _generalElectionCommitteeRepository.GetByCommitteeId(committeeId);

        return GeneralElectionCommitteeMapper.ToGeneralElectionCommitteeJustificationUpdateDto(generalElectionCommittee);
    }

    public async Task<PagedResultDto<GeneralElectionCommitteeListDto>> GetGeneralElectionCommitteeList(PagingParametersDto paging, GeneralElectionCommitteeFilterParametersDto? filter, string? sort, SortDirection? sortDirection)
    {
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

        existingCommittee.ReleaseGeneralElection = updateDto.ReleaseGeneralElection;
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

    public async Task<GeneralElectionCommitteeJustificationUpdateDto> UpdateGeneralElectionCommitteeJustifications(Guid id, GeneralElectionCommitteeJustificationUpdateDto updateDto)
    {
        _logger.LogInformation("Update justifications for general election committee {CommitteeId}", id);

        var existingGeneralElectionCommittee = await _generalElectionCommitteeRepository.GetByIdForUpdate(id, updateDto.RowVersion);

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

        _logger.LogInformation("Updated justifications for general election committee {CommitteeId}", id);

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
}
