using System.Text;
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
        var displayVersion = new StringBuilder();
        var version = Environment.GetEnvironmentVariable(ApplicationVersionEnvVariableName);

        var parts = version?.Split('_') ?? [];

        if (parts.Length >= 2)
        {
            displayVersion.Append(parts[0]);
            displayVersion.Append(" (");
            displayVersion.Append(string.Join("_", parts[1..]));
            displayVersion.Append(')');
        }
        else
        {
            displayVersion.Append(version); //Fallback
        }

        return Ok(new VersionDto { ApplicationVersion = displayVersion.ToString() });
    }
}
