namespace Bk.APG.Business.Models;

public class CommitteeLevel : MasterDataBase
{
    public const string FederalCouncilGuidAsString = "41756a60-f5ee-41d8-9449-e4abd36f4fe5";
    public static readonly Guid FederalCouncilGuid = Guid.Parse(FederalCouncilGuidAsString);

    public ICollection<Committee> Committees { get; set; } = new List<Committee>();
    public ICollection<GeneralElectionCommittee> GeneralElectionCommittees { get; set; } = new List<GeneralElectionCommittee>();

}
