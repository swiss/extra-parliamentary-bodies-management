using Bk.APG.Business.Models;
using Bk.APG.Business.Repositories;
using Bk.APG.CrossCutting.Exception;
using Microsoft.EntityFrameworkCore;

namespace Bk.APG.Infrastructure.DataSource.Repositories;

public class InterestRepository : IInterestRepository
{
    private readonly DataContext _dataContext;

    public InterestRepository(DataContext dataContext)
    {
        _dataContext = dataContext;
    }

    public async Task<IEnumerable<Interest>> GetAllByPersonId(Guid personId)
    {
        var interests = await _dataContext.Interests
            .Include(item => item.InterestCommittee)
            .Include(item => item.InterestFunction)
            .Include(item => item.InterestLegalForm)
            .Include(item => item.LegalForm)
            .Where(i => !i.IsDeleted)
            .Where(i => i.PersonId == personId)
            .AsSplitQuery()
            .AsNoTracking()
            .ToListAsync();

        return interests;
    }

    public async Task<IEnumerable<Interest>> GetAllByPersonIdForUpdate(Guid personId)
    {
        var interests = await _dataContext.Interests
            .Where(i => i.PersonId == personId)
            .ToListAsync();

        return interests;
    }

    public async Task<IEnumerable<Interest>> GetAllForOgdExport()
    {
        var today = DateOnly.FromDateTime(DateTime.Today);

        var interests = await _dataContext.Interests
            .Where(i => (i.BeginDate <= today && (i.EndDate > today || i.EndDate == null)) || (i.BeginDate == null && i.EndDate == null))
            .Include(item => item.Person!)
                .ThenInclude(item => item.Memberships)
            .Include(item => item.InterestCommittee)
            .Include(item => item.InterestFunction)
            .Include(item => item.LegalForm)
            .Where(c => c.Person!.Memberships.Any(m => m.BeginDate <= today && m.EndDate > today))
            .AsSplitQuery()
            .ToListAsync();

        return interests;
    }

    public async Task<Interest> GetByIdForUpdate(Guid id, uint? updateDtoRowVersion = null)
    {
        var interest = await _dataContext.Interests
            .Include(item => item.InterestCommittee)
            .Include(item => item.InterestFunction)
            .Include(item => item.InterestLegalForm)
            .FirstOrDefaultAsync(i => i.Id == id);

        if (interest is null)
        {
            throw new EntityNotFoundException($"Interest Id={id} not found");
        }

        if (updateDtoRowVersion.HasValue && updateDtoRowVersion != interest.RowVersion)
        {
            _dataContext.Entry(interest).Property(x => x.RowVersion).OriginalValue = updateDtoRowVersion.Value;
        }

        return interest;
    }

    public IEnumerable<Interest> GetAllUnverifiedInterests()
    {
        return _dataContext.Interests
            .Where(i => i.VerifiedSuccessfully == null)
            .ToList();
    }

    public async Task<Interest> Create(Interest interest)
    {
        var entry = await _dataContext.Interests.AddAsync(interest);

        return entry.Entity;
    }

    public Interest Update(Interest existing, Interest update)
    {
        existing.Text = update.Text;
        existing.InterestText = update.InterestText;
        existing.InterestLegalFormId = update.InterestLegalFormId;
        existing.InterestCommitteeId = update.InterestCommitteeId;
        existing.InterestFunctionId = update.InterestFunctionId;
        existing.LegalFormId = update.LegalFormId;
        existing.BeginDate = update.BeginDate;
        existing.EndDate = update.EndDate;
        existing.IsDeleted = update.IsDeleted;
        existing.Modified = update.Modified;
        existing.ModifiedBy = update.ModifiedBy;

        return existing;
    }

    public void Delete(Interest interest)
    {
        _dataContext.Interests.Remove(interest);
    }

    public void DeleteRange(IEnumerable<Interest> interests)
    {
        _dataContext.Interests.RemoveRange(interests);
    }

    public void CreateForMigration(Interest interest)
    {
        _dataContext.Interests.Add(interest);
    }

    public async Task CommitAsync()
    {
        await _dataContext.SaveChangesAsync();
    }

    public void CommitChanges()
    {
        _dataContext.SaveChanges();
    }
}
