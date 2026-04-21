using Bk.APG.Business.Policies;
using Bk.APG.Business.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Bk.APG.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Policy = APGPolicies.RequireAllowRole)]
public class OpenDataStackController : ControllerBase
{
    private readonly IOpenDataStackService _openDataStackService;

    public OpenDataStackController(IOpenDataStackService openDataStackService)
    {
        _openDataStackService = openDataStackService;
    }

    [HttpPost("token")]
    public async Task<IActionResult> ExchangeToken()
    {
        var accessToken = await HttpContext.GetTokenAsync("access_token");
        if (string.IsNullOrWhiteSpace(accessToken))
        {
            return Unauthorized();
        }

        var code = await _openDataStackService.ExchangeToken(accessToken);

        return Ok(code);
    }
}
