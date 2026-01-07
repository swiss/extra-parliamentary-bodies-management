namespace Bk.APG.Business.Models;

public class WorklistFilterParameters
{
    public string? Committee { get; set; }
    public IEnumerable<Guid>? DepartmentIds { get; set; }
    public IEnumerable<Guid>? OfficeIds { get; set; }
    public IEnumerable<Guid>? WorklistTaskStateIds { get; set; }
    public IEnumerable<Guid>? WorklistTaskTypeIds { get; set; }
    public string? AssignedBy { get; set; }
    public string? AssignedTo { get; set; }
    public DateTime? CreatedFrom { get; set; }
    public DateTime? CreatedTo { get; set; }
    public DateOnly? DueDateFrom { get; set; }
    public DateOnly? DueDateTo { get; set; }
}
