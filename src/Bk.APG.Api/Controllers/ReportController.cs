using System.ComponentModel.DataAnnotations;
using System.Net.Mime;
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
    private readonly IFormLetterService _formLetterService;

    public ReportController(IReportService parliamentaryReportService, IFormLetterService formLetterService)
    {
        _reportService = parliamentaryReportService;
        _formLetterService = formLetterService;
    }

    [HttpPost("download")]
    public async Task<ActionResult> GenerateReport([FromBody, Required] ReportFilterParametersDto filterDto)
    {
        var (fileName, content) = await _reportService.GetReport(filterDto);

        return File(content, WordMimeType, fileName);
    }

    [HttpPost("downloadFormLetter")]
    public async Task<ActionResult> GenerateReportFormLetter([FromBody, Required] FormLetterFilterParameters filterDto)
    {
        ArgumentNullException.ThrowIfNull(filterDto);

        if (filterDto.ExportType == "single")
        {
            // we export a ZIP File with all documents within
            var (fileName, zipFile) = await _formLetterService.CreateFormLetterAsZipFile(filterDto);

            return File(zipFile, MediaTypeNames.Application.Zip, fileName);
        }
        else
        {
            var (fileName, content) = await _formLetterService.CreateFormLetterSingleDocument(filterDto);

            if (filterDto.ExportFileType == "word")
            {
                return File(content, WordMimeType, fileName);
            }
            else
            {
                return File(content, MediaTypeNames.Application.Pdf, fileName);
            }
        }
    }
}
