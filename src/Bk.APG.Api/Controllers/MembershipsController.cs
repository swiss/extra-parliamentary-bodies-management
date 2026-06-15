using System.ComponentModel.DataAnnotations;
using Bk.APG.Business.Dtos;
using Bk.APG.Business.Policies;
using Bk.APG.Business.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Bk.APG.Api.Controllers;

[ApiController]
[Route("api/memberships")]
[Authorize(Policy = APGPolicies.RequireAllowRole)]
public class MembershipsController : ControllerBase
{
    private readonly IMembershipService _membershipService;
    public MembershipsController(IMembershipService membershipService)
    {
        _membershipService = membershipService;
    }

    [HttpPost("member")]
    [Authorize(Policy = APGPolicies.RequireAdminDepartmentOfficeOrSecretariatRole)]
    public async Task<ActionResult> CreateMember([FromBody, Required] MembershipCreateDto createDto)
    {
        var membership = await _membershipService.CreateMembership(createDto);

        return Ok(membership);
    }

    [HttpGet("{id:guid}/update")]
    [Authorize(Policy = APGPolicies.RequireAdminDepartmentOfficeOrSecretariatRole)]
    public async Task<ActionResult> GetByIdForUpdate(Guid id)
    {
        var result = await _membershipService.GetMembershipForUpdate(id);
        return Ok(result);
    }

    [HttpPut("{id:guid}")]
    [Authorize(Policy = APGPolicies.RequireAdminDepartmentOfficeOrSecretariatRole)]
    public async Task<ActionResult> Update([FromRoute] Guid id, [FromBody, Required] MembershipUpdateDto updateDto)
    {
        var person = await _membershipService.UpdateMembership(id, updateDto);

        return Ok(person);
    }

    [HttpDelete("{id:guid}")]
    [Authorize(Policy = APGPolicies.RequireAdminDepartmentOfficeOrSecretariatRole)]
    public async Task<ActionResult> DeleteMembership(Guid id)
    {
        await _membershipService.DeleteMembership(id);

        return Ok();
    }
}
