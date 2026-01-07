using Bk.APG.Api.Controllers;
using Bk.APG.Business.Dtos;
using Bk.APG.Business.Services;
using Microsoft.AspNetCore.Mvc;

namespace Bk.APG.Api.Tests.Controllers;

[TestFixture]
internal class ReportControllerTests
{
    private readonly IReportService _reportService = Substitute.For<IReportService>();

    private ReportController _controller = null!;

    [SetUp]
    public void SetUp()
    {
        _controller = new ReportController(_reportService);
    }

    [TearDown]
    public void TearDown()
    {
        _reportService.ClearSubstitute();
    }

    [Test]
    public async Task GenerateReport_ReturnsFileResult()
    {
        using var memoryStream = new MemoryStream();
        var filterDto = new ReportFilterParametersDto { DocumentType = ReportType.AppendixFederalCouncil };

        _reportService.GetReport(filterDto).Returns(("FooBar", memoryStream));

        var result = await _controller.GenerateReport(filterDto);

        Assert.That(result, Is.Not.Null);
        var resultObject = result as FileStreamResult;

        Assert.That(resultObject, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(resultObject.FileDownloadName, Is.EqualTo("FooBar"));
            Assert.That(resultObject.ContentType, Is.EqualTo("application/vnd.openxmlformats-officedocument.wordprocessingml.template"));
            Assert.That(resultObject.FileStream, Is.EqualTo(memoryStream));
        });
    }
}
