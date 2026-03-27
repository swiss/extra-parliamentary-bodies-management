using Bk.APG.Business.Models;

namespace Bk.APG.Business.Repositories;

public interface IDocumentStorageRepository
{
    void CreateForMigration(DocumentStorage documentStorage);
    Task<DocumentStorage> Create(DocumentStorage documentStorage);
    Task<DocumentStorage> GetByIdForUpdate(Guid documentStorageId);
    void Delete(DocumentStorage documentStorage);
}
