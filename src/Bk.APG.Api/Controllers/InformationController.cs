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

        var parts = version?.Split('_') ?? [];
        if (parts.Length == 2)
        {
            version = $"{parts[0]} ({parts[1]})";
        }

        return Ok(new VersionDto { ApplicationVersion = version });
    }
}
