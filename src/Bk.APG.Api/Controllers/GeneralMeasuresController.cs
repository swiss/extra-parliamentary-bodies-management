using Bk.APG.Business.Dtos;
using Bk.APG.Business.Policies;
using Bk.APG.Business.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using IAuthorizationService = Bk.APG.Business.Services.IAuthorizationService;

namespace Bk.APG.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Policy = APGPolicies.RequireAdminDepartmentRole)]
public class GeneralMeasuresController : ControllerBase
{
    private readonly IGeneralMeasureService _generalMeasureService;
    private readonly IAuthorizationService _authorizationService;

    public GeneralMeasuresController(IGeneralMeasureService generalMeasureService, IAuthorizationService authorizationService)
    {
        _generalMeasureService = generalMeasureService;
        _authorizationService = authorizationService;
    }

    [HttpGet]
    public async Task<IActionResult> GetGeneralMeasures()
    {
        var generalMeasures = await _generalMeasureService.GetGeneralMeasures();

        return Ok(generalMeasures);
    }

    [HttpPut]
    public async Task<IActionResult> UpdateGeneralMeasure([FromBody] GeneralMeasureUpdateDto generalMeasureUpdate)
    {
        if (_authorizationService.IsDepartment)
        {
            var department = await _authorizationService.GetDepartment();
            if (department?.Id != generalMeasureUpdate.DepartmentId)
            {
                return Forbid();
            }
        }

        await _generalMeasureService.AddOrUpdateGeneralMeasure(generalMeasureUpdate);

        return NoContent();
    }
}
