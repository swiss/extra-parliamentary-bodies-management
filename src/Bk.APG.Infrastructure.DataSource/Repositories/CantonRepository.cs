using Bk.APG.Business.Models;
using Bk.APG.Business.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Bk.APG.Infrastructure.DataSource.Repositories;

public class CantonRepository : ICantonRepository
{
    private readonly DataContext _dataContext;

    public CantonRepository(DataContext dataContext)
    {
        _dataContext = dataContext;
    }

    public async Task<IEnumerable<Canton>> GetAll()
    {
        return await _dataContext.Cantons.OrderBy(c => c.Sort).ToListAsync();
    }

    public async Task<Canton?> GetByUri(string uri)
    {
        return await _dataContext.Cantons.FirstOrDefaultAsync(item => item.Uri == uri);
    }

    public async Task<Canton?> GetById(Guid id)
    {
        return await _dataContext.Cantons
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == id);
    }

    public async Task<Canton> Create(Canton canton)
    {
        var entry = await _dataContext.Cantons.AddAsync(canton);
        await _dataContext.SaveChangesAsync();
        return entry.Entity;
    }

    public async Task<Canton> Update(Canton existing, Canton update)
    {
        ArgumentNullException.ThrowIfNull(existing);
        ArgumentNullException.ThrowIfNull(update);

        _dataContext.Cantons.Update(existing);

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
