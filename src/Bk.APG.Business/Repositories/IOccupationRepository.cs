using Bk.APG.Business.Models;

namespace Bk.APG.Business.Repositories;

public interface IOccupationRepository
{
    Task<IEnumerable<Occupation>> GetBySearchString(string searchString);
    Occupation? GetBySearchStringForMigration(string searchString);
    Task<Occupation?> GetById(Guid id);
    Occupation? GetByUri(string uri);
}
