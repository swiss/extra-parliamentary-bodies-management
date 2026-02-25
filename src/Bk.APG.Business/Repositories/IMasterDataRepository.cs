using Bk.APG.Business.Models;

namespace Bk.APG.Business.Repositories;

public interface IMasterDataRepository
{
    Task<IEnumerable<Language>> GetLanguages();
    Task<IEnumerable<Gender>> GetGenders();
    Task<IEnumerable<Salutation>> GetSalutations();
    Task<IEnumerable<InterestCommittee>> GetInterestCommittees();
    Task<IEnumerable<InterestFunction>> GetInterestFunctions();
    Task<IEnumerable<InterestLegalForm>> GetInterestLegalForms();
    Task<IEnumerable<LegalForm>> GetLegalForms();
    LegalForm? GetLegalFormByLegalFormId(string code);
    Task<IEnumerable<CommitteeLevel>> GetLevels();
    Task<IEnumerable<Department>> GetDepartments();
    Task<IEnumerable<Office>> GetOffices();
    Task<IEnumerable<CommitteeType>> GetCommitteeTypes();
    Task<IEnumerable<TermOfOffice>> GetTerms();
    Task<IEnumerable<TermOfOfficeDate>> GetTermDates();
    Task<IEnumerable<ElectionType>> GetElectionTypes();
    Task<IEnumerable<ElectionOffice>> GetElectionOffices();
    Task<IEnumerable<Function>> GetFunctions();
    Task<IEnumerable<ContactPointType>> GetContactPointTypes();
    Task<IEnumerable<MembershipAddition>> GetMembershipAdditions();
    Task<IEnumerable<MembershipAddition>> GetMembershipAdditionsByIds(Guid[] ids);
    Task<IEnumerable<AppointmentDecisionType>> GetAppointmentDecisionTypes();
    Task<IEnumerable<AppointmentDecisionLinkType>> GetAppointmentDecisionLinkTypes();
    Task<IEnumerable<LegislaturePeriod>> GetLegislaturePeriods();
    Task<IEnumerable<LegislaturePeriod>> GetLegislaturePeriodsByIds(ICollection<Guid> ids);
    Task<IEnumerable<Council>> GetCouncils();
    Task<IEnumerable<Canton>> GetCantons();
    Task<IEnumerable<WorklistTaskType>> GetWorklistTaskTypes();
    Task<IEnumerable<WorklistTaskState>> GetWorklistTaskStates();
    Task<ICollection<Occupation>> GetOccupations();
    Task<ICollection<Occupation>> GetOccupationsByIds(ICollection<Guid> ids);
    Task<IEnumerable<FormLetterSenderFunction>> GetFormLetterSenderFunctions();
    void AttachUnchanged<T>(T entity) where T : MasterDataBase;
    Task<T?> GetById<T>(Guid id) where T : MasterDataBase;
}
