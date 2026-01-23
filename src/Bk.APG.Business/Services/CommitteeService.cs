using Bk.APG.Business.Dtos;
using Bk.APG.Business.Mapper;
using Bk.APG.Business.Models;
using Bk.APG.Business.Repositories;
using Bk.APG.CrossCutting;
using Bk.APG.CrossCutting.Exception;
using Microsoft.Extensions.Logging;

namespace Bk.APG.Business.Services;

public class CommitteeService : ICommitteeService
{
    private readonly ICommitteeRepository _committeeRepository;
    private readonly IPersonRepository _personRepository;
    private readonly ICultureService _cultureService;
    private readonly IAuthorizationService _authorizationService;
    private readonly IEiamAssignmentService _eiamAssignmentService;
    private readonly IMasterDataRepository _masterDataRepository;
    private readonly IGeneralMeasureRepository _generalMeasureRepository;
    private readonly IMembershipRepository _membershipRepository;
    private readonly ILogger<CommitteeService> _logger;

    public CommitteeService(
        ICommitteeRepository committeeRepository,
        IPersonRepository personRepository,
        ICultureService cultureService,
        IAuthorizationService authorizationService,
        IEiamAssignmentService eiamAssignmentService,
        IMasterDataRepository masterDataRepository,
        IGeneralMeasureRepository generalMeasureRepository,
        IMembershipRepository membershipRepository,
        ILogger<CommitteeService> logger)
    {
        _committeeRepository = committeeRepository;
        _personRepository = personRepository;
        _cultureService = cultureService;
        _authorizationService = authorizationService;
        _eiamAssignmentService = eiamAssignmentService;
        _masterDataRepository = masterDataRepository;
        _generalMeasureRepository = generalMeasureRepository;
        _membershipRepository = membershipRepository;
        _logger = logger;
    }

    public async Task<CommitteeCreateDto> GetEmpty()
    {
        var departmentId = (await _authorizationService.GetDepartment())?.Id;
        var dto = new CommitteeCreateDto
        {
            BeginDate = DateOnly.FromDateTime(DateTime.Now),
            CommitteeTypeId = Guid.Empty,
            DescriptionGerman = string.Empty,
            DescriptionFrench = string.Empty,
            DescriptionItalian = string.Empty,
            DescriptionRomansh = string.Empty,
            LevelId = Guid.Empty,
            TermOfOfficeId = Guid.Empty,
            FederalLawEstablishment = null,
            MarketOrientated = null,
            SupervisionDuty = null,
            DepartmentId = departmentId ?? Guid.Empty,
            OfficeId = Guid.Empty,
            CanEditAll = _authorizationService.IsAdmin || _authorizationService.IsDepartment,
            CanEditDepartment = _authorizationService.IsAdmin,
            CanEditLegalbase = true
        };
        return dto;
    }

    public async Task<PagedResultDto<CommitteeListDto>> GetCommitteeList(PagingParametersDto paging, CommitteeFilterParametersDto? filter, string? sort, SortDirection? sortDirection)
    {
        var filterParameters = CommitteeMapper.ToCommitteeFilterParameters(filter);
        var pagingParameters = PagingMapper.ToPagingParameters(paging);
        var committees = await _committeeRepository.GetAll(pagingParameters, filterParameters, sort, sortDirection);
        return new PagedResultDto<CommitteeListDto>
        {
            Index = committees.Index,
            Total = committees.Total,
            Items = committees.Items.Select(committee => CommitteeMapper.ToCommitteeListDto(committee, _cultureService.GetCurrentUiCulture()))
        };
    }

    public async Task<IEnumerable<CommitteeListDto>> GetCommitteeListForExport(RequestAndReportsFilterParametersDto? filterParameters = null)
    {
        var (departmentId, officeId, committeeId) = await _eiamAssignmentService.GetPermittedIds();

        var filterParameterDto = CommitteeMapper.ToCommitteeExportFilterParametersDto(filterParameters);

        var committees = await _committeeRepository.GetAllForExport(departmentId, officeId, committeeId, filterParameterDto);

        var mappedCommittees = committees.Select(committee => CommitteeMapper.ToCommitteeListDto(committee, _cultureService.GetCurrentUiCulture()));

        return mappedCommittees.OrderBy(committee => committee.Description);
    }

    public async Task<IEnumerable<Committee>> GetCommitteesForGeneralElection()
    {
        var (departmentId, officeId, committeeId) = await _eiamAssignmentService.GetPermittedIds();

        return await _committeeRepository.GetAllForGeneralElection(departmentId, officeId, committeeId);
    }

    public async Task<CommitteeDetailDto> GetCommitteeDetail(Guid id)
    {
        var committee = await _committeeRepository.GetById(id);
        var dto = CommitteeMapper.ToCommitteeDetailDto(committee);
        dto.CanEdit = await _authorizationService.HasAccessToCommittee(committee);

        var generalGenderMeasure = await _generalMeasureRepository.GetGeneralGenderMeasure(committee.DepartmentId);
        dto.GeneralGenderMeasure = generalGenderMeasure?.Description;

        var generalLanguageMeasure = await _generalMeasureRepository.GetGeneralLanguageMeasure(committee.DepartmentId);
        dto.GeneralLanguageMeasure = generalLanguageMeasure?.Description;

        return dto;
    }

    public async Task<CommitteeUpdateDto> GetCommitteeForUpdate(Guid id)
    {
        var committee = await _committeeRepository.GetByIdForUpdate(id);
        var dto = CommitteeMapper.ToCommitteeUpdateDto(committee);
        dto.CanEditAll = _authorizationService.IsAdmin || (_authorizationService.IsDepartment && (committee.EndDate is null || committee.EndDate >= DateOnly.FromDateTime(DateTime.Today)) && committee.DepartmentId == (await _authorizationService.GetDepartment())?.Id);
        dto.CanEditDepartment = _authorizationService.IsAdmin;
        dto.CanEditLegalbase = await _authorizationService.HasAccessToCommittee(committee);

        return dto;
    }

    public async Task<CommitteeJustificationUpdateDto> GetCommitteeJustificationForUpdate(Guid id)
    {
        var committee = await _committeeRepository.GetByIdForUpdate(id);
        return CommitteeMapper.ToCommitteeJustificationUpdateDto(committee);
    }

    public async Task<CommitteeDetailDto> UpdateCommittee(Guid id, CommitteeUpdateDto updateDto)
    {
        _logger.LogInformation("Update committee {CommitteeId}", id);

        var existingCommittee = await _committeeRepository.GetByIdForUpdate(id, updateDto.RowVersion);

        await CheckAuthorizationForUpdate(existingCommittee);

        existingCommittee.BeginDate = updateDto.BeginDate;
        existingCommittee.EndDate = updateDto.EndDate;

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

        existingCommittee.VacanciesGeneralElection = updateDto.VacanciesInGeneralElection;

        await UpdateMembershipAdditionsInGeneralElection(updateDto, existingCommittee);

        existingCommittee.Modified = DateTime.UtcNow;
        existingCommittee.ModifiedBy = _authorizationService.GetCurrentUserName();

        await _committeeRepository.CommitChanges();

        _logger.LogInformation("Updated committee {CommitteeId}", id);

        return await GetCommitteeDetail(id);
    }

    private async Task UpdateMembershipAdditionsInGeneralElection(CommitteeUpdateDto updateDto, Committee existingCommittee)
    {
        // Store the current additions before clearing to preserve tracked entities
        var currentAdditions = existingCommittee.MembershipAdditionsInGeneralElection.ToList();

        existingCommittee.MembershipAdditionsInGeneralElection.Clear();

        if (updateDto.MembershipAdditionsInGeneralElection is { Length: > 0 })
        {
            foreach (var additionId in updateDto.MembershipAdditionsInGeneralElection)
            {
                // First try to find the entity in the already tracked entities
                var existingAddition = currentAdditions.FirstOrDefault(x => x.Id == additionId);

                if (existingAddition is not null)
                {
                    // Use the already tracked entity
                    existingCommittee.MembershipAdditionsInGeneralElection.Add(existingAddition);
                }
                else
                {
                    // This is a new addition, fetch and attach it
                    var newAddition = await _masterDataRepository.GetById<MembershipAddition>(additionId);
                    _masterDataRepository.AttachUnchanged(newAddition!);
                    existingCommittee.MembershipAdditionsInGeneralElection.Add(newAddition!);
                }
            }
        }
    }

    public async Task<CommitteeJustificationUpdateDto> UpdateCommitteeJustifications(Guid id, CommitteeJustificationUpdateDto updateDto)
    {
        _logger.LogInformation("Update justifications for committee {CommitteeId}", id);

        var existingCommittee = await _committeeRepository.GetByIdForUpdate(id, updateDto.RowVersion);

        await CheckAuthorizationForUpdate(existingCommittee);

        existingCommittee.JustificationMembers = updateDto.JustificationMembers;
        existingCommittee.JustificationGenders = updateDto.JustificationGenders;
        existingCommittee.MeasuresGenders = updateDto.MeasuresGenders;
        existingCommittee.JustificationLanguages = updateDto.JustificationLanguages;
        existingCommittee.MeasuresLanguages = updateDto.MeasuresLanguages;

        existingCommittee.Modified = DateTime.UtcNow;
        existingCommittee.ModifiedBy = _authorizationService.GetCurrentUserName();

        await _committeeRepository.CommitChanges();

        _logger.LogInformation("Updated justifications for committee {CommitteeId}", id);

        return CommitteeMapper.ToCommitteeJustificationUpdateDto(existingCommittee);
    }

    public async Task<IEnumerable<CommitteeDetailDto>> GetByDescription(string description)
    {
        var committees = (await _committeeRepository.GetByDescription(description)).Select(CommitteeMapper.ToCommitteeDetailDto);

        if (_authorizationService.IsDepartment || _authorizationService.IsOffice || _authorizationService.IsSecretariat)
        {
            var filteredCommittees = new List<CommitteeDetailDto>();

            foreach (var committee in committees)
            {
                if (await _authorizationService.IsCommitteeAssigned(committee.Id))
                {
                    filteredCommittees.Add(committee);
                }
            }

            return filteredCommittees;
        }

        return committees;
    }

    public async Task<CommitteeDetailDto> CreateCommittee(CommitteeCreateDto createDto)
    {
        if (!(_authorizationService.IsAdmin || (_authorizationService.IsDepartment && (await _authorizationService.GetDepartment())?.Id == createDto.DepartmentId)))
        {
            _logger.LogError("User is not allowed to create committee");

            throw new AuthorizationException("User is not allowed to create committee");
        }

        var committee = CommitteeMapper.FromCommitteeCreateDto(createDto, _authorizationService.GetCurrentUserName());

        committee.MembershipAdditionsInGeneralElection.Clear();
        if (createDto.MembershipAdditionsInGeneralElection is { Length: > 0 })
        {
            var additions = await _masterDataRepository.GetMembershipAdditionsByIds(createDto.MembershipAdditionsInGeneralElection);

            foreach (var addition in additions)
            {
                _masterDataRepository.AttachUnchanged(addition);
                committee.MembershipAdditionsInGeneralElection.Add(addition);
            }
        }

        var createdCommittee = await _committeeRepository.Create(committee);
        _logger.LogInformation("Created new committee {CommitteeId}", createdCommittee.Id);

        return await GetCommitteeDetail(createdCommittee.Id);
    }

