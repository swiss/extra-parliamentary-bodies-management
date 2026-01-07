using Bk.APG.Business.Models;

namespace Bk.APG.Business.Repositories;

public interface IContactPointRepository
{
    void CreateForMigration(ContactPoint contactPoint);
    Task<IEnumerable<ContactPoint>> GetAllUnverifiedContactPoints();
    Task<IEnumerable<ContactPoint>> GetContactPointsByCommitteeId(Guid committeeId);
    Task<ContactPoint> GetById(Guid id);
    Task<ContactPoint> GetByIdForUpdate(Guid id, uint? updateDtoRowVersion = null);
    Task Create(ContactPoint contactPoint);
    void Delete(ContactPoint contactPoint);
    Task CommitChanges();
    IEnumerable<ContactPoint> GetAllActiveContactPoints();
}
