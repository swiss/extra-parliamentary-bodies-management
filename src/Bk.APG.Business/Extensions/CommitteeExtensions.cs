using Bk.APG.Business.Dtos;
using Bk.APG.Business.Models;

namespace Bk.APG.Business.Extensions;

public static class CommitteeExtensions
{
    public static CommitteeQuotasDto GetQuotas(this Committee committee)
    {
        ArgumentNullException.ThrowIfNull(committee);

        var committeeType = committee.CommitteeType!;
        var isPercentageBased = committeeType.GermanThresholdPercentage is not null;

        return new CommitteeQuotasDto
        {
            MembersCount = committee.ActiveMemberCount,
            IsPercentageBased = isPercentageBased,
            PercentageLabel = isPercentageBased ? " % " : " ",
            FemaleThreshold = committeeType.FemaleThreshold ?? 0,
            FemaleQuota = committee.FemaleQuota,
            MaleThreshold = committeeType.MaleThreshold ?? 0,
            MaleQuota = committee.MaleQuota,
            GermanThreshold = (isPercentageBased ? committeeType.GermanThresholdPercentage : committeeType.GermanMinimalThreshold) ?? 0,
            GermanQuota = isPercentageBased ? committee.GermanQuota : committee.GermanCount,
            FrenchThreshold = (isPercentageBased ? committeeType.FrenchThresholdPercentage : committeeType.FrenchMinimalThreshold) ?? 0,
            FrenchQuota = isPercentageBased ? committee.FrenchQuota : committee.FrenchCount,
            ItalianThreshold = (isPercentageBased ? committeeType.ItalianThresholdPercentage : committeeType.ItalianMinimalThreshold) ?? 0,
            ItalianQuota = isPercentageBased ? committee.ItalianQuota : committee.ItalianCount,
            RomanshThreshold = (isPercentageBased ? committeeType.RomanshThresholdPercentage : committeeType.RomanshMinimalThreshold) ?? 0,
            RomanshQuota = isPercentageBased ? committee.RomanshQuota : committee.RomanshCount
        };
    }
}
