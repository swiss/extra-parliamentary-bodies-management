using Bk.APG.Business.Models;

namespace Bk.APG.Business.Repositories;

public interface IAddressRepository
{
    void CreateForMigration(Address address);
    Task<IEnumerable<Address>> GetAllUnverifiedAddresses();
    Task CommitChanges();
}
