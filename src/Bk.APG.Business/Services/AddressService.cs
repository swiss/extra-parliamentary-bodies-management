using Bk.APG.Business.Dtos;
using Microsoft.Extensions.Logging;

namespace Bk.APG.Business.Services;

public class AddressService : IAddressService
{
    private readonly ILogger<AddressService> _logger;
    private readonly IPostService _postService;

    public AddressService(ILogger<AddressService> logger, IPostService postService)
    {
        _logger = logger;
        _postService = postService;
    }

    public async Task<IEnumerable<AddressDto>> Search(AddressSearchDto search)
    {
        try
        {
            return await _postService.Search(search);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Failed to retrieve address search-result from Post API");
            return [];
        }
    }

    public async Task<AddressVerificationResultDto> Verify(AddressSearchDto search)
    {
        try
        {
            return await _postService.Verify(search);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Failed to verify address");
            return new AddressVerificationResultDto { Status = AddressVerificationStatus.Invalid };
        }
    }
}
