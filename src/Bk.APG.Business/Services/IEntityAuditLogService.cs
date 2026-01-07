using Bk.APG.Business.Dtos;
using Bk.APG.CrossCutting;

namespace Bk.APG.Business.Services;

public interface IEntityAuditLogService
{
    Task<PagedResult<EntityAuditLogDto>> GetAuditLogsForEntity(string entityId, string entityType, IEnumerable<string> relatedEntityIds, PagingParametersDto paging, string? sort, SortDirection? sortDirection);
}
