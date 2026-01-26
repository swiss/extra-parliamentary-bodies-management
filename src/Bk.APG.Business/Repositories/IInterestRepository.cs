using Bk.APG.Business.Models;

namespace Bk.APG.Business.Repositories;

public interface IInterestRepository
{
    Task<IEnumerable<Interest>> GetAllByPersonId(Guid personId);
    IEnumerable<Interest> GetAllForOgdExport();
    Task<IEnumerable<Interest>> GetAllByPersonIdForUpdate(Guid personId);
    Task<Interest> GetByIdForUpdate(Guid id, uint? updateDtoRowVersion = null);
    Task<Interest> Create(Interest interest);
    Interest Update(Interest existing, Interest update);
    void Delete(Interest interest);
    void DeleteRange(IEnumerable<Interest> interests);
    Task CommitAsync();
    IEnumerable<Interest> GetAllUnverifiedInterests();
    void CreateForMigration(Interest interest);
    void CommitChanges();
}
