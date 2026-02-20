namespace Bk.APG.Business.Dtos;

public class FormLetterSenderUpdateDto : FormLetterSenderModificationDto
{
    public required Guid Id { get; set; }
    public string? SignatureFileName { get; init; }
    public bool CanEditDepartment { get; set; }
}
