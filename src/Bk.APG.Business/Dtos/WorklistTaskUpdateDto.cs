namespace Bk.APG.Business.Dtos;

public class WorklistTaskUpdateDto
{
    public required Guid Id { get; init; }
    public required string WorklistTaskType { get; set; }
    public required string WorklistTaskState { get; set; }
    public required string AssignedTo { get; set; }
    public required string AssignedBy { get; set; }
    public string? Description { get; set; }
    public required DateOnly DueDate { get; set; }
    public bool CanEdit { get; set; }
    public bool CanForward { get; set; }
    public bool IsBigDepartment { get; set; }
}
