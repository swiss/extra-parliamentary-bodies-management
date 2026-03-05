namespace Bk.APG.Business.Models;

public class WorklistTaskType : MasterDataBase
{
    private const string GeneralElectionStartIdAsString = "6b8ea0f1-12d7-49de-ae38-fc57a87a6b1d";
    public static readonly Guid GeneralElectionStart = Guid.Parse(GeneralElectionStartIdAsString);

    public static readonly Guid GeneralElectionEnd = Guid.Parse("3f9c8b7a-6c1e-4e7d-9a2f-5d8c1b3a4f92");

    private const string GeneralElectionDispatchIdAsString = "f3e2b1c4-9d7a-4b2e-9231-7cb04a5b0d66";
    public static readonly Guid GeneralElectionDispatch = Guid.Parse(GeneralElectionDispatchIdAsString);

    public static readonly Guid CandidateListApprove = Guid.Parse("bd516645-e427-4dcf-b6e1-8464b8645818");
    public static readonly Guid CandidateListCreate = Guid.Parse("d2de9cdd-9d16-4564-8968-1df10f9fb3ce");
    public static readonly Guid CandidateListForward = Guid.Parse("95fd596a-1a7c-47d7-9b5f-50a5cc3fe43c");

    public static readonly Guid ReadyForFederalCouncilProposal = Guid.Parse("46aacc55-40b7-49d1-ac13-d6ab445a9943");

    public static readonly Guid GeneralElectionMissingJustifications = Guid.Parse("28f59879-a211-4816-b894-66f8d60961a8");
    public static readonly Guid GeneralElectionPersonInterests = Guid.Parse("e50e54df-081f-4f72-a7a6-246dc0538f2a");
    public static readonly Guid GeneralElectionPersonBaseData = Guid.Parse("267fed1d-e66c-439f-bb7a-15e63848c7dd");
    public static readonly Guid GeneralElectionMembershipValidation = Guid.Parse("c95c804c-512c-477a-8a63-ab8ae1eef2ec");
    public static readonly Guid GeneralElectionMissingSecretariat = Guid.Parse("18e4a819-0176-409a-ace1-7c91b634f7ae");
    public static readonly Guid GeneralElectionMissingDataProtectionOfficer = Guid.Parse("60bce508-9011-42d5-b501-907e5eaad326");

    public static readonly Guid GeneralMeasureCheck = Guid.Parse("c1577045-eda3-445c-8e10-5e326f4650bc");
    public static readonly Guid GeneralMeasureValidate = Guid.Parse("c262c4b0-8661-488d-a69f-c7ff6ced784a");

    public required bool CanBeCreatedManually { get; set; }
}
