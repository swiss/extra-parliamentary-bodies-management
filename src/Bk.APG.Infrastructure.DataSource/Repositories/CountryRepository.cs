using Bk.APG.Business.Models;
using Bk.APG.Business.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Bk.APG.Infrastructure.DataSource.Repositories;

public class CountryRepository : ICountryRepository
{
    private readonly DataContext _dataContext;

    public CountryRepository(DataContext dataContext)
    {
        _dataContext = dataContext;
    }

    public async Task<IEnumerable<Country>> GetAll()
    {
        return await _dataContext.Countries.OrderBy(c => c.Sort).ToListAsync();
    }

    public async Task<Country?> GetByUri(string uri)
    {
        return await _dataContext.Countries.FirstOrDefaultAsync(item => item.Uri == uri);
    }

    public async Task<Country?> GetById(Guid id)
    {
        return await _dataContext.Countries
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == id);
    }

    public async Task<Country> Create(Country canton)
    {
        var entry = await _dataContext.Countries.AddAsync(canton);
        await _dataContext.SaveChangesAsync();
        return entry.Entity;
    }

    public async Task<Country> Update(Country existing, Country update)
    {
        _dataContext.Countries.Update(existing);

        existing.Modified = update.Modified;
        existing.ModifiedBy = update.ModifiedBy;
        existing.TextDe = update.TextDe;
        existing.TextFr = update.TextFr;
        existing.TextIt = update.TextIt;
        existing.TextRm = update.TextRm;
        existing.DescriptionDe = update.DescriptionDe;
        existing.DescriptionFr = update.DescriptionFr;
        existing.DescriptionIt = update.DescriptionIt;
        existing.DescriptionRm = update.DescriptionRm;

        await _dataContext.SaveChangesAsync();

        return existing;
    }
}
