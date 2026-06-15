using Bk.APG.Business.Dtos;
using Bk.APG.Business.Policies;
using Bk.APG.Business.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using IAuthorizationService = Bk.APG.Business.Services.IAuthorizationService;

namespace Bk.APG.Api.Controllers;

[ApiController]
[Route("api/general-measures")]
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
        ArgumentNullException.ThrowIfNull(generalMeasureUpdate);

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

    [HttpPost("{departmentId:guid}/forward")]
    public async Task<IActionResult> Forward(Guid departmentId, [FromBody] GeneralMeasureForwardDto forwardDto)
    {
        ArgumentNullException.ThrowIfNull(forwardDto);

        if (forwardDto.ForwardToAdmin)
        {
            if (!_authorizationService.IsDepartment)
            {
                return Forbid();
            }

            var department = await _authorizationService.GetDepartment();
            if (department?.Id != departmentId)
            {
                return Forbid();
            }
        }
        else if (!_authorizationService.IsAdmin)
        {
            return Forbid();
        }

        await _generalMeasureService.Forward(departmentId, forwardDto.Message, forwardDto.ForwardToAdmin);

        return NoContent();
    }

    [HttpPost("{departmentId:guid}/validate")]
    public async Task<IActionResult> ValidateGeneralMeasure(Guid departmentId)
    {
        if (!_authorizationService.IsAdmin)
        {
            return Forbid();
        }

        await _generalMeasureService.Validate(departmentId);

        return NoContent();
    }
}
