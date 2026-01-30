namespace Bk.APG.Business.Services;

public interface IOgdExportService
{
    Task Export(CancellationToken ct = default);
}
