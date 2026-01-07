namespace Bk.APG.Business.Dtos;

public class CommitteeMemberDto
{
    public required Guid Id { get; init; }
    public required Guid PersonId { get; init; }
    public required string Surname { get; init; }
    public required string GivenName { get; init; }
    public required string Gender { get; init; }
    public required string Language { get; init; }
    public required string EmploymentLevel { get; init; }
    public required string Function { get; init; }
    public required DateOnly BeginDate { get; init; }
    public required DateOnly EndDate { get; init; }
    public required string ElectionType { get; init; }
    public required bool HasMembershipAddition { get; init; }
    public required bool IsActive { get; init; }
    public required bool IsFuture { get; init; }
    public bool NeedsAttention { get; init; }
}
