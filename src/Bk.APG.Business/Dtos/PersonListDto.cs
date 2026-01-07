namespace Bk.APG.Business.Dtos;

public class PersonListDto
{
    public required Guid Id { get; init; }
    public required string Surname { get; init; }
    public required string GivenName { get; init; }
    public required bool HasActiveMembership { get; init; }
    public required int BirthYear { get; init; }
    public string? Canton { get; init; }
    public string? City { get; init; }
    public required string Language { get; init; }
    public bool NeedsAttention { get; init; }
}
