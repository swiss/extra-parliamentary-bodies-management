namespace Bk.APG.Business.Models;

public class EntityAuditLog
{
    public const string UpdateAction = "Update";
    public const string InsertAction = "Insert";
    public const string DeleteAction = "Delete";

    public Guid Id { get; init; }
    public required DateTime AuditDate { get; set; }
    public required string AuditAction { get; set; }
    public required string AuditUser { get; set; }

    /// <summary>
    /// Contains a serialized JSON object with the data that was changed.
    /// </summary>
    public string? AuditData { get; set; }

    public required string EntityPrimaryKey { get; set; }
    public required string EntityType { get; set; }

    /// <summary>
    /// Contains a serialized JSON object with the state of the entity after the change.
    /// </summary>
    public required string EntitySnapshot { get; set; }
}
