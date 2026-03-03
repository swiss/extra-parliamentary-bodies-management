namespace Bk.APG.Business.Dtos;

public class AddressDetailsDto
{
    public required Guid Id { get; init; }
    public string? CompanyName { get; init; }
    public string? Street { get; init; }
    public string? PoBox { get; init; }
    public string? Country { get; init; }
    public string? Zip { get; init; }
    public string? City { get; init; }
    public string? Canton { get; init; }
    public string? Phone { get; init; }
    public string? Mobile { get; init; }
    public string? Email { get; init; }
    public bool ActiveAddress { get; init; }
}
