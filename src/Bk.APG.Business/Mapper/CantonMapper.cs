using System.Globalization;
using Bk.APG.Business.Dtos;
using Bk.APG.Business.Models;

namespace Bk.APG.Business.Mapper;

public static class CantonMapper
{
    public static CantonDto ToCantonDto(Canton canton)
    {
        var currentUiCulture = CultureInfo.CurrentUICulture;
        return new CantonDto
        {
            Id = canton.Id,
            Text = canton.GetText(currentUiCulture),
            Description = canton.GetDescription(currentUiCulture),
            Sort = canton.Sort,
        };
    }

    public static Canton ToCanton(MasterData.Models.Canton canton)
    {
        return new Canton
        {
            Created = default,
            CreatedBy = null!,
            Modified = default,
            ModifiedBy = null!,
            IsDeleted = false,
            TextDe = canton.ShortNameDe,
            TextFr = canton.ShortNameFr,
            TextIt = canton.ShortNameIt,
            TextRm = string.Empty,
            DescriptionDe = canton.NameDe,
            DescriptionFr = canton.NameFr,
            DescriptionIt = canton.NameIt,
            DescriptionRm = string.Empty,
            Uri = canton.Uri,
            Sort = 0
        };
    }
}
