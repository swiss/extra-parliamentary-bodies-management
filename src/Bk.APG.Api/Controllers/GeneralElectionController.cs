using Bk.APG.Business.Policies;
using Bk.APG.Business.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Bk.APG.Api.Controllers;

[ApiController]
[Route("api/general-election")]
[Authorize(Policy = APGPolicies.RequireAllowRole)]
public class GeneralElectionController : ControllerBase
{
    private readonly IGeneralElectionService _generalElectionService;

    public GeneralElectionController(IGeneralElectionService generalElectionService)
    {
        _generalElectionService = generalElectionService;
    }

    [HttpGet("toggle-available")]
    public async Task<IActionResult> GetIsGeneralElectionToggleAvailable()
    {
        var result = await _generalElectionService.IsGeneralElectionToggleAvailable();
        return Ok(result);
    }
}
