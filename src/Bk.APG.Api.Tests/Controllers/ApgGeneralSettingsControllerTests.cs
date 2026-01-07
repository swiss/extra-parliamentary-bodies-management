using Bk.APG.Api.Controllers;
using Bk.APG.Business.Services;
using Bk.APG.CrossCutting.Tests.Builders;
using Microsoft.AspNetCore.Mvc;

namespace Bk.APG.Api.Tests.Controllers;

[TestFixture]
internal class ApgGeneralSettingsControllerTests
{
    private readonly IApgGeneralSettingsService _apgGeneralSettingsService = Substitute.For<IApgGeneralSettingsService>();

    private ApgGeneralSettingsController _controller = null!;

    [SetUp]
    public void SetUp()
    {
        _controller = new ApgGeneralSettingsController(_apgGeneralSettingsService);
    }

    [TearDown]
    public void TearDown()
    {
        _apgGeneralSettingsService.ClearSubstitute();
    }

    [Test]
    public async Task GetCurrentOgdExportSettings_WhenCalled_ShouldCallServiceAndReturnResult()
    {
        _apgGeneralSettingsService
            .GetCurrentOgdExportSetting()
            .Returns(true);

        var response = await _controller.GetCurrentOgdExportSettings();

        Assert.That(response, Is.Not.Null);
        var responseObject = response as OkObjectResult;

        Assert.That(responseObject, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(responseObject.StatusCode, Is.EqualTo(200));
            Assert.That(responseObject.Value, Is.EqualTo(true));
        });
    }

    [Test]
    public async Task UpdateOgdExportSettings_WhenCalled_ShouldCallServiceAndReturnResult()
    {
        var settings = new ApgGeneralSettingsBuilder().WithIsOgdExportActivated(true).Build();

        _apgGeneralSettingsService
            .UpdateApgGeneralSettings(true)
            .Returns(settings);

        var response = await _controller.UpdateOgdExportSettings(true);

        Assert.That(response, Is.Not.Null);
        var responseObject = response as OkObjectResult;

        Assert.That(responseObject, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(responseObject.StatusCode, Is.EqualTo(200));
            Assert.That(responseObject.Value, Is.EqualTo(settings));
        });
    }
}
