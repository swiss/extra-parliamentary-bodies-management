using Bk.APG.Business.Models;

namespace Bk.APG.Business.Extensions;

public static class MembershipCandidateExtensions
{
    public static string GetEmploymentLevel(this MembershipCandidate membershipCandidate)
    {
        return membershipCandidate.MaximumEmploymentLevel is null ? "0%" : $"{membershipCandidate.MaximumEmploymentLevel}%";
    }
}
