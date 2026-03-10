namespace Bk.APG.Business.Dtos;

public class GeneralElectionCommitteeListDto
{
    public required Guid Id { get; init; }
    public required Guid CommitteeId { get; init; }
    public required string Description { get; init; }
    public required string Department { get; init; }
    public required string Office { get; init; }
    public required string CommitteeType { get; init; }
    public required bool IsNew { get; init; }
    public required int VacanciesGeneralElection { get; init; }
    public required bool StatusProposal { get; init; }
    public bool? IsMarketOrientated { get; init; }
    public bool? HasSupervisionDuty { get; init; }
    public required string ModifiedBy { get; init; }
    public required DateTime Modified { get; init; }
}
