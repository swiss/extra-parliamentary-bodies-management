using System.Globalization;

namespace Bk.APG.Business.Models;

public class MasterDataBase : EntityBase
{
    public int OgdId { get; set; }
    public required bool IsDeleted { get; set; }
    public required string TextDe { get; set; }
    public required string TextFr { get; set; }
    public required string TextIt { get; set; }
    public required string TextRm { get; set; }
    public required string DescriptionDe { get; set; }
    public required string DescriptionFr { get; set; }
    public required string DescriptionIt { get; set; }
    public required string DescriptionRm { get; set; }
    public required int Sort { get; set; }
    public required string Uri { get; init; }
    // this fields helps for datamigration. As soon as this is done, it can be removed
    public int OldId { get; set; }

    public string GetText()
    {
        return GetText(CultureInfo.CurrentUICulture);
    }

    public string GetText(CultureInfo cultureInfo)
    {
        ArgumentNullException.ThrowIfNull(cultureInfo);

        return cultureInfo.TwoLetterISOLanguageName switch
        {
            Language.German => TextDe,
            Language.French => !string.IsNullOrWhiteSpace(TextFr) ? TextFr : TextDe,
            Language.Italian => !string.IsNullOrWhiteSpace(TextIt) ? TextIt : TextDe,
            _ => TextDe
        };
    }

    public string GetDescription()
    {
        return GetDescription(CultureInfo.CurrentUICulture);
    }

    public string GetDescription(CultureInfo cultureInfo)
    {
        ArgumentNullException.ThrowIfNull(cultureInfo);

        return cultureInfo.TwoLetterISOLanguageName switch
        {
            Language.German => DescriptionDe,
            Language.French => !string.IsNullOrWhiteSpace(DescriptionFr) ? DescriptionFr : DescriptionDe,
            Language.Italian => !string.IsNullOrWhiteSpace(DescriptionIt) ? DescriptionIt : DescriptionDe,
            _ => DescriptionDe
        };
    }
}
