namespace Bk.APG.Business.Dtos;

public class RequestAndReportsFilterParametersDto
{
    public IEnumerable<Guid>? DepartmentIds { get; set; }
    public IEnumerable<Guid>? OfficeIds { get; set; }
    public IEnumerable<Guid>? CommitteeTypeIds { get; set; }
}
