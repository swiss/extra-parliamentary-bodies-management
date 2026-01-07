namespace Bk.APG.Business.Dtos;

public class CommitteeFilterParametersDto
{
    public string? FreeText { get; set; }
    public IEnumerable<Guid>? LevelIds { get; set; }
    public IEnumerable<Guid>? DepartmentIds { get; set; }
    public IEnumerable<Guid>? OfficeIds { get; set; }
    public IEnumerable<Guid>? CommitteeTypeIds { get; set; }
    public IEnumerable<Guid>? TermIds { get; set; }
    public IEnumerable<bool>? IsActive { get; set; }
    public IEnumerable<bool>? IsMarketOrientated { get; set; }
    public IEnumerable<bool>? HasSupervisionDuty { get; set; }
}
