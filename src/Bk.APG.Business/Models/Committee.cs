using System.ComponentModel.DataAnnotations.Schema;
using System.Globalization;
using System.Linq.Expressions;

namespace Bk.APG.Business.Models;

public class Committee : EntityBase
{
    public int OgdId { get; set; }
    public int CommitteeNumber { get; set; }
    public EiamAssignment? EiamAssignment { get; set; }
    public Guid? EiamAssignmentId { get; set; }
    public DateOnly BeginDate { get; set; }
    public DateOnly? EndDate { get; set; }

    public required string DescriptionGerman { get; set; }
    public required string DescriptionFrench { get; set; }
    public required string DescriptionItalian { get; set; }
    public required string DescriptionRomansh { get; set; }
    public Department? Department { get; set; }
    public Guid DepartmentId { get; set; }
    public Office? Office { get; set; }
    public Guid OfficeId { get; set; }
    public CommitteeLevel? CommitteeLevel { get; set; }
    public Guid CommitteeLevelId { get; set; }
    public CommitteeType? CommitteeType { get; set; }
    public Guid CommitteeTypeId { get; set; }
    public LegalForm? LegalForm { get; set; }
    public Guid? LegalFormId { get; set; }
    public string? OldLegalForm { get; set; }
    public string? LegalBase { get; set; }
    public TermOfOffice? TermOfOffice { get; set; }
    public Guid TermOfOfficeId { get; set; }
    public TermOfOfficeDate? TermOfOfficeDate { get; set; }
    public required Guid TermOfOfficeDateId { get; set; }
    public int? MinimalMembers { get; set; }
    public int? MaximalMembers { get; set; }
    public int? VacanciesGeneralElection { get; set; }
    public string? LinkAuthorityWebsite { get; set; }
    public string? RemarksBaseData { get; set; }
    public string? RemarksBaseDataAdmin { get; set; }
    public required bool IsDeleted { get; set; }
    public string? JustificationMembers { get; set; }
    public string? JustificationGenders { get; set; }
    public string? MeasuresGenders { get; set; }
    public string? JustificationLanguages { get; set; }
    public string? MeasuresLanguages { get; set; }

    public bool? ReleaseGeneralElection { get; set; }
    public bool? FederalLawEstablishment { get; set; }
    public bool? MarketOrientated { get; set; }
    public bool? SupervisionDuty { get; set; }
    public bool AdditionalAuthorityMembers { get; set; }
    public bool? FederalInstitution { get; set; }
    public string? LinkHomepageGerman { get; set; }
    public string? LinkHomepageFrench { get; set; }
    public string? LinkHomepageItalian { get; set; }
    public string? LinkHomepageRomansh { get; set; }

    public uint RowVersion { get; init; }

    public ICollection<Membership> Memberships { get; set; } = new List<Membership>();

    public ICollection<GeneralElectionCommittee> GeneralElectionCommittees { get; set; } = new List<GeneralElectionCommittee>();

    public ICollection<MembershipAddition> MembershipAdditionsInGeneralElection { get; set; } = new List<MembershipAddition>();

    public ICollection<AppointmentDecision> AppointmentDecisions { get; set; } = new List<AppointmentDecision>();

    public static readonly Expression<Func<Committee, bool>> IsActiveExpression = committee => committee.BeginDate <= DateOnly.FromDateTime(DateTime.Today) && (committee.EndDate == null || committee.EndDate >= DateOnly.FromDateTime(DateTime.Today));
    private static readonly Func<Committee, bool> _isActivePredicate = IsActiveExpression.Compile();

    [NotMapped]
    public bool IsActive => _isActivePredicate(this);

    public static readonly Expression<Func<Committee, bool>> CanCreateMembershipExpression = committee => (committee.BeginDate <= DateOnly.FromDateTime(DateTime.Today) && (committee.EndDate == null || committee.EndDate >= DateOnly.FromDateTime(DateTime.Today))) || committee.BeginDate > DateOnly.FromDateTime(DateTime.Now);
    private static readonly Func<Committee, bool> _canCreateMembershipPredicate = CanCreateMembershipExpression.Compile();

    [NotMapped]
    public bool CanCreateMembership => _canCreateMembershipPredicate(this);

    [NotMapped]
    public int ActiveMemberCount => Memberships.Count(x => x is { IsActive: true, IsDeleted: false, HasOtherElectionOffice: false });

    [NotMapped]
    public int FemaleCount => Memberships.Count(x => x is { IsActive: true, Person.Gender.Uri: Gender.Female, IsDeleted: false, HasOtherElectionOffice: false });

    [NotMapped]
    public double FemaleQuota => ActiveMemberCount > 0 ? Math.Round((double)FemaleCount / ActiveMemberCount * 100, 2) : 0;

