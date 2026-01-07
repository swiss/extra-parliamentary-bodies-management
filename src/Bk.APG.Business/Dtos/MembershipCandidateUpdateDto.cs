namespace Bk.APG.Business.Dtos;

public class MembershipCandidateUpdateDto
{
    public required Guid Id { get; set; }
    public Guid? PersonId { get; set; }
    public string? Surname { get; set; }
    public string? GivenName { get; set; }
    public int? BirthYear { get; set; }
    public Guid? GenderId { get; set; }
    public Guid? LanguageId { get; set; }
    public int? MaximumEmploymentLevel { get; set; }
    public required DateOnly BeginDate { get; set; }
    public required DateOnly EndDate { get; set; }
    public required Guid ElectionTypeId { get; init; }
    public required Guid FunctionId { get; init; }
    public required Guid ElectionOfficeId { get; set; }
    public Guid? MembershipAdditionId { get; set; }
    public bool InCorrelationWithFederalDuty { get; set; }
    public string? JustificationLongerDuty { get; set; }
    public string? JustificationShorterDuty { get; set; }
    public string? JustificationMemberInFederalDuty { get; set; }
    public string? JustificationMemberInFederalAssembly { get; set; }
    public string? RequirementsProfile { get; set; }
    public required uint RowVersion { get; init; }
}
