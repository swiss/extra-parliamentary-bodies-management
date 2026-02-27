namespace Bk.APG.Business.Dtos;

public class CommitteeUpdateDto
{
    public Guid Id { get; init; }
    public int CommitteeNumber { get; init; }
    public bool IsActive { get; init; }
    public required string DescriptionGerman { get; set; }
    public required string DescriptionFrench { get; set; }
    public required string DescriptionItalian { get; set; }
    public required string DescriptionRomansh { get; set; }
    public required Guid LevelId { get; set; }
    public required Guid OfficeId { get; set; }
    public required Guid DepartmentId { get; set; }
    public required Guid CommitteeTypeId { get; set; }
    public bool? FederalLawEstablishment { get; init; }
    public bool? SupervisionDuty { get; init; }
    public bool? MarketOrientated { get; init; }
    public Guid? LegalFormId { get; init; }
    public string? OldLegalForm { get; init; } // TODO: entfernen, wenn neues APG abgenommen ist
    public string? LegalBase { get; set; }
    public required DateOnly BeginDate { get; set; }
    public DateOnly? EndDate { get; set; }
    public required Guid TermOfOfficeId { get; set; }
    public int? MinimalMembers { get; set; }
    public int? MaximalMembers { get; set; }
    public bool AdditionalAuthorityMembers { get; init; }
    public string? LinkAuthorityWebsite { get; set; }
    public string? LinkHomepageGerman { get; set; }
    public string? LinkHomepageFrench { get; set; }
    public string? LinkHomepageItalian { get; set; }
    public string? LinkHomepageRomansh { get; set; }
    public bool? FederalInstitution { get; set; }
    public int? VacanciesInGeneralElection { get; set; }
    public Guid[]? MembershipAdditionsInGeneralElection { get; set; }
    public bool CanEditAll { get; set; }
    public bool CanEditDepartment { get; set; }
    public bool CanEditLegalbase { get; set; }
    public decimal MembersCount { get; set; }
    public bool FutureGeneralElectionCommittee { get; set; }
    public required uint RowVersion { get; init; }
}