    [NotMapped]
    public int MaleCount => Memberships.Count(x => x is { IsActive: true, Person.Gender.Uri: Gender.Male, IsDeleted: false, HasOtherElectionOffice: false });

    [NotMapped]
    public double MaleQuota => ActiveMemberCount > 0 ? Math.Round((double)MaleCount / ActiveMemberCount * 100, 2) : 0;

    [NotMapped]
    public bool FemaleUnderStuffed => ActiveMemberCount > 0 && ((double)FemaleCount / ActiveMemberCount * 100) < CommitteeType!.FemaleThreshold;

    [NotMapped]
    public bool MaleUnderStuffed => ActiveMemberCount > 0 && ((double)MaleCount / ActiveMemberCount * 100) < CommitteeType!.MaleThreshold;

    [NotMapped]
    public int FemalePresidentCount => Memberships.Count(x => x is { IsActive: true, Person.Gender.Uri: Gender.Female, Function.Uri: Function.PresidentUri, IsDeleted: false, HasOtherElectionOffice: false });

    [NotMapped]
    public int MalePresidentCount => Memberships.Count(x => x is { IsActive: true, Person.Gender.Uri: Gender.Male, Function.Uri: Function.PresidentUri, IsDeleted: false, HasOtherElectionOffice: false });

    [NotMapped]
    public int PresidentCount => Memberships.Count(x => x is { IsActive: true, Function.Uri: Function.PresidentUri, IsDeleted: false, HasOtherElectionOffice: false });

    [NotMapped]
    public int GermanCount => Memberships.Count(x => x is { IsActive: true, Person.Language.Uri: Language.GermanUri, IsDeleted: false, HasOtherElectionOffice: false });

    [NotMapped]
    public double GermanQuota => ActiveMemberCount > 0 ? Math.Round((double)GermanCount / ActiveMemberCount * 100, 2) : 0;

    [NotMapped]
    public int FrenchCount => Memberships.Count(x => x is { IsActive: true, Person.Language.Uri: Language.FrenchUri, IsDeleted: false, HasOtherElectionOffice: false });

    [NotMapped]
    public double FrenchQuota => ActiveMemberCount > 0 ? Math.Round((double)FrenchCount / ActiveMemberCount * 100, 2) : 0;

    [NotMapped]
    public int ItalianCount => Memberships.Count(x => x is { IsActive: true, Person.Language.Uri: Language.ItalianUri, IsDeleted: false, HasOtherElectionOffice: false });

    [NotMapped]
    public double ItalianQuota => ActiveMemberCount > 0 ? Math.Round((double)ItalianCount / ActiveMemberCount * 100, 2) : 0;

    [NotMapped]
    public int RomanshCount => Memberships.Count(x => x is { IsActive: true, Person.Language.Uri: Language.RomanshUri, IsDeleted: false, HasOtherElectionOffice: false });

    [NotMapped]
    public double RomanshQuota => ActiveMemberCount > 0 ? Math.Round((double)RomanshCount / ActiveMemberCount * 100, 2) : 0;

    [NotMapped]
    public bool GermanUnderStaffed => CommitteeType!.GermanMinimalThreshold != null
        ? GermanCount < CommitteeType!.GermanMinimalThreshold
        : CommitteeType!.GermanThresholdPercentage != null && GermanQuota < CommitteeType!.GermanThresholdPercentage;

    [NotMapped]
    public bool FrenchUnderStaffed => CommitteeType!.FrenchMinimalThreshold != null
        ? FrenchCount < CommitteeType!.FrenchMinimalThreshold
        : CommitteeType!.FrenchThresholdPercentage != null && FrenchQuota < CommitteeType!.FrenchThresholdPercentage;

    [NotMapped]
    public bool ItalianUnderStaffed => CommitteeType!.ItalianMinimalThreshold != null
        ? ItalianCount < CommitteeType!.ItalianMinimalThreshold
        : CommitteeType!.ItalianThresholdPercentage != null && ItalianQuota < CommitteeType!.ItalianThresholdPercentage;

    [NotMapped]
    public int FederalDutyCount => Memberships.Count(x => x is { IsActive: true, Person.FederalDuty: true, IsDeleted: false, HasOtherElectionOffice: false });

    [NotMapped]
    public int NotFederalDutyCount => Memberships.Count(x => x is { IsActive: true, Person.FederalDuty: false, IsDeleted: false, HasOtherElectionOffice: false });

    [NotMapped]
    public int IsCentralFederalAdministrationCount => Memberships.Count(x => x is { IsActive: true, Person.OfficeId: not null, Person.Office.IsCentralFederalAdministration: true, HasOtherElectionOffice: false });

