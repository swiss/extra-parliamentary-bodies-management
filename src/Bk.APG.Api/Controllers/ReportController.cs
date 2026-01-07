using System.ComponentModel.DataAnnotations;
using Bk.APG.Business.Dtos;
using Bk.APG.Business.Services;
using Microsoft.AspNetCore.Mvc;

namespace Bk.APG.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ReportController : ControllerBase
{
    private const string WordMimeType = "application/vnd.openxmlformats-officedocument.wordprocessingml.template";

    private readonly IReportService _reportService;

    public ReportController(IReportService parliamentaryReportService)
    {
        _reportService = parliamentaryReportService;
    }

    [HttpPost("download")]
    public async Task<ActionResult> GenerateReport([FromBody, Required] ReportFilterParametersDto filterDto)
    {
        var (fileName, content) = await _reportService.GetReport(filterDto);

        return File(content, WordMimeType, fileName);
    }
}
