namespace Bk.APG.Business.Models;

public class WorklistTaskType : MasterDataBase
{
    private const string GeneralElectionStartIdAsString = "6b8ea0f1-12d7-49de-ae38-fc57a87a6b1d";
    public static readonly Guid GeneralElectionStart = Guid.Parse(GeneralElectionStartIdAsString);

    private const string GeneralElectionDispatchIdAsString = "f3e2b1c4-9d7a-4b2e-9231-7cb04a5b0d66";
    public static readonly Guid GeneralElectionDispatch = Guid.Parse(GeneralElectionDispatchIdAsString);

    public static readonly Guid CandidateListApprove = Guid.Parse("bd516645-e427-4dcf-b6e1-8464b8645818");
    public static readonly Guid CandidateListCreate = Guid.Parse("d2de9cdd-9d16-4564-8968-1df10f9fb3ce");
    public static readonly Guid CandidateListForward = Guid.Parse("95fd596a-1a7c-47d7-9b5f-50a5cc3fe43c");

    public static readonly Guid ReadyForFederalCouncilProposal = Guid.Parse("46aacc55-40b7-49d1-ac13-d6ab445a9943");

    public static readonly Guid GeneralElectionMissingJustifications = Guid.Parse("28f59879-a211-4816-b894-66f8d60961a8");

    public required bool CanBeCreatedManually { get; set; }
}
