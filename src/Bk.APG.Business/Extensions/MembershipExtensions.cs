using Bk.APG.Business.Models;

namespace Bk.APG.Business.Extensions;

public static class MembershipExtensions
{
    public static string GetEmploymentLevel(this Membership membership)
    {
        ArgumentNullException.ThrowIfNull(membership);

        return membership.MaximumEmploymentLevel is null ? "0%" : $"{membership.MaximumEmploymentLevel}%";
    }
}
