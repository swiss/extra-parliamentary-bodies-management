namespace Bk.APG.Business.Dtos;

public class GeneralElectionCommitteeDetailDto
{
    public required Guid Id { get; init; }
    public required Guid CommitteeId { get; init; }
    public required int CommitteeNumber { get; set; }
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
    public string? OldLegalForm { get; set; }
    public string? LegalBase { get; set; }
    public bool? ReleaseGeneralElection { get; set; }
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
    public int? MinimalMembers { get; set; }
    public int? MaximalMembers { get; set; }
    public int MembersCount { get; set; }
    public int? VacanciesGeneralElection { get; set; }
    public int? CalculatedVacancies { get; set; }
    public string? GeneralGenderMeasure { get; set; }
    public string? GeneralLanguageMeasure { get; set; }
    public bool AdditionalAuthorityMembers { get; set; }
    public string? LinkAuthorityWebsite { get; set; }
    public string? RemarksBaseData { get; set; }
    public string? RemarksBaseDataAdmin { get; set; }
    public bool IsDeleted { get; set; }
    public bool IsActive { get; set; }
    public bool? FederalInstitution { get; set; }
    public bool IsValidated { get; set; }
    public bool WasValidatedOnce { get; set; }
    public bool CanEdit { get; set; }
    public bool CanEditSelectionProcedure { get; set; }

    public string? JustificationMembers { get; set; }
    public string? SelectionProcedure { get; set; }

    public bool JustificationsNeedAttention { get; set; }

    // Link only to current contactpoints
    public ICollection<ContactPointDetailDto> ContactPoints { get; set; } = new List<ContactPointDetailDto>();

    public required string CandidateListState { get; set; }
    public string? AssignedTo { get; set; }
    public bool WasGeneralElectionStartedForCommittee { get; set; }
    public bool CanSaveCandidateList { get; set; }
    public bool CanValidateCandidateList { get; set; }
    public bool CanForwardCandidateList { get; set; }
    public bool IsCandidateListCompleted { get; set; }
    public string? ReadyForProposalAssignedTo { get; set; }
    public bool CanForwardReadyForProposal { get; set; }
    public bool CanFinalizeReadyForProposal { get; set; }
    public bool IsReadyForProposal { get; set; }
    public required CommitteeQuotasDto Quotas { get; set; }
    public IEnumerable<MembershipCandidateDetailDto> Candidates { get; set; } = new List<MembershipCandidateDetailDto>();
}
