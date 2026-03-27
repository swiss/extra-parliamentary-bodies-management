using System.ComponentModel.DataAnnotations;
using Bk.APG.Business.Dtos;
using Bk.APG.Business.Policies;
using Bk.APG.Business.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Bk.APG.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Policy = APGPolicies.RequireAdminDepartmentRole)]
public class FormLetterSenderController : ControllerBase
{
    private readonly IFormLetterSenderService _formLetterSenderService;

    public FormLetterSenderController(IFormLetterSenderService formLetterSenderService)
    {
        _formLetterSenderService = formLetterSenderService;
    }

    [HttpGet("list")]
    public async Task<ActionResult> GetList()
    {
        var formLetterSenderList = await _formLetterSenderService.GetFormLetterSenderList();

        return Ok(formLetterSenderList);
    }

    [HttpGet("empty")]
    public async Task<ActionResult> GetEmpty()
    {
        var emptyFormLetterSender = await _formLetterSenderService.GetEmpty();

        return Ok(emptyFormLetterSender);
    }


    [HttpPost]
    public async Task<ActionResult> Create([FromForm, Required] FormLetterSenderCreateDto formLetterSenderCreateDto)
    {
        var formLetterSender = await _formLetterSenderService.CreateFormLetterSender(formLetterSenderCreateDto);

        return Ok(formLetterSender);
    }

    [HttpGet("{id:guid}/update")]
    public async Task<ActionResult> GetByIdForUpdate(Guid id)
    {
        var result = await _formLetterSenderService.GetFormLetterSenderForUpdate(id);
        return Ok(result);
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult> Update([FromRoute] Guid id, [FromForm, Required] FormLetterSenderUpdateDto formLetterSenderUpdateDto)
    {
        ArgumentNullException.ThrowIfNull(formLetterSenderUpdateDto);

        if (formLetterSenderUpdateDto.Id != id)
        {
            return BadRequest();
        }

        var formLetterSender = await _formLetterSenderService.UpdateFormLetterSender(id, formLetterSenderUpdateDto);

        return Ok(formLetterSender);
    }

    [HttpDelete("{id:guid}")]
    public async Task<ActionResult> Delete([FromRoute] Guid id)
    {
        await _formLetterSenderService.DeleteFormLetterSender(id);

        return NoContent();
    }
}
