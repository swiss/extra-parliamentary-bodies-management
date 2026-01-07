using System.ComponentModel.DataAnnotations;
using Bk.APG.Business.Dtos;
using Bk.APG.Business.Services;
using Microsoft.AspNetCore.Mvc;

namespace Bk.APG.Api.Controllers;

[ApiController]
[Route("api/person/{personId:guid}/interests")]
public class InterestController : ControllerBase
{
    private readonly IInterestService _interestService;

    public InterestController(IInterestService interestService)
    {
        _interestService = interestService;
    }

    [HttpGet]
    public async Task<ActionResult> GetAllByPersonId(Guid personId)
    {
        var results = await _interestService.GetInterestsForUpdateByPersonId(personId);
        return Ok(results);
    }

    [HttpPut]
    public async Task<ActionResult> UpdateInterests([FromRoute] Guid personId, [FromBody, Required] InterestUpdateDto[] updateDtos)
    {
        var interest = await _interestService.UpdateInterests(personId, updateDtos);
        return Ok(interest);
    }
}
