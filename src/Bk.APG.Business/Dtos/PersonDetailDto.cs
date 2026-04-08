namespace Bk.APG.Business.Dtos;

public class PersonDetailDto
{
    public required Guid Id { get; init; }
    public string? Salutation { get; init; }
    public required string Surname { get; init; }
    public required string GivenName { get; init; }
    public required int BirthYear { get; init; }
    public required bool MaskAddress { get; init; }
    public AddressDetailsDto? PrivateAddress { get; init; }
    public AddressDetailsDto? OfficeAddress { get; init; }
    public required string Language { get; init; }
    public required Guid LanguageId { get; init; }
    public required string CorrespondenceLanguage { get; init; }
    public required string Gender { get; init; }
    public required Guid GenderId { get; init; }
    public string? Title { get; init; }
    public string? Occupation { get; init; }
    public string? Employer { get; set; }
    public bool NoEmployment { get; set; }
    public bool FederalDuty { get; init; }
    public bool FederalAssembly { get; init; }
    public string? LegislaturePeriods { get; init; }
    public bool NoInterest { get; set; }
    public string? SalutationText { get; set; }
    public string? Office { get; set; }
    public string? Council { get; set; }
    public bool NeedsAttentionLongerDuty { get; set; }
    public bool NeedsAttentionShorterDuty { get; set; }
    public bool NeedsAttentionFederalDuty { get; set; }
    public bool NeedsAttentionFederalAssemblyAuthoritiesCommission { get; set; }
    public bool NeedsAttentionFederalAssemblyAdministrationCommission { get; set; }
    public bool NeedsAttentionBasicData { get; set; }
    public bool NeedsAttentionOccupation { get; set; }
    public bool NeedsAttentionInterests { get; set; }
    public bool NeedsAttentionMembershipExpired { get; set; }
    public bool NeedsAttentionRequirementsProfile { get; set; }

    public ICollection<MembershipDetailDto> Memberships { get; set; } = new List<MembershipDetailDto>();
    public ICollection<InterestListDto> Interests { get; set; } = new List<InterestListDto>();
    public string? Occupations { get; set; }
}
