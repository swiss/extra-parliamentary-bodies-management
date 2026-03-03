namespace Bk.APG.Business.Dtos;

public class CommitteeExportFilterParametersDto
{
    public IEnumerable<Guid>? DepartmentIds { get; init; }
    public IEnumerable<Guid>? OfficeIds { get; init; }
    public IEnumerable<Guid>? CommitteeTypeIds { get; init; }
    public string? ReportType { get; set; }
    public DateOnly? AnalysisDate1 { get; set; }
    public DateOnly? AnalysisDate2 { get; set; }
}
