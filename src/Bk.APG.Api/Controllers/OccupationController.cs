using System.ComponentModel.DataAnnotations;
using Bk.APG.Business.Policies;
using Bk.APG.Business.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Bk.APG.Api.Controllers;

[ApiController]
[Route("api/occupations")]
[Authorize(Policy = APGPolicies.RequireAdminRole)]
public class OccupationController : ControllerBase
{
    private readonly IOccupationService _occupationService;

    public OccupationController(IOccupationService occupationService)
    {
        _occupationService = occupationService;
    }

    [HttpGet("GetBySearchString")]
    public async Task<ActionResult> GetBySearchString([FromQuery, Required] string searchString)
    {
        var committeeTypes = await _occupationService.GetBySearchString(searchString);
        return Ok(committeeTypes);
    }
}
