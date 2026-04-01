using System.Globalization;
using Bk.APG.Business.Dtos;
using Bk.APG.Business.Models;

namespace Bk.APG.Business.Mapper;

public static class LanguageMapper
{
    public static LanguageDto ToLanguageDto(Language language)
    {
        ArgumentNullException.ThrowIfNull(language);

        var currentUiCulture = CultureInfo.CurrentUICulture;
        return new LanguageDto
        {
            Id = language.Id,
            Text = language.GetText(currentUiCulture),
            Description = language.GetDescription(currentUiCulture)
        };
    }
}
