namespace Bk.APG.Infrastructure.DataSource.Migration.Models;

public class Person
{
    public int Id { get; set; }
    public Guid Guid { get; set; }
    public int? SpracheId { get; set; }
    public int? KorrespSpracheId { get; set; }
    public int? GeschlechtId { get; set; }
    public string? Briefanrede { get; set; }
    public string? TitelText { get; set; }
    public required string Name { get; set; }
    public required string Vorname { get; set; }
    public int Jahrgang { get; set; }
    public string? Beruf { get; set; }
    public string? Organisation { get; set; }
    public bool AngestelltBD { get; set; }
    public bool MitgliedBV { get; set; }
    public string? Funktion { get; set; }
    public string? AnredeTitel { get; set; }
    public string? BemerkungPersonendaten { get; set; }
    public string? BemerkungInteressenbindung { get; set; }
    public string? BemerkungMitgliedschaft { get; set; }
    public string? BemerkungPersonendatenAdmin { get; set; }
    public string? BemerkungInteressenbindungAdmin { get; set; }
    public string? BemerkungMitgliedschaftAdmin { get; set; }
    public DateTime InsertDate { get; set; }
    public DateTime UpdateDate { get; set; }
    public string? LastupdateUser { get; set; }
    public DateTime? Histo { get; set; }
    public string? Interessenbindung_Alt { get; set; }

    // additional properties for datamigration
    public Guid AnredeGuid { get; set; }
    public Guid GeschlechtGuid { get; set; }
    public Guid? PrivatAdresseGuid { get; set; }
    public Guid? GeschäftsAdresseGuid { get; set; }
    public Guid AktiveAdresseGuid { get; set; }
    public Guid SpracheGuid { get; set; }
    public Guid KorrespondenzSpracheGuid { get; set; }
    public int AnzahlInteressen { get; set; }
}
