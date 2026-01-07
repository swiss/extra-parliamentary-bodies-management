namespace Bk.APG.Business.Models;

public class GeneralGenderMeasure : EntityBase
{
    public int OgdId { get; set; }
    public required string Description { get; set; }
    public Department? Department { get; set; }
    public required Guid DepartmentId { get; set; }
}
