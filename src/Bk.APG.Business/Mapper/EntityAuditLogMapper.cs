using System.Text.Json;
using System.Text.Json.Serialization;
using Bk.APG.Business.Dtos;
using Bk.APG.Business.Models;

namespace Bk.APG.Business.Mapper;

public static class EntityAuditLogMapper
{
    public static EntityAuditLogDto MapToDto(EntityAuditLog entityAuditLog)
    {
        ArgumentNullException.ThrowIfNull(entityAuditLog);

        var auditDataBefore = new List<EntityAuditLogDataDto>();
        var auditDataAfter = new List<EntityAuditLogDataDto>();

        if (entityAuditLog.AuditAction == EntityAuditLog.DeleteAction)
        {
            var snapshotData = DeserializeEntitySnapshot(entityAuditLog.EntitySnapshot);
            if (snapshotData != null)
            {
                auditDataBefore.AddRange(snapshotData);
            }
        }
        else if (entityAuditLog.AuditAction == EntityAuditLog.InsertAction)
        {
            var snapshotData = DeserializeEntitySnapshot(entityAuditLog.EntitySnapshot);
            if (snapshotData != null)
            {
                auditDataAfter.AddRange(snapshotData);
            }
        }
        else
        {
            if (!string.IsNullOrWhiteSpace(entityAuditLog.AuditData))
            {
                try
                {
                    var changes = JsonSerializer.Deserialize<AuditChange[]>(entityAuditLog.AuditData);

                    if (changes != null)
                    {
                        auditDataBefore.AddRange(changes
                            .Where(c => c.OriginalValue != null)
                            .Select(c => new EntityAuditLogDataDto { ColumnName = c.ColumnName, Data = c.OriginalValue?.ToString() }));

                        auditDataAfter.AddRange(changes
                            .Where(c => c.NewValue != null)
                            .Select(c => new EntityAuditLogDataDto { ColumnName = c.ColumnName, Data = c.NewValue?.ToString() }));
                    }
                }
                catch (JsonException)
                {
                    // If deserialization fails, leave the strings empty
                }
            }
        }

        return new EntityAuditLogDto
        {
            AuditDate = entityAuditLog.AuditDate,
            AuditAction = entityAuditLog.AuditAction,
            AuditUser = entityAuditLog.AuditUser,
            AuditDataBefore = auditDataBefore,
            AuditDataAfter = auditDataAfter,
            EntityType = entityAuditLog.EntityType
        };
    }

    private static List<EntityAuditLogDataDto>? DeserializeEntitySnapshot(string entitySnapshot)
    {
        if (string.IsNullOrWhiteSpace(entitySnapshot))
        {
            return null;
        }

        try
        {
            var snapshotDictionary = JsonSerializer.Deserialize<Dictionary<string, object>>(entitySnapshot);
            if (snapshotDictionary != null)
            {
                return snapshotDictionary
                    .Select(keyValuePairs => new EntityAuditLogDataDto
                    {
                        ColumnName = keyValuePairs.Key,
                        Data = keyValuePairs.Value?.ToString() // Compile-time nullability warning suppression: Value can be null!
                    })
                    .ToList();
            }
        }
        catch (JsonException)
        {
            // If deserialization fails, return null
        }

        return null;
    }


#pragma warning disable CA1812 //the class is instatiated via serialization
    private class AuditChange
    {
        [JsonPropertyName("columnName")]
        public string ColumnName { get; init; } = string.Empty;

        [JsonPropertyName("originalValue")]
        public object? OriginalValue { get; init; }

        [JsonPropertyName("newValue")]
        public object? NewValue { get; init; }
    }
#pragma warning restore CA1812
}
