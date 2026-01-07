namespace Bk.APG.Infrastructure.DataSource.Migration.Models;

public class Interessenbindung
{
    public int Id { get; set; }
    public Guid Guid { get; set; }
    public int PersId { get; set; }
    public int RechtsformId { get; set; }
    public int GremiumId { get; set; }
    public int FunktionId { get; set; }
    public required string Text { get; set; }
    public bool GEW { get; set; }
    public string? Bemerkungen { get; set; }
    public DateTime InsertDate { get; set; }
    public DateTime UpdateDate { get; set; }
    public string? LastupdateUser { get; set; }

    // additional fields for migration
    public Guid PersonGuid { get; set; }
    public Guid RechtsformGuid { get; set; }
    public Guid GremiumGuid { get; set; }
    public Guid FunktionGuid { get; set; }
}