    [NotMapped]
    public int IsDecentralizedFederalAdministrationCount => Memberships.Count(x => x is { IsActive: true, Person.OfficeId: not null, Person.Office.IsCentralFederalAdministration: false, HasOtherElectionOffice: false });

    [NotMapped]
    public int FederalAssemblyCount => Memberships.Count(x => x is { IsActive: true, Person.FederalAssembly: true, IsDeleted: false, HasOtherElectionOffice: false });

    [NotMapped]
    public int NotFederalAssemblyCount => Memberships.Count(x => x is { IsActive: true, Person.FederalAssembly: false, IsDeleted: false, HasOtherElectionOffice: false });

    [NotMapped]
    public bool ExtraParliamentaryCommission => CommitteeTypeId == CommitteeType.AuthoritiesCommissionGuid || CommitteeTypeId == CommitteeType.AdministrationCommissionGuid;

    [NotMapped]
    public int ActiveMemberCountFuture => Memberships.Count(x => x is { IsFuture: true, IsDeleted: false, HasOtherElectionOffice: false });

    [NotMapped]
    public int FemaleCountFuture => Memberships.Count(x => x is { IsFuture: true, Person.Gender.Uri: Gender.Female, IsDeleted: false, HasOtherElectionOffice: false });

    [NotMapped]
    public int MaleCountFuture => Memberships.Count(x => x is { IsFuture: true, Person.Gender.Uri: Gender.Male, IsDeleted: false, HasOtherElectionOffice: false });

    [NotMapped]
    public bool FemaleUnderStuffedFuture => ActiveMemberCountFuture > 0 && 100 / ActiveMemberCountFuture * FemaleCount < CommitteeType!.FemaleThreshold;

    [NotMapped]
    public bool MaleUnderStuffedFuture => ActiveMemberCountFuture > 0 && 100 / ActiveMemberCountFuture * MaleCount < CommitteeType!.MaleThreshold;

    [NotMapped]
    public int GermanCountFuture => Memberships.Count(x => x is { IsFuture: true, Person.Language.Uri: Language.GermanUri, IsDeleted: false, HasOtherElectionOffice: false });

    [NotMapped]
    public int FrenchCountFuture => Memberships.Count(x => x is { IsFuture: true, Person.Language.Uri: Language.FrenchUri, IsDeleted: false, HasOtherElectionOffice: false });

    [NotMapped]
    public int ItalianCountFuture => Memberships.Count(x => x is { IsFuture: true, Person.Language.Uri: Language.ItalianUri, IsDeleted: false, HasOtherElectionOffice: false });

    [NotMapped]
    public int RomanshCountFuture => Memberships.Count(x => x is { IsFuture: true, Person.Language.Uri: Language.RomanshUri, IsDeleted: false, HasOtherElectionOffice: false });

    [NotMapped]
    public int? VacanciesInCurrentTermOfOffice
    {
        get
        {
            if (CommitteeLevelId != CommitteeLevel.FederalCouncilGuid)
            {
                return null;
            }

            var membersCountAboveMinimum = ActiveMemberCount - MinimalMembers;
            return membersCountAboveMinimum >= 0 ? 0 : membersCountAboveMinimum * -1;
        }
    }

    [NotMapped]
    public bool IsInGeneralElection => CommitteeLevelId == CommitteeLevel.FederalCouncilGuid && TermOfOfficeId == TermOfOffice.Period4YearsInGeneralElectionGuid;

    public ICollection<ContactPoint> ContactPoints { get; set; } = new List<ContactPoint>();

    public string GetDescription()
    {
        return GetDescription(CultureInfo.CurrentUICulture);
    }

    public string GetDescription(CultureInfo cultureInfo)
    {
        ArgumentNullException.ThrowIfNull(cultureInfo);

        return cultureInfo.TwoLetterISOLanguageName switch
        {
            Language.German => DescriptionGerman,
            Language.French => !string.IsNullOrWhiteSpace(DescriptionFrench) ? DescriptionFrench : DescriptionGerman,
            Language.Italian => !string.IsNullOrWhiteSpace(DescriptionItalian) ? DescriptionItalian : DescriptionGerman,
            _ => DescriptionGerman
        };
    }

    [NotMapped]
    public bool NeedsAttention => IsActive &&
        (NeedsAttentionLongerDuty ||
            NeedsAttentionShorterDuty ||
            NeedsAttentionFederalDuty ||
            NeedsAttentionFederalAssembly ||
            NeedsAttentionNoMembers ||
            NeedsAttentionAboveMaxMembers ||
            NeedsAttentionMembershipExpired ||
            NeedsAttentionMembershipInterestOrOccupation ||
            NeedsAttentionRequirementsProfile ||
            NeedsAttentionDataProtectionOfficer ||
            NeedsAttentionSecretariat);

