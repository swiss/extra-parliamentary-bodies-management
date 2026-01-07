namespace Bk.APG.Business.Models;

public class CommitteeType : MasterDataBase
{
    // Verwaltungskommission
    public const string AdministrationCommissionGuidAsString = "0a4b7f1d-d8bf-4932-bece-dd2a51cc2d59";
    public static readonly Guid AdministrationCommissionGuid = Guid.Parse(AdministrationCommissionGuidAsString);
    // Behördenkommission
    public const string AuthoritiesCommissionGuidAsString = "f2e2af70-d1d4-42b5-b23a-793cbc220064";
    public static readonly Guid AuthoritiesCommissionGuid = Guid.Parse(AuthoritiesCommissionGuidAsString);
    // Leitungsorgane
    public const string ManagementCommitteeGuidAsString = "0959d68e-9c09-4ab3-9434-a5a4689c0305";
    public static readonly Guid ManagementCommitteeGuid = Guid.Parse(ManagementCommitteeGuidAsString);
    // Vertretungen des Bundes
    public const string FederalAgenciesCommitteeGuidAsString = "408865cf-2b92-4c19-ac66-ee8f4cea76e5";
    public static readonly Guid FederalAgenciesCommitteeGuid = Guid.Parse(FederalAgenciesCommitteeGuidAsString);

    public double? FemaleThreshold { get; set; }
    public double? MaleThreshold { get; set; }
    public int? GermanMinimalThreshold { get; set; }
    public int? FrenchMinimalThreshold { get; set; }
    public int? ItalianMinimalThreshold { get; set; }
    public int? RomanshMinimalThreshold { get; set; }
    public double? GermanThresholdPercentage { get; set; }
    public double? FrenchThresholdPercentage { get; set; }
    public double? ItalianThresholdPercentage { get; set; }
    public double? RomanshThresholdPercentage { get; set; }
    public uint RowVersion { get; set; }

    public ICollection<Committee> Committees { get; set; } = new List<Committee>();
    public ICollection<GeneralElectionCommittee> GeneralElectionCommittees { get; set; } = new List<GeneralElectionCommittee>();

}
