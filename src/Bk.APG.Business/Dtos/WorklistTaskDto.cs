namespace Bk.APG.Business.Dtos;

public class WorklistTaskDto
{
    public required Guid Id { get; init; }
    public required string AssignedTo { get; set; }
    public required string AssignedBy { get; set; }
    public string? NavigationUrl { get; set; }
    public Guid? ParentTaskId { get; set; }
    public string? Department { get; set; }
    public string? Office { get; set; }
    public string? Committee { get; set; }
    public required DateOnly DueDate { get; set; }
    public required string WorklistTaskType { get; set; }
    public required string WorklistTaskState { get; set; }
    public required string CreatedBy { get; set; }
    public required DateTime Created { get; set; }
    public bool IsInactive { get; set; }
    public bool IsCompleted { get; set; }
    public bool IsOverdue { get; set; }
}
