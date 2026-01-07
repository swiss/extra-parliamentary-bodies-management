namespace Bk.APG.Business.Dtos;

public class LegalFormDto
{
    public required Guid Id { get; init; }
    public required string LegalFormId { get; init; }
    public required string Text { get; init; }
    public required string Description { get; init; }
}
