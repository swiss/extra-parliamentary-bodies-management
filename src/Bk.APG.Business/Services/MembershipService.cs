using Bk.APG.Business.Dtos;
using Bk.APG.Business.Extensions;
using Bk.APG.Business.Mapper;
using Bk.APG.Business.Models;
using Bk.APG.Business.Repositories;
using Bk.APG.CrossCutting.Exception;
using Microsoft.Extensions.Logging;

namespace Bk.APG.Business.Services;

public class MembershipService : IMembershipService
{
    private readonly IMembershipRepository _membershipRepository;
    private readonly ICommitteeRepository _committeeRepository;
    private readonly IAuthorizationService _authorizationService;
    private readonly ICultureService _cultureService;
    private readonly IGeneralElectionService _generalElectionService;
    private readonly IGeneralElectionCommitteeService _generalElectionCommitteeService;
    private readonly ITermOfOfficeDateService _termOfOfficeDateService;
    private readonly IMasterDataRepository _masterDataRepository;
    private readonly IMembershipMirrorService _membershipMirrorService;
    private readonly ILogger<MembershipService> _logger;

    public MembershipService(IMembershipRepository membershipRepository, ICommitteeRepository committeeRepository, IAuthorizationService authorizationService,
        ICultureService cultureService, IGeneralElectionService generalElectionService, IGeneralElectionCommitteeService generalElectionCommitteeService, ITermOfOfficeDateService termOfOfficeDateService,
        IMasterDataRepository masterDataRepository, IMembershipMirrorService membershipMirrorService, ILogger<MembershipService> logger)
    {
        _membershipRepository = membershipRepository;
        _committeeRepository = committeeRepository;
        _authorizationService = authorizationService;
        _cultureService = cultureService;
        _generalElectionService = generalElectionService;
        _generalElectionCommitteeService = generalElectionCommitteeService;
        _termOfOfficeDateService = termOfOfficeDateService;
        _masterDataRepository = masterDataRepository;
        _membershipMirrorService = membershipMirrorService;
        _logger = logger;
    }

    public async Task<IEnumerable<PersonMembershipDto>> GetAllByPersonId(Guid personId)
    {
        var memberships = await _membershipRepository.GetAllByPersonId(personId);
        return memberships.Select(MembershipMapper.ToPersonMembershipDto);
    }

    private async Task<MembershipDetailDto> GetMembershipDetail(Guid id)
    {
        var membership = await _membershipRepository.GetById(id);
        return MembershipMapper.ToMembershipDetailDto(membership);
    }

    public async Task<MembershipListDto> GetAllByCommitteeId(Guid committeeId)
    {
        var memberships = (await _membershipRepository.GetAllByCommitteeId(committeeId)).ToArray();
        var committee = await _committeeRepository.GetById(committeeId);

        var activeMemberships = memberships.Where(m => m.IsActive || m.IsFuture).Select(MembershipMapper.ToCommitteeMemberDto);
        var inactiveMemberships = memberships.Where(m => m is { IsActive: false, IsFuture: false }).Select(MembershipMapper.ToCommitteeMemberDto);

        return new MembershipListDto
        {
            CommitteeQuotas = committee.GetQuotas(),
            ActiveMemberships = activeMemberships,
            InactiveMemberships = inactiveMemberships
        };
    }

    public async Task<IEnumerable<Membership>> GetAllActiveByCommitteeId(Guid committeeId)
    {
        return await _membershipRepository.GetAllActiveMembershipsForCommittee(committeeId);
    }

    public async Task<MembershipDetailDto> CreateMembership(MembershipCreateDto createDto)
    {
        _logger.LogInformation("Create membership for person {PersonId} in committee {CommitteeId}", createDto.PersonId, createDto.CommitteeId);

        var isGeneralElectionRunning = await _termOfOfficeDateService.CheckForRunningGeneralElection();

        var membership = MembershipMapper.FromMembershipCreateDto(createDto, _authorizationService.GetCurrentUserName());

        var newMembership = await _membershipRepository.Create(membership);

        var membershipWithPerson = await _membershipRepository.GetById(newMembership.Id);

        var committee = await _committeeRepository.GetById(membershipWithPerson.CommitteeId);

        // TODO: Code smell – MembershipService is handling GEW workflow logic.
        // Extract GEW-related behavior into an application/orchestrator service.
        if (isGeneralElectionRunning && await IsMembershipForGeneralElectionCommittee(membershipWithPerson.CommitteeId))
        {
            if (committee.GeneralElectionCommittees.FirstOrDefault()?.CandidateListStateId == CandidateListState.Validated)
            {
                await _generalElectionCommitteeService.InvalidateMembershipCandidateList(membershipWithPerson.CommitteeId);
            }
            else
            {
                await _generalElectionService.CreateNewMembershipCandidate(membershipWithPerson, true);
                _logger.LogInformation("Created membership candidate for general election for membership id {MembershipId}", newMembership.Id);

                if (committee.GeneralElectionCommittees.FirstOrDefault()?.CandidateListStateId == CandidateListState.ReadyForFederalCouncilProposalForwarded)
                {
                    await _generalElectionCommitteeService.SetFederalCouncilProposalToDirty(membershipWithPerson.CommitteeId);
                }
            }
        }

        _logger.LogInformation("Created membership with id {MembershipId}", newMembership.Id);

        return await GetMembershipDetail(membership.Id);
    }

