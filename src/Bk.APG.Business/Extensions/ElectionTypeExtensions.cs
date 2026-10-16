using Bk.APG.Business.Models;

namespace Bk.APG.Business.Extensions;

public static class ElectionTypeExtensions
{
    private static readonly Guid[] PresentElectionTypes =
    [
        ElectionType.RetirementGuid,
        ElectionType.MaximumMembershipDurationGuid,
        ElectionType.CommitteeDissolutionWithdrawalGuid,
        ElectionType.OtherRetirementReasonGuid
    ];

    private static readonly Guid[] FutureElectionTypes =
    [
        ElectionType.NewElectionGuid,
        ElectionType.ReElectionGuid
    ];

    public static List<Guid> GetElectionTypesForPresent(this IEnumerable<Guid> electionTypeIds)
    {
        ArgumentNullException.ThrowIfNull(electionTypeIds);

        return electionTypeIds.Intersect(PresentElectionTypes).ToList();
    }

    public static List<Guid> GetElectionTypesForFuture(this IEnumerable<Guid> electionTypeIds)
    {
        ArgumentNullException.ThrowIfNull(electionTypeIds);

        return electionTypeIds.Intersect(FutureElectionTypes).ToList();
    }
}
