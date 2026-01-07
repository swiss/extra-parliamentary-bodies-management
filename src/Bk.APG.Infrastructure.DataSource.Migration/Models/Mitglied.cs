namespace Bk.APG.Infrastructure.DataSource.Migration.Models;

public class Mitglied
{
    public int Id { get; set; }
    public Guid Guid { get; set; }

    public int GremId { get; set; }
    public int PerId { get; set; }
    public int FnkId { get; set; }
    public int WaaId { get; set; }
    public int WabId { get; set; }
    public int? MgzId { get; set; }
    public DateTime SeitDate { get; set; }
    public DateTime BisDate { get; set; }

    public string? Mitgliedzusatz { get; set; }
    public string? BemerkungStatus { get; set; }
    public string? BegrAmtszeit { get; set; }
    public string? BegrBvers { get; set; }
    public string? BegrAlter { get; set; }
    public string? BegrBangest { get; set; }
    public int? BeschäftigungsGradVon { get; set; }
    public int? BeschäftigungsGradBis { get; set; }
    public bool Vollamtlich { get; set; }
    public bool KomVer { get; set; }
    public string? Bemerkung { get; set; }
    public string? RechtsverhältnisBund { get; set; }
    public string? RechtsverhältnisGrem { get; set; }
    public bool GEW { get; set; }
    public DateTime InsertDate { get; set; }
    public DateTime UpdateDate { get; set; }
    public string? LastupdateUser { get; set; }
    public DateTime? Histo { get; set; }

    // additional fields for migration
    public Guid PersonGuid { get; set; }
    public Guid GremiumGuid { get; set; }
    public Guid WahlbehördeGuid { get; set; }
    public Guid WahlartGuid { get; set; }
    public Guid FunktionGuid { get; set; }
}
