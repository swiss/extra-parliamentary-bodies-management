namespace Bk.APG.Business.Dtos;

public class WorklistTaskForwardDto
{
    public required DateOnly CandidateListDueDate { get; set; }
    public string? CandidateListDescription { get; set; }
    public required DateOnly CommitteeDueDate { get; set; }
    public string? CommitteeDescription { get; set; }
}
