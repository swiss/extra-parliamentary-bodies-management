namespace Bk.APG.Business.Dtos;

public class GeneralElectionCommitteeExportFilterParametersDto
{
    public IEnumerable<Guid>? CorrespondenceLanguageIds { get; init; }
    public IEnumerable<Guid>? DepartmentIds { get; init; }
    public IEnumerable<Guid>? OfficeIds { get; init; }
    public IEnumerable<Guid>? CommitteeTypeIds { get; init; }
    public IEnumerable<Guid>? ElectionTypeIds { get; init; }
}
