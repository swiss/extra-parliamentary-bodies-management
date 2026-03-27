using System.Globalization;
using Bk.APG.Business.Dtos;
using Bk.APG.Business.Models;

namespace Bk.APG.Business.Mapper;

public static class LegalFormMapper
{
    public static LegalFormDto ToLegalFormDto(LegalForm legalForm)
    {
        ArgumentNullException.ThrowIfNull(legalForm);

        return new LegalFormDto
        {
            Id = legalForm.Id,
            LegalFormId = legalForm.LegalFormId,
            Text = legalForm.GetText(CultureInfo.CurrentUICulture),
            Description = legalForm.GetDescription(CultureInfo.CurrentUICulture),
        };
    }
}
