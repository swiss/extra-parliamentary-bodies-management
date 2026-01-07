using Bk.APG.Business.Extensions;
using Bk.APG.Business.Models;
using Bk.APG.Business.Repositories;
using Bk.APG.CrossCutting;
using Microsoft.EntityFrameworkCore;

namespace Bk.APG.Infrastructure.DataSource.Repositories;

public class EntityAuditLogRepository : IEntityAuditLogRepository
{
    private readonly DataContext _dataContext;

    public EntityAuditLogRepository(DataContext dataContext)
    {
        _dataContext = dataContext;
    }

    public async Task<PagedResult<EntityAuditLog>> GetEntityAuditLogs(string entityId, IEnumerable<string> relatedEntityIds, PagingParameters paging, string? sort, SortDirection? sortDirection)
    {
        // Build a dynamic SQL query that searches for the entityId or any related entity IDs in the entity_snapshot JSON
        // Using PostgreSQL's ::text casting to convert JSON to text and LIKE to search for IDs anywhere in the JSON
        var allEntityIds = new[] { entityId }.Concat(relatedEntityIds).ToArray();

        var conditions = string.Join(" OR ",
            allEntityIds.Select((_, index) => $"entity_snapshot::text LIKE '%' || {{{index}}} || '%'"));

        var sql = $"""
                               SELECT * FROM data.entity_audit_log
                               WHERE ({conditions})
                               AND NOT (audit_action = 'Update' AND (audit_data IS NULL OR audit_data = '[]'))
                   """;

        var query = _dataContext.EntityAuditLog
            .FromSqlRaw(sql, allEntityIds.Cast<object>().ToArray())
            .OrderByDescending(x => x.AuditDate);

        var count = await query.CountAsync();

        var items = await query
            .SortEntityAuditLogs(sort ?? "auditDate", sortDirection.GetValueOrDefault(SortDirection.Desc))
            .Skip(paging.PageIndex * paging.PageSize)
            .Take(paging.PageSize)
            .ToListAsync();

        return new PagedResult<EntityAuditLog>
        {
            Index = paging.PageIndex,
            Total = count,
            Items = items
        };
    }
}
