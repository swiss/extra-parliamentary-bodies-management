using System.ComponentModel.DataAnnotations.Schema;

namespace Bk.APG.Business.Models;

public class MembershipCandidate : EntityBase
{
    public required string Surname { get; set; }
    public required string GivenName { get; set; }
    public required int BirthYear { get; set; }
    public Membership? Membership { get; set; }
    public Guid? MembershipId { get; set; }
    public Person? Person { get; set; }
    public Guid? PersonId { get; set; }
    public Language? Language { get; set; }
    public Guid LanguageId { get; set; }
    public Gender? Gender { get; set; }
    public Guid GenderId { get; set; }
    public GeneralElectionCommittee? GeneralElectionCommittee { get; set; }
    public Guid GeneralElectionCommitteeId { get; set; }
    public int? MaximumEmploymentLevel { get; set; }
    public DateOnly BeginDate { get; set; }
    public DateOnly EndDate { get; set; }
    public ElectionType? ElectionType { get; set; }
    public Guid ElectionTypeId { get; set; }
    public Function? Function { get; set; }
    public Guid FunctionId { get; set; }
    public ElectionOffice? ElectionOffice { get; set; }
    public Guid ElectionOfficeId { get; set; }
    public string? OldMembershipAddition { get; set; }
    public MembershipAddition? MembershipAddition { get; set; }
    public Guid? MembershipAdditionId { get; set; }
    public string? JustificationLongerDuty { get; set; }
    public string? JustificationShorterDuty { get; set; }
    public string? JustificationMemberInFederalDuty { get; set; }
    public string? JustificationMemberInFederalAssembly { get; set; }
    public string? RequirementsProfile { get; set; }
    public string? Remarks { get; set; }
    public string? RemarksStatus { get; set; }
    public required bool InCorrelationWithFederalDuty { get; set; }
    public required bool IsDeleted { get; set; }
    public required bool IsSelected { get; set; }
    public uint RowVersion { get; set; }

    [NotMapped]
    public string FunctionName => Function is null ? string.Empty : Gender!.Uri == Gender.Female ? Function.GetFemaleText() : Function.GetText();

    [NotMapped]
    public bool JustificationLongerDutyNeeded => GeneralElectionCommittee?.ExtraParliamentaryCommission == true && EndDate > BeginDate && new DateOnly(1, 1, 1).Year + (EndDate.Year - BeginDate.Year) - 1 >= 12;

    [NotMapped]
    public bool JustificationShorterDutyNeeded => GeneralElectionCommittee?.TermOfOfficeId == TermOfOffice.Period4YearsInGeneralElectionGuid && EndDate > BeginDate && new DateOnly(1, 1, 1).Year + (EndDate.AddDays(1).Year - BeginDate.Year) - 1 < 4;

    [NotMapped]
    public bool NeedsAttention => (ElectionType!.Uri is ElectionType.NewElection or ElectionType.ReElection or ElectionType.Permanent) ||
        (JustificationLongerDutyNeeded && string.IsNullOrWhiteSpace(JustificationLongerDuty)) ||
        (JustificationShorterDutyNeeded && string.IsNullOrWhiteSpace(JustificationShorterDuty)) ||
        GeneralElectionCommittee?.CommitteeTypeId == CommitteeType.AuthoritiesCommissionGuid ||
        GeneralElectionCommittee?.CommitteeTypeId == CommitteeType.AdministrationCommissionGuid;

    [NotMapped]
    public decimal TermInYears => EndDate.DayNumber - (BeginDate.DayNumber / 365.25m);

    [NotMapped]
    public int Age => DateTime.UtcNow.Year - BirthYear;

    [NotMapped]
    public bool NeedsAttentionInterests => Person?.NeedsAttentionInterests == true ||
        (ElectionTypeId == ElectionType.NewElectionGuid && Person?.NoInterest == false &&
            (GeneralElectionCommittee?.CommitteeTypeId == CommitteeType.AuthoritiesCommissionGuid || GeneralElectionCommittee?.CommitteeTypeId == CommitteeType.AdministrationCommissionGuid || GeneralElectionCommittee?.CommitteeTypeId == CommitteeType.ManagementCommitteeGuid || GeneralElectionCommittee?.CommitteeTypeId == CommitteeType.FederalAgenciesCommitteeGuid) &&
            (Person?.Interests.Count == 0 || Person?.Interests.Any(i => string.IsNullOrWhiteSpace(i.InterestText) || i.LegalFormId is null || i.InterestCommitteeId == Guid.Empty || i.InterestFunctionId == Guid.Empty) == true));

    [NotMapped]
    public bool NeedsAttentionBasicDataOrOccupation => Person?.NeedsAttentionBasicData == true || Person?.NeedsAttentionOccupation == true ||
        (ElectionTypeId == ElectionType.NewElectionGuid && Person is { FederalDuty: false, NoEmployment: false } && (string.IsNullOrWhiteSpace(Person.Employer) || Person.Occupations.Count == 0) &&
            (GeneralElectionCommittee?.CommitteeTypeId == CommitteeType.AuthoritiesCommissionGuid || GeneralElectionCommittee?.CommitteeTypeId == CommitteeType.AdministrationCommissionGuid || GeneralElectionCommittee?.CommitteeTypeId == CommitteeType.ManagementCommitteeGuid || GeneralElectionCommittee?.CommitteeTypeId == CommitteeType.FederalAgenciesCommitteeGuid));
}
