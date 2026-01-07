using Bk.APG.Business.Models;
using Bk.APG.Business.Repositories;

namespace Bk.APG.Business.Services;

public class ApgGeneralSettingsService : IApgGeneralSettingsService
{
    private readonly IApgGeneralSettingsRepository _apgGeneralSettingsRepository;

    public ApgGeneralSettingsService(IApgGeneralSettingsRepository apgGeneralSettingsRepository)
    {
        _apgGeneralSettingsRepository = apgGeneralSettingsRepository;
    }

    public async Task<bool> GetCurrentOgdExportSetting()
    {
        var settings = await _apgGeneralSettingsRepository.GetCurrentApgGeneralSetting();

        return settings is { IsOgdExportActivated: true };
    }

    public async Task<ApgGeneralSettings?> GetApgGeneralSettingsForUpdate()
    {
        var settings = await _apgGeneralSettingsRepository.GetCurrentApgGeneralSetting();

        return settings;
    }

    public async Task<ApgGeneralSettings> UpdateApgGeneralSettings(bool ogdExportEnabled)
    {
        var currentSettings = await GetApgGeneralSettingsForUpdate();

        if (currentSettings != null)
        {
            var newSettings = currentSettings;
            newSettings.IsOgdExportActivated = ogdExportEnabled;

            return await _apgGeneralSettingsRepository.Update(currentSettings, newSettings);
        }

        return currentSettings!;
    }
}
