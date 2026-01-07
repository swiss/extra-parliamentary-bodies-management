using Bk.APG.Business.Models;

namespace Bk.APG.Business.Repositories;

public interface IApgGeneralSettingsRepository
{
    Task<ApgGeneralSettings?> GetCurrentApgGeneralSetting();
    Task<ApgGeneralSettings?> GetByIdForUpdate(Guid id);
    Task<ApgGeneralSettings> Update(ApgGeneralSettings existing, ApgGeneralSettings update);
}