    public async Task<CommitteeMembershipValidationResultDto> ValidateCommittee(Guid id, CommitteeMembershipValidationRequestDto validateDto)
    {
        var result = new CommitteeMembershipValidationResultDto { CommitteeId = validateDto.CommitteeId, PersonId = validateDto.PersonId };

        if (validateDto.EndDate != DateOnly.MinValue)
        {
            var committee = await GetCommitteeDetail(validateDto.CommitteeId);

            var allMemberships = (await _membershipRepository.GetAllByCommitteeId(id)).ToArray();
            var activeMemberships = allMemberships.Where(x => x.IsActive || x.IsFuture);
            var filteredMemberships = activeMemberships.Where(m => (m.BeginDate <= validateDto.BeginDate && m.EndDate >= validateDto.EndDate) ||
                                                                                  (m.BeginDate <= validateDto.BeginDate && m.EndDate <= validateDto.EndDate && m.EndDate > validateDto.BeginDate) ||
                                                                                  (m.BeginDate > validateDto.BeginDate && m.EndDate > validateDto.EndDate && m.BeginDate < validateDto.BeginDate) ||
                                                                                  (m.BeginDate > validateDto.BeginDate && m.EndDate < validateDto.EndDate)
            ).ToList();

            result.IsAlreadyActiveMember = !validateDto.IsUpdateMode && filteredMemberships.Any(m => m.PersonId == validateDto.PersonId);
            result.TooManyMembers = !validateDto.IsUpdateMode && filteredMemberships.Count + 1 > committee.MaximalMembers;

            var personMemberships = allMemberships.Where(x => x.PersonId == validateDto.PersonId).ToArray();
            result.CurrentTermOfOffice = MembershipTermCalculator.CalculateCurrentTermInYears(personMemberships);

            var estimatedTermInYears = MembershipTermCalculator.CalculateEstimatedTermInYears(validateDto.BeginDate, validateDto.EndDate);
            var currentTermInYearsWithoutCurrentMembership = MembershipTermCalculator.CalculateCurrentTermInYears(personMemberships.Where(x => x.Id != validateDto.CurrentMembershipId));
            result.EstimatedTermOfOffice = estimatedTermInYears + currentTermInYearsWithoutCurrentMembership;

            if ((committee.CommitteeTypeId == CommitteeType.AdministrationCommissionGuid || committee.CommitteeTypeId == CommitteeType.AuthoritiesCommissionGuid) && !validateDto.InCorrelationWithFederalDuty)
            {
                const int maximumAllowedYears = 16;
                result.MaximumDurationExceeded = result.EstimatedTermOfOffice > maximumAllowedYears;
            }

            var person = await _personRepository.GetById(validateDto.PersonId);
            result.IsFederalAssemblyAndAuthoritiesCommission = person.FederalAssembly && committee.CommitteeTypeId == CommitteeType.AuthoritiesCommissionGuid &&
                                                               person.LegislaturePeriods.Any(lp => (validateDto.BeginDate >= lp.StartDate && validateDto.BeginDate <= lp.EndDate) ||
                                                                                                   (validateDto.EndDate >= lp.StartDate && validateDto.EndDate <= lp.EndDate) ||
                                                                                                   (validateDto.BeginDate <= lp.StartDate && validateDto.EndDate >= lp.EndDate));
        }

        result.HasErrors = result.MaximumDurationExceeded || result.IsAlreadyActiveMember || result.TooManyMembers || result.IsFederalAssemblyAndAuthoritiesCommission;

        return result;
    }

    public async Task<IEnumerable<CommitteeTypeStatisticDto>> GetCommitteeTypeStatistic()
    {
        // we need to have "Leitungsorgane" and "Vertretungen des Bundes" summed together, as well as "Behördenkommissionen" and "Verwaltungskommissionen" -> APK
        var statisticDtos = new List<CommitteeTypeStatisticDto>();
        var statisticDto = new CommitteeTypeStatisticDto();

        var activeCommittees = await _committeeRepository.GetCommitteeDataForStatistics();

        var committeesWithActiveMembers = activeCommittees
        .Select(c => new Committee
        {
            Id = c.Id,
            CommitteeType = c.CommitteeType,
            CommitteeTypeId = c.CommitteeTypeId,
            ModifiedBy = c.ModifiedBy,
            Modified = c.Modified,
            CreatedBy = c.CreatedBy,
            Created = c.Created,
            TermOfOfficeDateId = c.TermOfOfficeDateId,
            Department = c.Department,
            DepartmentId = c.DepartmentId,
            IsDeleted = c.IsDeleted,
            DescriptionGerman = c.DescriptionGerman,
            DescriptionFrench = c.DescriptionFrench,
            DescriptionItalian = c.DescriptionItalian,
            DescriptionRomansh = c.DescriptionRomansh,
            Memberships = c.Memberships
                .Where(x =>
                    x.BeginDate <= DateOnly.FromDateTime(DateTime.Now) && (x.EndDate > DateOnly.FromDateTime(DateTime.Now) || (x.ElectionType != null && (x.ElectionType.Uri == ElectionType.NewElection || x.ElectionType.Uri == ElectionType.ReElection))))
                .ToList()
        })
        .ToList();

        var administrationCommissions = committeesWithActiveMembers.Where(c => c.CommitteeTypeId == CommitteeType.AdministrationCommissionGuid).ToList();
        var authoritiesCommissions = committeesWithActiveMembers.Where(c => c.CommitteeTypeId == CommitteeType.AuthoritiesCommissionGuid).ToList();
        var extraParliamentaryCommissions = administrationCommissions.Concat(authoritiesCommissions).ToList();

        var federalAgenciesCommissions = committeesWithActiveMembers.Where(c => c.CommitteeTypeId == CommitteeType.FederalAgenciesCommitteeGuid).ToList();
        var managmentCommissions = committeesWithActiveMembers.Where(c => c.CommitteeTypeId == CommitteeType.ManagementCommitteeGuid).ToList();
        var nonExtraParliamentaryCommissions = federalAgenciesCommissions.Concat(managmentCommissions).ToList();

        statisticDto = FillExtraParliamentaryCommissions(statisticDto, extraParliamentaryCommissions);

        statisticDto = FillNonExtraParliamentaryCommissions(statisticDto, nonExtraParliamentaryCommissions);

        statisticDto = FillAuthoritiesCommissions(statisticDto, authoritiesCommissions);

        statisticDto = FillAdministrationCommissions(statisticDto, administrationCommissions);

        statisticDto = FillFederalAgenciesCommissions(statisticDto, federalAgenciesCommissions);

        statisticDto = FillManagementCommissions(statisticDto, managmentCommissions);

        statisticDtos.Add(statisticDto);

        return statisticDtos;
    }

    private async Task CheckAuthorizationForUpdate(Committee committee)
    {
        if (!(_authorizationService.IsAdmin || (_authorizationService.IsDepartment && (await _authorizationService.GetDepartment())?.Id == committee.DepartmentId) ||
            ((_authorizationService.IsOffice || _authorizationService.IsSecretariat) && await _authorizationService.IsCommitteeAssigned(committee.Id))))
        {
            _logger.LogError("User is not allowed to edit committee {CommitteeId}", committee.Id);

            throw new AuthorizationException($"User is not allowed to edit committee with id: {committee.Id}");
        }

        if (committee.EndDate is not null && committee.EndDate < DateOnly.FromDateTime(DateTime.Today) && !_authorizationService.IsAdmin)
        {
            _logger.LogError("Committee {CommitteeId} can be updated by admin role only", committee.Id);

            throw new AuthorizationException($"Committee with id: {committee.Id} can be updated by admin role only");
        }
    }

