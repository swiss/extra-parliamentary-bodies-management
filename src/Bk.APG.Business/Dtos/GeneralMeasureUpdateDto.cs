namespace Bk.APG.Business.Dtos;

public class GeneralMeasureUpdateDto
{
    public required Guid DepartmentId { get; init; }
    public string? JustificationLanguages { get; init; }
    public string? JustificationGenders { get; init; }
}