    public async Task<MembershipUpdateDto> GetMembershipForUpdate(Guid id)
    {
        var membership = await _membershipRepository.GetById(id);

        var mappedMembership = MembershipMapper.ToMembershipUpdateDto(membership, _cultureService);

        mappedMembership.CanEdit = await CanEditMembership(membership);
        mappedMembership.CanEditBeginDate = (mappedMembership.CanEdit && membership.BeginDate > DateOnly.FromDateTime(DateTime.Now)) || _authorizationService.IsAdmin;
        mappedMembership.CanDelete = (mappedMembership.CanEdit && membership.BeginDate > DateOnly.FromDateTime(DateTime.Now)) || _authorizationService.IsAdmin;

        return mappedMembership;
    }

    public async Task<IEnumerable<MembershipCantonStatisticDto>> GetMembershipsForCantonStatistic(IEnumerable<Membership> memberships)
    {
        var cantons = await _masterDataRepository.GetCantons();
        var dtos = new List<MembershipCantonStatisticDto>();
        var currentYear = DateTime.Today.Year;

        foreach (var canton in cantons)
        {
            var filteredMemberships = memberships.Where(m => m.Person!.CorrespondenceAddress!.CantonId == canton.Id);

            var groupedMemberships = filteredMemberships.GroupBy(m => new { m.CommitteeId, m.Committee!.OgdId });

            foreach (var committeeGroup in groupedMemberships)
            {
                var committeeId = committeeGroup.Key.CommitteeId;
                var committeeOgdId = committeeGroup.Key.OgdId;

                var dto = new MembershipCantonStatisticDto
                {
                    CommitteeId = committeeId,
                    CommitteeOgdId = committeeOgdId,
                    CantonCount = committeeGroup.Count(),
                    CantonId = canton.Id,
                    CantonUri = canton.Uri,
                    CantonOgdId = canton.OgdId,
                };
                dtos.Add(dto);
            }
        }
        return dtos;
    }

