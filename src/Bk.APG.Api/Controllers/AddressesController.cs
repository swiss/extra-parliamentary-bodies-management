using Bk.APG.Business.Dtos;
using Bk.APG.Business.Services;
using Microsoft.AspNetCore.Mvc;

namespace Bk.APG.Api.Controllers;

[ApiController]
[Route("api/addresses")]
public class AddressesController : ControllerBase
{
    private readonly IAddressService _addressService;

    public AddressesController(IAddressService addressService)
    {
        _addressService = addressService;
    }

    [HttpGet("search")]
    public async Task<IActionResult> Search([FromQuery] AddressSearchDto dto)
    {
        var results = await _addressService.Search(dto);
        return Ok(results);
    }

    [HttpGet("verify")]
    public async Task<IActionResult> Verify([FromQuery] AddressSearchDto dto)
    {
        var results = await _addressService.Verify(dto);
        return Ok(results);
    }
}
