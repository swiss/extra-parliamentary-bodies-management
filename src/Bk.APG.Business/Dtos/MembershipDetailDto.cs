namespace Bk.APG.Business.Dtos;

public class MembershipDetailDto
{
    public required Guid Id { get; init; }
    public required string Committee { get; init; }
    public required string Function { get; init; }
    public required DateOnly BeginDate { get; init; }
    public DateOnly? EndDate { get; init; }
    public bool IsActive { get; init; }
    public bool IsFuture { get; init; }
    public bool NeedsAttention { get; init; }
}
