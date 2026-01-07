namespace Bk.APG.Business.Dtos;

public class UidDto
{
    public string? UidOrganisationId { get; set; }
    public required string OrganizationName { get; set; }
    public required int MatchQuality { get; set; }
    public string? Zip { get; set; }
    public string? City { get; set; }
    public Guid? LegalFormId { get; set; }
    public string? LegalFormText { get; set; }
}
