namespace Bk.APG.Business.Dtos;

public class PersonMinimalDto
{
    public required Guid Id { get; init; }
    public required string GivenName { get; init; }
    public required string Surname { get; init; }
}
