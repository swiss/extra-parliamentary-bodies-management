using System.Globalization;
using Bk.APG.Business.Dtos;
using Bk.APG.Business.Models;

namespace Bk.APG.Business.Mapper;

public static class InterestCommitteeMapper
{
    public static InterestCommitteeDto ToInterestCommitteeDto(InterestCommittee interestCommittee)
    {
        return new InterestCommitteeDto
        {
            Id = interestCommittee.Id,
            Text = interestCommittee.GetText(CultureInfo.CurrentUICulture),
            Description = interestCommittee.GetDescription(CultureInfo.CurrentUICulture),
        };
    }
}
