using Bk.APG.Business.Models;
using Bk.APG.Business.Repositories;
using Bk.APG.CrossCutting.Exception;
using Microsoft.EntityFrameworkCore;

namespace Bk.APG.Infrastructure.DataSource.Repositories;

public class TermOfOfficeDateRepository : ITermOfOfficeDateRepository
{
    private readonly DataContext _dataContext;

    public TermOfOfficeDateRepository(DataContext dataContext)
    {
        _dataContext = dataContext;
    }

    public async Task<IEnumerable<TermOfOfficeDate>> GetAll()
    {
        return await _dataContext.TermOfOfficeDates.Where(t => t.Id != TermOfOfficeDate.IndefiniteDurationGuid).OrderBy(t => t.Sort).ToListAsync();
    }

    public async Task<TermOfOfficeDate> GetById(Guid id)
    {
        var termOfOfficeDate = await _dataContext.TermOfOfficeDates
            .AsNoTracking()
            .FirstOrDefaultAsync(t => t.Id == id);

        if (termOfOfficeDate is null)
        {
            throw new EntityNotFoundException($"TermOfOfficeDate Id={id} not found");
        }

        return termOfOfficeDate;
    }

    public async Task<TermOfOfficeDate> Update(TermOfOfficeDate existing, TermOfOfficeDate update)
    {
        ArgumentNullException.ThrowIfNull(existing);
        ArgumentNullException.ThrowIfNull(update);

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
        existing.IsGeneralElection = update.IsGeneralElection;

        await _dataContext.SaveChangesAsync();

        return existing;
    }
}
