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

    public async Task<IEnumerable<CommitteeListDto>> GetCommitteeListForExport()
    {
        var (departmentId, officeId, committeeId) = await _eiamAssignmentService.GetPermittedIds();

        var committees = await _committeeRepository.GetAllForExport(departmentId, officeId, committeeId);

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

    public async Task<IEnumerable<CommitteeTypeDepartmentStatisticDto>> GetCommitteeTypeStatistic()
    {
        var list = new List<CommitteeTypeDepartmentStatisticDto>();

        var activeCommittees = await _committeeRepository.GetCommitteeDataForStatistics();

        var committeesWithActiveMembers = activeCommittees
        .Select(c => new Committee
        {
            Id = c.Id,
            CommitteeType = c.CommitteeType,
            ModifiedBy = c.ModifiedBy,
            Modified = c.Modified,
            CreatedBy = c.CreatedBy,
            Created = c.Created,
            TermOfOfficeDateId = c.TermOfOfficeDateId,
            Department = c.Department,
            IsDeleted = c.IsDeleted,
            DescriptionGerman = c.DescriptionGerman,
            DescriptionFrench = c.DescriptionFrench,
            DescriptionItalian = c.DescriptionItalian,
            DescriptionRomansh = c.DescriptionRomansh,
            Memberships = c.Memberships
                .Where(x =>
                    x.BeginDate <= DateOnly.FromDateTime(DateTime.Now) &&  x.EndDate > DateOnly.FromDateTime(DateTime.Now))
                .ToList()
        })
        .ToList();

        var groupedCommittees = activeCommittees.GroupBy(c => new { c.CommitteeTypeId, c.DepartmentId }).ToList();

        foreach (var committeeGroup in groupedCommittees)
        {
            var firstCommittee = committeeGroup.First();

            var committeeTypeId = committeeGroup.Key.CommitteeTypeId;
            var departmentId = committeeGroup.Key.DepartmentId;

            var dto = new CommitteeTypeDepartmentStatisticDto
            {
                CommitteeTypeId = committeeTypeId,
                CommitteeTypeOdgId = firstCommittee.CommitteeType!.OgdId,
                DepartmentOdgId = firstCommittee.Department!.OgdId,
                MembershipCount = committeeGroup.Count(),
                FemaleCount = firstCommittee.Memberships.Select (m => m.Memberships!.Person!.GenderId == Gender.FemaleGuid),

                CommitteeTypeCount = canton.Id,
                CantonOgdId = canton.OgdId,
            };
            dtos.Add(dto);
        }




        //public required Guid CommitteeTypeId { get; init; }
        //public required int CommitteeTypeOdgId { get; init; }
        //public required int DepartmentOdgId { get; init; }
        //public required int CommitteeTypeCount { get; init; }
        //public int FemaleCount { get; set; }
        //public decimal FemalePercentage { get; set; }
        //public int MaleCount { get; set; }
        //public decimal MalePercentage { get; set; }
        //public int GermanCount { get; set; }
        //public decimal GermanPercentage { get; set; }
        //public int FrenchCount { get; set; }
        //public decimal FrenchPercentage { get; set; }
        //public int ItalianCount { get; set; }
        //public decimal ItalianPercentage { get; set; }
        //public int RomanshCount { get; set; }
        //public decimal RomanshPercentage { get; set; }
        //public int FederalDutyCount { get; set; }
        //public int FederalAssemblyCount { get; set; }

        //var groupedMemberships = filteredMemberships.GroupBy(m => new { m.CommitteeId, m.Committee!.OgdId });

        return list;
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
}
