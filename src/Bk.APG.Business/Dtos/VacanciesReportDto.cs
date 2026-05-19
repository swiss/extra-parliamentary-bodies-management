namespace Bk.APG.Business.Dtos;

public class VacanciesReportDto
{
    public required string TermOfOfficeDateRange { get; set; }
    public required bool OnlyReleased { get; set; }
    public IEnumerable<ReportDepartmentWithCommitteesDto>? Departments { get; set; }
}
