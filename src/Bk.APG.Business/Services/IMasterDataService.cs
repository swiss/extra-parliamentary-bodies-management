using Bk.APG.Business.Dtos;

namespace Bk.APG.Business.Services;

public interface IMasterDataService
{
    Task<IEnumerable<LanguageDto>> GetLanguages();
    Task<IEnumerable<GenderDto>> GetGenders();
    Task<IEnumerable<SalutationDto>> GetSalutations();
    Task<IEnumerable<InterestCommitteeDto>> GetInterestCommittees();
    Task<IEnumerable<InterestFunctionDto>> GetInterestFunctions();
    Task<IEnumerable<InterestLegalFormDto>> GetInterestLegalForms();
    Task<IEnumerable<LegalFormDto>> GetLegalForms();
    string GetLegalFormTextByLegalFormId(string legalFormId);
    Guid GetLegalFormGuidByLegalFormId(string legalFormId);
    Task<IEnumerable<LevelDto>> GetLevels();
    Task<IEnumerable<DepartmentDto>> GetDepartments();
    Task<IEnumerable<DepartmentDto>> GetPermittedDepartments();
    Task<IEnumerable<OfficeDto>> GetOffices();
    Task<IEnumerable<OfficeDto>> GetPermittedOffices();
    Task<IEnumerable<OfficeDto>> GetOfficesByName(string officeName);
    Task<IEnumerable<OccupationDto>> GetOccupationsByName(string occupation);
    Task<IEnumerable<OfficeDto>> GetGeneralSecretariatOffices();
    Task<IEnumerable<CommitteeTypeDto>> GetCommitteeTypes();
    Task<CommitteeTypeDto> GetCommitteeTypeById(Guid id);
    Task<IEnumerable<TermDto>> GetTerms();
    Task<IEnumerable<TermDateDto>> GetTermDates();
    Task<IEnumerable<ElectionTypeDto>> GetElectionTypes();
    Task<IEnumerable<ElectionOfficeDto>> GetElectionOffices();
    Task<IEnumerable<FunctionDto>> GetFunctions();
    Task<Guid> GetContactPointGuidFromContactPointUri(string contactPointUri);
    Task<IEnumerable<MembershipAdditionDto>> GetMembershipAdditions();
    Task<IEnumerable<AppointmentDecisionTypeDto>> GetAppointmentDecisionTypes();
    Task<IEnumerable<AppointmentDecisionLinkTypeDto>> GetAppointmentDecisionLinkTypes();
    Task<IEnumerable<LegislaturePeriodDto>> GetLegislaturePeriods();
    Task<IEnumerable<CouncilDto>> GetCouncils();
    Task<IEnumerable<WorklistTaskTypeDto>> GetWorklistTaskTypes();
    Task<IEnumerable<WorklistTaskStateDto>> GetWorklistTaskStates();
}
