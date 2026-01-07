namespace Bk.APG.Business.Dtos;

public class GeneralGenderMeasureDto
{
    public required Guid Id { get; set; }
    public required Guid DepartmentId { get; set; }
    public required string Description { get; set; }
}
