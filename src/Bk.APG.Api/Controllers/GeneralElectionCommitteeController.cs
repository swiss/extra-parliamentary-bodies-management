using System.ComponentModel.DataAnnotations;
using Bk.APG.Business.Dtos;
using Bk.APG.Business.Policies;
using Bk.APG.Business.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Bk.APG.Api.Controllers;

[ApiController]
[Route("api/general-election/committees")]
[Authorize(Policy = APGPolicies.RequireAllowRole)]
public class GeneralElectionCommitteeController : ControllerBase
{
    private const string ExcelMimeType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";

    private readonly IMembershipCandidateService _membershipCandidateService;
    private readonly IGeneralElectionCommitteeService _generalElectionCommitteeService;
    private readonly IEiamAssignmentService _eiamAssignmentService;

    public GeneralElectionCommitteeController(IMembershipCandidateService membershipCandidateService,
        IGeneralElectionCommitteeService generalElectionCommitteeService,
        IEiamAssignmentService eiamAssignmentService)
    {
        _membershipCandidateService = membershipCandidateService;
        _generalElectionCommitteeService = generalElectionCommitteeService;
        _eiamAssignmentService = eiamAssignmentService;
    }

    [HttpPost("getDuplicateMembershipCandidate")]
    public async Task<ActionResult> GetDuplicateMembershipCandidate([FromBody, Required] MembershipCandidateCreateDto dto)
    {
        var result = await _membershipCandidateService.GetDuplicateMembershipCandidateForList(dto.CommitteeId, dto.Surname, dto.GivenName, dto.BirthYear, dto.GenderId, dto.LanguageId);
        return Ok(result);
    }

    [HttpGet]
    [Route("{committeeId:guid}/candidate-list/forward")]
    public async Task<IActionResult> GetAssignmentsCandidateListForward(Guid committeeId)
    {
        var candidateList = await _eiamAssignmentService.GetAllForCandidateListForward(committeeId);
        return Ok(candidateList);
    }

    [HttpPost]
    [Route("{committeeId:guid}/candidate-list/forward")]
    public async Task<IActionResult> ForwardCandidateList(Guid committeeId, [FromBody] CandidateListForwardDto forwardDto)
    {
        await _membershipCandidateService.ForwardCandidateList(committeeId, forwardDto);
        return Ok();
    }

    [HttpPost]
    [Route("{committeeId:guid}/candidate-list/validate")]
    public async Task<IActionResult> ValidateCandidateList(Guid committeeId, [FromBody] CandidateListValidationRequest candidateListValidationRequest)
    {
        var result = await _membershipCandidateService.ValidateCandidateList(committeeId, candidateListValidationRequest.SelectedCandidateIds, candidateListValidationRequest.DuplicateCheckConfirmed);
        return Ok(result);
    }

    [HttpPost]
    [Route("{committeeId:guid}/candidate-list/save")]
    public async Task<IActionResult> SaveCandidateList(Guid committeeId, [FromBody] IEnumerable<Guid> candidateIds)
    {
        await _membershipCandidateService.SaveCandidateList(committeeId, candidateIds);
        return Ok();
    }

    [HttpGet("list")]
    public async Task<IActionResult> GetGeneralElectionCommitteesGetAll([FromQuery, Required] PagingParametersDto pagingParameters, [FromQuery] GeneralElectionCommitteeFilterParametersDto? filterParameters, [FromQuery] SortParametersDto sortParameters)
    {
        var result = await _generalElectionCommitteeService.GetGeneralElectionCommitteeList(pagingParameters, filterParameters, sortParameters.Sort, sortParameters.Direction);
        return Ok(result);
    }

    [HttpGet]
    [Route("{committeeId:guid}")]
    public async Task<IActionResult> GetGeneralElectionCommittee(Guid committeeId)
    {
        var generalElectionCommittee = await _generalElectionCommitteeService.GetGeneralElectionCommittee(committeeId);
        return Ok(generalElectionCommittee);
    }

    [HttpGet("{committeeId:guid}/update")]
    public async Task<IActionResult> GetGeneralElectionCommitteeForUpdate(Guid committeeId)
    {
        var result = await _generalElectionCommitteeService.GetGeneralElectionCommitteeForUpdate(committeeId);
        return Ok(result);
    }

    [HttpPut("{committeeId:guid}")]
    [Authorize(Policy = APGPolicies.RequireAdminDepartmentOfficeOrSecretariatRole)]
    public async Task<ActionResult> Update([FromRoute] Guid committeeId, [FromBody, Required] GeneralElectionCommitteeUpdateDto updateDto)
    {
        var committee = await _generalElectionCommitteeService.UpdateGeneralElectionCommittee(committeeId, updateDto);
        return Ok(committee);
    }

    [HttpGet]
    [Route("{committeeId:guid}/members")]
    public async Task<IActionResult> GetMembers(Guid committeeId)
    {
        var candidates = await _membershipCandidateService.GetMembers(committeeId);
        return Ok(candidates);
    }

    [HttpGet("{committeeId:guid}/justifications")]
    public async Task<ActionResult> GetByIdForJustificationUpdate(Guid committeeId)
    {
        var result = await _generalElectionCommitteeService.GetGeneralElectionCommitteeJustificationForUpdate(committeeId);
        return Ok(result);
    }

    [HttpPut("{id:guid}/justifications")]
    [Authorize(Policy = APGPolicies.RequireAdminDepartmentOfficeOrSecretariatRole)]
    public async Task<ActionResult> UpdateJustifications([FromRoute] Guid id, [FromBody, Required] GeneralElectionCommitteeJustificationUpdateDto updateDto)
    {
        if (id != updateDto.Id)
        {
            return BadRequest();
        }

        var committee = await _generalElectionCommitteeService.UpdateGeneralElectionCommitteeJustifications(id, updateDto);
        return Ok(committee);
    }

    [HttpPut("{id:guid}/vacancies")]
    [Authorize(Policy = APGPolicies.RequireAdminDepartmentOfficeOrSecretariatRole)]
    public async Task<ActionResult> UpdateVacancies([FromRoute] Guid id, [FromBody] int vacancies)
    {
        var committee = await _generalElectionCommitteeService.UpdateGeneralElectionCommitteeVacancies(id, vacancies);
        return Ok(committee);
    }

    [HttpPost("{id:guid}/download")]
    public async Task<ActionResult> GenerateCommitteeTypeExport([FromRoute] Guid id
        , [FromBody] CandidateListExportRequestDto request)
    {
        var (fileName, content) = await _generalElectionCommitteeService.GenerateCandidateListExport(id, request.MembershipCandidateIds);

        return File(content, ExcelMimeType, fileName);
    }
}
