namespace Bk.APG.Business.Dtos;

public class WorklistTaskCreateDto
{
    public required Guid WorklistTaskTypeId { get; set; }
    public Guid? WorklistTaskStateId { get; set; }
    public string? Description { get; set; }
    public required DateOnly DueDate { get; set; }
    public Guid? AssignedToId { get; set; }
    public Guid? DepartmentId { get; set; }
    public Guid? OfficeId { get; set; }
    public Guid? ParentTaskId { get; set; }
    public Guid? CommitteeId { get; set; }
    public Guid? MembershipId { get; set; }
    public Guid? PersonId { get; set; }
    public Guid? GeneralElectionCommitteeId { get; set; }
    public Guid? CandidateListId { get; set; }
    public Guid? MembershipCandidateId { get; set; }
    public Guid? TermOfOfficeDateId { get; set; }
}
