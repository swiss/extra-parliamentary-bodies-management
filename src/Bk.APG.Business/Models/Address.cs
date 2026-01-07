namespace Bk.APG.Business.Models;

public class Address : EntityBase
{
    public string? CompanyName { get; set; }
    public string? Street { get; set; }
    public string? PoBox { get; set; }
    public string? CountryCode { get; set; }
    public string? Zip { get; set; }
    public string? City { get; set; }
    public Canton? Canton { get; set; }
    public Guid? CantonId { get; set; }
    public string? Phone { get; set; }
    public string? Mobile { get; set; }
    public string? Email { get; set; }
    public int OldId { get; set; }
    public bool VerifiedSuccessfully { get; set; }
    public int? VerificationCode { get; set; }
}
