using Bk.APG.Business.Policies;
using Bk.APG.Business.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Bk.APG.Api.Controllers;

[ApiController]
[Route("api/eiam-assignments")]
[Authorize(Policy = APGPolicies.RequireAllowRole)]
public class EiamAssignmentController : ControllerBase
{
    private readonly IEiamAssignmentService _eiamAssignmentService;

    public EiamAssignmentController(IEiamAssignmentService eiamAssignmentService)
    {
        _eiamAssignmentService = eiamAssignmentService;
    }

    [HttpGet("available")]
    public async Task<ActionResult> GetAvailableEiamAssignments()
    {
        var result = await _eiamAssignmentService.GetAvailableAssignments();
        return Ok(result);
    }

    [HttpGet("current")]
    public async Task<ActionResult> GetCurrentEiamAssignment()
    {
        var result = await _eiamAssignmentService.GetCurrentEiamAssignment();
        return Ok(result);
    }
}
