using System.ComponentModel.DataAnnotations.Schema;
using System.Linq.Expressions;
using Bk.APG.Business.Services;

namespace Bk.APG.Business.Models;

public class Membership : EntityBase
{
    public int OgdId { get; set; }
    public Person? Person { get; set; }
    public Guid PersonId { get; set; }
    public Committee? Committee { get; set; }
    public Guid CommitteeId { get; set; }
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
    public int OldId { get; set; }
    public string? Remarks { get; set; }
    public string? RemarksStatus { get; set; }
    public required bool InCorrelationWithFederalDuty { get; set; }
    public required bool IsDeleted { get; set; }
    public uint RowVersion { get; set; }

    public static readonly Expression<Func<Membership, bool>> IsActiveExpression = m =>
        (m.BeginDate <= DateOnly.FromDateTime(DateTime.Now) && m.EndDate >= DateOnly.FromDateTime(DateTime.Now)) ||
        (m.EndDate < DateOnly.FromDateTime(DateTime.Now) && m.ElectionType != null && (m.ElectionType.Uri == ElectionType.NewElection || m.ElectionType.Uri == ElectionType.ReElection));

    public static readonly Expression<Func<Membership, bool>> IsFutureExpression = m => m.BeginDate > DateOnly.FromDateTime(DateTime.Now) && m.EndDate > DateOnly.FromDateTime(DateTime.Now);

    public static readonly Expression<Func<Membership, bool>> HasOtherElectionOfficeExpression = m => m.ElectionOfficeId == ElectionOffice.OtherGuid;

    private static readonly Func<Membership, bool> _isActivePredicate = IsActiveExpression.Compile();
    private static readonly Func<Membership, bool> _isFuturePredicate = IsFutureExpression.Compile();
    private static readonly Func<Membership, bool> _hasOtherElectionOfficePredicate = HasOtherElectionOfficeExpression.Compile();

    [NotMapped]
    public string FunctionName => Person?.Gender is null || Function is null ? string.Empty : Person.Gender.Uri == Gender.Female ? Function.GetFemaleText() : Function.GetText();

    [NotMapped]
    public bool IsActive => _isActivePredicate(this);

    [NotMapped]
    public bool IsFuture => _isFuturePredicate(this);

    [NotMapped]
    public bool HasOtherElectionOffice => _hasOtherElectionOfficePredicate(this);

    [NotMapped]
    public bool JustificationLongerDutyNeeded => Committee?.ExtraParliamentaryCommission == true && MembershipTermCalculator.CalculateEstimatedTermInYears(BeginDate, EndDate) > 12;

    [NotMapped]
    public bool JustificationShorterDutyNeeded => Committee?.TermOfOfficeId == TermOfOffice.Period4YearsInGeneralElectionGuid && EndDate < Committee?.TermOfOfficeDate?.EndDate && EndDate > BeginDate;

    [NotMapped]
    public bool JustificationMemberInFederalDutyNeeded => Committee?.ExtraParliamentaryCommission == true && Person?.FederalDuty == true;

    [NotMapped]
    public bool JustificationMemberInFederalAssemblyNeeded => Committee?.ExtraParliamentaryCommission == true && Person?.FederalAssembly == true;

    [NotMapped]
    public bool NeedsAttention => IsActive &&
                                  (NeedsAttentionMembershipExpired ||
                                   NeedsAttentionLongerDuty ||
                                   NeedsAttentionShorterDuty ||
                                   NeedsAttentionFederalDuty ||
                                   NeedsAttentionFederalAssemblyAuthoritiesCommission ||
                                   NeedsAttentionFederalAssemblyAdministrationCommission ||
                                   NeedsAttentionRequirementsProfile ||
                                   (Person is not null && Person.NeedsAttentionBasicData));

    [NotMapped]
    public bool NeedsAttentionMembershipExpired => EndDate < DateOnly.FromDateTime(DateTime.Now) && ElectionType?.Uri is ElectionType.NewElection or ElectionType.ReElection;

    [NotMapped]
    public bool NeedsAttentionLongerDuty => JustificationLongerDutyNeeded && string.IsNullOrWhiteSpace(JustificationLongerDuty);

    [NotMapped]
    public bool NeedsAttentionShorterDuty => JustificationShorterDutyNeeded && string.IsNullOrWhiteSpace(JustificationShorterDuty);

    [NotMapped]
    public bool NeedsAttentionFederalDuty => JustificationMemberInFederalDutyNeeded && string.IsNullOrWhiteSpace(JustificationMemberInFederalDuty);

    [NotMapped]
    public bool NeedsAttentionFederalAssemblyAuthoritiesCommission => JustificationMemberInFederalAssemblyNeeded && Committee?.CommitteeTypeId == CommitteeType.AuthoritiesCommissionGuid && string.IsNullOrWhiteSpace(JustificationMemberInFederalAssembly);

    [NotMapped]
    public bool NeedsAttentionFederalAssemblyAdministrationCommission => JustificationMemberInFederalAssemblyNeeded && Committee?.CommitteeTypeId == CommitteeType.AdministrationCommissionGuid && string.IsNullOrWhiteSpace(JustificationMemberInFederalAssembly);

    [NotMapped]
    public bool NeedsAttentionRequirementsProfile => string.IsNullOrWhiteSpace(RequirementsProfile) && ElectionTypeId == ElectionType.NewElectionGuid &&
                                                     (Committee!.CommitteeTypeId == CommitteeType.ManagementCommitteeGuid ||
                                                      Committee!.CommitteeTypeId == CommitteeType.FederalAgenciesCommitteeGuid ||
                                                      Committee!.SupervisionDuty == true) && !HasOtherElectionOffice;
}
