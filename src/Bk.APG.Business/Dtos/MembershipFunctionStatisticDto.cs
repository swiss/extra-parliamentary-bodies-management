namespace Bk.APG.Business.Dtos;

public class MembershipFunctionStatisticDto
{
    public required Guid CommitteeId { get; init; }
    public required int CommitteeOgdId { get; init; }
    public required Guid FunctionId { get; init; }
    public required int FunctionOgdId { get; init; }
    public required int FunctionCount { get; init; }
}
