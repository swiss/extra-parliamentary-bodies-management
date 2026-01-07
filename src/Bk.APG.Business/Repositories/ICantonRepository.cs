using Bk.APG.Business.Models;

namespace Bk.APG.Business.Repositories;

public interface ICantonRepository
{
    Task<IEnumerable<Canton>> GetAll();
    Task<Canton?> GetByUri(string uri);
    Task<Canton?> GetById(Guid id);
    Task<Canton> Create(Canton canton);
    Task<Canton> Update(Canton existing, Canton update);
}
