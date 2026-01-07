namespace Bk.APG.Business.Dtos;

public class CandidateListForwardDto
{
    public required Guid ForwardToId { get; set; }
    public required string Description { get; set; }
    public required List<Guid> CandidateIds { get; set; }
}
