namespace Bk.APG.Business.Services;

public interface IDocumentService
{
    Task<string> UploadDocument(byte[] fileContentBytes);
    Task RemoveDocument(string documentId);
    Task<MemoryStream?> GetDocument(string documentId);
    Task SetupStorage();
}
