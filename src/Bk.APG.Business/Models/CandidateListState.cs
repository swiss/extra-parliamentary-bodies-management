namespace Bk.APG.Business.Models;

public class CandidateListState : MasterDataBase
{
    // Entwurf
    public const string DraftGuidAsString = "fa78e8d2-8c67-4e1c-8497-6bcac845b3d1";
    public static readonly Guid Draft = Guid.Parse(DraftGuidAsString);
    public static readonly Guid Proposed = Guid.Parse("c9d2b6e3-0f3f-40f3-8bb2-fdb6cf1f2073");
    public static readonly Guid ReadyProposalFedCouncil = Guid.Parse("b73a46c7-3222-4d5f-87ff-42c3e82b23dc");
    public static readonly Guid Completed = Guid.Parse("7ec5c44a-b6ae-45bc-8d91-5b8d2db7c897");

    public ICollection<GeneralElectionCommittee> GeneralElectionCommittees { get; set; } = new List<GeneralElectionCommittee>();
}
