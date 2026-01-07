using Bk.APG.Business.Models;
using Bk.APG.Business.Repositories;
using Bk.APG.CrossCutting.Exception;
using Microsoft.EntityFrameworkCore;

namespace Bk.APG.Infrastructure.DataSource.Repositories;

public class ContactPointRepository : IContactPointRepository
{
    private readonly DataContext _dataContext;

    public ContactPointRepository(DataContext dataContext)
    {
        _dataContext = dataContext;
    }

    public void CreateForMigration(ContactPoint contactPoint)
    {
        _dataContext.ContactPoints.Add(contactPoint);
    }

    public async Task<IEnumerable<ContactPoint>> GetAllUnverifiedContactPoints()
    {
        return await _dataContext.ContactPoints
            .Where(cp => !cp.VerifiedSuccessfully)
            .Where(cp => cp.Zip != null && cp.City != null)
            .ToListAsync();
    }

    public async Task<IEnumerable<ContactPoint>> GetContactPointsByCommitteeId(Guid committeeId)
    {
        return await GetContactPoints().AsNoTracking().Where(x => x.Committee!.Id == committeeId).ToListAsync();
    }

    public async Task<ContactPoint> GetById(Guid id)
    {
        var contactPoint = await GetContactPoints()
            .Include(c => c.Committee)
            .FirstOrDefaultAsync(p => p.Id == id);

        if (contactPoint is null)
        {
            throw new EntityNotFoundException($"ContactPoint Id={id} not found");
        }

        return contactPoint;
    }

    public async Task<ContactPoint> GetByIdForUpdate(Guid id, uint? updateDtoRowVersion = null)
    {
        var contactPoint = await GetContactPoints()
            .Include(c => c.Committee)
            .FirstOrDefaultAsync(p => p.Id == id);

        if (contactPoint is null)
        {
            throw new EntityNotFoundException($"ContactPoint Id={id} not found");
        }

        if (updateDtoRowVersion.HasValue && updateDtoRowVersion != contactPoint.RowVersion)
        {
            _dataContext.Entry(contactPoint).Property(x => x.RowVersion).OriginalValue = updateDtoRowVersion.Value;
        }

        return contactPoint;
    }

    public async Task Create(ContactPoint contactPoint)
    {
        await _dataContext.ContactPoints.AddAsync(contactPoint);
        await _dataContext.SaveChangesAsync();
    }

    public void Delete(ContactPoint contactPoint)
    {
        _dataContext.ContactPoints.Remove(contactPoint);
    }

    public async Task CommitChanges()
    {
        await _dataContext.SaveChangesAsync();
    }

    public IEnumerable<ContactPoint> GetAllActiveContactPoints()
    {
        return _dataContext.ContactPoints
            .AsNoTracking()
            .Include(c => c.ContactPointType)
            .Where(c => c.BeginDate <= DateOnly.FromDateTime(DateTime.Today) && (c.EndDate == null || c.EndDate > DateOnly.FromDateTime(DateTime.Today)))
            .AsEnumerable();
    }

    private IQueryable<ContactPoint> GetContactPoints()
    {
        var values = _dataContext.ContactPoints
            .Include(x => x.Committee)
            .Include(x => x.ContactPointType)
            .Include(x => x.Language)
            .Include(x => x.Gender);

        return values;
    }
}
