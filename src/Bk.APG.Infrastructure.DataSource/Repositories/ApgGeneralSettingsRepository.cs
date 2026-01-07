using Bk.APG.Business.Models;
using Bk.APG.Business.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Bk.APG.Infrastructure.DataSource.Repositories;

public class ApgGeneralSettingsRepository : IApgGeneralSettingsRepository
{
    private readonly DataContext _dataContext;

    public ApgGeneralSettingsRepository(DataContext dataContext)
    {
        _dataContext = dataContext;
    }

    public async Task<ApgGeneralSettings?> GetCurrentApgGeneralSetting()
    {
        return await _dataContext.ApgGeneralSettings.FirstOrDefaultAsync();
    }

    public async Task<ApgGeneralSettings?> GetByIdForUpdate(Guid id)
    {
        return await _dataContext.ApgGeneralSettings
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == id);
    }

    public async Task<ApgGeneralSettings> Update(ApgGeneralSettings existing, ApgGeneralSettings update)
    {
        _dataContext.ApgGeneralSettings.Update(existing);

        existing.Modified = update.Modified;
        existing.ModifiedBy = update.ModifiedBy;
        existing.IsOgdExportActivated = update.IsOgdExportActivated;

        await _dataContext.SaveChangesAsync();

        return existing;
    }
}
