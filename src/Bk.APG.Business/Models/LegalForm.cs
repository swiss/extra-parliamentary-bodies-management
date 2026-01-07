namespace Bk.APG.Business.Models;

public class LegalForm : MasterDataBase
{
    public required string LegalFormId { get; set; }

    public ICollection<Interest> Interests { get; set; } = new List<Interest>();
    public ICollection<Committee>? Committees { get; set; } = new List<Committee>();
    public ICollection<GeneralElectionCommittee>? GeneralElectionCommittees { get; set; } = new List<GeneralElectionCommittee>();
}
