using System.ComponentModel.DataAnnotations;
using Bk.APG.Business.Dtos;
using Bk.APG.Business.Policies;
using Bk.APG.Business.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Bk.APG.Api.Controllers;

[ApiController]
[Route("api/committeeTypes")]
[Authorize(Policy = APGPolicies.RequireAdminRole)]
public class CommitteeTypeController : ControllerBase
{
    private readonly ICommitteeTypeService _committeeTypeService;

    public CommitteeTypeController(ICommitteeTypeService committeeTypeService)
    {
        _committeeTypeService = committeeTypeService;
    }

    [HttpGet("list")]
    public async Task<ActionResult> GetAll()
    {
        var committeeTypes = await _committeeTypeService.GetCommitteeTypeList();
        return Ok(committeeTypes);
    }

    [HttpGet("{id:guid}/update")]
    public async Task<ActionResult> GetByIdForUpdate(Guid id)
    {
        var result = await _committeeTypeService.GetCommitteeTypeForUpdate(id);
        return Ok(result);
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult> Update([FromRoute] Guid id, [FromBody, Required] CommitteeTypeUpdateDto updateDto)
    {
        if (id != updateDto.Id)
        {
            return BadRequest();
        }

        var committee = await _committeeTypeService.UpdateCommitteeType(id, updateDto);
        return Ok(committee);
    }
}
