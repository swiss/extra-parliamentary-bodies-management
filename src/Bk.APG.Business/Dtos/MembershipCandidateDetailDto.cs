namespace Bk.APG.Business.Dtos;

public class MembershipCandidateDetailDto
{
    public required Guid Id { get; init; }
    public Guid? PersonId { get; set; }
    public required string Surname { get; init; }
    public required string GivenName { get; init; }
    public required int BirthYear { get; init; }
    public required string Language { get; init; }
    public required string Gender { get; init; }
    public required string Function { get; init; }
    public required Guid FunctionId { get; init; }
    public required DateOnly BeginDate { get; init; }
    public DateOnly? EndDate { get; init; }
    public required Guid ElectionTypeId { get; init; }
    public required string ElectionType { get; init; }
    public required string MembershipAddition { get; init; }
    public string? Remarks { get; set; }
    public string? RemarksStatus { get; set; }
    public bool NeedsAttention { get; init; }
    public bool IsSelected { get; set; }
    public int EstimatedTermOfOffice { get; init; }
}
