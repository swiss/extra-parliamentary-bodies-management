using Bk.APG.Api.Controllers;
using Bk.APG.Business.Services;
using Microsoft.AspNetCore.Mvc;

namespace Bk.APG.Api.Tests.Controllers;

[TestFixture]
internal class DataAnalysisControllerTests
{
    private readonly IDataAnalysisService _dataAnalysisService = Substitute.For<IDataAnalysisService>();

    private DataAnalysisController _controller = null!;
    private readonly DateOnly _dataAnalysisDate = DateOnly.FromDateTime(DateTime.UtcNow);

    [SetUp]
    public void SetUp()
    {
        _controller = new DataAnalysisController(_dataAnalysisService);
    }

    [TearDown]
    public void TearDown()
    {
        _dataAnalysisService.ClearSubstitute();
    }

    [Test]
    public async Task GenerateCommitteeTypeExport_ReturnsFileResult()
    {
        using var memoryStream = new MemoryStream();

        _dataAnalysisService.GenerateCommitteeTypeExport(_dataAnalysisDate).Returns(("FooBar", memoryStream));

        var result = await _controller.GenerateCommitteeTypeExport(_dataAnalysisDate);

        Assert.That(result, Is.Not.Null);
        var resultObject = result as FileStreamResult;

        Assert.That(resultObject, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(resultObject.FileDownloadName, Is.EqualTo("FooBar"));
            Assert.That(resultObject.ContentType, Is.EqualTo("application/vnd.openxmlformats-officedocument.spreadsheetml.sheet"));
            Assert.That(resultObject.FileStream, Is.EqualTo(memoryStream));
        });
    }

    [Test]
    public async Task GenerateCommitteeExport_ReturnsFileResult()
    {
        using var memoryStream = new MemoryStream();
        _dataAnalysisService.GenerateCommitteeExport(_dataAnalysisDate).Returns(("FooBar", memoryStream));

        var result = await _controller.GenerateCommitteeExport(_dataAnalysisDate);

        Assert.That(result, Is.Not.Null);
        var resultObject = result as FileStreamResult;

        Assert.That(resultObject, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(resultObject.FileDownloadName, Is.EqualTo("FooBar"));
            Assert.That(resultObject.ContentType, Is.EqualTo("application/vnd.openxmlformats-officedocument.spreadsheetml.sheet"));
            Assert.That(resultObject.FileStream, Is.EqualTo(memoryStream));
        });
    }

    [Test]
    public async Task GenerateMembershipExport_ReturnsFileResult()
    {
        using var memoryStream = new MemoryStream();
        _dataAnalysisService.GenerateMembershipExport(_dataAnalysisDate).Returns(("FooBar", memoryStream));

        var result = await _controller.GenerateMembershipExport(_dataAnalysisDate);

        Assert.That(result, Is.Not.Null);
        var resultObject = result as FileStreamResult;

        Assert.That(resultObject, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(resultObject.FileDownloadName, Is.EqualTo("FooBar"));
            Assert.That(resultObject.ContentType, Is.EqualTo("application/vnd.openxmlformats-officedocument.spreadsheetml.sheet"));
            Assert.That(resultObject.FileStream, Is.EqualTo(memoryStream));
        });
    }

    [Test]
    public async Task GenerateMembershipInterestExport_ReturnsFileResult()
    {
        using var memoryStream = new MemoryStream();
        _dataAnalysisService.GenerateMembershipInterestExport(_dataAnalysisDate).Returns(("FooBar", memoryStream));

        var result = await _controller.GenerateMembershipInterestsExport(_dataAnalysisDate);

        Assert.That(result, Is.Not.Null);
        var resultObject = result as FileStreamResult;

        Assert.That(resultObject, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(resultObject.FileDownloadName, Is.EqualTo("FooBar"));
            Assert.That(resultObject.ContentType, Is.EqualTo("application/vnd.openxmlformats-officedocument.spreadsheetml.sheet"));
            Assert.That(resultObject.FileStream, Is.EqualTo(memoryStream));
        });
    }

    [Test]
    public async Task GeneratePersonExport_ReturnsFileResult()
    {
        using var memoryStream = new MemoryStream();
        _dataAnalysisService.GeneratePersonExport(_dataAnalysisDate).Returns(("FooBar", memoryStream));

        var result = await _controller.GeneratePersonExport(_dataAnalysisDate);

        Assert.That(result, Is.Not.Null);
        var resultObject = result as FileStreamResult;

        Assert.That(resultObject, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(resultObject.FileDownloadName, Is.EqualTo("FooBar"));
            Assert.That(resultObject.ContentType, Is.EqualTo("application/vnd.openxmlformats-officedocument.spreadsheetml.sheet"));
            Assert.That(resultObject.FileStream, Is.EqualTo(memoryStream));
        });
    }

    [Test]
    public async Task GenerateSecretariatExport_ReturnsFileResult()
    {
        using var memoryStream = new MemoryStream();
        _dataAnalysisService.GenerateContactPointExport(_dataAnalysisDate, Arg.Any<Guid>()).Returns(("FooBar", memoryStream));

        var result = await _controller.GenerateSecretariatExport(_dataAnalysisDate);

        Assert.That(result, Is.Not.Null);
        var resultObject = result as FileStreamResult;

        Assert.That(resultObject, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(resultObject.FileDownloadName, Is.EqualTo("FooBar"));
            Assert.That(resultObject.ContentType, Is.EqualTo("application/vnd.openxmlformats-officedocument.spreadsheetml.sheet"));
            Assert.That(resultObject.FileStream, Is.EqualTo(memoryStream));
        });
    }

    [Test]
    public async Task GenerateDataProtectionOfficerExport_ReturnsFileResult()
    {
        using var memoryStream = new MemoryStream();
        _dataAnalysisService.GenerateContactPointExport(_dataAnalysisDate, Arg.Any<Guid>()).Returns(("FooBar", memoryStream));

        var result = await _controller.GenerateDataProtectionOfficerExport(_dataAnalysisDate);

        Assert.That(result, Is.Not.Null);
        var resultObject = result as FileStreamResult;

        Assert.That(resultObject, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(resultObject.FileDownloadName, Is.EqualTo("FooBar"));
            Assert.That(resultObject.ContentType, Is.EqualTo("application/vnd.openxmlformats-officedocument.spreadsheetml.sheet"));
            Assert.That(resultObject.FileStream, Is.EqualTo(memoryStream));
        });
    }

    [Test]
    public async Task GenerateRegionExport_ReturnsFileResult()
    {
        using var memoryStream = new MemoryStream();
        _dataAnalysisService.GenerateRegionExport(_dataAnalysisDate).Returns(("FooBar", memoryStream));

        var result = await _controller.GenerateRegionExport(_dataAnalysisDate);

        Assert.That(result, Is.Not.Null);
        var resultObject = result as FileStreamResult;

        Assert.That(resultObject, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(resultObject.FileDownloadName, Is.EqualTo("FooBar"));
            Assert.That(resultObject.ContentType, Is.EqualTo("application/vnd.openxmlformats-officedocument.spreadsheetml.sheet"));
            Assert.That(resultObject.FileStream, Is.EqualTo(memoryStream));
        });
    }

    [Test]
    public async Task GenerateAgeExport_ReturnsFileResult()
    {
        using var memoryStream = new MemoryStream();
        _dataAnalysisService.GenerateAgeExport(_dataAnalysisDate).Returns(("FooBar", memoryStream));

        var result = await _controller.GenerateAgeExport(_dataAnalysisDate);

        Assert.That(result, Is.Not.Null);
        var resultObject = result as FileStreamResult;

        Assert.That(resultObject, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(resultObject.FileDownloadName, Is.EqualTo("FooBar"));
            Assert.That(resultObject.ContentType, Is.EqualTo("application/vnd.openxmlformats-officedocument.spreadsheetml.sheet"));
            Assert.That(resultObject.FileStream, Is.EqualTo(memoryStream));
        });
    }
}
