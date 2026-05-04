using System.ComponentModel.DataAnnotations.Schema;
using System.Globalization;

namespace Bk.APG.Business.Models;

#pragma warning disable CA1716
public class Function : MasterDataBase
#pragma warning restore CA1716
{
    public const string PresidentUri = "www.todo.uri.A282A0CD-4A7D-48B6-9B52-9B216E9454FE";

    // Mitglied
    public const string MemberGuidAsString = "c2e8d46d-d827-412e-997b-d8afadaf41a7";
    public static readonly Guid MemberGuid = Guid.Parse(MemberGuidAsString);

    public required string TextFemaleDe { get; set; }
    public required string TextFemaleFr { get; set; }
    public required string TextFemaleIt { get; set; }
    public required string? TextFemaleRm { get; set; }

    public ICollection<Membership> Memberships { get; set; } = new List<Membership>();
    public ICollection<MembershipCandidate> MembershipCandidates { get; set; } = new List<MembershipCandidate>();

    public string GetFemaleText()
    {
        return GetFemaleText(CultureInfo.CurrentUICulture);
    }

    public string GetFemaleText(CultureInfo cultureInfo)
    {
        ArgumentNullException.ThrowIfNull(cultureInfo);

        return cultureInfo.TwoLetterISOLanguageName switch
        {
            Language.German => TextFemaleDe,
            Language.French => !string.IsNullOrWhiteSpace(TextFemaleFr) ? TextFemaleFr : TextFemaleDe,
            Language.Italian => !string.IsNullOrWhiteSpace(TextFemaleIt) ? TextFemaleIt : TextFemaleDe,
            _ => TextFemaleDe
        };
    }

    [NotMapped]
    public bool IsPresident => Uri == PresidentUri;
}
