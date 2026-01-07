namespace Bk.APG.Business.Dtos;

public class PersonMembershipDto
{
    public required Guid Id { get; init; }
    public required string Committee { get; init; }
    public required string Department { get; init; }
    public required string Function { get; init; }
    public required DateOnly BeginDate { get; init; }
    public DateOnly? EndDate { get; init; }
    public required string ElectionType { get; init; }
    public bool IsActive { get; init; }
    public bool NeedsAttention { get; init; }
}
