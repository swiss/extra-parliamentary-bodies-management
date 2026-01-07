using System.ComponentModel.DataAnnotations;
using Bk.APG.Business.Dtos;
using Bk.APG.Business.Policies;
using Bk.APG.Business.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Bk.APG.Api.Controllers;

[ApiController]
[Route("api/contactpoints")]
[Authorize(Policy = APGPolicies.RequireAllowRole)]
public class ContactPointController : ControllerBase
{
    private readonly IContactPointService _contactPointService;

    public ContactPointController(IContactPointService contactPointService)
    {
        _contactPointService = contactPointService;
    }

    [HttpGet("{id:guid}/list")]
    public async Task<ActionResult> GetAllByCommitteeId(Guid id)
    {
        var results = await _contactPointService.GetContactPointListByCommitteeId(id);
        return Ok(results);
    }

    [HttpGet("{committeeId:guid}/create")]
    [Authorize(Policy = APGPolicies.RequireAdminDepartmentOfficeOrSecretariatRole)]
    public async Task<ActionResult> GetEmpty(Guid committeeId)
    {
        var result = await _contactPointService.GetEmpty(committeeId);
        return Ok(result);
    }

    [HttpGet("{id:guid}/update")]
    [Authorize(Policy = APGPolicies.RequireAdminDepartmentOfficeOrSecretariatRole)]
    public async Task<ActionResult> GetByIdForUpdate(Guid id)
    {
        var result = await _contactPointService.GetByIdForUpdate(id);
        return Ok(result);
    }

    [HttpPut("{id:guid}")]
    [Authorize(Policy = APGPolicies.RequireAdminDepartmentOfficeOrSecretariatRole)]
    public async Task<ActionResult> Update([FromRoute] Guid id, [FromBody, Required] ContactPointUpdateDto updateDto)
    {
        await _contactPointService.Update(id, updateDto);
        return Ok();
    }

    [HttpPost()]
    [Authorize(Policy = APGPolicies.RequireAdminDepartmentOfficeOrSecretariatRole)]
    public async Task<ActionResult> Create([FromBody, Required] ContactPointCreateDto createDto)
    {
        var contactPoint = await _contactPointService.Create(createDto);
        return Ok(contactPoint);
    }

    [HttpDelete("{id:guid}")]
    [Authorize(Policy = APGPolicies.RequireAdminDepartmentOfficeOrSecretariatRole)]
    public async Task<ActionResult> Delete(Guid id)
    {
        await _contactPointService.Delete(id);
        return Ok();
    }
}
