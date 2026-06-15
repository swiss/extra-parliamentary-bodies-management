using System.ComponentModel.DataAnnotations;
using Bk.APG.Business.Dtos;
using Bk.APG.Business.Policies;
using Bk.APG.Business.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Bk.APG.Api.Controllers;

[ApiController]
[Route("api/appointment-decisions")]
[Authorize(Policy = APGPolicies.RequireAllowRole)]
public class AppointmentDecisionsController : ControllerBase
{
    private readonly IAppointmentDecisionService _appointmentDecisionService;
    private readonly IDocumentService _documentService;

    public AppointmentDecisionsController(IAppointmentDecisionService appointmentDecisionService, IDocumentService documentService)
    {
        _appointmentDecisionService = appointmentDecisionService;
        _documentService = documentService;
    }

    [HttpGet("{id:guid}/list")]
    public async Task<ActionResult> GetAllByCommitteeId(Guid id)
    {
        var results = await _appointmentDecisionService.GetAppointmentDecisionListByCommitteeId(id);
        return Ok(results);
    }

    [HttpGet("{id:guid}/update")]
    public async Task<ActionResult> GetAppointmentDecisionForUpdate(Guid id)
    {
        var results = await _appointmentDecisionService.GetByIdForUpdate(id);
        return Ok(results);
    }

    [HttpPost]
    [Authorize(Policy = APGPolicies.RequireAdminDepartmentOfficeOrSecretariatRole)]
    public async Task<ActionResult> CreateAppointmentDecision([FromForm, Required] AppointmentDecisionCreateDto createDto)
    {
        var appointmentDecision = await _appointmentDecisionService.CreateAppointmentDecision(createDto);

        return Ok(appointmentDecision);
    }

    [HttpPut("{id:guid}")]
    [Authorize(Policy = APGPolicies.RequireAdminDepartmentOfficeOrSecretariatRole)]
    public async Task<ActionResult> UpdateAppointmentDecision([FromRoute] Guid id, [FromForm, Required] AppointmentDecisionUpdateDto updateDto)
    {
        ArgumentNullException.ThrowIfNull(updateDto);

        if (id != updateDto.Id)
        {
            return BadRequest();
        }
        var appointmentDecision = await _appointmentDecisionService.UpdateAppointmentDecision(id, updateDto);

        return Ok(appointmentDecision);
    }

    [HttpDelete("{id:guid}")]
    [Authorize(Policy = APGPolicies.RequireAdminDepartmentOfficeOrSecretariatRole)]
    public async Task<ActionResult> DeleteAppointmentDecision(Guid id)
    {
        await _appointmentDecisionService.DeleteAppointmentDecision(id);

        return Ok();
    }

    [HttpGet("create")]
    [Authorize(Policy = APGPolicies.RequireAdminDepartmentOfficeOrSecretariatRole)]
    public ActionResult GetEmpty()
    {
        var dto = _appointmentDecisionService.GetEmpty();

        return Ok(dto);
    }

    [HttpGet("document")]
    public async Task<IActionResult> GetDocumentFromStorage([FromQuery, Required] string id)
    {
        var fileStream = await _documentService.GetDocument(id);

        return fileStream is not null
            ? File(fileStream, "application/pdf", id)
                : NotFound();
    }
}