    public IEnumerable<MembershipGenderLanguageStatisticDto> GetMembershipsForGenderLanguageStatistic(IEnumerable<Membership> memberships)
    {
        var dtos = new List<MembershipGenderLanguageStatisticDto>();
        var currentYear = DateTime.Today.Year;

        var groupedMemberships = memberships.GroupBy(m => new { m.CommitteeId, m.Committee!.OgdId });

        foreach (var committeeGroup in groupedMemberships)
        {
            var first = committeeGroup.First();

            var committeeId = committeeGroup.Key.CommitteeId;
            var committeeOgdId = committeeGroup.Key.OgdId;

            var dto = new MembershipGenderLanguageStatisticDto
            {
                CommitteeId = committeeId,
                CommitteeOgdId = committeeOgdId,
                CommitteeTypeId = first.Committee!.CommitteeType!.Id,
                CommitteeTypeOgdId = first.Committee!.CommitteeType!.OgdId,
                Department = first.Committee!.Department!.TextDe,
                DepartmentUri = first.Committee!.Department!.Uri,
                Count = committeeGroup.Count(),
                FemaleCount = committeeGroup.Count(m => m.Person!.GenderId == Gender.FemaleGuid),
                MaleCount = committeeGroup.Count(m => m.Person!.GenderId == Gender.MaleGuid),
                GermanCount = committeeGroup.Count(m => m.Person!.LanguageId == Language.GermanGuid),
                FrenchCount = committeeGroup.Count(m => m.Person!.LanguageId == Language.FrenchGuid),
                ItalianCount = committeeGroup.Count(m => m.Person!.LanguageId == Language.ItalianGuid),
                RomanshCount = committeeGroup.Count(m => m.Person!.LanguageId == Language.RomanshGuid),
                FederalDutyCount = committeeGroup.Count(m => m.Person!.FederalDuty),
                FederalAssemblyCount = committeeGroup.Count(m => m.Person!.FederalAssembly),
                UpTo30Count = committeeGroup.Count(m => currentYear - m.Person!.BirthYear < 31),
                From31To40Count = committeeGroup.Count(m => currentYear - m.Person!.BirthYear is >= 31 and <= 40),
                From41To50Count = committeeGroup.Count(m => currentYear - m.Person!.BirthYear is >= 41 and <= 50),
                From51To60Count = committeeGroup.Count(m => currentYear - m.Person!.BirthYear is >= 51 and <= 60),
                From61To70Count = committeeGroup.Count(m => currentYear - m.Person!.BirthYear is >= 61 and <= 70),
                Over70Count = committeeGroup.Count(m => currentYear - m.Person!.BirthYear > 70),
            };

            if (dto.Count > 0)
            {
                dto.FemalePercentage = Math.Round((decimal)dto.FemaleCount / dto.Count * 100, 2);
                dto.MalePercentage = Math.Round((decimal)dto.MaleCount / dto.Count * 100, 2);
                dto.GermanPercentage = Math.Round((decimal)dto.GermanCount / dto.Count * 100, 2);
                dto.FrenchPercentage = Math.Round((decimal)dto.FrenchCount / dto.Count * 100, 2);
                dto.ItalianPercentage = Math.Round((decimal)dto.ItalianCount / dto.Count * 100, 2);
                dto.RomanshPercentage = Math.Round((decimal)dto.RomanshCount / dto.Count * 100, 2);
                dto.UpTo30Percentage = Math.Round((decimal)dto.UpTo30Count / dto.Count * 100, 2);
                dto.From31To40Percentage = Math.Round((decimal)dto.From31To40Count / dto.Count * 100, 2);
                dto.From41To50Percentage = Math.Round((decimal)dto.From41To50Count / dto.Count * 100, 2);
                dto.From51To60Percentage = Math.Round((decimal)dto.From51To60Count / dto.Count * 100, 2);
                dto.From61To70Percentage = Math.Round((decimal)dto.From61To70Count / dto.Count * 100, 2);
                dto.Over70Percentage = Math.Round((decimal)dto.Over70Count / dto.Count * 100, 2);
            }
            dtos.Add(dto);
        }
        return dtos;
    }

    public IEnumerable<MembershipGenderLanguageStatisticDto> GetMembershipsForCommitteeTypeAndDepartmentGenderLanguageStatistic(IEnumerable<Membership> memberships)
    {
        var dtos = new List<MembershipGenderLanguageStatisticDto>();
        var currentYear = DateTime.Today.Year;

        var groupedMemberships = memberships.GroupBy(m => m.Committee!.CommitteeTypeId);

        foreach (var group in groupedMemberships)
        {
            var first = group.First();

            var committeeTypeId = group.Key;

            var dto = new MembershipGenderLanguageStatisticDto
            {
                CommitteeId = null,
                CommitteeOgdId = null,
                CommitteeTypeId = committeeTypeId,
                CommitteeTypeOgdId = first.Committee!.CommitteeType!.OgdId,
                Department = null,
                DepartmentUri = null,
                Count = group.Count(),
                FemaleCount = group.Count(m => m.Person!.GenderId == Gender.FemaleGuid),
                MaleCount = group.Count(m => m.Person!.GenderId == Gender.MaleGuid),
                GermanCount = group.Count(m => m.Person!.LanguageId == Language.GermanGuid),
                FrenchCount = group.Count(m => m.Person!.LanguageId == Language.FrenchGuid),
                ItalianCount = group.Count(m => m.Person!.LanguageId == Language.ItalianGuid),
                RomanshCount = group.Count(m => m.Person!.LanguageId == Language.RomanshGuid),
                FederalDutyCount = group.Count(m => m.Person!.FederalDuty),
                FederalAssemblyCount = group.Count(m => m.Person!.FederalAssembly),
                UpTo30Count = group.Count(m => currentYear - m.Person!.BirthYear < 31),
                From31To40Count = group.Count(m => currentYear - m.Person!.BirthYear is >= 31 and <= 40),
                From41To50Count = group.Count(m => currentYear - m.Person!.BirthYear is >= 41 and <= 50),
                From51To60Count = group.Count(m => currentYear - m.Person!.BirthYear is >= 51 and <= 60),
                From61To70Count = group.Count(m => currentYear - m.Person!.BirthYear is >= 61 and <= 70),
                Over70Count = group.Count(m => currentYear - m.Person!.BirthYear > 70),
            };

            if (dto.Count > 0)
            {
                dto.FemalePercentage = Math.Round((decimal)dto.FemaleCount / dto.Count * 100, 2);
                dto.MalePercentage = Math.Round((decimal)dto.MaleCount / dto.Count * 100, 2);
                dto.GermanPercentage = Math.Round((decimal)dto.GermanCount / dto.Count * 100, 2);
                dto.FrenchPercentage = Math.Round((decimal)dto.FrenchCount / dto.Count * 100, 2);
                dto.ItalianPercentage = Math.Round((decimal)dto.ItalianCount / dto.Count * 100, 2);
                dto.RomanshPercentage = Math.Round((decimal)dto.RomanshCount / dto.Count * 100, 2);
                dto.UpTo30Percentage = Math.Round((decimal)dto.UpTo30Count / dto.Count * 100, 2);
                dto.From31To40Percentage = Math.Round((decimal)dto.From31To40Count / dto.Count * 100, 2);
                dto.From41To50Percentage = Math.Round((decimal)dto.From41To50Count / dto.Count * 100, 2);
                dto.From51To60Percentage = Math.Round((decimal)dto.From51To60Count / dto.Count * 100, 2);
                dto.From61To70Percentage = Math.Round((decimal)dto.From61To70Count / dto.Count * 100, 2);
                dto.Over70Percentage = Math.Round((decimal)dto.Over70Count / dto.Count * 100, 2);
            }
            dtos.Add(dto);
        }

        return dtos;
    }

