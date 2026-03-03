namespace Bk.APG.Business.Dtos;

public class AddressDto
{
    public required Guid Id { get; init; }
    public string? CompanyName { get; init; }
    public string? Street { get; init; }
    public string? PoBox { get; init; }
    public string? Country { get; init; }
    public string? Zip { get; init; }
    public string? City { get; init; }
    public CantonDto? Canton { get; init; }
    public string? Phone { get; init; }
    public string? Mobile { get; init; }
    public string? Email { get; init; }
}
