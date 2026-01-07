namespace Bk.APG.Business.Dtos;

public class CandidateListValidationRequest
{
    public IEnumerable<Guid> SelectedCandidateIds { get; set; } = [];
    public bool DuplicateCheckConfirmed { get; set; }
}