    public IEnumerable<MembershipGenderLanguageStatisticDto> GetExtraAndNonExtraParliamentaryCommitteesStatistic(IEnumerable<Membership> memberships)
    {
        var dtos = new List<MembershipGenderLanguageStatisticDto>();
        var currentYear = DateTime.Today.Year;

        // As these are no official committeeTypes, we have to use self created values
        var extraParliamentaryCommissionId = CommitteeType.ExtraParliamentaryCommitteeGuid;
        var nonExtraParliamentaryCommissionId = CommitteeType.NonExtraParliamentaryCommitteeGuid;
        var extraParliamentaryCommissionOgdId = 888;
        var nonExtraParliamentaryCommissionOgdId = 999;
        var total = "TOTAL";
        var extraParliamentaryCommissionTotalDto = new MembershipGenderLanguageStatisticDto()
        {
            CommitteeTypeId = extraParliamentaryCommissionId,
            CommitteeTypeOgdId = extraParliamentaryCommissionOgdId,
            Department = total,
        };
        var nonExtraParliamentaryCommissionTotalDto = new MembershipGenderLanguageStatisticDto()
        {
            CommitteeTypeId = nonExtraParliamentaryCommissionId,
            CommitteeTypeOgdId = nonExtraParliamentaryCommissionOgdId,
            Department = total,
        };

        var groupedByCommitteeTypes = memberships.GroupBy(m => new
        {
            m.Committee!.DepartmentId,
            CommitteeTypeGroup =
                m.Committee.CommitteeTypeId == CommitteeType.AdministrationCommissionGuid ||
                m.Committee.CommitteeTypeId == CommitteeType.AuthoritiesCommissionGuid
                ? extraParliamentaryCommissionId
                : m.Committee.CommitteeTypeId == CommitteeType.FederalAgenciesCommitteeGuid ||
                m.Committee.CommitteeTypeId == CommitteeType.ManagementCommitteeGuid ?
                nonExtraParliamentaryCommissionId :
                m.Committee.CommitteeTypeId
        });

        foreach (var group in groupedByCommitteeTypes)
        {
            var first = group.First();

            var committeeTypeId = group.Key.CommitteeTypeGroup;

            var dto = new MembershipGenderLanguageStatisticDto
            {
                CommitteeId = null,
                CommitteeOgdId = null,
                CommitteeTypeId = committeeTypeId,
                CommitteeTypeOgdId = committeeTypeId == extraParliamentaryCommissionId ? extraParliamentaryCommissionOgdId : nonExtraParliamentaryCommissionOgdId,
                Department = first.Committee!.Department!.TextDe,
                DepartmentUri = first.Committee!.Department!.Uri,
                Count = group.Count(),
                FemaleCount = group.Count(m => m.Person!.GenderId == Gender.FemaleGuid),
                MaleCount = group.Count(m => m.Person!.GenderId == Gender.MaleGuid),
                GermanCount = group.Count(m => m.Person!.LanguageId == Language.GermanGuid),
                FrenchCount = group.Count(m => m.Person!.LanguageId == Language.FrenchGuid),
                ItalianCount = group.Count(m => m.Person!.LanguageId == Language.ItalianGuid),
                RomanshCount = group.Count(m => m.Person!.LanguageId == Language.RomanshGuid),
                FederalDutyCount = group.Count(m => m.Person!.FederalDuty),
                FederalAssemblyCount = group.Count(m => m.Person!.FederalAssembly),
                UpTo30Count = group.Count(m => currentYear - m.Person!.BirthYear < 31),
                From31To40Count = group.Count(m => currentYear - m.Person!.BirthYear is >= 31 and <= 40),
                From41To50Count = group.Count(m => currentYear - m.Person!.BirthYear is >= 41 and <= 50),
                From51To60Count = group.Count(m => currentYear - m.Person!.BirthYear is >= 51 and <= 60),
                From61To70Count = group.Count(m => currentYear - m.Person!.BirthYear is >= 61 and <= 70),
                Over70Count = group.Count(m => currentYear - m.Person!.BirthYear > 70),
            };

            if (dto.Count > 0)
            {
                dto.FemalePercentage = Math.Round((decimal)dto.FemaleCount / dto.Count * 100, 2);
                dto.MalePercentage = Math.Round((decimal)dto.MaleCount / dto.Count * 100, 2);
                dto.GermanPercentage = Math.Round((decimal)dto.GermanCount / dto.Count * 100, 2);
                dto.FrenchPercentage = Math.Round((decimal)dto.FrenchCount / dto.Count * 100, 2);
                dto.ItalianPercentage = Math.Round((decimal)dto.ItalianCount / dto.Count * 100, 2);
                dto.RomanshPercentage = Math.Round((decimal)dto.RomanshCount / dto.Count * 100, 2);
                dto.UpTo30Percentage = Math.Round((decimal)dto.UpTo30Count / dto.Count * 100, 2);
                dto.From31To40Percentage = Math.Round((decimal)dto.From31To40Count / dto.Count * 100, 2);
                dto.From41To50Percentage = Math.Round((decimal)dto.From41To50Count / dto.Count * 100, 2);
                dto.From51To60Percentage = Math.Round((decimal)dto.From51To60Count / dto.Count * 100, 2);
                dto.From61To70Percentage = Math.Round((decimal)dto.From61To70Count / dto.Count * 100, 2);
                dto.Over70Percentage = Math.Round((decimal)dto.Over70Count / dto.Count * 100, 2);
            }

            // we have to add the current values to the total dtos
            if (committeeTypeId == extraParliamentaryCommissionId)
            {
                extraParliamentaryCommissionTotalDto = AddCurrentDtoToTotalDto(dto, extraParliamentaryCommissionTotalDto);
            }
            else if (committeeTypeId == nonExtraParliamentaryCommissionId)
            {
                nonExtraParliamentaryCommissionTotalDto = AddCurrentDtoToTotalDto(dto, nonExtraParliamentaryCommissionTotalDto);
            }

            dtos.Add(dto);
        }

        dtos.Add(extraParliamentaryCommissionTotalDto);
        dtos.Add(nonExtraParliamentaryCommissionTotalDto);

        return dtos;
    }

