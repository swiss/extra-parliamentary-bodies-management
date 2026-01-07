using System.ComponentModel.DataAnnotations.Schema;

namespace Bk.APG.Business.Models;

public class WorklistTask : EntityBase
{
    public WorklistTask? ParentTask { get; set; }
    public Guid? ParentTaskId { get; set; }
    public ICollection<WorklistTask> ChildTasks { get; set; } = new List<WorklistTask>();

    public EiamAssignment? AssignedTo { get; init; }
    public required Guid AssignedToId { get; init; }
    public EiamAssignment? AssignedBy { get; init; }
    public required Guid AssignedById { get; init; }
    public required DateOnly DueDate { get; set; }
    public required string Description { get; set; }
    public WorklistTaskType? WorklistTaskType { get; set; }
    public required Guid WorklistTaskTypeId { get; set; }
    public WorklistTaskState? WorklistTaskState { get; set; }
    public required Guid WorklistTaskStateId { get; set; }

    // To ensure the responsibility for a certain task, we can attach it to Department and/or Office
    public Department? Department { get; set; }
    public Guid? DepartmentId { get; set; }
    public Office? Office { get; set; }
    public Guid? OfficeId { get; set; }

    // If the tasks connects to the current view, we will link to these fields
    public Committee? Committee { get; set; }
    public Guid? CommitteeId { get; set; }
    public Membership? Membership { get; set; }
    public Guid? MembershipId { get; set; }
    public Person? Person { get; set; }
    public Guid? PersonId { get; set; }

    // If the tasks connects to the GeneralElection view, we will link to these fields
    public GeneralElectionCommittee? GeneralElectionCommittee { get; set; }
    public Guid? GeneralElectionCommitteeId { get; set; }
    public MembershipCandidate? MembershipCandidate { get; set; }
    public Guid? MembershipCandidateId { get; set; }
    public TermOfOfficeDate? TermOfOfficeDate { get; set; }
    public Guid? TermOfOfficeDateId { get; set; }

    public uint RowVersion { get; set; }

    [NotMapped]
    public bool IsOverdue => DateOnly.FromDateTime(DateTime.Now) > DueDate;
}