    private static CommitteeTypeStatisticDto FillExtraParliamentaryCommissions(CommitteeTypeStatisticDto statisticDto, List<Committee> extraParliamentaryCommissions)
    {
        statisticDto.ExtraParliamentaryCommissionsCount = extraParliamentaryCommissions.SelectMany(c => c.Memberships).Count();
        statisticDto.ExtraParliamentaryCommissionsEdaCount = extraParliamentaryCommissions.Where(c => c.DepartmentId == Department.EdaGuid).SelectMany(c => c.Memberships).Count();
        statisticDto.ExtraParliamentaryCommissionsEdiCount = extraParliamentaryCommissions.Where(c => c.DepartmentId == Department.EdiGuid).SelectMany(c => c.Memberships).Count();
        statisticDto.ExtraParliamentaryCommissionsEjpdCount = extraParliamentaryCommissions.Where(c => c.DepartmentId == Department.EjpdGuid).SelectMany(c => c.Memberships).Count();
        statisticDto.ExtraParliamentaryCommissionsVbsCount = extraParliamentaryCommissions.Where(c => c.DepartmentId == Department.VbsGuid).SelectMany(c => c.Memberships).Count();
        statisticDto.ExtraParliamentaryCommissionsEfdCount = extraParliamentaryCommissions.Where(c => c.DepartmentId == Department.EfdGuid).SelectMany(c => c.Memberships).Count();
        statisticDto.ExtraParliamentaryCommissionsWbfCount = extraParliamentaryCommissions.Where(c => c.DepartmentId == Department.WbfGuid).SelectMany(c => c.Memberships).Count();
        statisticDto.ExtraParliamentaryCommissionsUvekCount = extraParliamentaryCommissions.Where(c => c.DepartmentId == Department.UvekGuid).SelectMany(c => c.Memberships).Count();

        // Gender by department
        statisticDto.ExtraParliamentaryCommissionsEdaFemaleCount = extraParliamentaryCommissions.Where(c => c.DepartmentId == Department.EdaGuid).SelectMany(c => c.Memberships).Count(m => m.Person!.GenderId == Gender.FemaleGuid);
        statisticDto.ExtraParliamentaryCommissionsEdiFemaleCount = extraParliamentaryCommissions.Where(c => c.DepartmentId == Department.EdiGuid).SelectMany(c => c.Memberships).Count(m => m.Person!.GenderId == Gender.FemaleGuid);
        statisticDto.ExtraParliamentaryCommissionsEjpdFemaleCount = extraParliamentaryCommissions.Where(c => c.DepartmentId == Department.EjpdGuid).SelectMany(c => c.Memberships).Count(m => m.Person!.GenderId == Gender.FemaleGuid);
        statisticDto.ExtraParliamentaryCommissionsVbsFemaleCount = extraParliamentaryCommissions.Where(c => c.DepartmentId == Department.VbsGuid).SelectMany(c => c.Memberships).Count(m => m.Person!.GenderId == Gender.FemaleGuid);
        statisticDto.ExtraParliamentaryCommissionsEfdFemaleCount = extraParliamentaryCommissions.Where(c => c.DepartmentId == Department.EfdGuid).SelectMany(c => c.Memberships).Count(m => m.Person!.GenderId == Gender.FemaleGuid);
        statisticDto.ExtraParliamentaryCommissionsWbfFemaleCount = extraParliamentaryCommissions.Where(c => c.DepartmentId == Department.WbfGuid).SelectMany(c => c.Memberships).Count(m => m.Person!.GenderId == Gender.FemaleGuid);
        statisticDto.ExtraParliamentaryCommissionsUvekFemaleCount = extraParliamentaryCommissions.Where(c => c.DepartmentId == Department.UvekGuid).SelectMany(c => c.Memberships).Count(m => m.Person!.GenderId == Gender.FemaleGuid);

        statisticDto.ExtraParliamentaryCommissionsEdaMaleCount = extraParliamentaryCommissions.Where(c => c.DepartmentId == Department.EdaGuid).SelectMany(c => c.Memberships).Count(m => m.Person!.GenderId == Gender.MaleGuid);
        statisticDto.ExtraParliamentaryCommissionsEdiMaleCount = extraParliamentaryCommissions.Where(c => c.DepartmentId == Department.EdiGuid).SelectMany(c => c.Memberships).Count(m => m.Person!.GenderId == Gender.MaleGuid);
        statisticDto.ExtraParliamentaryCommissionsEjpdMaleCount = extraParliamentaryCommissions.Where(c => c.DepartmentId == Department.EjpdGuid).SelectMany(c => c.Memberships).Count(m => m.Person!.GenderId == Gender.MaleGuid);
        statisticDto.ExtraParliamentaryCommissionsVbsMaleCount = extraParliamentaryCommissions.Where(c => c.DepartmentId == Department.VbsGuid).SelectMany(c => c.Memberships).Count(m => m.Person!.GenderId == Gender.MaleGuid);
        statisticDto.ExtraParliamentaryCommissionsEfdMaleCount = extraParliamentaryCommissions.Where(c => c.DepartmentId == Department.EfdGuid).SelectMany(c => c.Memberships).Count(m => m.Person!.GenderId == Gender.MaleGuid);
        statisticDto.ExtraParliamentaryCommissionsWbfMaleCount = extraParliamentaryCommissions.Where(c => c.DepartmentId == Department.WbfGuid).SelectMany(c => c.Memberships).Count(m => m.Person!.GenderId == Gender.MaleGuid);
        statisticDto.ExtraParliamentaryCommissionsUvekMaleCount = extraParliamentaryCommissions.Where(c => c.DepartmentId == Department.UvekGuid).SelectMany(c => c.Memberships).Count(m => m.Person!.GenderId == Gender.MaleGuid);

        // Language by department
        statisticDto.ExtraParliamentaryCommissionsEdaGermanCount = extraParliamentaryCommissions.Where(c => c.DepartmentId == Department.EdaGuid).SelectMany(c => c.Memberships).Count(m => m.Person!.LanguageId == Language.GermanGuid);
        statisticDto.ExtraParliamentaryCommissionsEdiGermanCount = extraParliamentaryCommissions.Where(c => c.DepartmentId == Department.EdiGuid).SelectMany(c => c.Memberships).Count(m => m.Person!.LanguageId == Language.GermanGuid);
        statisticDto.ExtraParliamentaryCommissionsEjpdGermanCount = extraParliamentaryCommissions.Where(c => c.DepartmentId == Department.EjpdGuid).SelectMany(c => c.Memberships).Count(m => m.Person!.LanguageId == Language.GermanGuid);
        statisticDto.ExtraParliamentaryCommissionsVbsGermanCount = extraParliamentaryCommissions.Where(c => c.DepartmentId == Department.VbsGuid).SelectMany(c => c.Memberships).Count(m => m.Person!.LanguageId == Language.GermanGuid);
        statisticDto.ExtraParliamentaryCommissionsEfdGermanCount = extraParliamentaryCommissions.Where(c => c.DepartmentId == Department.EfdGuid).SelectMany(c => c.Memberships).Count(m => m.Person!.LanguageId == Language.GermanGuid);
        statisticDto.ExtraParliamentaryCommissionsWbfGermanCount = extraParliamentaryCommissions.Where(c => c.DepartmentId == Department.WbfGuid).SelectMany(c => c.Memberships).Count(m => m.Person!.LanguageId == Language.GermanGuid);
        statisticDto.ExtraParliamentaryCommissionsUvekGermanCount = extraParliamentaryCommissions.Where(c => c.DepartmentId == Department.UvekGuid).SelectMany(c => c.Memberships).Count(m => m.Person!.LanguageId == Language.GermanGuid);

        statisticDto.ExtraParliamentaryCommissionsEdaFrenchCount = extraParliamentaryCommissions.Where(c => c.DepartmentId == Department.EdaGuid).SelectMany(c => c.Memberships).Count(m => m.Person!.LanguageId == Language.FrenchGuid);
        statisticDto.ExtraParliamentaryCommissionsEdiFrenchCount = extraParliamentaryCommissions.Where(c => c.DepartmentId == Department.EdiGuid).SelectMany(c => c.Memberships).Count(m => m.Person!.LanguageId == Language.FrenchGuid);
        statisticDto.ExtraParliamentaryCommissionsEjpdFrenchCount = extraParliamentaryCommissions.Where(c => c.DepartmentId == Department.EjpdGuid).SelectMany(c => c.Memberships).Count(m => m.Person!.LanguageId == Language.FrenchGuid);
        statisticDto.ExtraParliamentaryCommissionsVbsFrenchCount = extraParliamentaryCommissions.Where(c => c.DepartmentId == Department.VbsGuid).SelectMany(c => c.Memberships).Count(m => m.Person!.LanguageId == Language.FrenchGuid);
        statisticDto.ExtraParliamentaryCommissionsEfdFrenchCount = extraParliamentaryCommissions.Where(c => c.DepartmentId == Department.EfdGuid).SelectMany(c => c.Memberships).Count(m => m.Person!.LanguageId == Language.FrenchGuid);
        statisticDto.ExtraParliamentaryCommissionsWbfFrenchCount = extraParliamentaryCommissions.Where(c => c.DepartmentId == Department.WbfGuid).SelectMany(c => c.Memberships).Count(m => m.Person!.LanguageId == Language.FrenchGuid);
        statisticDto.ExtraParliamentaryCommissionsUvekFrenchCount = extraParliamentaryCommissions.Where(c => c.DepartmentId == Department.UvekGuid).SelectMany(c => c.Memberships).Count(m => m.Person!.LanguageId == Language.FrenchGuid);

        statisticDto.ExtraParliamentaryCommissionsEdaItalianCount = extraParliamentaryCommissions.Where(c => c.DepartmentId == Department.EdaGuid).SelectMany(c => c.Memberships).Count(m => m.Person!.LanguageId == Language.ItalianGuid);
        statisticDto.ExtraParliamentaryCommissionsEdiItalianCount = extraParliamentaryCommissions.Where(c => c.DepartmentId == Department.EdiGuid).SelectMany(c => c.Memberships).Count(m => m.Person!.LanguageId == Language.ItalianGuid);
        statisticDto.ExtraParliamentaryCommissionsEjpdItalianCount = extraParliamentaryCommissions.Where(c => c.DepartmentId == Department.EjpdGuid).SelectMany(c => c.Memberships).Count(m => m.Person!.LanguageId == Language.ItalianGuid);
        statisticDto.ExtraParliamentaryCommissionsVbsItalianCount = extraParliamentaryCommissions.Where(c => c.DepartmentId == Department.VbsGuid).SelectMany(c => c.Memberships).Count(m => m.Person!.LanguageId == Language.ItalianGuid);
        statisticDto.ExtraParliamentaryCommissionsEfdItalianCount = extraParliamentaryCommissions.Where(c => c.DepartmentId == Department.EfdGuid).SelectMany(c => c.Memberships).Count(m => m.Person!.LanguageId == Language.ItalianGuid);
        statisticDto.ExtraParliamentaryCommissionsWbfItalianCount = extraParliamentaryCommissions.Where(c => c.DepartmentId == Department.WbfGuid).SelectMany(c => c.Memberships).Count(m => m.Person!.LanguageId == Language.ItalianGuid);
        statisticDto.ExtraParliamentaryCommissionsUvekItalianCount = extraParliamentaryCommissions.Where(c => c.DepartmentId == Department.UvekGuid).SelectMany(c => c.Memberships).Count(m => m.Person!.LanguageId == Language.ItalianGuid);

        statisticDto.ExtraParliamentaryCommissionsEdaRomanshCount = extraParliamentaryCommissions.Where(c => c.DepartmentId == Department.EdaGuid).SelectMany(c => c.Memberships).Count(m => m.Person!.LanguageId == Language.RomanshGuid);
        statisticDto.ExtraParliamentaryCommissionsEdiRomanshCount = extraParliamentaryCommissions.Where(c => c.DepartmentId == Department.EdiGuid).SelectMany(c => c.Memberships).Count(m => m.Person!.LanguageId == Language.RomanshGuid);
        statisticDto.ExtraParliamentaryCommissionsEjpdRomanshCount = extraParliamentaryCommissions.Where(c => c.DepartmentId == Department.EjpdGuid).SelectMany(c => c.Memberships).Count(m => m.Person!.LanguageId == Language.RomanshGuid);
        statisticDto.ExtraParliamentaryCommissionsVbsRomanshCount = extraParliamentaryCommissions.Where(c => c.DepartmentId == Department.VbsGuid).SelectMany(c => c.Memberships).Count(m => m.Person!.LanguageId == Language.RomanshGuid);
        statisticDto.ExtraParliamentaryCommissionsEfdRomanshCount = extraParliamentaryCommissions.Where(c => c.DepartmentId == Department.EfdGuid).SelectMany(c => c.Memberships).Count(m => m.Person!.LanguageId == Language.RomanshGuid);
        statisticDto.ExtraParliamentaryCommissionsWbfRomanshCount = extraParliamentaryCommissions.Where(c => c.DepartmentId == Department.WbfGuid).SelectMany(c => c.Memberships).Count(m => m.Person!.LanguageId == Language.RomanshGuid);
        statisticDto.ExtraParliamentaryCommissionsUvekRomanshCount = extraParliamentaryCommissions.Where(c => c.DepartmentId == Department.UvekGuid).SelectMany(c => c.Memberships).Count(m => m.Person!.LanguageId == Language.RomanshGuid);

        statisticDto.ExtraParliamentaryCommissionsTotalFemaleCount = extraParliamentaryCommissions.SelectMany(c => c.Memberships).Count(m => m.Person!.GenderId == Gender.FemaleGuid);
        statisticDto.ExtraParliamentaryCommissionsTotalMaleCount = extraParliamentaryCommissions.SelectMany(c => c.Memberships).Count(m => m.Person!.GenderId == Gender.MaleGuid);
        statisticDto.ExtraParliamentaryCommissionsTotalGermanCount = extraParliamentaryCommissions.SelectMany(c => c.Memberships).Count(m => m.Person!.LanguageId == Language.GermanGuid);
        statisticDto.ExtraParliamentaryCommissionsTotalFrenchCount = extraParliamentaryCommissions.SelectMany(c => c.Memberships).Count(m => m.Person!.LanguageId == Language.FrenchGuid);
        statisticDto.ExtraParliamentaryCommissionsTotalItalianCount = extraParliamentaryCommissions.SelectMany(c => c.Memberships).Count(m => m.Person!.LanguageId == Language.ItalianGuid);
        statisticDto.ExtraParliamentaryCommissionsTotalRomanshCount = extraParliamentaryCommissions.SelectMany(c => c.Memberships).Count(m => m.Person!.LanguageId == Language.RomanshGuid);

        if (statisticDto.ExtraParliamentaryCommissionsTotalFemaleCount > 0)
        {
            statisticDto.ExtraParliamentaryCommissionsTotalFemalePercentage = Math.Round((decimal)statisticDto.ExtraParliamentaryCommissionsTotalFemaleCount / statisticDto.ExtraParliamentaryCommissionsCount * 100, 2);
        }
        if (statisticDto.ExtraParliamentaryCommissionsTotalMaleCount > 0)
        {
            statisticDto.ExtraParliamentaryCommissionsTotalMalePercentage = Math.Round((decimal)statisticDto.ExtraParliamentaryCommissionsTotalMaleCount / statisticDto.ExtraParliamentaryCommissionsCount * 100, 2);
        }
        if (statisticDto.ExtraParliamentaryCommissionsTotalGermanCount > 0)
        {
            statisticDto.ExtraParliamentaryCommissionsTotalGermanPercentage = Math.Round((decimal)statisticDto.ExtraParliamentaryCommissionsTotalGermanCount / statisticDto.ExtraParliamentaryCommissionsCount * 100, 2);
        }
        if (statisticDto.ExtraParliamentaryCommissionsTotalFrenchCount > 0)
        {
            statisticDto.ExtraParliamentaryCommissionsTotalFrenchPercentage = Math.Round((decimal)statisticDto.ExtraParliamentaryCommissionsTotalFrenchCount / statisticDto.ExtraParliamentaryCommissionsCount * 100, 2);
        }
        if (statisticDto.ExtraParliamentaryCommissionsTotalItalianCount > 0)
        {
            statisticDto.ExtraParliamentaryCommissionsTotalItalianPercentage = Math.Round((decimal)statisticDto.ExtraParliamentaryCommissionsTotalItalianCount / statisticDto.ExtraParliamentaryCommissionsCount * 100, 2);
        }
        if (statisticDto.ExtraParliamentaryCommissionsTotalRomanshCount > 0)
        {
            statisticDto.ExtraParliamentaryCommissionsTotalRomanshPercentage = Math.Round((decimal)statisticDto.ExtraParliamentaryCommissionsTotalRomanshCount / statisticDto.ExtraParliamentaryCommissionsCount * 100, 2);
        }

        // Calculate the values by department
        if (statisticDto.ExtraParliamentaryCommissionsEdaCount > 0)
        {
            statisticDto.ExtraParliamentaryCommissionsEdaFemalePercentage = Math.Round((decimal)statisticDto.ExtraParliamentaryCommissionsEdaFemaleCount / statisticDto.ExtraParliamentaryCommissionsEdaCount * 100, 2);
            statisticDto.ExtraParliamentaryCommissionsEdaMalePercentage = Math.Round((decimal)statisticDto.ExtraParliamentaryCommissionsEdaMaleCount / statisticDto.ExtraParliamentaryCommissionsEdaCount * 100, 2);
            statisticDto.ExtraParliamentaryCommissionsEdaGermanPercentage = Math.Round((decimal)statisticDto.ExtraParliamentaryCommissionsEdaGermanCount / statisticDto.ExtraParliamentaryCommissionsEdaCount * 100, 2);
            statisticDto.ExtraParliamentaryCommissionsEdaFrenchPercentage = Math.Round((decimal)statisticDto.ExtraParliamentaryCommissionsEdaFrenchCount / statisticDto.ExtraParliamentaryCommissionsEdaCount * 100, 2);
            statisticDto.ExtraParliamentaryCommissionsEdaItalianPercentage = Math.Round((decimal)statisticDto.ExtraParliamentaryCommissionsEdaItalianCount / statisticDto.ExtraParliamentaryCommissionsEdaCount * 100, 2);
            statisticDto.ExtraParliamentaryCommissionsEdaRomanshPercentage = Math.Round((decimal)statisticDto.ExtraParliamentaryCommissionsEdaRomanshCount / statisticDto.ExtraParliamentaryCommissionsEdaCount * 100, 2);
        }
        if (statisticDto.ExtraParliamentaryCommissionsEdiCount > 0)
        {
            statisticDto.ExtraParliamentaryCommissionsEdiFemalePercentage = Math.Round((decimal)statisticDto.ExtraParliamentaryCommissionsEdiFemaleCount / statisticDto.ExtraParliamentaryCommissionsEdiCount * 100, 2);
            statisticDto.ExtraParliamentaryCommissionsEdiMalePercentage = Math.Round((decimal)statisticDto.ExtraParliamentaryCommissionsEdiMaleCount / statisticDto.ExtraParliamentaryCommissionsEdiCount * 100, 2);
            statisticDto.ExtraParliamentaryCommissionsEdiGermanPercentage = Math.Round((decimal)statisticDto.ExtraParliamentaryCommissionsEdiGermanCount / statisticDto.ExtraParliamentaryCommissionsEdiCount * 100, 2);
            statisticDto.ExtraParliamentaryCommissionsEdiFrenchPercentage = Math.Round((decimal)statisticDto.ExtraParliamentaryCommissionsEdiFrenchCount / statisticDto.ExtraParliamentaryCommissionsEdiCount * 100, 2);
            statisticDto.ExtraParliamentaryCommissionsEdiItalianPercentage = Math.Round((decimal)statisticDto.ExtraParliamentaryCommissionsEdiItalianCount / statisticDto.ExtraParliamentaryCommissionsEdiCount * 100, 2);
            statisticDto.ExtraParliamentaryCommissionsEdiRomanshPercentage = Math.Round((decimal)statisticDto.ExtraParliamentaryCommissionsEdiRomanshCount / statisticDto.ExtraParliamentaryCommissionsEdiCount * 100, 2);
        }
        if (statisticDto.ExtraParliamentaryCommissionsEjpdCount > 0)
        {
            statisticDto.ExtraParliamentaryCommissionsEjpdFemalePercentage = Math.Round((decimal)statisticDto.ExtraParliamentaryCommissionsEjpdFemaleCount / statisticDto.ExtraParliamentaryCommissionsEjpdCount * 100, 2);
            statisticDto.ExtraParliamentaryCommissionsEjpdMalePercentage = Math.Round((decimal)statisticDto.ExtraParliamentaryCommissionsEjpdMaleCount / statisticDto.ExtraParliamentaryCommissionsEjpdCount * 100, 2);
            statisticDto.ExtraParliamentaryCommissionsEjpdGermanPercentage = Math.Round((decimal)statisticDto.ExtraParliamentaryCommissionsEjpdGermanCount / statisticDto.ExtraParliamentaryCommissionsEjpdCount * 100, 2);
            statisticDto.ExtraParliamentaryCommissionsEjpdFrenchPercentage = Math.Round((decimal)statisticDto.ExtraParliamentaryCommissionsEjpdFrenchCount / statisticDto.ExtraParliamentaryCommissionsEjpdCount * 100, 2);
            statisticDto.ExtraParliamentaryCommissionsEjpdItalianPercentage = Math.Round((decimal)statisticDto.ExtraParliamentaryCommissionsEjpdItalianCount / statisticDto.ExtraParliamentaryCommissionsEjpdCount * 100, 2);
            statisticDto.ExtraParliamentaryCommissionsEjpdRomanshPercentage = Math.Round((decimal)statisticDto.ExtraParliamentaryCommissionsEjpdRomanshCount / statisticDto.ExtraParliamentaryCommissionsEjpdCount * 100, 2);
        }
        if (statisticDto.ExtraParliamentaryCommissionsVbsCount > 0)
        {
            statisticDto.ExtraParliamentaryCommissionsVbsFemalePercentage = Math.Round((decimal)statisticDto.ExtraParliamentaryCommissionsVbsFemaleCount / statisticDto.ExtraParliamentaryCommissionsVbsCount * 100, 2);
            statisticDto.ExtraParliamentaryCommissionsVbsMalePercentage = Math.Round((decimal)statisticDto.ExtraParliamentaryCommissionsVbsMaleCount / statisticDto.ExtraParliamentaryCommissionsVbsCount * 100, 2);
            statisticDto.ExtraParliamentaryCommissionsVbsGermanPercentage = Math.Round((decimal)statisticDto.ExtraParliamentaryCommissionsVbsGermanCount / statisticDto.ExtraParliamentaryCommissionsVbsCount * 100, 2);
            statisticDto.ExtraParliamentaryCommissionsVbsFrenchPercentage = Math.Round((decimal)statisticDto.ExtraParliamentaryCommissionsVbsFrenchCount / statisticDto.ExtraParliamentaryCommissionsVbsCount * 100, 2);
            statisticDto.ExtraParliamentaryCommissionsVbsItalianPercentage = Math.Round((decimal)statisticDto.ExtraParliamentaryCommissionsVbsItalianCount / statisticDto.ExtraParliamentaryCommissionsVbsCount * 100, 2);
            statisticDto.ExtraParliamentaryCommissionsVbsRomanshPercentage = Math.Round((decimal)statisticDto.ExtraParliamentaryCommissionsVbsRomanshCount / statisticDto.ExtraParliamentaryCommissionsVbsCount * 100, 2);
        }
        if (statisticDto.ExtraParliamentaryCommissionsEfdCount > 0)
        {
            statisticDto.ExtraParliamentaryCommissionsEfdFemalePercentage = Math.Round((decimal)statisticDto.ExtraParliamentaryCommissionsEfdFemaleCount / statisticDto.ExtraParliamentaryCommissionsEfdCount * 100, 2);
            statisticDto.ExtraParliamentaryCommissionsEfdMalePercentage = Math.Round((decimal)statisticDto.ExtraParliamentaryCommissionsEfdMaleCount / statisticDto.ExtraParliamentaryCommissionsEfdCount * 100, 2);
            statisticDto.ExtraParliamentaryCommissionsEfdGermanPercentage = Math.Round((decimal)statisticDto.ExtraParliamentaryCommissionsEfdGermanCount / statisticDto.ExtraParliamentaryCommissionsEfdCount * 100, 2);
            statisticDto.ExtraParliamentaryCommissionsEfdFrenchPercentage = Math.Round((decimal)statisticDto.ExtraParliamentaryCommissionsEfdFrenchCount / statisticDto.ExtraParliamentaryCommissionsEfdCount * 100, 2);
            statisticDto.ExtraParliamentaryCommissionsEfdItalianPercentage = Math.Round((decimal)statisticDto.ExtraParliamentaryCommissionsEfdItalianCount / statisticDto.ExtraParliamentaryCommissionsEfdCount * 100, 2);
            statisticDto.ExtraParliamentaryCommissionsEfdRomanshPercentage = Math.Round((decimal)statisticDto.ExtraParliamentaryCommissionsEfdRomanshCount / statisticDto.ExtraParliamentaryCommissionsEfdCount * 100, 2);
        }
        if (statisticDto.ExtraParliamentaryCommissionsWbfCount > 0)
        {
            statisticDto.ExtraParliamentaryCommissionsWbfFemalePercentage = Math.Round((decimal)statisticDto.ExtraParliamentaryCommissionsWbfFemaleCount / statisticDto.ExtraParliamentaryCommissionsWbfCount * 100, 2);
            statisticDto.ExtraParliamentaryCommissionsWbfMalePercentage = Math.Round((decimal)statisticDto.ExtraParliamentaryCommissionsWbfMaleCount / statisticDto.ExtraParliamentaryCommissionsWbfCount * 100, 2);
            statisticDto.ExtraParliamentaryCommissionsWbfGermanPercentage = Math.Round((decimal)statisticDto.ExtraParliamentaryCommissionsWbfGermanCount / statisticDto.ExtraParliamentaryCommissionsWbfCount * 100, 2);
            statisticDto.ExtraParliamentaryCommissionsWbfFrenchPercentage = Math.Round((decimal)statisticDto.ExtraParliamentaryCommissionsWbfFrenchCount / statisticDto.ExtraParliamentaryCommissionsWbfCount * 100, 2);
            statisticDto.ExtraParliamentaryCommissionsWbfItalianPercentage = Math.Round((decimal)statisticDto.ExtraParliamentaryCommissionsWbfItalianCount / statisticDto.ExtraParliamentaryCommissionsWbfCount * 100, 2);
            statisticDto.ExtraParliamentaryCommissionsWbfRomanshPercentage = Math.Round((decimal)statisticDto.ExtraParliamentaryCommissionsWbfRomanshCount / statisticDto.ExtraParliamentaryCommissionsWbfCount * 100, 2);
        }
        if (statisticDto.ExtraParliamentaryCommissionsUvekCount > 0)
        {
            statisticDto.ExtraParliamentaryCommissionsUvekFemalePercentage = Math.Round((decimal)statisticDto.ExtraParliamentaryCommissionsUvekFemaleCount / statisticDto.ExtraParliamentaryCommissionsUvekCount * 100, 2);
            statisticDto.ExtraParliamentaryCommissionsUvekMalePercentage = Math.Round((decimal)statisticDto.ExtraParliamentaryCommissionsUvekMaleCount / statisticDto.ExtraParliamentaryCommissionsUvekCount * 100, 2);
            statisticDto.ExtraParliamentaryCommissionsUvekGermanPercentage = Math.Round((decimal)statisticDto.ExtraParliamentaryCommissionsUvekGermanCount / statisticDto.ExtraParliamentaryCommissionsUvekCount * 100, 2);
            statisticDto.ExtraParliamentaryCommissionsUvekFrenchPercentage = Math.Round((decimal)statisticDto.ExtraParliamentaryCommissionsUvekFrenchCount / statisticDto.ExtraParliamentaryCommissionsUvekCount * 100, 2);
            statisticDto.ExtraParliamentaryCommissionsUvekItalianPercentage = Math.Round((decimal)statisticDto.ExtraParliamentaryCommissionsUvekItalianCount / statisticDto.ExtraParliamentaryCommissionsUvekCount * 100, 2);
            statisticDto.ExtraParliamentaryCommissionsUvekRomanshPercentage = Math.Round((decimal)statisticDto.ExtraParliamentaryCommissionsUvekRomanshCount / statisticDto.ExtraParliamentaryCommissionsUvekCount * 100, 2);
        }

        return statisticDto;
    }

