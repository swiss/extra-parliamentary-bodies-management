using Bk.APG.Business.Models;
using Bk.APG.Business.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Bk.APG.Infrastructure.DataSource.Repositories;

public class OccupationRepository : IOccupationRepository
{
    private readonly DataContext _dataContext;

    public OccupationRepository(DataContext context)
    {
        _dataContext = context;
    }

    public async Task<IEnumerable<Occupation>> GetBySearchString(string searchString)
    {
        return await _dataContext.Occupations
            .Where(o => o.TextDe.Contains(searchString) ||
                        o.TextFr.Contains(searchString) ||
                        o.TextIt.Contains(searchString) ||
                        o.TextFemaleDe.Contains(searchString) ||
                        o.TextFemaleFr.Contains(searchString) ||
                        o.TextFemaleIt.Contains(searchString))
            .ToListAsync();
    }

    public Occupation? GetBySearchStringForMigration(string searchString)
    {
        return _dataContext.Occupations
            .FirstOrDefault(o => o.TextDe == searchString ||
                                 o.TextFr == searchString ||
                                 o.TextIt == searchString ||
                                 o.TextFemaleDe == searchString ||
                                 o.TextFemaleFr == searchString ||
                                 o.TextFemaleIt == searchString);
    }

    public async Task<Occupation?> GetById(Guid id)
    {
        return await _dataContext.Occupations
            .AsNoTracking()
            .SingleOrDefaultAsync(x => x.Id == id);
    }

    public Occupation? GetByUri(string uri)
    {
        return _dataContext.Occupations
            .AsNoTracking()
            .SingleOrDefault(x => x.Uri == uri);
    }
}
