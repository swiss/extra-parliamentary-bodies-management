using Bk.APG.Business.Models;
using Bk.APG.Business.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Bk.APG.Infrastructure.DataSource.Repositories;

public class GeneralMeasureRepository : IGeneralMeasureRepository
{
    private readonly DataContext _dataContext;

    public GeneralMeasureRepository(DataContext dataContext)
    {
        _dataContext = dataContext;
    }

    public async Task<IEnumerable<GeneralGenderMeasure>> GetGeneralGenderMeasures()
    {
        return await _dataContext.GeneralGenderMeasures
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<IEnumerable<GeneralLanguageMeasure>> GetGeneralLanguageMeasures()
    {
        return await _dataContext.GeneralLanguageMeasures
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<GeneralGenderMeasure?> GetGeneralGenderMeasure(Guid departmentId)
    {
        return await _dataContext.GeneralGenderMeasures
            .Where(x => x.DepartmentId == departmentId)
            .AsNoTracking()
            .SingleOrDefaultAsync();
    }

    public async Task<GeneralLanguageMeasure?> GetGeneralLanguageMeasure(Guid departmentId)
    {
        return await _dataContext.GeneralLanguageMeasures
            .Where(x => x.DepartmentId == departmentId)
            .AsNoTracking()
            .SingleOrDefaultAsync();
    }

    public async Task<GeneralGenderMeasure?> GetGeneralGenderMeasureForUpdate(Guid departmentId)
    {
        return await _dataContext.GeneralGenderMeasures
            .Where(x => x.DepartmentId == departmentId)
            .SingleOrDefaultAsync();
    }

    public async Task<GeneralLanguageMeasure?> GetGeneralLanguageMeasureForUpdate(Guid departmentId)
    {
        return await _dataContext.GeneralLanguageMeasures
            .Where(x => x.DepartmentId == departmentId)
            .SingleOrDefaultAsync();
    }

    public async Task AddGeneralGenderMeasure(GeneralGenderMeasure generalGenderMeasure)
    {
        await _dataContext.GeneralGenderMeasures.AddAsync(generalGenderMeasure);
        await _dataContext.SaveChangesAsync();
    }

    public async Task AddGeneralLanguageMeasure(GeneralLanguageMeasure generalLanguageMeasure)
    {
        await _dataContext.GeneralLanguageMeasures.AddAsync(generalLanguageMeasure);
        await _dataContext.SaveChangesAsync();
    }

    public async Task CommitChanges()
    {
        await _dataContext.SaveChangesAsync();
    }
}
