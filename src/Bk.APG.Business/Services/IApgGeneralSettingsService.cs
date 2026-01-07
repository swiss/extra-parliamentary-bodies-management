using Bk.APG.Business.Models;

namespace Bk.APG.Business.Services;

public interface IApgGeneralSettingsService
{
    Task<bool> GetCurrentOgdExportSetting();
    Task<ApgGeneralSettings?> GetApgGeneralSettingsForUpdate();
    Task<ApgGeneralSettings> UpdateApgGeneralSettings(bool ogdExportEnabled);
}
