using Bk.APG.Business.Models;
using Bk.APG.Business.Repositories;
using Bk.APG.CrossCutting.Exception;
using Microsoft.EntityFrameworkCore;

namespace Bk.APG.Infrastructure.DataSource.Repositories;

public class MembershipCandidateRepository : IMembershipCandidateRepository
{
    private readonly DataContext _dataContext;

    public MembershipCandidateRepository(DataContext dataContext)
    {
        _dataContext = dataContext;
    }

    public async Task<MembershipCandidate> GetById(Guid id)
    {
        var membershipCandidate = await _dataContext.MembershipCandidates
            .Include(m => m.GeneralElectionCommittee)
            .Include(m => m.Function)
            .Include(m => m.ElectionType)
            .Include(m => m.MembershipAddition)
            .Include(m => m.Person!.Language)
            .Include(m => m.Person!.Gender)
            .Include(m => m.Language)
            .Include(m => m.Gender)
            .Include(m => m.MembershipAddition)
            .AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);

        if (membershipCandidate is null)
        {
            throw new EntityNotFoundException($"Membership candidate Id={id} not found");
        }

        return membershipCandidate;
    }

    public async Task<MembershipCandidate> Create(MembershipCandidate membershipCandidate)
    {
        var createdMembershipCandidate = await _dataContext.MembershipCandidates.AddAsync(membershipCandidate);
        await _dataContext.SaveChangesAsync();

        return createdMembershipCandidate.Entity;
    }

    public async Task<MembershipCandidate> GetByIdForUpdate(Guid id, uint? updateDtoRowVersion = null)
    {
        var membershipCandidate = await _dataContext.MembershipCandidates
            .Include(m => m.MembershipAddition)
            .Include(m => m.ElectionOffice)
            .Include(m => m.GeneralElectionCommittee!.TermOfOfficeDate)
            .Include(m => m.Person!.Memberships)
            .Include(m => m.Person!.LegislaturePeriods)
            .AsSplitQuery()
            .FirstOrDefaultAsync(m => m.Id == id);

        if (membershipCandidate is null)
        {
            throw new EntityNotFoundException($"Membership candidate Id={id} not found");
        }

        if (updateDtoRowVersion.HasValue && updateDtoRowVersion != membershipCandidate.RowVersion)
        {
            _dataContext.Entry(membershipCandidate).Property(x => x.RowVersion).OriginalValue = updateDtoRowVersion.Value;
        }

        return membershipCandidate;
    }

    public async Task<IEnumerable<MembershipCandidate>> GetByCommitteeId(Guid committeeId)
    {
        return await _dataContext.MembershipCandidates
            .Include(x => x.GeneralElectionCommittee)
            .Include(x => x.Gender)
            .Include(x => x.Language)
            .Include(x => x.Function)
            .Include(x => x.MembershipAddition)
            .Include(x => x.ElectionType)
            .Include(x => x.Person)
            .ThenInclude(x => x!.Gender)
            .Include(x => x.Person)
            .ThenInclude(x => x!.Language)
            .AsNoTracking()
            .AsSplitQuery()
            .Where(x => x.GeneralElectionCommittee!.CommitteeId == committeeId)
            .Where(x => !x.IsDeleted)
            .ToListAsync();
    }

    public async Task<MembershipCandidate?> GetByMembershipIdForUpdate(Guid membershipId, uint? updateDtoRowVersion = null)
    {
        var membershipCandidate = await _dataContext.MembershipCandidates
            .Include(m => m.MembershipAddition)
            .Include(m => m.ElectionOffice)
            .FirstOrDefaultAsync(m => m.MembershipId == membershipId);

        if (updateDtoRowVersion.HasValue && membershipCandidate is not null && updateDtoRowVersion != membershipCandidate.RowVersion)
        {
            _dataContext.Entry(membershipCandidate).Property(x => x.RowVersion).OriginalValue = updateDtoRowVersion.Value;
        }

        return membershipCandidate;
    }

    public async Task<IEnumerable<MembershipCandidate>> GetByPersonIdForUpdate(Guid personId)
    {
        var membershipCandidates = await _dataContext.MembershipCandidates
            .Include(m => m.MembershipAddition)
            .Include(m => m.ElectionOffice)
            .Where(m => m.PersonId == personId)
            .ToListAsync();

        return membershipCandidates;
    }

    public async Task CommitChanges()
    {
        await _dataContext.SaveChangesAsync();
    }

    public async Task Delete(MembershipCandidate membershipCandidate)
    {
        _dataContext.MembershipCandidates.Remove(membershipCandidate);
        await _dataContext.SaveChangesAsync();
    }

    public void DeleteRange(IEnumerable<MembershipCandidate> membershipCandidates)
    {
        _dataContext.MembershipCandidates.RemoveRange(membershipCandidates);
    }
}
