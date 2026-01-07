namespace Bk.APG.Infrastructure.DataSource.Migration.Models;

public class Journal
{
    public int Id { get; set; }
    public Guid Guid { get; set; }
    public int GremId { get; set; }
    public int? LinkTypId { get; set; }
    public int? CodeId { get; set; }
    public DateTime Datum { get; set; }
    public string Kategorie { get; set; } = null!;
    public string? Text { get; set; }
    public string? Link { get; set; }
    public string Status { get; set; } = null!;
    public byte[]? FileContent { get; set; }
    public string? FileName { get; set; }
    public DateTime InsertDate { get; set; }
    public DateTime UpdateDate { get; set; }
    public string? LastUpdateUser { get; set; }

    // additional properties for data migration
    public Guid GremiumGuid { get; set; }
    public Guid JournalCodeGuid { get; set; }
    public Guid LinkTypGuid { get; set; }
    public Guid OriginalLanguageId { get; set; }
}
