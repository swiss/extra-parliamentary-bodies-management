namespace Bk.APG.Business.Dtos;

public class CommitteeDetailDto
{
    public required Guid Id { get; init; }
    public int CommitteeNumber { get; set; }
    public required string Description { get; set; }
    public required string DescriptionDe { get; set; }
    public required string DescriptionFr { get; set; }
    public required string DescriptionIt { get; set; }
    public string? DescriptionRm { get; set; }
    public string? CommitteeLevel { get; set; }
    public string? Department { get; set; }
    public string? Office { get; set; }
    public string? CommitteeType { get; set; }
    public Guid CommitteeTypeId { get; set; }
    public string? LegalForm { get; set; }
    public string? OldLegalForm { get; set; } // TODO: entfernen, wenn neues APG abgenommen ist
    public string? LegalBase { get; set; }
    public bool? FederalLawEstablishment { get; set; }
    public bool? MarketOrientated { get; set; }
    public bool ExtraParliamentaryCommission { get; set; }
    public bool? SupervisionDuty { get; set; }
    public DateOnly BeginDate { get; set; }
    public DateOnly? EndDate { get; set; }
    public string? TermOfOffice { get; set; }
    public DateOnly? TermOfOfficeBeginDate { get; set; }
    public DateOnly? TermOfOfficeEndDate { get; set; }
    public bool Period4YearsInGeneralElection { get; set; }
    public int MembersCount { get; set; }
    public int? MinimalMembers { get; set; }
    public int? MaximalMembers { get; set; }
    public int? VacanciesGeneralElection { get; set; }
    public bool AdditionalAuthorityMembers { get; set; }
    public string? LinkAuthorityWebsite { get; set; }
    public string? RemarksBaseData { get; set; }
    public string? RemarksBaseDataAdmin { get; set; }
    public bool IsDeleted { get; set; }
    public bool IsActive { get; set; }
    public bool CanCreateMembership { get; set; }
    public bool CanCreateJustification { get; set; }
    public bool CanEdit { get; set; }
    public bool? FederalInstitution { get; set; }
    public bool? SelfOrganized { get; set; }

    public string? JustificationMembers { get; set; }

    public double? FemaleThreshold { get; set; }
    public double? FemaleQuota { get; set; }
    public double? MaleThreshold { get; set; }
    public double? MaleQuota { get; set; }
    public string? JustificationGenders { get; set; }
    public string? MeasuresGenders { get; set; }
    public string? GeneralGenderMeasure { get; set; }
    public bool IsPercentageBased { get; set; }
    public double? GermanThreshold { get; set; }
    public double? GermanQuota { get; set; }
    public double? FrenchThreshold { get; set; }
    public double? FrenchQuota { get; set; }
    public double? ItalianThreshold { get; set; }
    public double? ItalianQuota { get; set; }
    public double? RomanshThreshold { get; set; }
    public double? RomanshQuota { get; set; }
    public string? JustificationLanguages { get; set; }
    public string? MeasuresLanguages { get; set; }
    public string? GeneralLanguageMeasure { get; set; }
    public int? VacanciesInCurrentTermOfOffice { get; set; }
    public bool NeedsAttentionLongerDuty { get; set; }
    public bool NeedsAttentionShorterDuty { get; set; }
    public bool NeedsAttentionFederalDuty { get; set; }
    public bool NeedsAttentionFederalAssembly { get; set; }
    public bool NeedsAttentionNoMembers { get; set; }
    public bool NeedsAttentionAboveMaxMembers { get; set; }
    public bool NeedsAttentionDataProtectionOfficer { get; set; }
    public bool NeedsAttentionSecretariat { get; set; }
    public bool NeedsAttentionBasicData { get; set; }
    public bool NeedsAttentionMembershipExpired { get; set; }
    public bool NeedsAttentionMembershipInterestOrOccupation { get; set; }
    public bool NeedsAttentionRequirementsProfile { get; set; }
    public bool FutureGeneralElectionCommittee { get; set; }

    // General Election related fields
    public bool IsFederalCouncilProposalDirty { get; set; }
    public bool IsReadyForProposalForCurrentRole { get; set; }

    public ICollection<ContactPointDetailDto> ContactPoints { get; set; } = new List<ContactPointDetailDto>();
}