    [NotMapped]
    public bool NeedsAttentionLongerDuty => Memberships.Any(y => y is { IsActive: true, NeedsAttentionLongerDuty: true });

    [NotMapped]
    public bool NeedsAttentionShorterDuty => Memberships.Any(y => y is { IsActive: true, NeedsAttentionShorterDuty: true });

    [NotMapped]
    public bool NeedsAttentionFederalDuty => Memberships.Any(y => y is { IsActive: true, NeedsAttentionFederalDuty: true });

    [NotMapped]
    public bool NeedsAttentionFederalAssembly => Memberships.Any(y => y.IsActive && (y.NeedsAttentionFederalAssemblyAuthoritiesCommission || y.NeedsAttentionFederalAssemblyAdministrationCommission));

    [NotMapped]
    public bool NeedsAttentionBasicData => string.IsNullOrWhiteSpace(DescriptionGerman) || string.IsNullOrWhiteSpace(DescriptionFrench) || string.IsNullOrWhiteSpace(DescriptionItalian) || string.IsNullOrWhiteSpace(DescriptionRomansh) ||
        (FederalLawEstablishment == true && string.IsNullOrWhiteSpace(LegalBase)) ||
        (CommitteeLevelId == CommitteeLevel.FederalCouncilGuid && (!MinimalMembers.HasValue || !MaximalMembers.HasValue)) ||
        ((CommitteeTypeId == CommitteeType.AdministrationCommissionGuid || CommitteeTypeId == CommitteeType.AuthoritiesCommissionGuid) && (!FederalLawEstablishment.HasValue || !SupervisionDuty.HasValue || !MarketOrientated.HasValue)) ||
        ((CommitteeTypeId == CommitteeType.FederalAgenciesCommitteeGuid || CommitteeTypeId == CommitteeType.ManagementCommitteeGuid) && !FederalInstitution.HasValue) ||
        (CommitteeTypeId == CommitteeType.ManagementCommitteeGuid && !LegalFormId.HasValue);

    [NotMapped]
    public bool NeedsAttentionNoMembers => !Memberships.Any(y => y.IsActive);

    [NotMapped]
    public bool NeedsAttentionAboveMaxMembers => ExtraParliamentaryCommission && MaximalMembers > 0 && ActiveMemberCount > MaximalMembers;

    [NotMapped]
    public bool NeedsAttentionMembershipExpired => Memberships.Any(y => y.NeedsAttentionMembershipExpired);

    [NotMapped]
    public bool NeedsAttentionMembershipInterestOrOccupation => Memberships.Any(m => m is { IsActive: true, Person: not null } &&
                                                                                     (m.Person.NeedsAttentionOccupation ||
                                                                                      (!m.Person.NoInterest && m.Person.Interests.Count == 0 &&
                                                                                       (m.Committee?.CommitteeTypeId == CommitteeType.AuthoritiesCommissionGuid ||
                                                                                        m.Committee?.CommitteeTypeId == CommitteeType.AdministrationCommissionGuid ||
                                                                                        m.Committee?.CommitteeTypeId == CommitteeType.ManagementCommitteeGuid ||
                                                                                        m.Committee?.CommitteeTypeId == CommitteeType.FederalAgenciesCommitteeGuid))));

    [NotMapped]
    public bool NeedsAttentionRequirementsProfile => Memberships.Any(m => m is { IsActive: true, NeedsAttentionRequirementsProfile: true });

    [NotMapped]
    public bool NeedsAttentionDataProtectionOfficer => (CommitteeTypeId == CommitteeType.AdministrationCommissionGuid || CommitteeTypeId == CommitteeType.AuthoritiesCommissionGuid) &&
        ContactPoints.FirstOrDefault(y => (y.EndDate is null || y.EndDate > DateOnly.FromDateTime(DateTime.Now)) && y.ContactPointTypeId == ContactPointType.DataProtectionOfficerGuid) is null;

    [NotMapped]
    public bool NeedsAttentionSecretariat => ContactPoints.FirstOrDefault(x => x.ContactPointTypeId == ContactPointType.SecretariatGuid && (x.EndDate is null || x.EndDate > DateOnly.FromDateTime(DateTime.UtcNow))) is null;

    [NotMapped]
    public AppointmentDecision? LatestInstitutionAppointmentDecision =>
        AppointmentDecisions
            .Where(x => x.AppointmentDecisionTypeId == AppointmentDecisionType.Institution)
            .Where(x => x.OriginalDocument != null)
            .OrderByDescending(x => x.OriginalDocument!.Modified)
            .FirstOrDefault();

    [NotMapped]
    public bool FutureGeneralElectionCommittee => BeginDate > TermOfOfficeDate!.EndDate && IsInGeneralElection;
}
