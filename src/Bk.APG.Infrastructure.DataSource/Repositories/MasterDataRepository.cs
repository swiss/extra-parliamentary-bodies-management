using Bk.APG.Business.Models;
using Bk.APG.Business.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Bk.APG.Infrastructure.DataSource.Repositories;

public class MasterDataRepository : IMasterDataRepository
{
    private readonly DataContext _dataContext;

    public MasterDataRepository(DataContext dataContext)
    {
        _dataContext = dataContext;
    }

    public async Task<IEnumerable<Language>> GetLanguages()
    {
        return await _dataContext.Languages.Where(l => !l.IsDeleted).OrderBy(l => l.Sort)
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<IEnumerable<Gender>> GetGenders()
    {
        return await _dataContext.Genders.Where(l => !l.IsDeleted).OrderBy(l => l.Sort)
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<IEnumerable<Salutation>> GetSalutations()
    {
        return await _dataContext.Salutations.Where(l => !l.IsDeleted).OrderBy(l => l.Sort)
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<IEnumerable<InterestCommittee>> GetInterestCommittees()
    {
        return await _dataContext.InterestCommittees.Where(l => !l.IsDeleted).OrderBy(l => l.Sort)
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<IEnumerable<InterestFunction>> GetInterestFunctions()
    {
        return await _dataContext.InterestFunctions.Where(l => !l.IsDeleted).OrderBy(l => l.Sort)
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<IEnumerable<InterestLegalForm>> GetInterestLegalForms()
    {
        return await _dataContext.InterestLegalForms.Where(l => !l.IsDeleted).OrderBy(l => l.Sort)
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<IEnumerable<LegalForm>> GetLegalForms()
    {
        return await _dataContext.LegalForms.OrderBy(l => l.Sort)
            .AsNoTracking()
            .ToListAsync();
    }

    public LegalForm? GetLegalFormByLegalFormId(string legalFormId)
    {
        return _dataContext.LegalForms.FirstOrDefault(lf => lf.LegalFormId == legalFormId);
    }

    public async Task<IEnumerable<CommitteeLevel>> GetLevels()
    {
        return await _dataContext.CommitteeLevels
            .Where(l => !l.IsDeleted)
            .OrderBy(x => x.Sort)
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<IEnumerable<Department>> GetDepartments()
    {
        return await _dataContext.Departments
            .Where(l => !l.IsDeleted)
            .OrderBy(x => x.Sort)
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<IEnumerable<Office>> GetOffices()
    {
        return await _dataContext.Offices
            .Where(o => !o.IsDeleted)
            .OrderBy(o => o.Sort)
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<IEnumerable<CommitteeType>> GetCommitteeTypes()
    {
        return await _dataContext.CommitteeTypes
            .Where(l => !l.IsDeleted)
            .OrderBy(x => x.Sort)
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<IEnumerable<TermOfOffice>> GetTerms()
    {
        return await _dataContext.TermsOfOffice
            .Where(l => !l.IsDeleted)
            .OrderBy(x => x.Sort)
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<IEnumerable<TermOfOfficeDate>> GetTermDates()
    {
        return await _dataContext.TermOfOfficeDates
            .Where(l => !l.IsDeleted)
            .OrderBy(x => x.Sort)
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<IEnumerable<ElectionType>> GetElectionTypes()
    {
        return await _dataContext.ElectionTypes
            .Where(l => !l.IsDeleted)
            .OrderBy(x => x.Sort)
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<IEnumerable<ElectionOffice>> GetElectionOffices()
    {
        return await _dataContext.ElectionOffices
            .Where(l => !l.IsDeleted)
            .OrderBy(x => x.Sort)
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<IEnumerable<Function>> GetFunctions()
    {
        return await _dataContext.Functions
            .Where(l => !l.IsDeleted)
            .OrderBy(x => x.Sort)
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<IEnumerable<ContactPointType>> GetContactPointTypes()
    {
        return await _dataContext.ContactPointTypes
            .Where(l => !l.IsDeleted)
            .OrderBy(x => x.Sort)
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<IEnumerable<MembershipAddition>> GetMembershipAdditions()
    {
        return await _dataContext.MembershipAdditions
            .Where(l => !l.IsDeleted)
            .OrderBy(x => x.Sort)
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<IEnumerable<MembershipAddition>> GetMembershipAdditionsByIds(Guid[] ids)
    {
        return await _dataContext.MembershipAdditions
            .Where(l => !l.IsDeleted)
            .Where(x => ids.Contains(x.Id))
            .OrderBy(x => x.Sort)
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<IEnumerable<AppointmentDecisionType>> GetAppointmentDecisionTypes()
    {
        return await _dataContext.AppointmentDecisionTypes
            .Where(l => !l.IsDeleted)
            .OrderBy(x => x.Sort)
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<IEnumerable<AppointmentDecisionLinkType>> GetAppointmentDecisionLinkTypes()
    {
        return await _dataContext.AppointmentDecisionLinkTypes
            .Where(l => !l.IsDeleted)
            .OrderBy(x => x.Sort)
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<IEnumerable<LegislaturePeriod>> GetLegislaturePeriods()
    {
        return await _dataContext.LegislaturePeriods
            .Where(l => !l.IsDeleted)
            .OrderBy(x => x.Sort)
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<IEnumerable<LegislaturePeriod>> GetLegislaturePeriodsByIds(ICollection<Guid> ids)
    {
        return ids.Count != 0
            ? await _dataContext.LegislaturePeriods
                .Where(x => ids.Contains(x.Id))
                .ToListAsync()
            : [];
    }

    public async Task<IEnumerable<Council>> GetCouncils()
    {
        return await _dataContext.Councils
            .Where(l => !l.IsDeleted)
            .OrderBy(x => x.Sort)
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<IEnumerable<Canton>> GetCantons()
    {
        return await _dataContext.Cantons
            .Where(x => !x.IsDeleted)
            .OrderBy(x => x.Sort)
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<IEnumerable<WorklistTaskType>> GetWorklistTaskTypes()
    {
        return await _dataContext.WorklistTaskTypes
            .Where(x => !x.IsDeleted)
            .OrderBy(x => x.Sort)
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<IEnumerable<WorklistTaskState>> GetWorklistTaskStates()
    {
        return await _dataContext.WorklistTaskStates
            .Where(x => !x.IsDeleted)
            .OrderBy(x => x.Sort)
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<ICollection<Occupation>> GetOccupations()
    {
        return await _dataContext.Occupations
            .Where(o => !o.IsDeleted)
            .OrderBy(x => x.Sort)
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<ICollection<Occupation>> GetOccupationsByIds(ICollection<Guid> ids)
    {
        return ids.Count != 0
            ? await _dataContext.Occupations
                .Where(o => !o.IsDeleted)
                .Where(x => ids.Contains(x.Id))
                .ToListAsync()
            : [];
    }

    public async Task<IEnumerable<FormLetterSenderFunction>> GetFormLetterSenderFunctions()
    {
        return await _dataContext.FormLetterSenderFunctions
            .Where(x => !x.IsDeleted)
            .OrderBy(x => x.Sort)
            .AsNoTracking()
            .ToListAsync();
    }

    public void AttachUnchanged<T>(T entity) where T : MasterDataBase
    {
        var entry = _dataContext.Entry(entity);

        if (entry.State == EntityState.Detached)
        {
            _dataContext.Set<T>().Attach(entity);
        }

        entry.State = EntityState.Unchanged;
    }

    public async Task<T?> GetById<T>(Guid id) where T : MasterDataBase
    {
        return await _dataContext.Set<T>()
            .AsNoTracking()
            .Where(x => !x.IsDeleted)
            .FirstOrDefaultAsync(x => x.Id == id);
    }
}
