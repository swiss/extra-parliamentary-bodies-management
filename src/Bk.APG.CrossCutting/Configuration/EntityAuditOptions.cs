namespace Bk.APG.CrossCutting.Configuration;

public class EntityAuditOptions
{
    public const string SectionKey = "EntityAudit";

    public bool Enabled { get; init; }
    public string[] Entities { get; init; } = [];
}
