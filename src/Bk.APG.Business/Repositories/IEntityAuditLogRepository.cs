using Bk.APG.Business.Models;
using Bk.APG.CrossCutting;

namespace Bk.APG.Business.Repositories;

public interface IEntityAuditLogRepository
{
    Task<PagedResult<EntityAuditLog>> GetEntityAuditLogs(string entityId, IEnumerable<string> relatedEntityIds, PagingParameters paging, string? sort, SortDirection? sortDirection);
}
