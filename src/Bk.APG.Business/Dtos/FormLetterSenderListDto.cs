namespace Bk.APG.Business.Dtos;

public class FormLetterSenderListDto
{
    public required Guid Id { get; init; }
    public required string Description { get; init; }
    public required string Surname { get; init; }
    public required string GivenName { get; init; }
    public required string SenderFunction { get; init; }
    public required string Department { get; init; }
}