    private static MembershipGenderLanguageStatisticDto AddCurrentDtoToTotalDto(MembershipGenderLanguageStatisticDto dto, MembershipGenderLanguageStatisticDto totalDto)
    {
        totalDto.Count = totalDto.Count += dto.Count;
        totalDto.FemaleCount = totalDto.FemaleCount += dto.FemaleCount;
        totalDto.MaleCount = totalDto.MaleCount += dto.MaleCount;
        totalDto.GermanCount = totalDto.GermanCount += dto.GermanCount;
        totalDto.FrenchCount = totalDto.FrenchCount += dto.FrenchCount;
        totalDto.ItalianCount = totalDto.ItalianCount += dto.ItalianCount;
        totalDto.RomanshCount = totalDto.RomanshCount += dto.RomanshCount;
        totalDto.FederalDutyCount = totalDto.FederalDutyCount += dto.FederalDutyCount;
        totalDto.FederalAssemblyCount = totalDto.FederalAssemblyCount += dto.FederalAssemblyCount;
        totalDto.UpTo30Count = totalDto.UpTo30Count += dto.UpTo30Count;
        totalDto.From31To40Count = totalDto.From31To40Count += dto.From31To40Count;
        totalDto.From41To50Count = totalDto.From41To50Count += dto.From41To50Count;
        totalDto.From51To60Count = totalDto.From51To60Count += dto.From51To60Count;
        totalDto.From61To70Count = totalDto.From61To70Count += dto.From61To70Count;
        totalDto.Over70Count = totalDto.Over70Count += dto.Over70Count;

        if (totalDto.Count > 0)
        {
            totalDto.FemalePercentage = Math.Round((decimal)totalDto.FemaleCount / totalDto.Count * 100, 2);
            totalDto.MalePercentage = Math.Round((decimal)totalDto.MaleCount / totalDto.Count * 100, 2);
            totalDto.GermanPercentage = Math.Round((decimal)totalDto.GermanCount / totalDto.Count * 100, 2);
            totalDto.FrenchPercentage = Math.Round((decimal)totalDto.FrenchCount / totalDto.Count * 100, 2);
            totalDto.ItalianPercentage = Math.Round((decimal)totalDto.ItalianCount / totalDto.Count * 100, 2);
            totalDto.RomanshPercentage = Math.Round((decimal)totalDto.RomanshCount / totalDto.Count * 100, 2);
            totalDto.UpTo30Percentage = Math.Round((decimal)totalDto.UpTo30Count / totalDto.Count * 100, 2);
            totalDto.From31To40Percentage = Math.Round((decimal)totalDto.From31To40Count / totalDto.Count * 100, 2);
            totalDto.From41To50Percentage = Math.Round((decimal)totalDto.From41To50Count / totalDto.Count * 100, 2);
            totalDto.From51To60Percentage = Math.Round((decimal)totalDto.From51To60Count / totalDto.Count * 100, 2);
            totalDto.From61To70Percentage = Math.Round((decimal)totalDto.From61To70Count / totalDto.Count * 100, 2);
            totalDto.Over70Percentage = Math.Round((decimal)totalDto.Over70Count / totalDto.Count * 100, 2);
        }

        return totalDto;
    }

