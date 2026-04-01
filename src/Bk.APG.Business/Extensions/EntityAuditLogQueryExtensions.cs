using Bk.APG.Business.Models;
using Bk.APG.CrossCutting;

namespace Bk.APG.Business.Extensions;

public static class EntityAuditLogQueryExtensions
{
    public static IQueryable<EntityAuditLog> SortEntityAuditLogs(this IQueryable<EntityAuditLog> entityAuditLogs, string sort, SortDirection sortDirection)
    {
        ArgumentNullException.ThrowIfNull(sort);

#pragma warning disable CA1308
        return sort.ToLowerInvariant() switch
#pragma warning restore CA1308
        {
            "audituser" => sortDirection == SortDirection.Asc
                ? entityAuditLogs.OrderBy(x => x.AuditUser)
                : entityAuditLogs.OrderByDescending(x => x.AuditUser),
            "auditdate" => sortDirection == SortDirection.Asc
                ? entityAuditLogs.OrderBy(x => x.AuditDate)
                : entityAuditLogs.OrderByDescending(x => x.AuditDate),
            "entitytype" => sortDirection == SortDirection.Asc
                ? entityAuditLogs.OrderBy(x => x.EntityType)
                : entityAuditLogs.OrderByDescending(x => x.EntityType),
            "auditaction" => sortDirection == SortDirection.Asc
                ? entityAuditLogs.OrderBy(x => x.AuditAction)
                : entityAuditLogs.OrderByDescending(x => x.AuditAction),
            _ => entityAuditLogs
        };
    }
}
