namespace Bk.APG.Infrastructure.DataSource.Migration.Models;

public class Adresse
{
    public int Id { get; set; }
    public int? PersId { get; set; }
    public string? Strasse { get; set; }
    public string? LänderCode { get; set; }
    public string? Plz { get; set; }
    public string? Ort { get; set; }
    public string? Kanton { get; set; }
    public string? Email { get; set; }
    public string? Mobile { get; set; }
    public string? Telefon { get; set; }
    public string? Fax { get; set; }
    public string? Firma { get; set; }
    public string? Postfach { get; set; }
    public bool? AdrGeschäftlich { get; set; }
    public bool? Anschriftadresse { get; set; }
    public int? IdPlzVerzeichnis { get; set; }
    public int? PlzZusatz { get; set; }
    public DateTime InsertDate { get; set; }
    public DateTime UpdateDate { get; set; }
    public string? LastupdateUser { get; set; }
    public DateTime? Histo { get; set; }
    public int? OriginalPER_ANSCHRIFT { get; set; }

    // additional fields from query
    public Guid Guid { get; set; }
    public Guid? KantonGuid { get; set; }
}
