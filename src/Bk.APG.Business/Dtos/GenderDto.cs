namespace Bk.APG.Business.Dtos;

public class GenderDto
{
    public required Guid Id { get; init; }
    public required string Text { get; init; }
    public required string Description { get; init; }
    public required int Sort { get; init; }
}
