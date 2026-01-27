using Bk.APG.Business.Dtos;
using Bk.APG.Business.Models;

namespace Bk.APG.Business.Services;

public interface ICountryService
{
    Task<IEnumerable<CountryDto>> GetAll();
    Task<Country> CreateOrUpdate(Country country);
}
