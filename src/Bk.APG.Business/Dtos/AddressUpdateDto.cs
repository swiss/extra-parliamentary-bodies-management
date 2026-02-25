namespace Bk.APG.Business.Dtos;

public class AddressUpdateDto
{
    public Guid? Id { get; init; }
    public string? CompanyName { get; init; }
    public string? Street { get; init; }
    public string? PoBox { get; init; }
    public Guid? CountryId { get; init; }
    public string? Zip { get; init; }
    public string? City { get; init; }
    public Guid? CantonId { get; init; }
    public string? Phone { get; init; }
    public string? Mobile { get; init; }
    public string? Email { get; init; }
    public bool ActiveAddress { get; init; }
}
