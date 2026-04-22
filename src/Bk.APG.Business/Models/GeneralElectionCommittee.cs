using System.ComponentModel.DataAnnotations.Schema;
using System.Globalization;

namespace Bk.APG.Business.Models;

public class GeneralElectionCommittee : EntityBase
{
    public DateOnly BeginDate { get; set; }
    public DateOnly? EndDate { get; set; }
    public DateOnly? SecretariatReadyForProposalDueDate { get; set; }
    public DateOnly? OfficeReadyForProposalDueDate { get; set; }

    public required string DescriptionGerman { get; set; }
    public required string DescriptionFrench { get; set; }
    public required string DescriptionItalian { get; set; }
    public required string DescriptionRomansh { get; set; }
    public Committee? Committee { get; set; }
    public Guid CommitteeId { get; set; }
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
    public required bool IsValidated { get; set; }
    public required bool WasValidatedOnce { get; set; }
    public required bool IsFederalCouncilProposalDirty { get; set; }
    public string? SelectionProcedure { get; set; }

    public CandidateListState? CandidateListState { get; set; }
    public Guid? CandidateListStateId { get; set; }
    public string? AssignedToRole { get; set; }

    public uint RowVersion { get; init; }

    public ICollection<MembershipCandidate> MembershipCandidates { get; set; } = new List<MembershipCandidate>();

    [NotMapped]
    public bool ExtraParliamentaryCommission => CommitteeTypeId == CommitteeType.AuthoritiesCommissionGuid || CommitteeTypeId == CommitteeType.AdministrationCommissionGuid;

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
    public int ActiveMemberCount => MembershipCandidates.Count(x => x is { IsSelected: true, IsDeleted: false });

    [NotMapped]
    public int? CalculatedVacancies => MembershipCandidates.Count(x => x is { IsSelected: true, IsDeleted: false }) < MinimalMembers ?
        MinimalMembers - MembershipCandidates.Count(x => x is { IsSelected: true, IsDeleted: false }) : 0;

    [NotMapped]
    public int FemaleCount => MembershipCandidates.Count(x => x is { IsSelected: true, Gender.Uri: Gender.Female, IsDeleted: false });

    [NotMapped]
    public double FemaleQuota => ActiveMemberCount > 0 ? Math.Round((double)FemaleCount / ActiveMemberCount * 100, 2) : 0;

    [NotMapped]
    public int MaleCount => MembershipCandidates.Count(x => x is { IsSelected: true, Gender.Uri: Gender.Male, IsDeleted: false });

    [NotMapped]
    public double MaleQuota => ActiveMemberCount > 0 ? Math.Round((double)MaleCount / ActiveMemberCount * 100, 2) : 0;

    [NotMapped]
    public bool FemaleUnderStaffed => ActiveMemberCount > 0 && ((double)FemaleCount / ActiveMemberCount * 100) < CommitteeType!.FemaleThreshold;

    [NotMapped]
    public bool MaleUnderStaffed => ActiveMemberCount > 0 && ((double)MaleCount / ActiveMemberCount * 100) < CommitteeType!.MaleThreshold;

    [NotMapped]
    public bool IsJustificationGendersRequired => IsValidated && (MaleUnderStaffed || FemaleUnderStaffed);

    [NotMapped]
    public int GermanCount => MembershipCandidates.Count(x => x is { IsSelected: true, Language.Uri: Language.GermanUri, IsDeleted: false });

    [NotMapped]
    public double GermanQuota => ActiveMemberCount > 0 ? Math.Round((double)GermanCount / ActiveMemberCount * 100, 2) : 0;

    [NotMapped]
    public int FrenchCount => MembershipCandidates.Count(x => x is { IsSelected: true, Language.Uri: Language.FrenchUri, IsDeleted: false });

    [NotMapped]
    public double FrenchQuota => ActiveMemberCount > 0 ? Math.Round((double)FrenchCount / ActiveMemberCount * 100, 2) : 0;

    [NotMapped]
    public int ItalianCount => MembershipCandidates.Count(x => x is { IsSelected: true, Language.Uri: Language.ItalianUri, IsDeleted: false });

    [NotMapped]
    public double ItalianQuota => ActiveMemberCount > 0 ? Math.Round((double)ItalianCount / ActiveMemberCount * 100, 2) : 0;

    [NotMapped]
    public int RomanshCount => MembershipCandidates.Count(x => x is { IsSelected: true, Language.Uri: Language.RomanshUri, IsDeleted: false });

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
    public bool IsJustificationLanguagesRequired => IsValidated && (GermanUnderStaffed || FrenchUnderStaffed || ItalianUnderStaffed);

    [NotMapped]
    public bool JustificationsNeedAttention => (IsJustificationGendersRequired && (string.IsNullOrWhiteSpace(JustificationGenders) || (string.IsNullOrWhiteSpace(MeasuresGenders) && string.IsNullOrWhiteSpace(Department!.GeneralGenderMeasure?.Description))))
                                               || (IsJustificationLanguagesRequired && (string.IsNullOrWhiteSpace(JustificationLanguages) || (string.IsNullOrWhiteSpace(MeasuresLanguages) && string.IsNullOrWhiteSpace(Department!.GeneralLanguageMeasure?.Description))));
}
