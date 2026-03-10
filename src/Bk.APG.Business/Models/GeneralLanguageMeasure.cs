namespace Bk.APG.Business.Models;

public class GeneralLanguageMeasure : EntityBase
{
    public required string Description { get; set; }
    public Department? Department { get; set; }
    public required Guid DepartmentId { get; set; }
}
