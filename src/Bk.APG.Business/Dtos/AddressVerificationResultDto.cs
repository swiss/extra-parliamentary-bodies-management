namespace Bk.APG.Business.Dtos;

public enum AddressVerificationStatus
{
    Ok,
    Corrected,
    Ambiguous,
    Invalid
}

public class AddressVerificationResultDto
{
    /// <summary>
    /// Holds value when<see cref="Status"/> is Ok or Corrected
    /// </summary>
    public AddressDto? Address { get; init; }
    public AddressVerificationStatus Status { get; init; }
}
