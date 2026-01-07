using System.ComponentModel.DataAnnotations;
using Bk.APG.Business.Dtos;
using Bk.APG.Business.Models;
using Bk.APG.Business.Policies;
using Bk.APG.Business.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Bk.APG.Api.Controllers;

[ApiController]
[Route("api/worklist-tasks")]
[Authorize(Policy = APGPolicies.RequireAllowRole)]
public class WorklistTaskController : ControllerBase
{
    private readonly IWorklistTaskService _worklistTaskService;
    private readonly IGeneralElectionService _generalElectionService;

    public WorklistTaskController(IWorklistTaskService worklistTaskService, IGeneralElectionService generalElectionService)
    {
        _worklistTaskService = worklistTaskService;
        _generalElectionService = generalElectionService;
    }

    [HttpGet]
    public async Task<ActionResult> GetAll(
        [FromQuery, Required] PagingParametersDto pagingParameters,
        [FromQuery] WorklistFilterParametersDto? filterParameters,
        [FromQuery] SortParametersDto sortParameters)
    {
        var worklistTasks = await _worklistTaskService.GetWorklistTasks(pagingParameters, filterParameters, sortParameters.Sort, sortParameters.Direction);
        return Ok(worklistTasks);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult> GetByIdForUpdate(Guid id)
    {
        var result = await _worklistTaskService.GetWorklistTaskForUpdate(id);
        return Ok(result);
    }

    [HttpPost]
    [Authorize(Policy = APGPolicies.RequireAdminRole)]
    public async Task<ActionResult> CreateWorklistTask([FromBody] WorklistTaskCreateDto worklistTaskCreateDto)
    {
        if (worklistTaskCreateDto.WorklistTaskTypeId == WorklistTaskType.GeneralElectionStart)
        {
            var generalElectionResult = await _generalElectionService.PrepareGeneralElection(worklistTaskCreateDto);
            return Ok(generalElectionResult);
        }

        await _worklistTaskService.CreateWorklistTaskByAdmin(worklistTaskCreateDto);
        return Ok();
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult> Update([FromRoute] Guid id, [FromBody, Required] WorklistTaskUpdateDto updateDto)
    {
        if (id != updateDto.Id)
        {
            return BadRequest();
        }

        var result = await _worklistTaskService.UpdateWorklistTask(id, updateDto);
        return Ok(result);
    }

    [HttpPost("{id:guid}/forward")]
    public async Task<ActionResult> Forward([FromRoute] Guid id, [FromBody, Required] WorklistTaskForwardDto forwardDto)
    {
        await _worklistTaskService.ForwardWorklistTask(id, forwardDto);
        return Ok();
    }
}
