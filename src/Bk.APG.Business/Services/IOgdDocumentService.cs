namespace Bk.APG.Business.Services;

public interface IOgdDocumentService
{
    Task UploadDocument(string path, string fileName, Stream fileStream);
    Task SetupBucket();
}
