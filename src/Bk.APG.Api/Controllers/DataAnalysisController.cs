using Bk.APG.Business.Models;
using Bk.APG.Business.Services;
using Microsoft.AspNetCore.Mvc;

namespace Bk.APG.Api.Controllers;

[ApiController]
[Route("api/data-analysis")]
public class DataAnalysisController : ControllerBase
{
    private const string ExcelMimeType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";

    private readonly IDataAnalysisService _dataAnalysisService;

    public DataAnalysisController(IDataAnalysisService dataAnalysisService)
    {
        _dataAnalysisService = dataAnalysisService;
    }

    [HttpGet("committee-type/{dataAnalysisDate}")]
    public async Task<ActionResult> GenerateCommitteeTypeExport([FromRoute] DateOnly dataAnalysisDate)
    {
        var (fileName, content) = await _dataAnalysisService.GenerateCommitteeTypeExport(dataAnalysisDate);

        return File(content, ExcelMimeType, fileName);
    }

    [HttpGet("committee/{dataAnalysisDate}")]
    public async Task<ActionResult> GenerateCommitteeExport([FromRoute] DateOnly dataAnalysisDate)
    {
        var (fileName, content) = await _dataAnalysisService.GenerateCommitteeExport(dataAnalysisDate);

        return File(content, ExcelMimeType, fileName);
    }

    [HttpGet("membership/{dataAnalysisDate}")]
    public async Task<ActionResult> GenerateMembershipExport([FromRoute] DateOnly dataAnalysisDate)
    {
        var (fileName, content) = await _dataAnalysisService.GenerateMembershipExport(dataAnalysisDate);

        return File(content, ExcelMimeType, fileName);
    }

    [HttpGet("membershipInterests/{dataAnalysisDate}")]
    public async Task<ActionResult> GenerateMembershipInterestsExport([FromRoute] DateOnly dataAnalysisDate)
    {
        var (fileName, content) = await _dataAnalysisService.GenerateMembershipInterestExport(dataAnalysisDate);

        return File(content, ExcelMimeType, fileName);
    }

    [HttpGet("person/{dataAnalysisDate}")]
    public async Task<ActionResult> GeneratePersonExport([FromRoute] DateOnly dataAnalysisDate)
    {
        var (fileName, content) = await _dataAnalysisService.GeneratePersonExport(dataAnalysisDate);

        return File(content, ExcelMimeType, fileName);
    }

    [HttpGet("secretariat/{dataAnalysisDate}")]
    public async Task<ActionResult> GenerateSecretariatExport([FromRoute] DateOnly dataAnalysisDate)
    {
        var (fileName, content) = await _dataAnalysisService.GenerateContactPointExport(dataAnalysisDate, ContactPointType.SecretariatGuid);

        return File(content, ExcelMimeType, fileName);
    }

    [HttpGet("dataProtectionOfficer/{dataAnalysisDate}")]
    public async Task<ActionResult> GenerateDataProtectionOfficerExport([FromRoute] DateOnly dataAnalysisDate)
    {
        var (fileName, content) = await _dataAnalysisService.GenerateContactPointExport(dataAnalysisDate, ContactPointType.DataProtectionOfficerGuid);

        return File(content, ExcelMimeType, fileName);
    }

    [HttpGet("region/{dataAnalysisDate}")]
    public async Task<ActionResult> GenerateRegionExport([FromRoute] DateOnly dataAnalysisDate)
    {
        var (fileName, content) = await _dataAnalysisService.GenerateRegionExport(dataAnalysisDate);

        return File(content, ExcelMimeType, fileName);
    }

    [HttpGet("age/{dataAnalysisDate}")]
    public async Task<ActionResult> GenerateAgeExport([FromRoute] DateOnly dataAnalysisDate)
    {
        var (fileName, content) = await _dataAnalysisService.GenerateAgeExport(dataAnalysisDate);

        return File(content, ExcelMimeType, fileName);
    }
}
