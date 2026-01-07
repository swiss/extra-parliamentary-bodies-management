namespace Bk.APG.Business.Models;

public class CommitteeFilterParameters
{
    public string? FreeText { get; init; }
    public IEnumerable<Guid>? LevelIds { get; init; }
    public IEnumerable<Guid>? DepartmentIds { get; init; }
    public IEnumerable<Guid>? OfficeIds { get; init; }
    public IEnumerable<Guid>? CommitteeTypeIds { get; init; }
    public IEnumerable<Guid>? TermIds { get; init; }
    public IEnumerable<bool>? IsActive { get; init; }
    public IEnumerable<bool>? IsMarketOrientated { get; init; }
    public IEnumerable<bool>? HasSupervisionDuty { get; init; }
}
