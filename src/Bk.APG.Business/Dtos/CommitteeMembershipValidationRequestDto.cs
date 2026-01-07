namespace Bk.APG.Business.Dtos;

public class CommitteeMembershipValidationRequestDto
{
    public required Guid CommitteeId { get; init; }
    public required Guid PersonId { get; init; }
    public Guid? CurrentMembershipId { get; set; }
    public required DateOnly BeginDate { get; init; }
    public required DateOnly EndDate { get; set; }
    public required bool InCorrelationWithFederalDuty { get; set; }
    public required bool IsUpdateMode { get; set; }
}
