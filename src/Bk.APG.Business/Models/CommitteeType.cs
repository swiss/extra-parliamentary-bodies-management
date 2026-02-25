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
    // Vertretungen des Bundes in grenzüberschreitenden Gremien
    public const string CrossBorderFederalAgenciesCommitteeGuidAsString = "469134d7-fb82-42ac-8feb-c2df6b7d032b";
    public static readonly Guid CrossBorderFederalAgenciesCommitteeGuid = Guid.Parse(CrossBorderFederalAgenciesCommitteeGuidAsString);

    // APK, no master data, just to help us to have a constant GUID
    public const string ExtraParliamentaryCommitteeGuidAsString = "7e5c6a3b-9d2b-4c0f-8f91-6d5a1a2f3e47";
    public static readonly Guid ExtraParliamentaryCommitteeGuid = Guid.Parse(ExtraParliamentaryCommitteeGuidAsString);
    // Nicht APK (Leitungsorgange/Vertretungen des Bundes OHNE grenzüberschreitende!)
    public const string NonExtraParliamentaryCommitteeGuidAsString = "c1b84a90-5f37-4b92-9d6a-2e8f7c4d1b55";
    public static readonly Guid NonExtraParliamentaryCommitteeGuid = Guid.Parse(NonExtraParliamentaryCommitteeGuidAsString);

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
