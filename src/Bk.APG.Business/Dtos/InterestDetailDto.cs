namespace Bk.APG.Business.Dtos;

public class InterestDetailDto
{
    public required Guid Id { get; init; }
    public string? Text { get; set; }
    public string? InterestText { get; set; }
    public required Guid InterestCommitteeId { get; init; }
    public required Guid InterestFunctionId { get; init; }
    public Guid? InterestLegalFormId { get; init; }
    public Guid? LegalFormId { get; init; }
    public DateTime? BeginDate { get; set; }
    public DateTime? EndDate { get; set; }
    public string? UidOrganisationId { get; init; }
}
