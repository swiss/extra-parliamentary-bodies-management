namespace Bk.APG.Business.Dtos;

public class MembershipCantonStatisticDto
{
    public required Guid CommitteeId { get; init; }
    public required int CommitteeOgdId { get; init; }
    public required Guid CantonId { get; init; }
    public required int CantonOgdId { get; init; }
    public required string CantonUri { get; init; }
    public required int CantonCount { get; init; }
}
