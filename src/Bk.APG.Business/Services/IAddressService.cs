using Bk.APG.Business.Dtos;

namespace Bk.APG.Business.Services;

public interface IAddressService
{
    Task<IEnumerable<AddressDto>> Search(AddressSearchDto search);
    Task<AddressVerificationResultDto> Verify(AddressSearchDto search);
}
