namespace Bk.APG.Business.Dtos;

public class CommitteeListDto
{
    public required Guid Id { get; init; }
    public required int CommitteeId { get; init; }
    public required string Description { get; init; }
    public required string Level { get; init; }
    public required string Department { get; init; }
    public required string Office { get; init; }
    public required string CommitteeType { get; init; }
    public required string Term { get; init; }
    public bool IsActive { get; init; }
    public bool? IsMarketOrientated { get; init; }
    public bool? HasSupervisionDuty { get; init; }
    public bool NeedsAttention { get; init; }
}
