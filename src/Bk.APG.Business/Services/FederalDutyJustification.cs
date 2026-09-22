using Bk.APG.Business.Models;

namespace Bk.APG.Business.Services;

public static class FederalDutyJustification
{
    public const string GermanText =
        "Die Amtszeitbeschränkung gilt nicht für Bundesangestellte, deren Mitgliedschaft für die Aufgabenerfüllung erforderlich ist oder in einem anderen Erlass zwingend vorgeschrieben wird (Art. 8i Abs. 3 RVOV).";

    public static bool IsStandardText(string? text)
    {
        return text == GermanText;
    }

    public static bool IsApplicableCommitteeType(Guid? committeeTypeId)
    {
        return committeeTypeId == CommitteeType.AuthoritiesCommissionGuid ||
               committeeTypeId == CommitteeType.AdministrationCommissionGuid;
    }

    public static string? Normalize(string? currentText, bool enabled, Guid? committeeTypeId)
    {
        if (enabled && IsApplicableCommitteeType(committeeTypeId))
        {
            return GermanText;
        }

        if (IsStandardText(currentText))
        {
            return null;
        }

        return currentText;
    }
}
