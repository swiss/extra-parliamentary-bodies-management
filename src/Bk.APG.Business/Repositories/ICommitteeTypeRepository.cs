using Bk.APG.Business.Models;

namespace Bk.APG.Business.Repositories;

public interface ICommitteeTypeRepository
{
    Task<IEnumerable<CommitteeType>> GetList();
    Task<CommitteeType> GetByIdForUpdate(Guid id, uint? updateDtoRowVersion = null);
    Task CommitChanges();
}
