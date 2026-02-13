using Bk.APG.Business.Models;

namespace Bk.APG.Business.Repositories;

public interface IFormLetterSenderRepository
{
    Task<IEnumerable<FormLetterSender>> GetAll();
    Task<FormLetterSender> Create(FormLetterSender formLetterSender);
    Task<FormLetterSender> GetByIdForUpdate(Guid id);
    void Delete(FormLetterSender formLetterSender);
    Task CommitChanges();
}
