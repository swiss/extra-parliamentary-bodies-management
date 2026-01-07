namespace Bk.APG.Business.Models;

public class Language : MasterDataBase
{
    public const string German = "de";
    public const string French = "fr";
    public const string Italian = "it";

    public const string GermanUri = "http://publications.europa.eu/resource/authority/language/DEU";
    public const string FrenchUri = "http://publications.europa.eu/resource/authority/language/FRA";
    public const string ItalianUri = "http://publications.europa.eu/resource/authority/language/ITA";
    public const string RomanshUri = "http://publications.europa.eu/resource/authority/language/ROH";

    public const string GermanId = "8d537725-5674-48a9-9f45-85a2aa45649c";
    public const string FrenchId = "c09377e5-67fe-440a-ab7e-8ee6de44f7ca";
    public const string ItalianId = "caff8c43-2247-4d33-bfc5-b990bdb39c34";
    public const string RomanshId = "fb209c78-1036-4c6a-a6c5-aebdc408963c";

    public static readonly Guid GermanGuid = Guid.Parse(GermanId);
    public static readonly Guid FrenchGuid = Guid.Parse(FrenchId);
    public static readonly Guid ItalianGuid = Guid.Parse(ItalianId);
    public static readonly Guid RomanshGuid = Guid.Parse(RomanshId);
}
