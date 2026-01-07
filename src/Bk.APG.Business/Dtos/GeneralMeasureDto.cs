namespace Bk.APG.Business.Dtos;

public class GeneralMeasureDto
{
    public required Guid DepartmentId { get; init; }
    public required string Department { get; init; }
    public string? JustificationLanguages { get; init; }
    public string? JustificationGenders { get; init; }
}
