namespace Bk.APG.Business.Dtos;

public class EiamAssignmentDto
{
    public required Guid Id { get; init; }
    public required string Text { get; init; }
    public Guid? DepartmentId { get; init; }
}
