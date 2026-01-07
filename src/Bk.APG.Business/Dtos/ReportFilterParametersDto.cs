namespace Bk.APG.Business.Dtos;

public class ReportFilterParametersDto
{
    public required ReportType DocumentType { get; set; }
    public DateOnly? AnalysisDate1 { get; set; }
    public DateOnly? AnalysisDate2 { get; set; }
    public IEnumerable<Guid>? DepartmentIds { get; set; }
    public IEnumerable<Guid>? OfficeIds { get; set; }
    public IEnumerable<Guid>? CommitteeTypeIds { get; set; }
    public IEnumerable<Guid>? CommitteeIds { get; set; }
    public bool CommitteesWithActiveMembership { get; set; }
    public bool ReleasedCommittees { get; set; }
    public bool IsGeneralElection { get; set; }
}

public enum ReportType
{
    ParliamentaryReport,
    AppendixFederalCouncil,
    AppendixFederalCouncilCheck,
    DissolvedCommittees,
    Vacancies,
    CompareListGE,
    AttachmentFCR_GE,
    FCD,
    IN_GE,
    ElectoralListFC,
    ElectoralListOnline
}
