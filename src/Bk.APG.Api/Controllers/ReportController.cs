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

    [HttpPost("downloadFormLetter")]
    public async Task<ActionResult> GenerateReportFormLetter([FromBody, Required] FormLetterFilterParameters filterDto)
    {
        if (filterDto.ExportSingleDocuments == "single")
        {
            // we export a ZIP File with all documents within
            Response.ContentType = "application/zip";
            Response.Headers.Append("Content-Disposition", "attachment; filename=documents.zip");

            var (fileName, zipFile) = await _reportService.CreateFormLetterAsZipFile(filterDto);

            return File(zipFile, "application/zip", fileName);
        }
        else
        {
            var (fileName, content) = await _reportService.CreateFormLetterSingleDocument(filterDto);

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