    private static CommitteeTypeStatisticDto FillNonExtraParliamentaryCommissions(CommitteeTypeStatisticDto statisticDto, List<Committee> nonExtraParliamentaryCommissions)
    {
        // Nicht APKs / NonExtraParliamentaryCommissions
        statisticDto.NonExtraParliamentaryCommissionsCount = nonExtraParliamentaryCommissions.SelectMany(c => c.Memberships).Count();
        statisticDto.NonExtraParliamentaryCommissionsEdaCount = nonExtraParliamentaryCommissions.Where(c => c.DepartmentId == Department.EdaGuid).SelectMany(c => c.Memberships).Count();
        statisticDto.NonExtraParliamentaryCommissionsEdiCount = nonExtraParliamentaryCommissions.Where(c => c.DepartmentId == Department.EdiGuid).SelectMany(c => c.Memberships).Count();
        statisticDto.NonExtraParliamentaryCommissionsEjpdCount = nonExtraParliamentaryCommissions.Where(c => c.DepartmentId == Department.EjpdGuid).SelectMany(c => c.Memberships).Count();
        statisticDto.NonExtraParliamentaryCommissionsVbsCount = nonExtraParliamentaryCommissions.Where(c => c.DepartmentId == Department.VbsGuid).SelectMany(c => c.Memberships).Count();
        statisticDto.NonExtraParliamentaryCommissionsEfdCount = nonExtraParliamentaryCommissions.Where(c => c.DepartmentId == Department.EfdGuid).SelectMany(c => c.Memberships).Count();
        statisticDto.NonExtraParliamentaryCommissionsWbfCount = nonExtraParliamentaryCommissions.Where(c => c.DepartmentId == Department.WbfGuid).SelectMany(c => c.Memberships).Count();
        statisticDto.NonExtraParliamentaryCommissionsUvekCount = nonExtraParliamentaryCommissions.Where(c => c.DepartmentId == Department.UvekGuid).SelectMany(c => c.Memberships).Count();

        // Gender by department
        statisticDto.NonExtraParliamentaryCommissionsEdaFemaleCount = nonExtraParliamentaryCommissions.Where(c => c.DepartmentId == Department.EdaGuid).SelectMany(c => c.Memberships).Count(m => m.Person!.GenderId == Gender.FemaleGuid);
        statisticDto.NonExtraParliamentaryCommissionsEdiFemaleCount = nonExtraParliamentaryCommissions.Where(c => c.DepartmentId == Department.EdiGuid).SelectMany(c => c.Memberships).Count(m => m.Person!.GenderId == Gender.FemaleGuid);
        statisticDto.NonExtraParliamentaryCommissionsEjpdFemaleCount = nonExtraParliamentaryCommissions.Where(c => c.DepartmentId == Department.EjpdGuid).SelectMany(c => c.Memberships).Count(m => m.Person!.GenderId == Gender.FemaleGuid);
        statisticDto.NonExtraParliamentaryCommissionsVbsFemaleCount = nonExtraParliamentaryCommissions.Where(c => c.DepartmentId == Department.VbsGuid).SelectMany(c => c.Memberships).Count(m => m.Person!.GenderId == Gender.FemaleGuid);
        statisticDto.NonExtraParliamentaryCommissionsEfdFemaleCount = nonExtraParliamentaryCommissions.Where(c => c.DepartmentId == Department.EfdGuid).SelectMany(c => c.Memberships).Count(m => m.Person!.GenderId == Gender.FemaleGuid);
        statisticDto.NonExtraParliamentaryCommissionsWbfFemaleCount = nonExtraParliamentaryCommissions.Where(c => c.DepartmentId == Department.WbfGuid).SelectMany(c => c.Memberships).Count(m => m.Person!.GenderId == Gender.FemaleGuid);
        statisticDto.NonExtraParliamentaryCommissionsUvekFemaleCount = nonExtraParliamentaryCommissions.Where(c => c.DepartmentId == Department.UvekGuid).SelectMany(c => c.Memberships).Count(m => m.Person!.GenderId == Gender.FemaleGuid);

        statisticDto.NonExtraParliamentaryCommissionsEdaMaleCount = nonExtraParliamentaryCommissions.Where(c => c.DepartmentId == Department.EdaGuid).SelectMany(c => c.Memberships).Count(m => m.Person!.GenderId == Gender.MaleGuid);
        statisticDto.NonExtraParliamentaryCommissionsEdiMaleCount = nonExtraParliamentaryCommissions.Where(c => c.DepartmentId == Department.EdiGuid).SelectMany(c => c.Memberships).Count(m => m.Person!.GenderId == Gender.MaleGuid);
        statisticDto.NonExtraParliamentaryCommissionsEjpdMaleCount = nonExtraParliamentaryCommissions.Where(c => c.DepartmentId == Department.EjpdGuid).SelectMany(c => c.Memberships).Count(m => m.Person!.GenderId == Gender.MaleGuid);
        statisticDto.NonExtraParliamentaryCommissionsVbsMaleCount = nonExtraParliamentaryCommissions.Where(c => c.DepartmentId == Department.VbsGuid).SelectMany(c => c.Memberships).Count(m => m.Person!.GenderId == Gender.MaleGuid);
        statisticDto.NonExtraParliamentaryCommissionsEfdMaleCount = nonExtraParliamentaryCommissions.Where(c => c.DepartmentId == Department.EfdGuid).SelectMany(c => c.Memberships).Count(m => m.Person!.GenderId == Gender.MaleGuid);
        statisticDto.NonExtraParliamentaryCommissionsWbfMaleCount = nonExtraParliamentaryCommissions.Where(c => c.DepartmentId == Department.WbfGuid).SelectMany(c => c.Memberships).Count(m => m.Person!.GenderId == Gender.MaleGuid);
        statisticDto.NonExtraParliamentaryCommissionsUvekMaleCount = nonExtraParliamentaryCommissions.Where(c => c.DepartmentId == Department.UvekGuid).SelectMany(c => c.Memberships).Count(m => m.Person!.GenderId == Gender.MaleGuid);

        // Language by department
        statisticDto.NonExtraParliamentaryCommissionsEdaGermanCount = nonExtraParliamentaryCommissions.Where(c => c.DepartmentId == Department.EdaGuid).SelectMany(c => c.Memberships).Count(m => m.Person!.LanguageId == Language.GermanGuid);
        statisticDto.NonExtraParliamentaryCommissionsEdiGermanCount = nonExtraParliamentaryCommissions.Where(c => c.DepartmentId == Department.EdiGuid).SelectMany(c => c.Memberships).Count(m => m.Person!.LanguageId == Language.GermanGuid);
        statisticDto.NonExtraParliamentaryCommissionsEjpdGermanCount = nonExtraParliamentaryCommissions.Where(c => c.DepartmentId == Department.EjpdGuid).SelectMany(c => c.Memberships).Count(m => m.Person!.LanguageId == Language.GermanGuid);
        statisticDto.NonExtraParliamentaryCommissionsVbsGermanCount = nonExtraParliamentaryCommissions.Where(c => c.DepartmentId == Department.VbsGuid).SelectMany(c => c.Memberships).Count(m => m.Person!.LanguageId == Language.GermanGuid);
        statisticDto.NonExtraParliamentaryCommissionsEfdGermanCount = nonExtraParliamentaryCommissions.Where(c => c.DepartmentId == Department.EfdGuid).SelectMany(c => c.Memberships).Count(m => m.Person!.LanguageId == Language.GermanGuid);
        statisticDto.NonExtraParliamentaryCommissionsWbfGermanCount = nonExtraParliamentaryCommissions.Where(c => c.DepartmentId == Department.WbfGuid).SelectMany(c => c.Memberships).Count(m => m.Person!.LanguageId == Language.GermanGuid);
        statisticDto.NonExtraParliamentaryCommissionsUvekGermanCount = nonExtraParliamentaryCommissions.Where(c => c.DepartmentId == Department.UvekGuid).SelectMany(c => c.Memberships).Count(m => m.Person!.LanguageId == Language.GermanGuid);

        statisticDto.NonExtraParliamentaryCommissionsEdaFrenchCount = nonExtraParliamentaryCommissions.Where(c => c.DepartmentId == Department.EdaGuid).SelectMany(c => c.Memberships).Count(m => m.Person!.LanguageId == Language.FrenchGuid);
        statisticDto.NonExtraParliamentaryCommissionsEdiFrenchCount = nonExtraParliamentaryCommissions.Where(c => c.DepartmentId == Department.EdiGuid).SelectMany(c => c.Memberships).Count(m => m.Person!.LanguageId == Language.FrenchGuid);
        statisticDto.NonExtraParliamentaryCommissionsEjpdFrenchCount = nonExtraParliamentaryCommissions.Where(c => c.DepartmentId == Department.EjpdGuid).SelectMany(c => c.Memberships).Count(m => m.Person!.LanguageId == Language.FrenchGuid);
        statisticDto.NonExtraParliamentaryCommissionsVbsFrenchCount = nonExtraParliamentaryCommissions.Where(c => c.DepartmentId == Department.VbsGuid).SelectMany(c => c.Memberships).Count(m => m.Person!.LanguageId == Language.FrenchGuid);
        statisticDto.NonExtraParliamentaryCommissionsEfdFrenchCount = nonExtraParliamentaryCommissions.Where(c => c.DepartmentId == Department.EfdGuid).SelectMany(c => c.Memberships).Count(m => m.Person!.LanguageId == Language.FrenchGuid);
        statisticDto.NonExtraParliamentaryCommissionsWbfFrenchCount = nonExtraParliamentaryCommissions.Where(c => c.DepartmentId == Department.WbfGuid).SelectMany(c => c.Memberships).Count(m => m.Person!.LanguageId == Language.FrenchGuid);
        statisticDto.NonExtraParliamentaryCommissionsUvekFrenchCount = nonExtraParliamentaryCommissions.Where(c => c.DepartmentId == Department.UvekGuid).SelectMany(c => c.Memberships).Count(m => m.Person!.LanguageId == Language.FrenchGuid);

        statisticDto.NonExtraParliamentaryCommissionsEdaItalianCount = nonExtraParliamentaryCommissions.Where(c => c.DepartmentId == Department.EdaGuid).SelectMany(c => c.Memberships).Count(m => m.Person!.LanguageId == Language.ItalianGuid);
        statisticDto.NonExtraParliamentaryCommissionsEdiItalianCount = nonExtraParliamentaryCommissions.Where(c => c.DepartmentId == Department.EdiGuid).SelectMany(c => c.Memberships).Count(m => m.Person!.LanguageId == Language.ItalianGuid);
        statisticDto.NonExtraParliamentaryCommissionsEjpdItalianCount = nonExtraParliamentaryCommissions.Where(c => c.DepartmentId == Department.EjpdGuid).SelectMany(c => c.Memberships).Count(m => m.Person!.LanguageId == Language.ItalianGuid);
        statisticDto.NonExtraParliamentaryCommissionsVbsItalianCount = nonExtraParliamentaryCommissions.Where(c => c.DepartmentId == Department.VbsGuid).SelectMany(c => c.Memberships).Count(m => m.Person!.LanguageId == Language.ItalianGuid);
        statisticDto.NonExtraParliamentaryCommissionsEfdItalianCount = nonExtraParliamentaryCommissions.Where(c => c.DepartmentId == Department.EfdGuid).SelectMany(c => c.Memberships).Count(m => m.Person!.LanguageId == Language.ItalianGuid);
        statisticDto.NonExtraParliamentaryCommissionsWbfItalianCount = nonExtraParliamentaryCommissions.Where(c => c.DepartmentId == Department.WbfGuid).SelectMany(c => c.Memberships).Count(m => m.Person!.LanguageId == Language.ItalianGuid);
        statisticDto.NonExtraParliamentaryCommissionsUvekItalianCount = nonExtraParliamentaryCommissions.Where(c => c.DepartmentId == Department.UvekGuid).SelectMany(c => c.Memberships).Count(m => m.Person!.LanguageId == Language.ItalianGuid);

        statisticDto.NonExtraParliamentaryCommissionsEdaRomanshCount = nonExtraParliamentaryCommissions.Where(c => c.DepartmentId == Department.EdaGuid).SelectMany(c => c.Memberships).Count(m => m.Person!.LanguageId == Language.RomanshGuid);
        statisticDto.NonExtraParliamentaryCommissionsEdiRomanshCount = nonExtraParliamentaryCommissions.Where(c => c.DepartmentId == Department.EdiGuid).SelectMany(c => c.Memberships).Count(m => m.Person!.LanguageId == Language.RomanshGuid);
        statisticDto.NonExtraParliamentaryCommissionsEjpdRomanshCount = nonExtraParliamentaryCommissions.Where(c => c.DepartmentId == Department.EjpdGuid).SelectMany(c => c.Memberships).Count(m => m.Person!.LanguageId == Language.RomanshGuid);
        statisticDto.NonExtraParliamentaryCommissionsVbsRomanshCount = nonExtraParliamentaryCommissions.Where(c => c.DepartmentId == Department.VbsGuid).SelectMany(c => c.Memberships).Count(m => m.Person!.LanguageId == Language.RomanshGuid);
        statisticDto.NonExtraParliamentaryCommissionsEfdRomanshCount = nonExtraParliamentaryCommissions.Where(c => c.DepartmentId == Department.EfdGuid).SelectMany(c => c.Memberships).Count(m => m.Person!.LanguageId == Language.RomanshGuid);
        statisticDto.NonExtraParliamentaryCommissionsWbfRomanshCount = nonExtraParliamentaryCommissions.Where(c => c.DepartmentId == Department.WbfGuid).SelectMany(c => c.Memberships).Count(m => m.Person!.LanguageId == Language.RomanshGuid);
        statisticDto.NonExtraParliamentaryCommissionsUvekRomanshCount = nonExtraParliamentaryCommissions.Where(c => c.DepartmentId == Department.UvekGuid).SelectMany(c => c.Memberships).Count(m => m.Person!.LanguageId == Language.RomanshGuid);

        statisticDto.NonExtraParliamentaryCommissionsTotalFemaleCount = nonExtraParliamentaryCommissions.SelectMany(c => c.Memberships).Count(m => m.Person!.GenderId == Gender.FemaleGuid);
        statisticDto.NonExtraParliamentaryCommissionsTotalMaleCount = nonExtraParliamentaryCommissions.SelectMany(c => c.Memberships).Count(m => m.Person!.GenderId == Gender.MaleGuid);
        statisticDto.NonExtraParliamentaryCommissionsTotalGermanCount = nonExtraParliamentaryCommissions.SelectMany(c => c.Memberships).Count(m => m.Person!.LanguageId == Language.GermanGuid);
        statisticDto.NonExtraParliamentaryCommissionsTotalFrenchCount = nonExtraParliamentaryCommissions.SelectMany(c => c.Memberships).Count(m => m.Person!.LanguageId == Language.FrenchGuid);
        statisticDto.NonExtraParliamentaryCommissionsTotalItalianCount = nonExtraParliamentaryCommissions.SelectMany(c => c.Memberships).Count(m => m.Person!.LanguageId == Language.ItalianGuid);
        statisticDto.NonExtraParliamentaryCommissionsTotalRomanshCount = nonExtraParliamentaryCommissions.SelectMany(c => c.Memberships).Count(m => m.Person!.LanguageId == Language.RomanshGuid);

        if (statisticDto.NonExtraParliamentaryCommissionsTotalFemaleCount > 0)
        {
            statisticDto.NonExtraParliamentaryCommissionsTotalFemalePercentage = Math.Round((decimal)statisticDto.NonExtraParliamentaryCommissionsTotalFemaleCount / statisticDto.NonExtraParliamentaryCommissionsCount * 100, 2);
        }
        if (statisticDto.NonExtraParliamentaryCommissionsTotalMaleCount > 0)
        {
            statisticDto.NonExtraParliamentaryCommissionsTotalMalePercentage = Math.Round((decimal)statisticDto.NonExtraParliamentaryCommissionsTotalMaleCount / statisticDto.NonExtraParliamentaryCommissionsCount * 100, 2);
        }
        if (statisticDto.NonExtraParliamentaryCommissionsTotalGermanCount > 0)
        {
            statisticDto.NonExtraParliamentaryCommissionsTotalGermanPercentage = Math.Round((decimal)statisticDto.NonExtraParliamentaryCommissionsTotalGermanCount / statisticDto.NonExtraParliamentaryCommissionsCount * 100, 2);
        }
        if (statisticDto.NonExtraParliamentaryCommissionsTotalFrenchCount > 0)
        {
            statisticDto.NonExtraParliamentaryCommissionsTotalFrenchPercentage = Math.Round((decimal)statisticDto.NonExtraParliamentaryCommissionsTotalFrenchCount / statisticDto.NonExtraParliamentaryCommissionsCount * 100, 2);
        }
        if (statisticDto.NonExtraParliamentaryCommissionsTotalItalianCount > 0)
        {
            statisticDto.NonExtraParliamentaryCommissionsTotalItalianPercentage = Math.Round((decimal)statisticDto.NonExtraParliamentaryCommissionsTotalItalianCount / statisticDto.NonExtraParliamentaryCommissionsCount * 100, 2);
        }
        if (statisticDto.NonExtraParliamentaryCommissionsTotalRomanshCount > 0)
        {
            statisticDto.NonExtraParliamentaryCommissionsTotalRomanshPercentage = Math.Round((decimal)statisticDto.NonExtraParliamentaryCommissionsTotalRomanshCount / statisticDto.NonExtraParliamentaryCommissionsCount * 100, 2);
        }

        // Calculate by department
        if (statisticDto.NonExtraParliamentaryCommissionsEdaCount > 0)
        {
            statisticDto.NonExtraParliamentaryCommissionsEdaFemalePercentage = Math.Round((decimal)statisticDto.NonExtraParliamentaryCommissionsEdaFemaleCount / statisticDto.NonExtraParliamentaryCommissionsEdaCount * 100, 2);
            statisticDto.NonExtraParliamentaryCommissionsEdaMalePercentage = Math.Round((decimal)statisticDto.NonExtraParliamentaryCommissionsEdaMaleCount / statisticDto.NonExtraParliamentaryCommissionsEdaCount * 100, 2);
            statisticDto.NonExtraParliamentaryCommissionsEdaGermanPercentage = Math.Round((decimal)statisticDto.NonExtraParliamentaryCommissionsEdaGermanCount / statisticDto.NonExtraParliamentaryCommissionsEdaCount * 100, 2);
            statisticDto.NonExtraParliamentaryCommissionsEdaFrenchPercentage = Math.Round((decimal)statisticDto.NonExtraParliamentaryCommissionsEdaFrenchCount / statisticDto.NonExtraParliamentaryCommissionsEdaCount * 100, 2);
            statisticDto.NonExtraParliamentaryCommissionsEdaItalianPercentage = Math.Round((decimal)statisticDto.NonExtraParliamentaryCommissionsEdaItalianCount / statisticDto.NonExtraParliamentaryCommissionsEdaCount * 100, 2);
            statisticDto.NonExtraParliamentaryCommissionsEdaRomanshPercentage = Math.Round((decimal)statisticDto.NonExtraParliamentaryCommissionsEdaRomanshCount / statisticDto.NonExtraParliamentaryCommissionsEdaCount * 100, 2);
        }
        if (statisticDto.NonExtraParliamentaryCommissionsEdiCount > 0)
        {
            statisticDto.NonExtraParliamentaryCommissionsEdiFemalePercentage = Math.Round((decimal)statisticDto.NonExtraParliamentaryCommissionsEdiFemaleCount / statisticDto.NonExtraParliamentaryCommissionsEdiCount * 100, 2);
            statisticDto.NonExtraParliamentaryCommissionsEdiMalePercentage = Math.Round((decimal)statisticDto.NonExtraParliamentaryCommissionsEdiMaleCount / statisticDto.NonExtraParliamentaryCommissionsEdiCount * 100, 2);
            statisticDto.NonExtraParliamentaryCommissionsEdiGermanPercentage = Math.Round((decimal)statisticDto.NonExtraParliamentaryCommissionsEdiGermanCount / statisticDto.NonExtraParliamentaryCommissionsEdiCount * 100, 2);
            statisticDto.NonExtraParliamentaryCommissionsEdiFrenchPercentage = Math.Round((decimal)statisticDto.NonExtraParliamentaryCommissionsEdiFrenchCount / statisticDto.NonExtraParliamentaryCommissionsEdiCount * 100, 2);
            statisticDto.NonExtraParliamentaryCommissionsEdiItalianPercentage = Math.Round((decimal)statisticDto.NonExtraParliamentaryCommissionsEdiItalianCount / statisticDto.NonExtraParliamentaryCommissionsEdiCount * 100, 2);
            statisticDto.NonExtraParliamentaryCommissionsEdiRomanshPercentage = Math.Round((decimal)statisticDto.NonExtraParliamentaryCommissionsEdiRomanshCount / statisticDto.NonExtraParliamentaryCommissionsEdiCount * 100, 2);
        }
        if (statisticDto.NonExtraParliamentaryCommissionsEjpdCount > 0)
        {
            statisticDto.NonExtraParliamentaryCommissionsEjpdFemalePercentage = Math.Round((decimal)statisticDto.NonExtraParliamentaryCommissionsEjpdFemaleCount / statisticDto.NonExtraParliamentaryCommissionsEjpdCount * 100, 2);
            statisticDto.NonExtraParliamentaryCommissionsEjpdMalePercentage = Math.Round((decimal)statisticDto.NonExtraParliamentaryCommissionsEjpdMaleCount / statisticDto.NonExtraParliamentaryCommissionsEjpdCount * 100, 2);
            statisticDto.NonExtraParliamentaryCommissionsEjpdGermanPercentage = Math.Round((decimal)statisticDto.NonExtraParliamentaryCommissionsEjpdGermanCount / statisticDto.NonExtraParliamentaryCommissionsEjpdCount * 100, 2);
            statisticDto.NonExtraParliamentaryCommissionsEjpdFrenchPercentage = Math.Round((decimal)statisticDto.NonExtraParliamentaryCommissionsEjpdFrenchCount / statisticDto.NonExtraParliamentaryCommissionsEjpdCount * 100, 2);
            statisticDto.NonExtraParliamentaryCommissionsEjpdItalianPercentage = Math.Round((decimal)statisticDto.NonExtraParliamentaryCommissionsEjpdItalianCount / statisticDto.NonExtraParliamentaryCommissionsEjpdCount * 100, 2);
            statisticDto.NonExtraParliamentaryCommissionsEjpdRomanshPercentage = Math.Round((decimal)statisticDto.NonExtraParliamentaryCommissionsEjpdRomanshCount / statisticDto.NonExtraParliamentaryCommissionsEjpdCount * 100, 2);
        }
        if (statisticDto.NonExtraParliamentaryCommissionsVbsCount > 0)
        {
            statisticDto.NonExtraParliamentaryCommissionsVbsFemalePercentage = Math.Round((decimal)statisticDto.NonExtraParliamentaryCommissionsVbsFemaleCount / statisticDto.NonExtraParliamentaryCommissionsVbsCount * 100, 2);
            statisticDto.NonExtraParliamentaryCommissionsVbsMalePercentage = Math.Round((decimal)statisticDto.NonExtraParliamentaryCommissionsVbsMaleCount / statisticDto.NonExtraParliamentaryCommissionsVbsCount * 100, 2);
            statisticDto.NonExtraParliamentaryCommissionsVbsGermanPercentage = Math.Round((decimal)statisticDto.NonExtraParliamentaryCommissionsVbsGermanCount / statisticDto.NonExtraParliamentaryCommissionsVbsCount * 100, 2);
            statisticDto.NonExtraParliamentaryCommissionsVbsFrenchPercentage = Math.Round((decimal)statisticDto.NonExtraParliamentaryCommissionsVbsFrenchCount / statisticDto.NonExtraParliamentaryCommissionsVbsCount * 100, 2);
            statisticDto.NonExtraParliamentaryCommissionsVbsItalianPercentage = Math.Round((decimal)statisticDto.NonExtraParliamentaryCommissionsVbsItalianCount / statisticDto.NonExtraParliamentaryCommissionsVbsCount * 100, 2);
            statisticDto.NonExtraParliamentaryCommissionsVbsRomanshPercentage = Math.Round((decimal)statisticDto.NonExtraParliamentaryCommissionsVbsRomanshCount / statisticDto.NonExtraParliamentaryCommissionsVbsCount * 100, 2);
        }
        if (statisticDto.NonExtraParliamentaryCommissionsEfdCount > 0)
        {
            statisticDto.NonExtraParliamentaryCommissionsEfdFemalePercentage = Math.Round((decimal)statisticDto.NonExtraParliamentaryCommissionsEfdFemaleCount / statisticDto.NonExtraParliamentaryCommissionsEfdCount * 100, 2);
            statisticDto.NonExtraParliamentaryCommissionsEfdMalePercentage = Math.Round((decimal)statisticDto.NonExtraParliamentaryCommissionsEfdMaleCount / statisticDto.NonExtraParliamentaryCommissionsEfdCount * 100, 2);
            statisticDto.NonExtraParliamentaryCommissionsEfdGermanPercentage = Math.Round((decimal)statisticDto.NonExtraParliamentaryCommissionsEfdGermanCount / statisticDto.NonExtraParliamentaryCommissionsEfdCount * 100, 2);
            statisticDto.NonExtraParliamentaryCommissionsEfdFrenchPercentage = Math.Round((decimal)statisticDto.NonExtraParliamentaryCommissionsEfdFrenchCount / statisticDto.NonExtraParliamentaryCommissionsEfdCount * 100, 2);
            statisticDto.NonExtraParliamentaryCommissionsEfdItalianPercentage = Math.Round((decimal)statisticDto.NonExtraParliamentaryCommissionsEfdItalianCount / statisticDto.NonExtraParliamentaryCommissionsEfdCount * 100, 2);
            statisticDto.NonExtraParliamentaryCommissionsEfdRomanshPercentage = Math.Round((decimal)statisticDto.NonExtraParliamentaryCommissionsEfdRomanshCount / statisticDto.NonExtraParliamentaryCommissionsEfdCount * 100, 2);
        }
        if (statisticDto.NonExtraParliamentaryCommissionsWbfCount > 0)
        {
            statisticDto.NonExtraParliamentaryCommissionsWbfFemalePercentage = Math.Round((decimal)statisticDto.NonExtraParliamentaryCommissionsWbfFemaleCount / statisticDto.NonExtraParliamentaryCommissionsWbfCount * 100, 2);
            statisticDto.NonExtraParliamentaryCommissionsWbfMalePercentage = Math.Round((decimal)statisticDto.NonExtraParliamentaryCommissionsWbfMaleCount / statisticDto.NonExtraParliamentaryCommissionsWbfCount * 100, 2);
            statisticDto.NonExtraParliamentaryCommissionsWbfGermanPercentage = Math.Round((decimal)statisticDto.NonExtraParliamentaryCommissionsWbfGermanCount / statisticDto.NonExtraParliamentaryCommissionsWbfCount * 100, 2);
            statisticDto.NonExtraParliamentaryCommissionsWbfFrenchPercentage = Math.Round((decimal)statisticDto.NonExtraParliamentaryCommissionsWbfFrenchCount / statisticDto.NonExtraParliamentaryCommissionsWbfCount * 100, 2);
            statisticDto.NonExtraParliamentaryCommissionsWbfItalianPercentage = Math.Round((decimal)statisticDto.NonExtraParliamentaryCommissionsWbfItalianCount / statisticDto.NonExtraParliamentaryCommissionsWbfCount * 100, 2);
            statisticDto.NonExtraParliamentaryCommissionsWbfRomanshPercentage = Math.Round((decimal)statisticDto.NonExtraParliamentaryCommissionsWbfRomanshCount / statisticDto.NonExtraParliamentaryCommissionsWbfCount * 100, 2);
        }
        if (statisticDto.NonExtraParliamentaryCommissionsUvekCount > 0)
        {
            statisticDto.NonExtraParliamentaryCommissionsUvekFemalePercentage = Math.Round((decimal)statisticDto.NonExtraParliamentaryCommissionsUvekFemaleCount / statisticDto.NonExtraParliamentaryCommissionsUvekCount * 100, 2);
            statisticDto.NonExtraParliamentaryCommissionsUvekMalePercentage = Math.Round((decimal)statisticDto.NonExtraParliamentaryCommissionsUvekMaleCount / statisticDto.NonExtraParliamentaryCommissionsUvekCount * 100, 2);
            statisticDto.NonExtraParliamentaryCommissionsUvekGermanPercentage = Math.Round((decimal)statisticDto.NonExtraParliamentaryCommissionsUvekGermanCount / statisticDto.NonExtraParliamentaryCommissionsUvekCount * 100, 2);
            statisticDto.NonExtraParliamentaryCommissionsUvekFrenchPercentage = Math.Round((decimal)statisticDto.NonExtraParliamentaryCommissionsUvekFrenchCount / statisticDto.NonExtraParliamentaryCommissionsUvekCount * 100, 2);
            statisticDto.NonExtraParliamentaryCommissionsUvekItalianPercentage = Math.Round((decimal)statisticDto.NonExtraParliamentaryCommissionsUvekItalianCount / statisticDto.NonExtraParliamentaryCommissionsUvekCount * 100, 2);
            statisticDto.NonExtraParliamentaryCommissionsUvekRomanshPercentage = Math.Round((decimal)statisticDto.NonExtraParliamentaryCommissionsUvekRomanshCount / statisticDto.NonExtraParliamentaryCommissionsUvekCount * 100, 2);
        }

        return statisticDto;
    }

