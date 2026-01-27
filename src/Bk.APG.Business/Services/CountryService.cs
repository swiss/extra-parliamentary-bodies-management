using Bk.APG.Business.Dtos;
using Bk.APG.Business.Mapper;
using Bk.APG.Business.Models;
using Bk.APG.Business.Repositories;

namespace Bk.APG.Business.Services;

public class CountryService : ICountryService
{
    private readonly ICountryRepository _countryRepository;

    public CountryService(ICountryRepository countryRepository)
    {
        _countryRepository = countryRepository;
    }

    public async Task<IEnumerable<CountryDto>> GetAll()
    {
        var cantons = await _countryRepository.GetAll();

        return cantons.Select(c => CountryMapper.ToCountryDto(c)).OrderBy(c => c.Sort).ThenBy(c => c.Text);
    }

    public async Task<Country> CreateOrUpdate(Country country)
    {
        var now = DateTime.UtcNow;

        var countryFromDb = await _countryRepository.GetByUri(country.Uri);
        if (countryFromDb is null)
        {
            var countryToCreate = new Country
            {
                Created = now,
                CreatedBy = country.ModifiedBy,
                Modified = now,
                ModifiedBy = country.ModifiedBy,
                IsDeleted = false,
                TextDe = country.TextDe,
                TextFr = country.TextFr,
                TextIt = country.TextIt,
                TextRm = country.TextRm,
                DescriptionDe = country.DescriptionDe,
                DescriptionFr = country.DescriptionFr,
                DescriptionIt = country.DescriptionIt,
                DescriptionRm = country.DescriptionRm,
                Uri = country.Uri,
                Sort = country.Sort,
            };

            var newCountry = await _countryRepository.Create(countryToCreate);
            return newCountry;
        }

        var updatedCountry = await _countryRepository.Update(countryFromDb, country);

        return updatedCountry;
    }
}
