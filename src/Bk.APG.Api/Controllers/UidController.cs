using Bk.APG.Infrastructure.Service.UID.Service;
using Microsoft.AspNetCore.Mvc;

namespace Bk.APG.Api.Controllers;

[ApiController]
[Route("api/uid")]
public class UidController : ControllerBase
{
    private readonly IUidService _uidService;

    public UidController(IUidService uidService)
    {
        _uidService = uidService;
    }

    [HttpGet("search")]
    public async Task<ActionResult> Search([FromQuery] string organizationName)
    {
        var result = await _uidService.Search(organizationName);
        return Ok(result);
    }
}
