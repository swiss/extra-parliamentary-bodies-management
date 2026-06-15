using System.ComponentModel.DataAnnotations;
using Bk.APG.Business.Dtos;
using Bk.APG.Business.Policies;
using Bk.APG.Business.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Bk.APG.Api.Controllers;

[ApiController]
[Authorize(Policy = APGPolicies.RequireAllowRole)]
[Route("api/persons")]
public class PersonsController : ControllerBase
{
    private readonly IPersonService _personService;
    private readonly IMembershipService _membershipService;
    private readonly ISalutationGeneratorService _salutationGeneratorService;
    private readonly IInterestService _interestService;

    public PersonsController(IPersonService personService, IMembershipService membershipService, ISalutationGeneratorService salutationGeneratorService, IInterestService interestService)
    {
        _personService = personService;
        _membershipService = membershipService;
        _salutationGeneratorService = salutationGeneratorService;
        _interestService = interestService;
    }

    [HttpGet("list")]
    public async Task<ActionResult> GetAll([FromQuery, Required] PagingParametersDto pagingParametersDto, [FromQuery] PersonFilterParametersDto? filterParametersDto, [FromQuery] SortParametersDto? sortParametersDto)
    {
        var results = await _personService.GetPersonList(pagingParametersDto, filterParametersDto, sortParametersDto?.Sort, sortParametersDto?.Direction);
        return Ok(results);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult> GetById(Guid id)
    {
        var result = await _personService.GetPersonDetail(id);
        return Ok(result);
    }

    [HttpGet("create")]
    [Authorize(Policy = APGPolicies.RequireAdminDepartmentOfficeOrSecretariatRole)]
    public ActionResult GetEmpty()
    {
        var dto = _personService.GetEmpty();

        return Ok(dto);
    }

    [HttpGet("{id:guid}/update")]
    [Authorize(Policy = APGPolicies.RequireAdminDepartmentOfficeOrSecretariatRole)]
    public async Task<ActionResult> GetByIdForUpdate(Guid id)
    {
        var result = await _personService.GetPersonForUpdate(id);
        return Ok(result);
    }

    [HttpGet("similar")]
    public async Task<ActionResult> GetSimilarPersons([FromQuery, Required] string surname, [FromQuery, Required] string givenName, [FromQuery, Required] int birthYear, [FromQuery, Required] int birthYearRange)
    {
        var result = await _personService.GetSimilarPersons(surname, givenName, birthYear, birthYearRange);
        return Ok(result);
    }

    [HttpPut("{id:guid}")]
    [Authorize(Policy = APGPolicies.RequireAdminDepartmentOfficeOrSecretariatRole)]
    public async Task<ActionResult> Update([FromRoute] Guid id, [FromBody, Required] PersonUpdateDto updateDto)
    {
        var person = await _personService.UpdatePerson(id, updateDto);

        return Ok(person);
    }

    [HttpDelete("{id:guid}")]
    [Authorize(Policy = APGPolicies.RequireAdminDepartmentOfficeOrSecretariatRole)]
    public async Task<ActionResult> Delete([FromRoute] Guid id)
    {
        await _personService.DeletePerson(id);

        return Ok();
    }

    [HttpPost]
    [Authorize(Policy = APGPolicies.RequireAdminDepartmentOfficeOrSecretariatRole)]
    public async Task<ActionResult> Create([FromBody, Required] PersonCreateDto createDto)
    {
        var person = await _personService.CreatePerson(createDto);

        return Ok(person);
    }

    [HttpGet("{personId:guid}/memberships")]
    public async Task<ActionResult> GetMemberships([FromRoute] Guid personId)
    {
        var memberships = await _membershipService.GetAllByPersonId(personId);
        return Ok(memberships);
    }

    [HttpGet("get-by-name")]
    public async Task<ActionResult> GetByName([FromQuery, Required] string name)
    {
        var result = await _personService.GetByName(name);
        return Ok(result);
    }

    [HttpGet("salutation")]
    public async Task<ActionResult> GenerateSalutation([FromQuery, Required] Guid genderId, [FromQuery, Required] Guid correspondenceLanguageId, [FromQuery, Required] string surname, [FromQuery] string? title)
    {
        var salutation = await _salutationGeneratorService.CreateSalutationTextForPerson(genderId, correspondenceLanguageId, surname, title);
        return Ok(salutation);
    }

    [HttpGet("{personId:guid}/interests")]
    public async Task<ActionResult> GetInterests(Guid personId)
    {
        var results = await _interestService.GetInterestsForUpdateByPersonId(personId);
        return Ok(results);
    }

    [HttpPut("{personId:guid}/interests")]
    public async Task<ActionResult> UpdateInterests([FromRoute] Guid personId, [FromBody, Required] InterestUpdateDto[] updateDtos)
    {
        var interest = await _interestService.UpdateInterests(personId, updateDtos);
        return Ok(interest);
    }
}
