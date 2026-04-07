namespace Bk.APG.Business.Dtos;

public class FormLetterFilterParameters
{
    public IEnumerable<Guid>? DepartmentIds { get; set; }
    public IEnumerable<Guid>? OfficeIds { get; set; }
    public IEnumerable<Guid>? CommitteeTypeIds { get; set; }
    public IEnumerable<Guid>? CommitteeIds { get; set; }
    public IEnumerable<Guid>? CorrespondenceLanguageIds { get; set; }
    public IEnumerable<Guid>? ElectionTypeIds { get; set; }
    public Guid FormLetterSenderId { get; set; }
    public DateOnly? EndDateCurrentTermOfOfficeDate { get; set; }
    public string? ExportType { get; set; }
    public string? ExportFileType { get; set; }
    public DateOnly? FormLetterDate { get; set; }
}
