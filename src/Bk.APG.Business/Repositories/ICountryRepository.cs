using Bk.APG.Business.Models;

namespace Bk.APG.Business.Repositories;

public interface ICountryRepository
{
    Task<IEnumerable<Country>> GetAll();
    Task<Country?> GetByUri(string uri);
    Task<Country?> GetById(Guid id);
    Task<Country> Create(Country canton);
    Task<Country> Update(Country existing, Country update);
}