    private static CommitteeTypeStatisticDto FillAuthoritiesCommissions(CommitteeTypeStatisticDto statisticDto, List<Committee> authoritiesCommissions)
    {
        // AuthoritiesCommissions/Behördenkommissionen
        statisticDto.AuthoritiesCommissionsCount = authoritiesCommissions.SelectMany(c => c.Memberships).Count();
        statisticDto.AuthoritiesCommissionsEdaCount = authoritiesCommissions.Where(c => c.DepartmentId == Department.EdaGuid).SelectMany(c => c.Memberships).Count();
        statisticDto.AuthoritiesCommissionsEdiCount = authoritiesCommissions.Where(c => c.DepartmentId == Department.EdiGuid).SelectMany(c => c.Memberships).Count();
        statisticDto.AuthoritiesCommissionsEjpdCount = authoritiesCommissions.Where(c => c.DepartmentId == Department.EjpdGuid).SelectMany(c => c.Memberships).Count();
        statisticDto.AuthoritiesCommissionsVbsCount = authoritiesCommissions.Where(c => c.DepartmentId == Department.VbsGuid).SelectMany(c => c.Memberships).Count();
        statisticDto.AuthoritiesCommissionsEfdCount = authoritiesCommissions.Where(c => c.DepartmentId == Department.EfdGuid).SelectMany(c => c.Memberships).Count();
        statisticDto.AuthoritiesCommissionsWbfCount = authoritiesCommissions.Where(c => c.DepartmentId == Department.WbfGuid).SelectMany(c => c.Memberships).Count();
        statisticDto.AuthoritiesCommissionsUvekCount = authoritiesCommissions.Where(c => c.DepartmentId == Department.UvekGuid).SelectMany(c => c.Memberships).Count();

        statisticDto.AuthoritiesCommissionsFemaleCount = authoritiesCommissions.SelectMany(c => c.Memberships).Count(m => m.Person!.GenderId == Gender.FemaleGuid);
        statisticDto.AuthoritiesCommissionsMaleCount = authoritiesCommissions.SelectMany(c => c.Memberships).Count(m => m.Person!.GenderId == Gender.MaleGuid);
        statisticDto.AuthoritiesCommissionsGermanCount = authoritiesCommissions.SelectMany(c => c.Memberships).Count(m => m.Person!.LanguageId == Language.GermanGuid);
        statisticDto.AuthoritiesCommissionsFrenchCount = authoritiesCommissions.SelectMany(c => c.Memberships).Count(m => m.Person!.LanguageId == Language.FrenchGuid);
        statisticDto.AuthoritiesCommissionsItalianCount = authoritiesCommissions.SelectMany(c => c.Memberships).Count(m => m.Person!.LanguageId == Language.ItalianGuid);
        statisticDto.AuthoritiesCommissionsRomanshCount = authoritiesCommissions.SelectMany(c => c.Memberships).Count(m => m.Person!.LanguageId == Language.RomanshGuid);

        if (statisticDto.AuthoritiesCommissionsCount > 0)
        {
            statisticDto.AuthoritiesCommissionsFemalePercentage = Math.Round((decimal)statisticDto.AuthoritiesCommissionsFemaleCount / statisticDto.AuthoritiesCommissionsCount * 100, 2);
            statisticDto.AuthoritiesCommissionsMalePercentage = Math.Round((decimal)statisticDto.AuthoritiesCommissionsMaleCount / statisticDto.AuthoritiesCommissionsCount * 100, 2);
            statisticDto.AuthoritiesCommissionsGermanPercentage = Math.Round((decimal)statisticDto.AuthoritiesCommissionsGermanCount / statisticDto.AuthoritiesCommissionsCount * 100, 2);
            statisticDto.AuthoritiesCommissionsFrenchPercentage = Math.Round((decimal)statisticDto.AuthoritiesCommissionsFrenchCount / statisticDto.AuthoritiesCommissionsCount * 100, 2);
            statisticDto.AuthoritiesCommissionsItalianPercentage = Math.Round((decimal)statisticDto.AuthoritiesCommissionsItalianCount / statisticDto.AuthoritiesCommissionsCount * 100, 2);
            statisticDto.AuthoritiesCommissionsRomanshPercentage = Math.Round((decimal)statisticDto.AuthoritiesCommissionsRomanshCount / statisticDto.AuthoritiesCommissionsCount * 100, 2);
        }

        return statisticDto;
    }

