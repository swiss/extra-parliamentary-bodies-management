using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.RegularExpressions;

namespace Bk.APG.Business.Models;

public class Person : EntityBase
{
    public int OgdId { get; set; }
    public required string Surname { get; set; }
    public required string GivenName { get; set; }
    public required int BirthYear { get; set; }
    public string? Occupation { get; set; }
    public bool FederalDuty { get; set; }
    public bool FederalAssembly { get; set; }
    public string? Title { get; set; }
    public Salutation? Salutation { get; set; }
    public Guid? SalutationId { get; set; }
    public string? SalutationText { get; set; }
    public Language? Language { get; set; }
    public Guid LanguageId { get; set; }
    public Language? CorrespondenceLanguage { get; set; }
    public Guid CorrespondenceLanguageId { get; set; }
    public Gender? Gender { get; set; }
    public Guid GenderId { get; set; }
    public Address? OfficeAddress { get; set; }
    public Guid? OfficeAddressId { get; set; }
    public Address? PrivateAddress { get; set; }
    public Guid? PrivateAddressId { get; set; }
    public Address? CorrespondenceAddress { get; set; }
    public Guid? CorrespondenceAddressId { get; set; }
    public Council? Council { get; set; }
    public Guid? CouncilId { get; set; }
    public Office? Office { get; set; }
    public Guid? OfficeId { get; set; }
    public int OldId { get; set; }
    public string? RemarksPersonData { get; set; }
    public string? RemarksPersonDataAdmin { get; set; }
    public bool NoInterest { get; set; }
    public bool NoEmployment { get; set; }
    public string? Employer { get; set; }
    public uint RowVersion { get; set; }

    public ICollection<Membership> Memberships { get; set; } = new List<Membership>();

    public ICollection<Interest> Interests { get; set; } = new List<Interest>();

    public ICollection<LegislaturePeriod> LegislaturePeriods { get; set; } = new List<LegislaturePeriod>();

    public ICollection<Occupation> Occupations { get; set; } = new List<Occupation>();

    [NotMapped]
    public bool NeedsAttention =>
        NeedsAttentionLongerDuty ||
        NeedsAttentionShorterDuty ||
        NeedsAttentionFederalDuty ||
        NeedsAttentionFederalAssemblyAuthoritiesCommission ||
        NeedsAttentionFederalAssemblyAdministrationCommission ||
        NeedsAttentionInterests ||
        NeedsAttentionOccupation;

    [NotMapped]
    public bool NeedsAttentionLongerDuty => Memberships.Any(y => y.IsActive && y.NeedsAttentionLongerDuty);

    [NotMapped]
    public bool NeedsAttentionShorterDuty => Memberships.Any(y => y.IsActive && y.NeedsAttentionShorterDuty);

    [NotMapped]
    public bool NeedsAttentionFederalDuty => Memberships.Any(y => y.IsActive && y.NeedsAttentionFederalDuty);

    [NotMapped]
    public bool NeedsAttentionFederalAssemblyAuthoritiesCommission => Memberships.Any(y => y.IsActive && y.NeedsAttentionFederalAssemblyAuthoritiesCommission);

    [NotMapped]
    public bool NeedsAttentionFederalAssemblyAdministrationCommission => Memberships.Any(y => y.IsActive && y.NeedsAttentionFederalAssemblyAdministrationCommission);

    [NotMapped]
    public bool NeedsAttentionInterests => !NoInterest && Memberships.Any(m => m.IsActive && ((m.Committee?.CommitteeTypeId == CommitteeType.AuthoritiesCommissionGuid) ||
        m.Committee?.CommitteeTypeId == CommitteeType.AdministrationCommissionGuid ||
        m.Committee?.CommitteeTypeId == CommitteeType.ManagementCommitteeGuid ||
        m.Committee?.CommitteeTypeId == CommitteeType.FederalAgenciesCommitteeGuid)) &&
        (Interests.Count == 0 /* TODO REACTIVATE || Interests.Any(i => string.IsNullOrWhiteSpace(i.InterestText) || i.LegalFormId is null || i.InterestCommitteeId == Guid.Empty || i.InterestFunctionId == Guid.Empty) */);

    [NotMapped]
#pragma warning disable CA1051
    public bool NeedsAttentionOccupation = false; // TODO REACTIVATE
#pragma warning restore CA1051
    //public bool NeedsAttentionOccupation => !FederalDuty && !NoEmployment &&Memberships.Any(y => y.IsActive && (y.Committee?.CommitteeTypeId == CommitteeType.AuthoritiesCommissionGuid ||
    //    y.Committee?.CommitteeTypeId == CommitteeType.AdministrationCommissionGuid ||
    //    y.Committee?.CommitteeTypeId == CommitteeType.ManagementCommitteeGuid ||
    //    y.Committee?.CommitteeTypeId == CommitteeType.FederalAgenciesCommitteeGuid) &&
    //    (string.IsNullOrWhiteSpace(Employer) || string.IsNullOrWhiteSpace(Occupation)));

    [NotMapped]
    public bool NeedsAttentionBasicData => !IsValidPhoneNumber(OfficeAddress?.Phone) || !IsValidPhoneNumber(PrivateAddress?.Phone) ||
        !IsValidPhoneNumber(OfficeAddress?.Mobile) || !IsValidPhoneNumber(PrivateAddress?.Mobile) ||
        !IsValidEmail(OfficeAddress?.Email) || !IsValidEmail(PrivateAddress?.Email);

    [NotMapped]
    public bool NeedsAttentionMembershipExpired => Memberships.Any(y => y.NeedsAttentionMembershipExpired);

    [NotMapped]
    public int Age => DateTime.UtcNow.Year - BirthYear;

    [NotMapped]
    public int ActiveMembershipCount => Memberships.Count(x => x.IsActive);

    [NotMapped]
    public int TotalEmploymentLevel => Memberships
        .Where(x => x.IsActive)
        .Where(x => !x.IsDeleted)
        .Sum(x => x.MaximumEmploymentLevel ?? 0);

    [NotMapped]
    public IEnumerable<Committee> ActiveCommittees => Memberships
        .Where(x => x.IsActive)
        .Where(x => !x.IsDeleted)
        .Select(x => x.Committee!);

    [NotMapped]
    public bool IsFemale => Gender?.Uri == Gender.Female;

    [NotMapped]
    public bool IsMale => Gender?.Uri == Gender.Male;

    [NotMapped]
    public bool IsGerman => Language?.Uri == Language.GermanUri;

    [NotMapped]
    public bool IsFrench => Language?.Uri == Language.FrenchUri;

    [NotMapped]
    public bool IsItalian => Language?.Uri == Language.ItalianUri;

    [NotMapped]
    public bool IsRomansh => Language?.Uri == Language.RomanshUri;

    private static bool IsValidPhoneNumber(string? phoneNumber)
    {
        return string.IsNullOrWhiteSpace(phoneNumber) || Regex.IsMatch(phoneNumber, @"^\+(?:[0-9] ?){6,19}[0-9]$");
    }

    private static bool IsValidEmail(string? emailAddress)
    {
        return string.IsNullOrWhiteSpace(emailAddress) || new EmailAddressAttribute().IsValid(emailAddress);
    }
}
