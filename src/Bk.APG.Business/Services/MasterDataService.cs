using System.Globalization;
using Bk.APG.Business.Dtos;
using Bk.APG.Business.Mapper;
using Bk.APG.Business.Models;
using Bk.APG.Business.Repositories;

namespace Bk.APG.Business.Services;

public class MasterDataService : IMasterDataService
{
    private readonly IMasterDataRepository _masterDataRepository;
    private readonly ICultureService _cultureService;
    private readonly ITermOfOfficeDateService _termOfOfficeDateService;
    private readonly IAuthorizationService _authorizationService;

    public MasterDataService(
        IMasterDataRepository masterDataRepository,
        ICultureService cultureService,
        ITermOfOfficeDateService termOfOfficeDateService,
        IAuthorizationService authorizationService)
    {
        _masterDataRepository = masterDataRepository;
        _cultureService = cultureService;
        _termOfOfficeDateService = termOfOfficeDateService;
        _authorizationService = authorizationService;
    }

    public async Task<IEnumerable<LanguageDto>> GetLanguages()
    {
        var languages = await _masterDataRepository.GetLanguages();

        return languages.Select(LanguageMapper.ToLanguageDto);
    }

    public async Task<IEnumerable<GenderDto>> GetGenders()
    {
        var genders = await _masterDataRepository.GetGenders();

        return genders.Select(GenderMapper.ToGenderDto).OrderBy(g => g.Sort).ThenBy(g => g.Text);
    }

    public async Task<IEnumerable<SalutationDto>> GetSalutations()
    {
        var salutations = await _masterDataRepository.GetSalutations();

        return salutations.Select(SalutationMapper.ToSalutationDto).OrderBy(s => s.Sort).ThenBy(s => s.Text);
    }

    public async Task<IEnumerable<InterestCommitteeDto>> GetInterestCommittees()
    {
        var interestCommittees = await _masterDataRepository.GetInterestCommittees();

        return interestCommittees.Select(InterestCommitteeMapper.ToInterestCommitteeDto).OrderBy(ic => ic.Text);
    }

    public async Task<IEnumerable<InterestFunctionDto>> GetInterestFunctions()
    {
        var interestFunctions = await _masterDataRepository.GetInterestFunctions();

        return interestFunctions.Select(InterestFunctionMapper.ToInterestFunctionDto).OrderBy(i => i.Text);
    }

    public async Task<IEnumerable<InterestLegalFormDto>> GetInterestLegalForms()
    {
        var interestLegalForms = await _masterDataRepository.GetInterestLegalForms();

        return interestLegalForms.Select(InterestLegalFormMapper.ToInterestLegalFormDto).OrderBy(lf => lf.Text);
    }

    public async Task<IEnumerable<LegalFormDto>> GetLegalForms()
    {
        var legalForms = await _masterDataRepository.GetLegalForms();

        return legalForms.Select(LegalFormMapper.ToLegalFormDto).OrderBy(lf => lf.Text);
    }

    public string GetLegalFormTextByLegalFormId(string legalFormId)
    {
        var legalForm = _masterDataRepository.GetLegalFormByLegalFormId(legalFormId);

        return legalForm == null ? string.Empty : legalForm.GetText();
    }

    public Guid GetLegalFormGuidByLegalFormId(string legalFormId)
    {
        var legalForm = _masterDataRepository.GetLegalFormByLegalFormId(legalFormId);

        return legalForm?.Id ?? Guid.Empty;
    }

    public async Task<IEnumerable<LevelDto>> GetLevels()
    {
        var levels = await _masterDataRepository.GetLevels();

        return levels
            .Select(x => MasterDataMapper.MapToMasterDataDto<LevelDto>(x, _cultureService.GetCurrentUiCulture()))
            .OrderBy(x => x.Text);
    }

    public async Task<IEnumerable<DepartmentDto>> GetDepartments()
    {
        var departments = await _masterDataRepository.GetDepartments();

        return departments
            .Select(MasterDataMapper.MapToDepartmentDto)
            .OrderBy(x => x.Text);
    }

    public async Task<IEnumerable<DepartmentDto>> GetPermittedDepartments()
    {
        var department = await _authorizationService.GetDepartment();
        var departments = await _masterDataRepository.GetDepartments();

        if (!_authorizationService.IsAdmin && department != null)
        {
            departments = departments.Where(d => d.Id == department.Id).ToList();
        }

        return departments
            .Select(MasterDataMapper.MapToDepartmentDto)
            .OrderBy(x => x.Text);
    }

    public async Task<IEnumerable<OfficeDto>> GetOffices()
    {
        var offices = await _masterDataRepository.GetOffices();

        return offices
            .Select(x =>
            {
                var officeDto = MasterDataMapper.MapToMasterDataDto<OfficeDto>(x, _cultureService.GetCurrentUiCulture());
                officeDto.DepartmentId = x.DepartmentId;
                return officeDto;
            })
            .OrderBy(x => x.Text);
    }

