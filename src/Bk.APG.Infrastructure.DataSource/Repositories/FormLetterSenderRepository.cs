using Bk.APG.Business.Models;
using Bk.APG.Business.Repositories;
using Bk.APG.CrossCutting.Exception;
using Microsoft.EntityFrameworkCore;

namespace Bk.APG.Infrastructure.DataSource.Repositories;

public class FormLetterSenderRepository : IFormLetterSenderRepository
{
    private readonly DataContext _dataContext;

    public FormLetterSenderRepository(DataContext dataContext)
    {
        _dataContext = dataContext;
    }

    public async Task<IEnumerable<FormLetterSender>> GetAll()
    {
        return await _dataContext.FormLetterSenders
            .Include(x => x.SenderFunction)
            .Include(x => x.Department)
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<FormLetterSender> Create(FormLetterSender formLetterSender)
    {
        var entry = await _dataContext.FormLetterSenders.AddAsync(formLetterSender);
        await _dataContext.SaveChangesAsync();

        return entry.Entity;
    }

    public async Task<FormLetterSender> GetByIdForUpdate(Guid id)
    {
        var formLetterSender = await _dataContext.FormLetterSenders
            .Include(x => x.SignatureFileReference)
            .FirstOrDefaultAsync(x => x.Id == id);
        return formLetterSender ?? throw new EntityNotFoundException($"Form letter sender with id {id} was not found.");
    }

    public void Delete(FormLetterSender formLetterSender)
    {
        _dataContext.FormLetterSenders.Remove(formLetterSender);
    }

    public async Task CommitChanges()
    {
        await _dataContext.SaveChangesAsync();
    }
}
