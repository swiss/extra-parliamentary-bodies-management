using System.Globalization;
using Bk.APG.Business.Dtos;
using Bk.APG.Business.Models;

namespace Bk.APG.Business.Mapper;

public static class SalutationMapper
{
    public static SalutationDto ToSalutationDto(Salutation salutation)
    {
        ArgumentNullException.ThrowIfNull(salutation);

        return new SalutationDto
        {
            Id = salutation.Id,
            Text = salutation.GetText(CultureInfo.CurrentUICulture),
            Description = salutation.GetDescription(CultureInfo.CurrentUICulture),
            Sort = salutation.Sort,
        };
    }
}
