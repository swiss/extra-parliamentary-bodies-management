namespace Bk.APG.Business.Dtos;

public class CandidateListExportRequestDto
{
    public List<Guid> MembershipCandidateIds { get; set; } = new();
}
