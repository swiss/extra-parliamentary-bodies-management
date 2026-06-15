using System.ComponentModel.DataAnnotations;
using Bk.APG.Business.Dtos;
using Bk.APG.Business.Policies;
using Bk.APG.Business.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Bk.APG.Api.Controllers;

[ApiController]
[Route("api/general-election/membership-candidates")]
public class MembershipCandidatesController : ControllerBase
{
    private readonly IMembershipCandidateService _membershipCandidateService;

    public MembershipCandidatesController(IMembershipCandidateService membershipCandidateService)
    {
        _membershipCandidateService = membershipCandidateService;
    }

    [HttpGet("{id:guid}/update")]
    [Authorize(Policy = APGPolicies.RequireAdminDepartmentOfficeOrSecretariatRole)]
    public async Task<ActionResult> GetByMembershipCandidateIdForUpdate(Guid id)
    {
        var result = await _membershipCandidateService.GetMembershipCandidateForUpdate(id);
        return Ok(result);
    }

    [HttpPatch("{id:guid}")]
    [Authorize(Policy = APGPolicies.RequireAdminDepartmentOfficeOrSecretariatRole)]
    public async Task<IActionResult> PartialUpdateMembershipCandidate([FromRoute] Guid id, [FromBody, Required] MembershipCandidatePartialUpdateDto membershipCandidatePartialUpdate)
    {
        await _membershipCandidateService.PartialUpdateMembershipCandidate(id, membershipCandidatePartialUpdate);

        return NoContent();
    }

    [HttpPut("{id:guid}")]
    [Authorize(Policy = APGPolicies.RequireAdminDepartmentOfficeOrSecretariatRole)]
    public async Task<IActionResult> UpdateMembershipCandidate([FromRoute] Guid id, [FromBody, Required] MembershipCandidateUpdateDto membershipCandidateUpdate)
    {
        ArgumentNullException.ThrowIfNull(membershipCandidateUpdate);

        if (membershipCandidateUpdate.PersonId is null && (string.IsNullOrWhiteSpace(membershipCandidateUpdate.GivenName)
                                                           || string.IsNullOrWhiteSpace(membershipCandidateUpdate.Surname)
                                                           || membershipCandidateUpdate.GenderId is null
                                                           || membershipCandidateUpdate.LanguageId is null))
        {
            return BadRequest("Either PersonId must be provided or all of GivenName, Surname, GenderId, and LanguageId must be provided.");
        }

        await _membershipCandidateService.UpdateMembershipCandidate(id, membershipCandidateUpdate);

        return NoContent();
    }

    [HttpPost]
    [Authorize(Policy = APGPolicies.RequireAdminDepartmentOfficeOrSecretariatRole)]
    public async Task<IActionResult> CreateMembershipCandidate([FromBody, Required] MembershipCandidateCreateDto membershipCandidateCreate)
    {
        var membershipCandidate = await _membershipCandidateService.CreateMembershipCandidate(membershipCandidateCreate);

        return Ok(membershipCandidate);
    }

    [HttpDelete("{id:guid}")]
    [Authorize(Policy = APGPolicies.RequireAdminDepartmentOfficeOrSecretariatRole)]
    public async Task<IActionResult> DeleteMembershipCandidate(Guid id)
    {
        await _membershipCandidateService.DeleteMembershipCandidate(id);

        return Ok();
    }
}
