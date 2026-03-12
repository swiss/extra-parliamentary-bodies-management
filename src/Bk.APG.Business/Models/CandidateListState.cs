namespace Bk.APG.Business.Models;

public class CandidateListState : MasterDataBase
{
    public const string DraftGuidAsString = "fa78e8d2-8c67-4e1c-8497-6bcac845b3d1";
    public const string ProposedGuidAsString = "c9d2b6e3-0f3f-40f3-8bb2-fdb6cf1f2073";
    public const string ReadyForFederalCouncilProposalForwardedGuidAsString = "192e3c42-e67d-4d6d-8051-e5c2a1cbbccd";
    public const string ReadyForFederalCouncilProposalFinalizedGuidAsString = "b73a46c7-3222-4d5f-87ff-42c3e82b23dc";
    public const string ValidatedGuidAsString = "7ec5c44a-b6ae-45bc-8d91-5b8d2db7c897";

    public static readonly Guid Draft = Guid.Parse(DraftGuidAsString);
    public static readonly Guid Proposed = Guid.Parse(ProposedGuidAsString);
    public static readonly Guid ReadyForFederalCouncilProposalForwarded = Guid.Parse(ReadyForFederalCouncilProposalForwardedGuidAsString);
    public static readonly Guid ReadyForFederalCouncilProposalFinalized = Guid.Parse(ReadyForFederalCouncilProposalFinalizedGuidAsString);
    public static readonly Guid Validated = Guid.Parse(ValidatedGuidAsString);

    public ICollection<GeneralElectionCommittee> GeneralElectionCommittees { get; set; } = new List<GeneralElectionCommittee>();
}
