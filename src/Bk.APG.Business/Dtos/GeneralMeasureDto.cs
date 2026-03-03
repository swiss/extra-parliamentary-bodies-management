namespace Bk.APG.Business.Dtos;

public class GeneralMeasureDto
{
    public required Guid DepartmentId { get; init; }
    public required string Department { get; init; }
    public string? JustificationLanguages { get; init; }
    public string? JustificationGenders { get; init; }
    public required bool IsDepartmentTaskActive { get; init; }
    public required bool IsAdminTaskActive { get; init; }
    public required bool CanForwardToAdmin { get; init; }
    public required bool CanValidate { get; init; }
    public required bool CanForwardToDepartment { get; init; }
}
