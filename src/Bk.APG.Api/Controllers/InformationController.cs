using Bk.APG.Business.Dtos;
using Microsoft.AspNetCore.Mvc;

namespace Bk.APG.Api.Controllers;

[ApiController]
[Route("api/info")]
public class InformationController : ControllerBase
{
    private const string ApplicationVersionEnvVariableName = "APPLICATION_VERSION";

    [HttpGet("version")]
    public ActionResult GetApplicationVersion()
    {
        var version = Environment.GetEnvironmentVariable(ApplicationVersionEnvVariableName);

        return Ok(new VersionDto { ApplicationVersion = version });
    }
}