    public async Task<IEnumerable<OfficeDto>> GetPermittedOffices()
    {
        var department = await _authorizationService.GetDepartment();

        var offices = await _masterDataRepository.GetOffices();

        if (_authorizationService.IsSecretariat)
        {
            // return empty list, not permitted for an entire office
            offices = offices.Where(o => o.Id == Guid.Empty).ToList();
        }
        else if (_authorizationService.IsOffice)
        {
            var office = await _authorizationService.GetOffice();
            if (office != null)
            {
                offices = offices.Where(o => o.Id == office.Id).ToList();
            }
        }
        else if (department != null)
        {
            offices = offices.Where(o => o.DepartmentId == department.Id).ToList();
        }

        return offices
            .Select(x =>
            {
                var officeDto = MasterDataMapper.MapToMasterDataDto<OfficeDto>(x, _cultureService.GetCurrentUiCulture());
                officeDto.DepartmentId = x.DepartmentId;
                return officeDto;
            })
            .OrderBy(x => x.Text);
    }

    public async Task<IEnumerable<OfficeDto>> GetOfficesByName(string officeName)
    {
        ArgumentNullException.ThrowIfNull(officeName);

        var officeNameUpperCase = officeName.ToUpperInvariant();

        var offices = await _masterDataRepository.GetOffices();

        var myOffices = offices
            .Where(o => o.TextDe.Contains(officeNameUpperCase, StringComparison.InvariantCultureIgnoreCase) || o.TextFr.Contains(officeNameUpperCase, StringComparison.InvariantCultureIgnoreCase) ||
                        o.TextIt.Contains(officeNameUpperCase, StringComparison.InvariantCultureIgnoreCase) || o.DescriptionDe.Contains(officeNameUpperCase, StringComparison.InvariantCultureIgnoreCase) ||
                        o.DescriptionFr.Contains(officeNameUpperCase, StringComparison.InvariantCultureIgnoreCase) || o.DescriptionIt.Contains(officeNameUpperCase, StringComparison.InvariantCultureIgnoreCase))
            .Select(x => MasterDataMapper.MapToMasterDataDto<OfficeDto>(x, _cultureService.GetCurrentUiCulture()))
            .OrderBy(x => x.Description);

        return myOffices;
    }

    public async Task<IEnumerable<OccupationDto>> GetOccupationsByName(string occupation)
    {
        ArgumentNullException.ThrowIfNull(occupation);

        var occupationUpperCase = occupation.ToUpperInvariant();

        var occupations = await _masterDataRepository.GetOccupations();

        var filteredOccupations = occupations.Where(o => o.TextDe.Contains(occupationUpperCase, StringComparison.InvariantCultureIgnoreCase) || o.TextFr.Contains(occupationUpperCase, StringComparison.InvariantCultureIgnoreCase) ||
                                                         o.TextIt.Contains(occupationUpperCase, StringComparison.InvariantCultureIgnoreCase) || o.TextFemaleDe.Contains(occupationUpperCase, StringComparison.InvariantCultureIgnoreCase) ||
                                                         o.TextFemaleFr.Contains(occupationUpperCase, StringComparison.InvariantCultureIgnoreCase) || o.TextFemaleIt.Contains(occupationUpperCase, StringComparison.InvariantCultureIgnoreCase));

        var mappedOccuptations = filteredOccupations.Select(o => MasterDataMapper.MapToOccupationDto(o, _cultureService.GetCurrentUiCulture())).OrderBy(x => x.Text);

        return mappedOccuptations;
    }

    public async Task<IEnumerable<CommitteeTypeDto>> GetCommitteeTypes()
    {
        var committeeTypes = await _masterDataRepository.GetCommitteeTypes();

        return committeeTypes
            .Select(x => MasterDataMapper.MapToMasterDataDto<CommitteeTypeDto>(x, _cultureService.GetCurrentUiCulture()))
            .OrderBy(x => x.Text);
    }

    public async Task<IEnumerable<TermDto>> GetTerms()
    {
        var terms = await _masterDataRepository.GetTerms();

        return terms
            .Select(x => MasterDataMapper.MapToMasterDataDto<TermDto>(x, _cultureService.GetCurrentUiCulture()))
            .OrderBy(x => x.Text);
    }

    public async Task<IEnumerable<TermDateDto>> GetTermDates()
    {
        var termDates = await _masterDataRepository.GetTermDates();

        return termDates
            .Select(x => MasterDataMapper.MapTermOfOfficeDateToMasterDataDto<TermDateDto>(x, _cultureService.GetCurrentUiCulture()))
            .OrderBy(x => x.Text);
    }

