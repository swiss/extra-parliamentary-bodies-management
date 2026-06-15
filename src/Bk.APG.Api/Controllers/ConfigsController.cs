using Bk.APG.CrossCutting.Configuration;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace Bk.APG.Api.Controllers;

[ApiController]
[Route("api/configs")]
public class ConfigsController : ControllerBase
{
    private readonly FrontendOptions _frontendOptions;

    public ConfigsController(IOptions<FrontendOptions> frontendOptions)
    {
        ArgumentNullException.ThrowIfNull(frontendOptions);

        _frontendOptions = frontendOptions.Value;
    }

    [HttpGet("frontend")]
    public Task<ActionResult> GetFrontendOptions()
    {
        return Task.FromResult<ActionResult>(Ok(_frontendOptions));
    }
}
