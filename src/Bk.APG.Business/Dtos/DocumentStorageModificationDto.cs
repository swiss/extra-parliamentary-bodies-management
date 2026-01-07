using Microsoft.AspNetCore.Http;

namespace Bk.APG.Business.Dtos;

public class DocumentStorageModificationDto
{
    public Guid? Id { get; set; }
    public required string DisplayName { get; set; }
    public required bool IsOriginal { get; set; }
    public required Guid LanguageId { get; set; }
    public IFormFile? File { get; set; }
}