    private static CommitteeTypeStatisticDto FillAdministrationCommissions(CommitteeTypeStatisticDto statisticDto, List<Committee> administrationCommissions)
    {
        // AdministrationCommissions/Verwaltungskommissionen
        statisticDto.AdministrationCommissionsCount = administrationCommissions.SelectMany(c => c.Memberships).Count();
        statisticDto.AdministrationCommissionsEdaCount = administrationCommissions.Where(c => c.DepartmentId == Department.EdaGuid).SelectMany(c => c.Memberships).Count();
        statisticDto.AdministrationCommissionsEdiCount = administrationCommissions.Where(c => c.DepartmentId == Department.EdiGuid).SelectMany(c => c.Memberships).Count();
        statisticDto.AdministrationCommissionsEjpdCount = administrationCommissions.Where(c => c.DepartmentId == Department.EjpdGuid).SelectMany(c => c.Memberships).Count();
        statisticDto.AdministrationCommissionsVbsCount = administrationCommissions.Where(c => c.DepartmentId == Department.VbsGuid).SelectMany(c => c.Memberships).Count();
        statisticDto.AdministrationCommissionsEfdCount = administrationCommissions.Where(c => c.DepartmentId == Department.EfdGuid).SelectMany(c => c.Memberships).Count();
        statisticDto.AdministrationCommissionsWbfCount = administrationCommissions.Where(c => c.DepartmentId == Department.WbfGuid).SelectMany(c => c.Memberships).Count();
        statisticDto.AdministrationCommissionsUvekCount = administrationCommissions.Where(c => c.DepartmentId == Department.UvekGuid).SelectMany(c => c.Memberships).Count();

        statisticDto.AdministrationCommissionsFemaleCount = administrationCommissions.SelectMany(c => c.Memberships).Count(m => m.Person!.GenderId == Gender.FemaleGuid);
        statisticDto.AdministrationCommissionsMaleCount = administrationCommissions.SelectMany(c => c.Memberships).Count(m => m.Person!.GenderId == Gender.MaleGuid);
        statisticDto.AdministrationCommissionsGermanCount = administrationCommissions.SelectMany(c => c.Memberships).Count(m => m.Person!.LanguageId == Language.GermanGuid);
        statisticDto.AdministrationCommissionsFrenchCount = administrationCommissions.SelectMany(c => c.Memberships).Count(m => m.Person!.LanguageId == Language.FrenchGuid);
        statisticDto.AdministrationCommissionsItalianCount = administrationCommissions.SelectMany(c => c.Memberships).Count(m => m.Person!.LanguageId == Language.ItalianGuid);
        statisticDto.AdministrationCommissionsRomanshCount = administrationCommissions.SelectMany(c => c.Memberships).Count(m => m.Person!.LanguageId == Language.RomanshGuid);

        if (statisticDto.AdministrationCommissionsCount > 0)
        {
            statisticDto.AdministrationCommissionsFemalePercentage = Math.Round((decimal)statisticDto.AdministrationCommissionsFemaleCount / statisticDto.AdministrationCommissionsCount * 100, 2);
            statisticDto.AdministrationCommissionsMalePercentage = Math.Round((decimal)statisticDto.AdministrationCommissionsMaleCount / statisticDto.AdministrationCommissionsCount * 100, 2);
            statisticDto.AdministrationCommissionsGermanPercentage = Math.Round((decimal)statisticDto.AdministrationCommissionsGermanCount / statisticDto.AdministrationCommissionsCount * 100, 2);
            statisticDto.AdministrationCommissionsFrenchPercentage = Math.Round((decimal)statisticDto.AdministrationCommissionsFrenchCount / statisticDto.AdministrationCommissionsCount * 100, 2);
            statisticDto.AdministrationCommissionsItalianPercentage = Math.Round((decimal)statisticDto.AdministrationCommissionsItalianCount / statisticDto.AdministrationCommissionsCount * 100, 2);
            statisticDto.AdministrationCommissionsRomanshPercentage = Math.Round((decimal)statisticDto.AdministrationCommissionsRomanshCount / statisticDto.AdministrationCommissionsCount * 100, 2);
        }

        return statisticDto;
    }

