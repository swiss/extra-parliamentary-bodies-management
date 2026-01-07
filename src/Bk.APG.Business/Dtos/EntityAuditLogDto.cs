namespace Bk.APG.Business.Dtos;

public class EntityAuditLogDto
{
    public required DateTime AuditDate { get; init; }
    public required string AuditAction { get; init; }
    public required string AuditUser { get; init; }
    public IEnumerable<EntityAuditLogDataDto> AuditDataBefore { get; set; } = [];
    public IEnumerable<EntityAuditLogDataDto> AuditDataAfter { get; set; } = [];
    public required string EntityType { get; init; }
}

public class EntityAuditLogDataDto
{
    public required string ColumnName { get; init; }
    public string? Data { get; set; }
}
