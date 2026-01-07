using Bk.APG.Business.Dtos;

namespace Bk.APG.Business.Services;

public interface IReportService
{
    Task<(string fileName, Stream content)> GetReport(ReportFilterParametersDto filterDto);
}
