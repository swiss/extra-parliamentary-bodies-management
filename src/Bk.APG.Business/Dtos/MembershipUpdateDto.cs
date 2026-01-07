namespace Bk.APG.Business.Dtos;

public class MembershipUpdateDto
{
    public required Guid Id { get; set; }
    public required Guid PersonId { get; set; }
    public required Guid CommitteeId { get; set; }
    public int? MaximumEmploymentLevel { get; set; }
    public required DateOnly BeginDate { get; set; }
    public required DateOnly EndDate { get; set; }
    public required Guid ElectionTypeId { get; set; }
    public required Guid FunctionId { get; set; }
    public string? FunctionName { get; set; }
    public required Guid ElectionOfficeId { get; set; }
    public string? ElectionOfficeName { get; set; }
    public string? OldMembershipAddition { get; set; }
    public MembershipAdditionDto? MembershipAddition { get; set; }
    public Guid? MembershipAdditionId { get; set; }
    public string? JustificationLongerDuty { get; set; }
    public string? JustificationShorterDuty { get; set; }
    public string? JustificationMemberInFederalDuty { get; set; }
    public string? JustificationMemberInFederalAssembly { get; set; }
    public string? RequirementsProfile { get; set; }
    public string? Remarks { get; set; }
    public string? RemarksStatus { get; set; }
    public bool InCorrelationWithFederalDuty { get; set; }
    public required uint RowVersion { get; init; }
    public bool CanEdit { get; set; }
    public bool CanEditBeginDate { get; set; }
    public bool CanDelete { get; set; }
}
