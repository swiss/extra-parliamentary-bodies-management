namespace Bk.APG.Business.Dtos;

public class MembershipCandidatePartialUpdateDto
{
    public required Guid FunctionId { get; set; }
    public string? Remarks { get; set; }
    public string? RemarksStatus { get; set; }
}
