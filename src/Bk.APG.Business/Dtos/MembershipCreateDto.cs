namespace Bk.APG.Business.Dtos;

public class MembershipCreateDto
{
    public Guid CommitteeId { get; set; }
    public Guid PersonId { get; set; }
    public int? MaximumEmploymentLevel { get; set; }
    public DateOnly BeginDate { get; set; }
    public DateOnly EndDate { get; set; }
    public Guid ElectionTypeId { get; set; }
    public Guid FunctionId { get; set; }
    public Guid ElectionOfficeId { get; set; }
    public Guid? MembershipAdditionId { get; set; }
    public string? JustificationLongerDuty { get; set; }
    public string? JustificationShorterDuty { get; set; }
    public string? JustificationMemberInFederalDuty { get; set; }
    public string? JustificationMemberInFederalAssembly { get; set; }
    public string? RequirementsProfile { get; set; }
    public string? Remarks { get; set; }
    public string? RemarksStatus { get; set; }
    public bool InCorrelationWithFederalDuty { get; set; }
}
