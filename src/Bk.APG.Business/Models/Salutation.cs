namespace Bk.APG.Business.Models;

public class Salutation : MasterDataBase
{
    public const string ManGuidAsString = "72dcba6d-c45e-489e-ac42-bb61359668b3";
    public static readonly Guid ManGuid = new(ManGuidAsString);

    public const string WomanGuidAsString = "83a925b0-db23-42c8-98b7-daaf5be02a4d";
    public static readonly Guid WomanGuid = new(ManGuidAsString);

    public Guid? GenderId { get; set; }
    public Gender? Gender { get; set; }
}
