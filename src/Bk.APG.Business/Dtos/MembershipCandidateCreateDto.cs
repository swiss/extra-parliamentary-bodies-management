namespace Bk.APG.Business.Dtos;

public class MembershipCandidateCreateDto
{
    public required Guid CommitteeId { get; set; }
    public Guid? PersonId { get; set; }
    public required string Surname { get; set; }
    public required string GivenName { get; set; }
    public int BirthYear { get; set; }
    public required Guid GenderId { get; set; }
    public required Guid LanguageId { get; set; }
    public required Guid FunctionId { get; set; }
    public string? Remarks { get; set; }
    public string? RemarksStatus { get; set; }
}
