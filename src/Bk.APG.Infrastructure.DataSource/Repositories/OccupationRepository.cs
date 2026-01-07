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

    // Used, to cleanup the migrated data.
    public async Task RemoveAllGermanDuplicatesForCleanup()
    {
        var duplicates = await _dataContext.Occupations
            .GroupBy(e => e.TextDe)
            .Where(g => g.Count() > 1)
            .Skip(1)
            // .SelectMany(g => g.OrderBy(e => e.Id).Skip(1))  // skip first, take rest as duplicates
            .ToListAsync();

        //_dataContext.Occupations.RemoveRange(duplicates);
        //_dataContext.SaveChanges();
    }

    public async Task<Occupation?> GetById(Guid id)
    {
        return await _dataContext.Occupations
            .AsNoTracking()
            .SingleOrDefaultAsync(x => x.Id == id);
    }
}