    private static CommitteeTypeStatisticDto FillFederalAgenciesCommissions(CommitteeTypeStatisticDto statisticDto, List<Committee> federalAgenciesCommissions)
    {
        // FederalAgenciesCommissions/Vertretungen des Bundes
        statisticDto.FederalAgenciesCommitteesCount = federalAgenciesCommissions.SelectMany(c => c.Memberships).Count();
        statisticDto.FederalAgenciesCommitteesEdaCount = federalAgenciesCommissions.Where(c => c.DepartmentId == Department.EdaGuid).SelectMany(c => c.Memberships).Count();
        statisticDto.FederalAgenciesCommitteesEdiCount = federalAgenciesCommissions.Where(c => c.DepartmentId == Department.EdiGuid).SelectMany(c => c.Memberships).Count();
        statisticDto.FederalAgenciesCommitteesEjpdCount = federalAgenciesCommissions.Where(c => c.DepartmentId == Department.EjpdGuid).SelectMany(c => c.Memberships).Count();
        statisticDto.FederalAgenciesCommitteesVbsCount = federalAgenciesCommissions.Where(c => c.DepartmentId == Department.VbsGuid).SelectMany(c => c.Memberships).Count();
        statisticDto.FederalAgenciesCommitteesEfdCount = federalAgenciesCommissions.Where(c => c.DepartmentId == Department.EfdGuid).SelectMany(c => c.Memberships).Count();
        statisticDto.FederalAgenciesCommitteesWbfCount = federalAgenciesCommissions.Where(c => c.DepartmentId == Department.WbfGuid).SelectMany(c => c.Memberships).Count();
        statisticDto.FederalAgenciesCommitteesUvekCount = federalAgenciesCommissions.Where(c => c.DepartmentId == Department.UvekGuid).SelectMany(c => c.Memberships).Count();

        statisticDto.FederalAgenciesCommitteesFemaleCount = federalAgenciesCommissions.SelectMany(c => c.Memberships).Count(m => m.Person!.GenderId == Gender.FemaleGuid);
        statisticDto.FederalAgenciesCommitteesMaleCount = federalAgenciesCommissions.SelectMany(c => c.Memberships).Count(m => m.Person!.GenderId == Gender.MaleGuid);
        statisticDto.FederalAgenciesCommitteesGermanCount = federalAgenciesCommissions.SelectMany(c => c.Memberships).Count(m => m.Person!.LanguageId == Language.GermanGuid);
        statisticDto.FederalAgenciesCommitteesFrenchCount = federalAgenciesCommissions.SelectMany(c => c.Memberships).Count(m => m.Person!.LanguageId == Language.FrenchGuid);
        statisticDto.FederalAgenciesCommitteesItalianCount = federalAgenciesCommissions.SelectMany(c => c.Memberships).Count(m => m.Person!.LanguageId == Language.ItalianGuid);
        statisticDto.FederalAgenciesCommitteesRomanshCount = federalAgenciesCommissions.SelectMany(c => c.Memberships).Count(m => m.Person!.LanguageId == Language.RomanshGuid);

        if (statisticDto.FederalAgenciesCommitteesCount > 0)
        {
            statisticDto.FederalAgenciesCommitteesFemalePercentage = Math.Round((decimal)statisticDto.FederalAgenciesCommitteesFemaleCount / statisticDto.FederalAgenciesCommitteesCount * 100, 2);
            statisticDto.FederalAgenciesCommitteesMalePercentage = Math.Round((decimal)statisticDto.FederalAgenciesCommitteesMaleCount / statisticDto.FederalAgenciesCommitteesCount * 100, 2);
            statisticDto.FederalAgenciesCommitteesGermanPercentage = Math.Round((decimal)statisticDto.FederalAgenciesCommitteesGermanCount / statisticDto.FederalAgenciesCommitteesCount * 100, 2);
            statisticDto.FederalAgenciesCommitteesFrenchPercentage = Math.Round((decimal)statisticDto.FederalAgenciesCommitteesFrenchCount / statisticDto.FederalAgenciesCommitteesCount * 100, 2);
            statisticDto.FederalAgenciesCommitteesItalianPercentage = Math.Round((decimal)statisticDto.FederalAgenciesCommitteesItalianCount / statisticDto.FederalAgenciesCommitteesCount * 100, 2);
            statisticDto.FederalAgenciesCommitteesRomanshPercentage = Math.Round((decimal)statisticDto.FederalAgenciesCommitteesRomanshCount / statisticDto.FederalAgenciesCommitteesCount * 100, 2);
        }

        return statisticDto;
    }

