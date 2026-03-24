namespace Bk.APG.Business.Models;

public class Gender : MasterDataBase
{
    public const string Male = "https://register.ld.admin.ch/i14y/concept/sex/1";
    public const string Female = "https://register.ld.admin.ch/i14y/concept/sex/2";

    public const string MaleId = "d45a5c83-ddf8-4e78-9fd9-0cf2a8914d15";
    public const string FemaleId = "aa36da2a-b1d5-4b1e-a659-3f488dbc4d1e";

    public static readonly Guid MaleGuid = Guid.Parse(MaleId);
    public static readonly Guid FemaleGuid = Guid.Parse(FemaleId);
}
