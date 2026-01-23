using Bk.APG.Business.Dtos;
using Bk.APG.Business.Models;
using Bk.APG.Business.Repositories;
using Bk.APG.CrossCutting.Exception;
using Microsoft.EntityFrameworkCore;

namespace Bk.APG.Infrastructure.DataSource.Repositories;

public class MembershipRepository : IMembershipRepository
{
    private readonly DataContext _dataContext;

    public MembershipRepository(DataContext dataContext)
    {
        _dataContext = dataContext;
    }

    public void CreateForMigration(Membership membership)
    {
        _dataContext.Memberships.Add(membership);
    }

    public async Task<IEnumerable<Membership>> GetAllByPersonId(Guid personId)
    {
        var list = await _dataContext.Memberships
            .Include(m => m.Committee!.Department)
            .Include(m => m.Committee!.CommitteeType)
            .Include(m => m.Person!.Interests)
            .Include(m => m.Function)
            .Include(m => m.ElectionType)
            .Include(m => m.MembershipAddition)
            .Where(m => m.PersonId == personId)
            .AsSingleQuery()
            .ToListAsync();

        return list;
    }

    public async Task<IEnumerable<Membership>> GetAllByCommitteeId(Guid committeeId)
    {
        var list = await _dataContext.Memberships
            .Include(m => m.Committee!.Department)
            .Include(m => m.Committee!.CommitteeType)
            .Include(m => m.Person!.Interests)
            .Include(m => m.Person!.Gender)
            .Include(m => m.Person!.Language)
            .Include(m => m.Function)
            .Include(m => m.ElectionType)
            .Include(m => m.MembershipAddition)
            .Where(m => m.Committee!.Id == committeeId)
            .AsSingleQuery()
            .ToListAsync();

        return list;
    }

    public async Task<IEnumerable<Membership>> GetAllActiveMemberships()
    {
        return await _dataContext.Memberships
            .Include(m => m.Committee)
            .Include(m => m.Person!.Interests)
            .AsSingleQuery()
            .Where(m => m.EndDate >= DateOnly.FromDateTime(DateTime.Now) && m.BeginDate <= DateOnly.FromDateTime(DateTime.Now))
            .ToListAsync();
    }

    public async Task<IEnumerable<Membership>> GetAllActiveMembershipsForCommittee(Guid committeeId)
    {
        return await _dataContext.Memberships
            .Include(m => m.Person)
            .AsSingleQuery()
            .Where(m => m.EndDate >= DateOnly.FromDateTime(DateTime.Now) && m.BeginDate <= DateOnly.FromDateTime(DateTime.Now))
            .Where(m => m.CommitteeId == committeeId)
            .ToListAsync();
    }

    public async Task<IEnumerable<Membership>> GetAllMembershipsForCommitteeAndPerson(Guid committeeId, Guid personId)
    {
        return await _dataContext.Memberships
            .Include(m => m.Person)
            .AsNoTracking()
            .AsSingleQuery()
            .Where(m => m.PersonId == personId)
            .Where(m => m.CommitteeId == committeeId)
            .ToListAsync();
    }

    public async Task<Membership> GetById(Guid id)
    {
        var membership = await _dataContext.Memberships
            .Include(m => m.Function)
            .Include(m => m.ElectionType)
            .Include(m => m.ElectionOffice)
            .Include(m => m.MembershipAddition)
            .Include(m => m.Committee)
            .Include(m => m.Person!.Gender)
            .AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);

        if (membership is null)
        {
            throw new EntityNotFoundException($"Membership Id={id} not found");
        }

        return membership;
    }

    public IEnumerable<Membership> GetAllActiveForOgdExport()
    {
        return
            _dataContext.Memberships
                .Where(m => m.BeginDate <= DateOnly.FromDateTime(DateTime.Today) && m.EndDate > DateOnly.FromDateTime(DateTime.Today))
                .Include(m => m.Person!.Gender)
                .Include(m => m.Person!.CorrespondenceAddress!.Canton)
                .Include(m => m.Person!.Language)
                .Include(m => m.Committee!.Department)
                .Include(m => m.Committee!.CommitteeType)
                .Include(m => m.MembershipAddition)
                .Include(m => m.Function)
                .Include(m => m.ElectionOffice)
                .AsSingleQuery()
                .AsNoTracking()
                .AsEnumerable();
    }

    public async Task<Membership> Create(Membership membership)
    {
        var entry = await _dataContext.Memberships.AddAsync(membership);
        await _dataContext.SaveChangesAsync();

        return entry.Entity;
    }

    public async Task<Membership> GetByIdForUpdate(Guid id, uint? updateDtoRowVersion = null)
    {
        var membership = await _dataContext.Memberships
            .Include(m => m.MembershipAddition)
            .Include(m => m.ElectionOffice)
            .Include(m => m.ElectionType)
            .Include(m => m.Committee)
            .Include(m => m.Person)
            .FirstOrDefaultAsync(p => p.Id == id);

        if (membership is null)
        {
            throw new EntityNotFoundException($"Membership Id={id} not found");
        }

        if (updateDtoRowVersion.HasValue && updateDtoRowVersion != membership.RowVersion)
        {
            _dataContext.Entry(membership).Property(x => x.RowVersion).OriginalValue = updateDtoRowVersion.Value;
        }

        return membership;
    }

    public IEnumerable<MembershipFunctionStatisticDto> GetMembershipFunctionsForStatistics()
    {
        var today = DateOnly.FromDateTime(DateTime.Today);

        var result =
            from m in _dataContext.Memberships
            join f in _dataContext.Functions on m.FunctionId equals f.Id
            where m.BeginDate < today &&
                  m.EndDate > today &&
                  _dataContext.Committees.Any(c =>
                        c.Id == m.CommitteeId &&
                        c.BeginDate <= today &&
                        (c.EndDate == null || c.EndDate > today))
            group new { m, f } by new { m.CommitteeId, CommitteeOgdId = m.Committee!.OgdId, m.FunctionId, FunctionOgdId = f.OgdId } into g
            orderby g.Key.CommitteeId, g.Key.CommitteeOgdId
            select new MembershipFunctionStatisticDto
            {
                CommitteeId = g.Key.CommitteeId,
                CommitteeOgdId = g.Key.CommitteeOgdId,
                FunctionId = g.Key.FunctionId,
                FunctionOgdId = g.Key.FunctionOgdId,
                FunctionCount = g.Count()
            };

        var rows = result.ToList();

        // To identify the total lines, we return here the id 0, which will then be mapped to URI "TOTAL" (as requested from Michael Luggen)
        var totals = rows
            .GroupBy(r => new { r.CommitteeId, r.CommitteeOgdId })
            .Select(g => new MembershipFunctionStatisticDto
            {
                CommitteeId = g.Key.CommitteeId,
                CommitteeOgdId = g.Key.CommitteeOgdId,
                FunctionId = Guid.Empty,
                FunctionOgdId = 0,
                FunctionCount = g.Sum(x => x.FunctionCount)
            })
            .ToList();

        var final = rows.Concat(totals)
            .OrderBy(r => r.CommitteeId)
            .ThenBy(r => r.FunctionId == Guid.Empty ? 0 : 1) // totals last
            .ToList();

        return final;
    }

    public async Task CommitChanges()
    {
        await _dataContext.SaveChangesAsync();
    }

    public void Delete(Membership membership)
    {
        _dataContext.Remove(membership);
    }

    public void DeleteRange(IEnumerable<Membership> memberships)
    {
        _dataContext.Memberships.RemoveRange(memberships);
    }
}