    private static CommitteeTypeStatisticDto FillManagementCommissions(CommitteeTypeStatisticDto statisticDto, List<Committee> managmentCommissions)
    {
        // ManagmentCommissions/Leitungsorgane
        statisticDto.ManagementCommitteesCount = managmentCommissions.SelectMany(c => c.Memberships).Count();
        statisticDto.ManagementCommitteesEdaCount = managmentCommissions.Where(c => c.DepartmentId == Department.EdaGuid).SelectMany(c => c.Memberships).Count();
        statisticDto.ManagementCommitteesEdiCount = managmentCommissions.Where(c => c.DepartmentId == Department.EdiGuid).SelectMany(c => c.Memberships).Count();
        statisticDto.ManagementCommitteesEjpdCount = managmentCommissions.Where(c => c.DepartmentId == Department.EjpdGuid).SelectMany(c => c.Memberships).Count();
        statisticDto.ManagementCommitteesVbsCount = managmentCommissions.Where(c => c.DepartmentId == Department.VbsGuid).SelectMany(c => c.Memberships).Count();
        statisticDto.ManagementCommitteesEfdCount = managmentCommissions.Where(c => c.DepartmentId == Department.EfdGuid).SelectMany(c => c.Memberships).Count();
        statisticDto.ManagementCommitteesWbfCount = managmentCommissions.Where(c => c.DepartmentId == Department.WbfGuid).SelectMany(c => c.Memberships).Count();
        statisticDto.ManagementCommitteesUvekCount = managmentCommissions.Where(c => c.DepartmentId == Department.UvekGuid).SelectMany(c => c.Memberships).Count();

        statisticDto.ManagementCommitteesFemaleCount = managmentCommissions.SelectMany(c => c.Memberships).Count(m => m.Person!.GenderId == Gender.FemaleGuid);
        statisticDto.ManagementCommitteesMaleCount = managmentCommissions.SelectMany(c => c.Memberships).Count(m => m.Person!.GenderId == Gender.MaleGuid);
        statisticDto.ManagementCommitteesGermanCount = managmentCommissions.SelectMany(c => c.Memberships).Count(m => m.Person!.LanguageId == Language.GermanGuid);
        statisticDto.ManagementCommitteesFrenchCount = managmentCommissions.SelectMany(c => c.Memberships).Count(m => m.Person!.LanguageId == Language.FrenchGuid);
        statisticDto.ManagementCommitteesItalianCount = managmentCommissions.SelectMany(c => c.Memberships).Count(m => m.Person!.LanguageId == Language.ItalianGuid);
        statisticDto.ManagementCommitteesRomanshCount = managmentCommissions.SelectMany(c => c.Memberships).Count(m => m.Person!.LanguageId == Language.RomanshGuid);

        if (statisticDto.ManagementCommitteesCount > 0)
        {
            statisticDto.ManagementCommitteesFemalePercentage = Math.Round((decimal)statisticDto.ManagementCommitteesFemaleCount / statisticDto.ManagementCommitteesCount * 100, 2);
            statisticDto.ManagementCommitteesMalePercentage = Math.Round((decimal)statisticDto.ManagementCommitteesMaleCount / statisticDto.ManagementCommitteesCount * 100, 2);
            statisticDto.ManagementCommitteesGermanPercentage = Math.Round((decimal)statisticDto.ManagementCommitteesGermanCount / statisticDto.ManagementCommitteesCount * 100, 2);
            statisticDto.ManagementCommitteesFrenchPercentage = Math.Round((decimal)statisticDto.ManagementCommitteesFrenchCount / statisticDto.ManagementCommitteesCount * 100, 2);
            statisticDto.ManagementCommitteesItalianPercentage = Math.Round((decimal)statisticDto.ManagementCommitteesItalianCount / statisticDto.ManagementCommitteesCount * 100, 2);
            statisticDto.ManagementCommitteesRomanshPercentage = Math.Round((decimal)statisticDto.ManagementCommitteesRomanshCount / statisticDto.ManagementCommitteesCount * 100, 2);
        }

        return statisticDto;
    }
}
