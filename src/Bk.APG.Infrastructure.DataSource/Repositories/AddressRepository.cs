using Bk.APG.Business.Models;
using Bk.APG.Business.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Bk.APG.Infrastructure.DataSource.Repositories;

public class AddressRepository : IAddressRepository
{
    private readonly DataContext _dataContext;

    public AddressRepository(DataContext dataContext)
    {
        _dataContext = dataContext;
    }

    public void CreateForMigration(Address address)
    {
        _dataContext.Addresses.Add(address);
    }

    public async Task<IEnumerable<Address>> GetAllUnverifiedAddresses()
    {
        return await _dataContext.Addresses
            .Where(a => !a.VerifiedSuccessfully)
            .ToListAsync();
    }

    public async Task CommitChanges()
    {
        await _dataContext.SaveChangesAsync();
    }
}
