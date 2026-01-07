namespace Bk.APG.Business.Dtos;

public class WorklistTaskForwardDto
{
    public required DateOnly CandidateListDueDate { get; set; }
    public required string CandidateListDescription { get; set; }
    public required DateOnly CommitteeDueDate { get; set; }
    public required string CommitteeDescription { get; set; }
}
