using System.Globalization;
using Bk.APG.Business.Dtos;
using Bk.APG.Business.Models;

namespace Bk.APG.Business.Mapper;

public static class CountryMapper
{
    public static CountryDto ToCountryDto(Country country)
    {
        var currentUiCulture = CultureInfo.CurrentUICulture;
        return new CountryDto
        {
            Id = country.Id,
            Text = country.GetText(currentUiCulture),
            Description = country.GetDescription(currentUiCulture),
            Sort = country.Sort,
        };
    }

    public static Country ToCountry(MasterData.Models.Country country)
    {
        return new Country
        {
            Created = default,
            CreatedBy = null!,
            Modified = default,
            ModifiedBy = null!,
            IsDeleted = false,
            TextDe = country.ShortNameDe,
            TextFr = country.ShortNameFr,
            TextIt = country.ShortNameIt,
            TextRm = string.Empty,
            DescriptionDe = country.NameDe,
            DescriptionFr = country.NameFr,
            DescriptionIt = country.NameIt,
            DescriptionRm = string.Empty,
            Uri = country.Uri,
            Sort = 0
        };
    }
}
