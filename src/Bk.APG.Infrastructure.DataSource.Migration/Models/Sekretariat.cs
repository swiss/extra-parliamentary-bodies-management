namespace Bk.APG.Infrastructure.DataSource.Migration.Models;

public class Sekretariat
{
    public int Id { get; set; }
    public Guid Guid { get; set; }
    public int GremId { get; set; }
    public int? GeschlechtId { get; set; }
    public int? SpracheId { get; set; }
    public DateTime Seit { get; set; }
    public DateTime? Bis { get; set; }
    public string? NameOrganisation { get; set; }
    public string? Vorname { get; set; }
    public string? Firma { get; set; }
    public string? Plz { get; set; }
    public string? Ort { get; set; }
    public string? AkadTitel { get; set; }
    public string? TitelBriefanrede { get; set; }
    public string? Briefanrede { get; set; }
    public string? Strasse { get; set; }
    public string? Postfach { get; set; }
    public string? Tel { get; set; }
    public string? Fax { get; set; }
    public string? Mobile { get; set; }
    public string? EMail { get; set; }
    public DateTime InsertDate { get; set; }
    public DateTime UpdateDate { get; set; }
    public string? LastUpdateUser { get; set; }

    // additional properties for datamigration
    public bool? Datenschutzberater { get; set; }
    public string? Sektion { get; set; }
    public string? Nachname { get; set; }
    public string? PersonTel { get; set; }
    public string? PersonMobile { get; set; }
    public string? PersonEMail { get; set; }
    public Guid AnredeGuid { get; set; }
    public Guid GeschlechtGuid { get; set; }
    public Guid SpracheGuid { get; set; }
    public Guid GremiumGuid { get; set; }
    public Guid ContactPointTypeGuid { get; set; }
}
