using System.Globalization;
using Bk.APG.Business.Dtos;
using Bk.APG.Business.Models;

namespace Bk.APG.Business.Mapper;

public static class InterestFunctionMapper
{
    public static InterestFunctionDto ToInterestFunctionDto(InterestFunction interestFunction)
    {
        ArgumentNullException.ThrowIfNull(interestFunction);

        return new InterestFunctionDto
        {
            Id = interestFunction.Id,
            Text = interestFunction.GetText(CultureInfo.CurrentUICulture),
            Description = interestFunction.GetDescription(CultureInfo.CurrentUICulture),
        };
    }
}