    public async Task<IEnumerable<MembershipStatisticByCantonDto>> GetMembershipsForDetailedCantonStatistic(IEnumerable<Membership> memberships)
    {
        var cantons = await _masterDataRepository.GetCantons();
        var dtos = new List<MembershipStatisticByCantonDto>();
        var currentYear = DateTime.Today.Year;

        foreach (var canton in cantons)
        {
            var filteredMemberships = memberships.Where(m => m.Person!.CorrespondenceAddress!.CantonId == canton.Id);

            var groupedMemberships = filteredMemberships.GroupBy(m => new { m.CommitteeId, m.Committee!.OgdId });

            foreach (var committeeGroup in groupedMemberships)
            {
                var committeeId = committeeGroup.Key.CommitteeId;
                var committeeOgdId = committeeGroup.Key.OgdId;

                var dto = new MembershipStatisticByCantonDto
                {
                    CommitteeId = committeeId,
                    CommitteeOgdId = committeeOgdId,
                    CantonCount = committeeGroup.Count(),
                    CantonId = canton.Id,
                    CantonOgdId = canton.OgdId,
                    FemaleCount = committeeGroup.Count(m => m.Person!.GenderId == Gender.FemaleGuid),
                    MaleCount = committeeGroup.Count(m => m.Person!.GenderId == Gender.MaleGuid),
                    GermanCount = committeeGroup.Count(m => m.Person!.LanguageId == Language.GermanGuid),
                    FrenchCount = committeeGroup.Count(m => m.Person!.LanguageId == Language.FrenchGuid),
                    ItalianCount = committeeGroup.Count(m => m.Person!.LanguageId == Language.ItalianGuid),
                    RomanshCount = committeeGroup.Count(m => m.Person!.LanguageId == Language.RomanshGuid),
                    FederalDutyCount = committeeGroup.Count(m => m.Person!.FederalDuty),
                    FederalAssemblyCount = committeeGroup.Count(m => m.Person!.FederalAssembly),
                    UpTo30Count = committeeGroup.Count(m => currentYear - m.Person!.BirthYear < 31),
                    From31To40Count = committeeGroup.Count(m => currentYear - m.Person!.BirthYear is >= 31 and <= 40),
                    From41To50Count = committeeGroup.Count(m => currentYear - m.Person!.BirthYear is >= 41 and <= 50),
                    From51To60Count = committeeGroup.Count(m => currentYear - m.Person!.BirthYear is >= 51 and <= 60),
                    From61To70Count = committeeGroup.Count(m => currentYear - m.Person!.BirthYear is >= 61 and <= 70),
                    Over70Count = committeeGroup.Count(m => currentYear - m.Person!.BirthYear > 70),
                };

                if (dto.CantonCount > 0)
                {
                    dto.FemalePercentage = Math.Round((decimal)dto.FemaleCount / dto.CantonCount * 100, 2);
                    dto.MalePercentage = Math.Round((decimal)dto.MaleCount / dto.CantonCount * 100, 2);
                    dto.GermanPercentage = Math.Round((decimal)dto.GermanCount / dto.CantonCount * 100, 2);
                    dto.FrenchPercentage = Math.Round((decimal)dto.FrenchCount / dto.CantonCount * 100, 2);
                    dto.ItalianPercentage = Math.Round((decimal)dto.ItalianCount / dto.CantonCount * 100, 2);
                    dto.RomanshPercentage = Math.Round((decimal)dto.RomanshCount / dto.CantonCount * 100, 2);
                    dto.UpTo30Percentage = Math.Round((decimal)dto.UpTo30Count / dto.CantonCount * 100, 2);
                    dto.From31To40Percentage = Math.Round((decimal)dto.From31To40Count / dto.CantonCount * 100, 2);
                    dto.From41To50Percentage = Math.Round((decimal)dto.From41To50Count / dto.CantonCount * 100, 2);
                    dto.From51To60Percentage = Math.Round((decimal)dto.From51To60Count / dto.CantonCount * 100, 2);
                    dto.From61To70Percentage = Math.Round((decimal)dto.From61To70Count / dto.CantonCount * 100, 2);
                    dto.Over70Percentage = Math.Round((decimal)dto.Over70Count / dto.CantonCount * 100, 2);
                }
                dtos.Add(dto);
            }
        }
        return dtos;
    }

