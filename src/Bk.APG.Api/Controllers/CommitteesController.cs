using System.ComponentModel.DataAnnotations;
using Bk.APG.Business.Dtos;
using Bk.APG.Business.Policies;
using Bk.APG.Business.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Bk.APG.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Policy = APGPolicies.RequireAllowRole)]
public class CommitteesController : ControllerBase
{
    private readonly ICommitteeService _committeeService;
    private readonly IMembershipService _membershipService;

    public CommitteesController(ICommitteeService committeeService, IMembershipService membershipService)
    {
        _committeeService = committeeService;
        _membershipService = membershipService;
    }

    [HttpGet("list")]
    public async Task<ActionResult> GetAll([FromQuery, Required] PagingParametersDto pagingParameters, [FromQuery] CommitteeFilterParametersDto? filterParameters, [FromQuery] SortParametersDto sortParameters)
    {
        var committees = await _committeeService.GetCommitteeList(pagingParameters, filterParameters, sortParameters.Sort, sortParameters.Direction);
        return Ok(committees);
    }

    [HttpGet("listExport")]
    public async Task<ActionResult> GetAllForExport([FromQuery] RequestAndReportsFilterParametersDto? filterParameters)
    {
        var committees = await _committeeService.GetCommitteeListForExport(filterParameters);
        return Ok(committees);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult> GetById(Guid id)
    {
        var result = await _committeeService.GetCommitteeDetail(id);
        return Ok(result);
    }

    [HttpGet("{id:guid}/update")]
    public async Task<ActionResult> GetByIdForUpdate(Guid id)
    {
        var result = await _committeeService.GetCommitteeForUpdate(id);
        return Ok(result);
    }

    [HttpGet("create")]
    [Authorize(Policy = APGPolicies.RequireAdminDepartmentRole)]
    public async Task<ActionResult> GetEmpty()
    {
        var dto = await _committeeService.GetEmpty();

        return Ok(dto);
    }

    [HttpGet("{id:guid}/justifications")]
    public async Task<ActionResult> GetByIdForJustificationUpdate(Guid id)
    {
        var result = await _committeeService.GetCommitteeJustificationForUpdate(id);
        return Ok(result);
    }

    [HttpPut("{id:guid}")]
    [Authorize(Policy = APGPolicies.RequireAdminDepartmentOfficeOrSecretariatRole)]
    public async Task<ActionResult> Update([FromRoute] Guid id, [FromBody, Required] CommitteeUpdateDto updateDto)
    {
        if (id != updateDto.Id)
        {
            return BadRequest();
        }

        var committee = await _committeeService.UpdateCommittee(id, updateDto);
        return Ok(committee);
    }

    [HttpPut("{id:guid}/justifications")]
    [Authorize(Policy = APGPolicies.RequireAdminDepartmentOfficeOrSecretariatRole)]
    public async Task<ActionResult> UpdateJustifications([FromRoute] Guid id, [FromBody, Required] CommitteeJustificationUpdateDto updateDto)
    {
        if (id != updateDto.Id)
        {
            return BadRequest();
        }

        var committee = await _committeeService.UpdateCommitteeJustifications(id, updateDto);
        return Ok(committee);
    }

    [HttpPost]
    [Authorize(Policy = APGPolicies.RequireAdminDepartmentRole)]
    public async Task<ActionResult> Create([FromBody, Required] CommitteeCreateDto createDto)
    {
        var committee = await _committeeService.CreateCommittee(createDto);

        return Ok(committee);
    }

    [HttpGet("{committeeId:guid}/members")]
    public async Task<ActionResult> GetMembers([FromRoute] Guid committeeId)
    {
        var members = await _membershipService.GetAllByCommitteeId(committeeId);
        return Ok(members);
    }

    [HttpGet("getByDescription")]
    public async Task<ActionResult> GetByDescription([FromQuery, Required] string desc)
    {
        var result = await _committeeService.GetByDescription(desc);
        return Ok(result);
    }

    [HttpPost("member")]
    public async Task<ActionResult> CreateMember([FromBody, Required] MembershipCreateDto createDto)
    {
        var membership = await _membershipService.CreateMembership(createDto);

        return Ok(membership);
    }

    [HttpGet("{id:guid}/checkMemberships")]
    public async Task<ActionResult> CheckMemberships([FromRoute] Guid id, [FromQuery, Required] CommitteeMembershipValidationRequestDto validateDto)
    {
        var validationResults = await _committeeService.ValidateCommittee(id, validateDto);
        return Ok(validationResults);
    }
}
