namespace Bk.APG.Business.Models;

public class WorklistTaskState : MasterDataBase
{
    private const string ActiveIdAsString = "4d5fbb42-93b7-4a71-b0a4-3a3fbd6f9e75";
    public static readonly Guid Active = Guid.Parse(ActiveIdAsString);

    private const string InactiveIdAsString = "0c4f6a7d-3e42-49a0-9b1c-27a2e7d2f621";
    public static readonly Guid Inactive = Guid.Parse(InactiveIdAsString);

    public static readonly Guid Completed = Guid.Parse("a5c92d78-70f3-4ca1-9e2e-814f8d7f30c0");
}
