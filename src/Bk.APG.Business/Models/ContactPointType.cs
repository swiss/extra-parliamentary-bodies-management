namespace Bk.APG.Business.Models;

public class ContactPointType : MasterDataBase
{
    public static readonly Guid SecretariatGuid = Guid.Parse("a52067bf-5819-4567-8650-ca042c2ff2c7");
    public static readonly Guid DataProtectionOfficerGuid = Guid.Parse("cc71de49-4144-41c2-987d-9a5e584f948f");

    public ICollection<ContactPoint> ContactPoints { get; set; } = new List<ContactPoint>();
}
