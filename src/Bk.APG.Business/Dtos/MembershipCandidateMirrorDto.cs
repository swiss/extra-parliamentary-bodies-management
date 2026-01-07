namespace Bk.APG.Business.Dtos;

public class MembershipCandidateMirrorDto
{
    public int? MaximumEmploymentLevel { get; set; }
    public Guid ElectionTypeId { get; set; }
    public Guid FunctionId { get; set; }
    public Guid ElectionOfficeId { get; set; }
    public Guid? MembershipAdditionId { get; set; }
    public string? Remarks { get; set; }
    public string? RemarksStatus { get; set; }
    public bool InCorrelationWithFederalDuty { get; set; }
    public required DateTime Modified { get; set; }
    public required string ModifiedBy { get; set; }
}
