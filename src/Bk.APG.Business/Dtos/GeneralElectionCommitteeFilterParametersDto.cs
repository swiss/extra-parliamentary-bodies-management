namespace Bk.APG.Business.Dtos;

public class GeneralElectionCommitteeFilterParametersDto
{
    public string? FreeText { get; init; }
    public IEnumerable<Guid>? LevelIds { get; init; }
    public IEnumerable<Guid>? DepartmentIds { get; init; }
    public IEnumerable<Guid>? OfficeIds { get; init; }
    public IEnumerable<Guid>? CommitteeTypeIds { get; init; }
    public IEnumerable<bool>? IsMarketOrientated { get; init; }
    public IEnumerable<bool>? HasSupervisionDuty { get; init; }
    public IEnumerable<bool>? Vacancies { get; init; }
    public IEnumerable<bool>? IsNew { get; init; }
    public IEnumerable<bool>? StatusProposal { get; init; }
}
