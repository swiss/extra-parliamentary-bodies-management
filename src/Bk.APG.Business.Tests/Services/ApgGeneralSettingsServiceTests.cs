using Bk.APG.Business.Models;
using Bk.APG.Business.Repositories;
using Bk.APG.Business.Services;
using Bk.APG.CrossCutting.Tests.Builders;

namespace Bk.APG.Business.Tests.Services;

[TestFixture]
internal class ApgGeneralSettingsServiceTests
{
    private readonly IApgGeneralSettingsRepository _apgGeneralSettingsRepository = Substitute.For<IApgGeneralSettingsRepository>();

    private ApgGeneralSettingsService _service = null!;

    [SetUp]
    public void SetUp()
    {
        _service = new ApgGeneralSettingsService(_apgGeneralSettingsRepository);
    }

    [TearDown]
    public void TearDown()
    {
        _apgGeneralSettingsRepository.ClearSubstitute();
    }

    [Test]
    public async Task GetCurrentOgdExportSetting_WhenCalled_ShouldCallRepository()
    {
        var settings = new ApgGeneralSettingsBuilder().WithId(Guid.NewGuid()).WithIsOgdExportActivated(true).Build();

        _apgGeneralSettingsRepository.GetCurrentApgGeneralSetting().Returns(settings);

        var result = await _service.GetCurrentOgdExportSetting();

        await _apgGeneralSettingsRepository.Received(1).GetCurrentApgGeneralSetting();
        Assert.That(result, Is.True);
    }

    [Test]
    public async Task GetApgGeneralSettingsForUpdates_WhenCalled_ShouldCallRepositoryAndReturnResult()
    {
        var guid = Guid.NewGuid();

        var settings = new ApgGeneralSettingsBuilder().WithId(guid).WithIsOgdExportActivated(true).Build();

        _apgGeneralSettingsRepository.GetCurrentApgGeneralSetting().Returns(settings);
        var result = await _service.GetApgGeneralSettingsForUpdate();

        await _apgGeneralSettingsRepository.Received(1).GetCurrentApgGeneralSetting();
        Assert.That(result, Is.Not.Null);
        Assert.That(result.IsOgdExportActivated, Is.True);
    }

    [Test]
    public async Task UpdateApgGeneralSettings_WithData_ShouldCallRepository()
    {
        var guid = Guid.NewGuid();
        var settings = new ApgGeneralSettingsBuilder().WithId(guid).WithIsOgdExportActivated(false).Build();

        _apgGeneralSettingsRepository.GetCurrentApgGeneralSetting().Returns(settings);
        _apgGeneralSettingsRepository.Update(Arg.Any<ApgGeneralSettings>(), Arg.Any<ApgGeneralSettings>()).Returns(settings);

        var changedSettings = await _service.UpdateApgGeneralSettings(true);

        Assert.That(changedSettings, Is.Not.Null);
        Assert.That(changedSettings, Is.InstanceOf<ApgGeneralSettings>());

        await _apgGeneralSettingsRepository.Received(1).GetCurrentApgGeneralSetting();
        await _apgGeneralSettingsRepository.Received(1).Update(Arg.Any<ApgGeneralSettings>(), Arg.Any<ApgGeneralSettings>());
    }
}
