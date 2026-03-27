using System.Globalization;
using Bk.APG.Business.Dtos;
using Bk.APG.Business.Models;

namespace Bk.APG.Business.Mapper;

public static class InterestLegalFormMapper
{
    public static InterestLegalFormDto ToInterestLegalFormDto(InterestLegalForm interestLegalForm)
    {
        ArgumentNullException.ThrowIfNull(interestLegalForm);

        return new InterestLegalFormDto
        {
            Id = interestLegalForm.Id,
            Text = interestLegalForm.GetText(CultureInfo.CurrentUICulture),
            Description = interestLegalForm.GetDescription(CultureInfo.CurrentUICulture),
        };
    }
}
