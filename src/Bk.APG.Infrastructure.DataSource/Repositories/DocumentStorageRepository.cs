using Bk.APG.Business.Models;
using Bk.APG.Business.Repositories;
using Bk.APG.CrossCutting.Exception;
using Microsoft.EntityFrameworkCore;

namespace Bk.APG.Infrastructure.DataSource.Repositories;

public class DocumentStorageRepository : IDocumentStorageRepository
{
    private readonly DataContext _dataContext;

    public DocumentStorageRepository(DataContext dataContext)
    {
        _dataContext = dataContext;
    }

    public void CreateForMigration(DocumentStorage documentStorage)
    {
        _dataContext.DocumentStorages.Add(documentStorage);
    }

    public async Task<DocumentStorage> Create(DocumentStorage documentStorage)
    {
        var entry = await _dataContext.DocumentStorages.AddAsync(documentStorage);

        return entry.Entity;
    }

    public async Task<DocumentStorage> GetByIdForUpdate(Guid documentStorageId)
    {
        var documentStorage = await _dataContext.DocumentStorages.FirstOrDefaultAsync(x => x.Id == documentStorageId);

        if (documentStorage is null)
        {
            throw new EntityNotFoundException($"Document Storage Id={documentStorageId} not found");
        }

        return documentStorage;
    }

    public void Delete(DocumentStorage documentStorage)
    {
        _dataContext.DocumentStorages.Remove(documentStorage);
    }
}