    public async Task<MembershipUpdateDto> UpdateMembership(Guid id, MembershipUpdateDto updateDto)
    {
        _logger.LogInformation("Update membership with id {MembershipId} started.", id);

        var existingEntry = await _membershipRepository.GetByIdForUpdate(id, updateDto.RowVersion);

        // new, not only the shortening of a membership, but also terminating it, will cause a delete of GE candidate
        var deleteCandidate = updateDto.EndDate < existingEntry.EndDate
            || updateDto.ElectionTypeId == ElectionType.MembershipEndedBecauseOfDeathGuid
            || updateDto.ElectionTypeId == ElectionType.MaximumMembershipDurationGuid
            || updateDto.ElectionTypeId == ElectionType.OtherRetirementReasonGuid
            || updateDto.ElectionTypeId == ElectionType.RetirementGuid;

        var isGeneralElectionRunning = await _termOfOfficeDateService.CheckForRunningGeneralElection();
        var canEdit = await CanEditMembership(existingEntry);
        var currentUserName = _authorizationService.GetCurrentUserName();

        // if a membership is inactive, then only admins are allowed to edit
        if ((!existingEntry.IsActive && !_authorizationService.IsAdmin) || !canEdit)
        {
            throw new AuthorizationException("Not permitted to edit inactive memberships with this role");
        }

        // if a membership is active and begin date changed, throw exception for non admins
        if (existingEntry.IsActive && !_authorizationService.IsAdmin && existingEntry.BeginDate != updateDto.BeginDate)
        {
            throw new AuthorizationException("Not permitted to edit the start date of an active membership with this role");
        }

        var wasMetadataChanged = HasMetadataChanged(existingEntry, updateDto);

        existingEntry.BeginDate = updateDto.BeginDate;
        existingEntry.PersonId = updateDto.PersonId;
        existingEntry.MaximumEmploymentLevel = updateDto.MaximumEmploymentLevel;
        existingEntry.EndDate = updateDto.EndDate;
        existingEntry.ElectionTypeId = updateDto.ElectionTypeId;
        existingEntry.FunctionId = updateDto.FunctionId;
        existingEntry.ElectionOfficeId = updateDto.ElectionOfficeId;
        existingEntry.MembershipAdditionId = updateDto.MembershipAdditionId;
        existingEntry.JustificationLongerDuty = updateDto.JustificationLongerDuty;
        existingEntry.JustificationShorterDuty = updateDto.JustificationShorterDuty;
        existingEntry.JustificationMemberInFederalDuty = updateDto.JustificationMemberInFederalDuty;
        existingEntry.JustificationMemberInFederalAssembly = updateDto.JustificationMemberInFederalAssembly;
        existingEntry.RequirementsProfile = updateDto.RequirementsProfile;
        existingEntry.Remarks = updateDto.Remarks;
        existingEntry.RemarksStatus = updateDto.RemarksStatus;
        existingEntry.InCorrelationWithFederalDuty = updateDto.InCorrelationWithFederalDuty;
        existingEntry.ModifiedBy = currentUserName;
        existingEntry.Modified = DateTime.UtcNow;

        await _membershipRepository.CommitChanges();
        _logger.LogInformation("Updated membership with id {MembershipId}", id);

        // TODO: Code smell – MembershipService is handling GEW workflow logic.
        // Extract GEW-related behavior into an application/orchestrator service.
        if (isGeneralElectionRunning && await IsMembershipForGeneralElectionCommittee(existingEntry.CommitteeId))
        {
            if (existingEntry.Committee?.GeneralElectionCommittees.FirstOrDefault()?.CandidateListStateId == CandidateListState.Validated)
            {
                await _generalElectionCommitteeService.InvalidateMembershipCandidateList(existingEntry.CommitteeId);
            }

            await _membershipMirrorService.MirrorOrDeleteMembershipForGeneralElection(existingEntry, deleteCandidate, wasMetadataChanged);

            if (wasMetadataChanged && existingEntry.Committee?.GeneralElectionCommittees.FirstOrDefault()?.CandidateListStateId == CandidateListState.ReadyForFederalCouncilProposalForwarded)
            {
                await _generalElectionCommitteeService.SetFederalCouncilProposalToDirty(existingEntry.CommitteeId);
            }

            _logger.LogInformation("Mirrored membership changes for general election for membership id {MembershipId}", existingEntry.Id);
        }

        return MembershipMapper.ToMembershipUpdateDto(existingEntry, _cultureService);
    }

