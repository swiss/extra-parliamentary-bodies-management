namespace Bk.APG.Business.Dtos;

public class InterestListDto
{
    public required Guid Id { get; init; }
    public required string Text { get; set; }
    public required string Committee { get; init; }
    public required string Function { get; init; }
    public required string LegalForm { get; init; }
}
