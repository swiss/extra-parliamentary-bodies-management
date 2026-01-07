using Bk.APG.Business.Policies;
using Bk.APG.Business.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Bk.APG.Api.Controllers;

[ApiController]
[Route("api/apgGeneralSettings")]
[Authorize(Policy = APGPolicies.RequireAdminRole)]
public class ApgGeneralSettingsController : ControllerBase
{
    private readonly IApgGeneralSettingsService _apgGeneralSettingsService;

    public ApgGeneralSettingsController(IApgGeneralSettingsService apgGeneralSettingsService)
    {
        _apgGeneralSettingsService = apgGeneralSettingsService;
    }

    [HttpGet("")]
    public async Task<IActionResult> GetCurrentOgdExportSettings()
    {
        var result = await _apgGeneralSettingsService.GetCurrentOgdExportSetting();
        return Ok(result);
    }

    [HttpPut]
    public async Task<ActionResult> UpdateOgdExportSettings([FromBody] bool enabled)
    {
        var settings = await _apgGeneralSettingsService.UpdateApgGeneralSettings(enabled);
        return Ok(settings);
    }
}
