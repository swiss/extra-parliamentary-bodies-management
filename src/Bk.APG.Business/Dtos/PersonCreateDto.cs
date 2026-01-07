namespace Bk.APG.Business.Dtos;

public class PersonCreateDto
{
    public required string Surname { get; set; }
    public required string GivenName { get; set; }
    public int BirthYear { get; set; }
    public AddressUpdateDto? PrivateAddress { get; set; }
    public AddressUpdateDto? OfficeAddress { get; set; }
    public Guid LanguageId { get; set; }
    public Guid GenderId { get; set; }
    public string? Title { get; set; }
    public string? Occupation { get; set; }
    public string? Employer { get; set; }
    public bool NoEmployment { get; set; }
    public bool FederalDuty { get; set; }
    public bool FederalAssembly { get; set; }
    public bool NoInterest { get; set; }
    public Guid CorrespondenceLanguageId { get; set; }
    public Guid? SalutationId { get; set; }
    public string? SalutationText { get; set; }
    public Guid? OfficeId { get; set; }
    public Guid? CouncilId { get; set; }
    public bool HasActiveMembership { get; init; }

    public ICollection<Guid> LegislaturePeriodIds { get; set; } = [];
    public ICollection<OccupationDto> Occupations { get; set; } = [];
}
