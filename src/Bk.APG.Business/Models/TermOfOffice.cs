namespace Bk.APG.Business.Models;

public class TermOfOffice : MasterDataBase
{
    // Amtsperiode/Term of office
    public const string Period4YearsInGeneralElectionGuidAsString = "67017795-1ac4-4761-a3cc-474477a9072b";
    public static readonly Guid Period4YearsInGeneralElectionGuid = new(Period4YearsInGeneralElectionGuidAsString);
    public int DurationInYears { get; set; }

    public ICollection<Committee> Committees { get; set; } = new List<Committee>();
    public ICollection<GeneralElectionCommittee> GeneralElectionCommittees { get; set; } = new List<GeneralElectionCommittee>();
}
