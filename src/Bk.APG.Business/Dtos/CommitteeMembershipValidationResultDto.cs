namespace Bk.APG.Business.Dtos;

public class CommitteeMembershipValidationResultDto
{
    public required Guid CommitteeId { get; init; }
    public required Guid PersonId { get; init; }
    public bool HasErrors { get; set; }
    public bool TooManyMembers { get; set; }
    public bool IsAlreadyActiveMember { get; set; }
    public bool MaximumDurationExceeded { get; set; }
    public bool IsFederalAssemblyAndAuthoritiesCommission { get; set; }
    public int CurrentTermOfOffice { get; set; }
    public int EstimatedTermOfOffice { get; set; }
}
