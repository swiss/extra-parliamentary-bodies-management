namespace Bk.APG.Business.Dtos;

public class PersonUpdateDto
{
    public required Guid Id { get; init; }
    public required string Surname { get; set; }
    public required string GivenName { get; set; }
    public required int BirthYear { get; set; }
    public required bool MaskAddress { get; set; }
    public AddressUpdateDto? PrivateAddress { get; set; }
    public AddressUpdateDto? OfficeAddress { get; set; }
    public required Guid LanguageId { get; set; }
    public required Guid GenderId { get; set; }
    public string? Title { get; init; }
    public string? Occupation { get; init; }
    public string? Employer { get; set; }
    public bool NoEmployment { get; set; }
    public bool FederalDuty { get; init; }
    public bool FederalAssembly { get; set; }
    public bool NoInterest { get; set; }
    public required Guid CorrespondenceLanguageId { get; set; }
    public Guid? SalutationId { get; init; }
    public string? SalutationText { get; set; }
    public Guid? OfficeId { get; set; }
    public Guid? CouncilId { get; set; }
    public bool HasInterests { get; init; }
    public List<Guid> LegislaturePeriodIds { get; init; } = [];
    public bool IsMissingJustificationFederalAssembly { get; init; }
    public required uint RowVersion { get; init; }
    public bool HasActiveMembership { get; init; }
    public List<OccupationDto> Occupations { get; init; } = [];
    public bool NeedsAttentionOccupation { get; init; }
    public bool CanDelete { get; init; }
}
