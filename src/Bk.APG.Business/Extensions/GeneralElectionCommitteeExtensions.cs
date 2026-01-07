using Bk.APG.Business.Dtos;
using Bk.APG.Business.Models;

namespace Bk.APG.Business.Extensions;

public static class GeneralElectionCommitteeExtensions
{
    public static CommitteeQuotasDto GetQuotas(this GeneralElectionCommittee generalElectionCommittee)
    {
        var committeeType = generalElectionCommittee.CommitteeType!;
        var isPercentageBased = committeeType.GermanThresholdPercentage is not null;

        return new CommitteeQuotasDto
        {
            MembersCount = generalElectionCommittee.ActiveMemberCount,
            IsPercentageBased = isPercentageBased,
            PercentageLabel = isPercentageBased ? " % " : " ",
            FemaleThreshold = committeeType.FemaleThreshold ?? 0,
            FemaleQuota = generalElectionCommittee.FemaleQuota,
            MaleThreshold = committeeType.MaleThreshold ?? 0,
            MaleQuota = generalElectionCommittee.MaleQuota,
            GermanThreshold = (isPercentageBased ? committeeType.GermanThresholdPercentage : committeeType.GermanMinimalThreshold) ?? 0,
            GermanQuota = isPercentageBased ? generalElectionCommittee.GermanQuota : generalElectionCommittee.GermanCount,
            FrenchThreshold = (isPercentageBased ? committeeType.FrenchThresholdPercentage : committeeType.FrenchMinimalThreshold) ?? 0,
            FrenchQuota = isPercentageBased ? generalElectionCommittee.FrenchQuota : generalElectionCommittee.FrenchCount,
            ItalianThreshold = (isPercentageBased ? committeeType.ItalianThresholdPercentage : committeeType.ItalianMinimalThreshold) ?? 0,
            ItalianQuota = isPercentageBased ? generalElectionCommittee.ItalianQuota : generalElectionCommittee.ItalianCount,
            RomanshThreshold = (isPercentageBased ? committeeType.RomanshThresholdPercentage : committeeType.RomanshMinimalThreshold) ?? 0,
            RomanshQuota = isPercentageBased ? generalElectionCommittee.RomanshQuota : generalElectionCommittee.RomanshCount
        };
    }
}