    public async Task DeleteMembership(Guid id)
    {
        var membership = await _membershipRepository.GetByIdForUpdate(id);

        var canEdit = await CanEditMembership(membership);
        var canDelete = (canEdit && membership.BeginDate > DateOnly.FromDateTime(DateTime.Now)) || _authorizationService.IsAdmin;
        var isGeneralElectionRunning = await _termOfOfficeDateService.CheckForRunningGeneralElection();

        if (!canDelete)
        {
            throw new AuthorizationException($"Not permitted to delete membership id {id} with this role");
        }

        // TODO: Code smell – MembershipService is handling GEW workflow logic.
        // Extract GEW-related behavior into an application/orchestrator service.
        if (isGeneralElectionRunning && membership.IsActive && await IsMembershipForGeneralElectionCommittee(membership.CommitteeId))
        {
            if (membership.Committee?.GeneralElectionCommittees.FirstOrDefault()?.CandidateListStateId == CandidateListState.Validated)
            {
                await _generalElectionCommitteeService.InvalidateMembershipCandidateList(membership.CommitteeId);
            }
            if (membership.Committee?.GeneralElectionCommittees.FirstOrDefault()?.CandidateListStateId == CandidateListState.ReadyForFederalCouncilProposalForwarded)
            {
                await _generalElectionCommitteeService.SetFederalCouncilProposalToDirty(membership.CommitteeId);
            }
            // when an active membership is deleted, a copied candidate is deleted as well!
            await _membershipMirrorService.MirrorOrDeleteMembershipForGeneralElection(membership, true, false);
        }

        _membershipRepository.Delete(membership);
        await _membershipRepository.CommitChanges();
    }

    private static bool HasMetadataChanged(Membership existing, MembershipUpdateDto dto)
    {
        return existing.MaximumEmploymentLevel != dto.MaximumEmploymentLevel ||
               existing.EndDate != dto.EndDate ||
               existing.ElectionTypeId != dto.ElectionTypeId ||
               existing.FunctionId != dto.FunctionId ||
               existing.InCorrelationWithFederalDuty != dto.InCorrelationWithFederalDuty;
    }

    private async Task<bool> CanEditMembership(Membership membership)
    {
        return _authorizationService.IsAdmin ||
            ((membership.IsActive || membership.IsFuture) && ((_authorizationService.IsDepartment &&
                membership.Committee!.DepartmentId == (await _authorizationService.GetDepartment())?.Id)
                || ((_authorizationService.IsOffice || _authorizationService.IsSecretariat)
                && (await _authorizationService.IsCommitteeAssigned(membership.Committee!.Id)))));
    }

    private async Task<bool> IsMembershipForGeneralElectionCommittee(Guid committeeId)
    {
        var committee = await _committeeRepository.GetById(committeeId);
        return committee.IsInGeneralElection;
    }
}
