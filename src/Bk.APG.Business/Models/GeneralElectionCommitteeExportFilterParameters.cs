namespace Bk.APG.Business.Models;

public class GeneralElectionCommitteeExportFilterParameters
{
    public IEnumerable<Guid>? CorrespondenceLanguageIds { get; init; }
    public IEnumerable<Guid>? DepartmentIds { get; init; }
    public IEnumerable<Guid>? OfficeIds { get; init; }
    public IEnumerable<Guid>? CommitteeTypeIds { get; init; }
    public IEnumerable<Guid>? ElectionTypeIds { get; init; }
    public IEnumerable<Guid>? CommitteeIds { get; set; }
}
