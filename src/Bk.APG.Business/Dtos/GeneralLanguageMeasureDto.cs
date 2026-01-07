namespace Bk.APG.Business.Dtos;

public class GeneralLanguageMeasureDto
{
    public required Guid Id { get; set; }
    public required Guid DepartmentId { get; set; }
    public required string Description { get; set; }
}
