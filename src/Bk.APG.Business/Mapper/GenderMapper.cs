using System.Globalization;
using Bk.APG.Business.Dtos;
using Bk.APG.Business.Models;

namespace Bk.APG.Business.Mapper;

public static class GenderMapper
{
    public static GenderDto ToGenderDto(Gender gender)
    {
        return new GenderDto
        {
            Id = gender.Id,
            Text = gender.GetText(CultureInfo.CurrentUICulture),
            Description = gender.GetDescription(CultureInfo.CurrentUICulture),
            Sort = gender.Sort,
        };
    }
}
