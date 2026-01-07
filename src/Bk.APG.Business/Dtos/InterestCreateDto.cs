namespace Bk.APG.Business.Dtos;

public class InterestCreateDto
{
    public required Guid Id { get; init; }
    public required Guid PersonId { get; init; }
    public string? Text { get; set; }
    public required string InterestText { get; set; }
    public required Guid InterestCommitteeId { get; init; }
    public required Guid InterestFunctionId { get; init; }
    public Guid? InterestLegalFormId { get; init; }
    public Guid? LegalFormId { get; init; }
    public DateOnly? BeginDate { get; set; }
    public DateOnly? EndDate { get; set; }
    public string? UidOrganisationId { get; init; }
}
