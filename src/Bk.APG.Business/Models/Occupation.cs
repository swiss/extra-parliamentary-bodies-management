using System.Globalization;

namespace Bk.APG.Business.Models;

public class Occupation : MasterDataBase
{
    public required string TextFemaleDe { get; set; }
    public required string TextFemaleFr { get; set; }
    public required string TextFemaleIt { get; set; }
    public required string TextFemaleRm { get; set; }

    public ICollection<Person> Persons { get; set; } = new List<Person>();

    public string GetFemaleText(CultureInfo cultureInfo)
    {
        return cultureInfo.TwoLetterISOLanguageName switch
        {
            Language.German => TextFemaleDe,
            Language.French => !string.IsNullOrWhiteSpace(TextFemaleFr) ? TextFemaleFr : TextFemaleDe,
            Language.Italian => !string.IsNullOrWhiteSpace(TextFemaleIt) ? TextFemaleIt : TextFemaleDe,
            _ => TextFemaleDe
        };
    }
}