    public async Task<IEnumerable<ElectionTypeDto>> GetElectionTypes()
    {
        var electionTypes = await _masterDataRepository.GetElectionTypes();

        return electionTypes
            .Select(x => MasterDataMapper.MapToMasterDataDto<ElectionTypeDto>(x, _cultureService.GetCurrentUiCulture()))
            .OrderBy(x => x.Text);
    }

    public async Task<IEnumerable<ElectionOfficeDto>> GetElectionOffices()
    {
        var electionOffices = await _masterDataRepository.GetElectionOffices();

        return electionOffices
            .Select(x => MasterDataMapper.MapToMasterDataDto<ElectionOfficeDto>(x, _cultureService.GetCurrentUiCulture()))
            .OrderBy(x => x.Text);
    }

    public async Task<IEnumerable<FunctionDto>> GetFunctions()
    {
        var functions = await _masterDataRepository.GetFunctions();

        return functions
            .Select(x => MasterDataMapper.MapFunctionToMasterDataDto<FunctionDto>(x, _cultureService.GetCurrentUiCulture()))
            .OrderBy(x => x.Text);
    }

    public async Task<Guid> GetContactPointGuidFromContactPointUri(string contactPointUri)
    {
        var contactPoints = await _masterDataRepository.GetContactPointTypes();

        return contactPoints.SingleOrDefault(cp => cp.Uri == contactPointUri)!.Id;
    }

    public async Task<IEnumerable<MembershipAdditionDto>> GetMembershipAdditions()
    {
        var membershipAdditions = await _masterDataRepository.GetMembershipAdditions();

        return membershipAdditions
            .Select(x => MasterDataMapper.MapToMasterDataDto<MembershipAdditionDto>(x, _cultureService.GetCurrentUiCulture()))
            .OrderBy(x => x.Text);
    }

    public async Task<IEnumerable<AppointmentDecisionTypeDto>> GetAppointmentDecisionTypes()
    {
        var appointmentDecisionTypes = await _masterDataRepository.GetAppointmentDecisionTypes();

        return appointmentDecisionTypes
            .Select(x => MasterDataMapper.MapToMasterDataDto<AppointmentDecisionTypeDto>(x, _cultureService.GetCurrentUiCulture()))
            .OrderBy(x => x.Text);
    }

    public async Task<IEnumerable<AppointmentDecisionLinkTypeDto>> GetAppointmentDecisionLinkTypes()
    {
        var appointmentDecisionTypes = await _masterDataRepository.GetAppointmentDecisionLinkTypes();

        return appointmentDecisionTypes
            .Select(x => MasterDataMapper.MapToMasterDataDto<AppointmentDecisionLinkTypeDto>(x, _cultureService.GetCurrentUiCulture()))
            .OrderBy(x => x.Text);
    }

    public async Task<IEnumerable<LegislaturePeriodDto>> GetLegislaturePeriods()
    {
        var legislaturePeriods = await _masterDataRepository.GetLegislaturePeriods();

        return legislaturePeriods.Select(MasterDataMapper.MapToLegislaturePeriodDto).OrderBy(x => x.Text);
    }

    public async Task<IEnumerable<CouncilDto>> GetCouncils()
    {
        var councils = await _masterDataRepository.GetCouncils();

        return councils.Select(MasterDataMapper.MapToCouncilDto).OrderBy(x => x.Text);
    }

    public async Task<IEnumerable<WorklistTaskTypeDto>> GetWorklistTaskTypes()
    {
        var worklistTaskTypes = await _masterDataRepository.GetWorklistTaskTypes();
        if (await _termOfOfficeDateService.CheckForRunningGeneralElection())
        {
            worklistTaskTypes = worklistTaskTypes.Where(x => x.Id != WorklistTaskType.GeneralElectionStart);
        }

        return worklistTaskTypes.Select(x => MasterDataMapper.MapWorklistTaskTypeToMasterDataDto<WorklistTaskTypeDto>(x, CultureInfo.CurrentUICulture));
    }

    public async Task<IEnumerable<WorklistTaskStateDto>> GetWorklistTaskStates()
    {
        var worklistTaskStates = await _masterDataRepository.GetWorklistTaskStates();

        return worklistTaskStates.Select(x => MasterDataMapper.MapToMasterDataDto<WorklistTaskStateDto>(x, CultureInfo.CurrentUICulture)).OrderBy(x => x.Text);
    }

    public async Task<IEnumerable<FormLetterSenderFunctionDto>> GetFormLetterSenderFunctions()
    {
        var formLetterSenderFunctions = await _masterDataRepository.GetFormLetterSenderFunctions();

        return formLetterSenderFunctions.Select(x => MasterDataMapper.MapToMasterDataDto<FormLetterSenderFunctionDto>(x, CultureInfo.CurrentUICulture)).OrderBy(x => x.Text);
    }
}
