using Bk.APG.Business.Models;

namespace Bk.APG.Business.Repositories;

public interface IEiamAssignmentRepository
{
    Task<EiamAssignment> GetByExternalId(string externalId);
    Task<EiamAssignment> GetById(Guid id);
}
