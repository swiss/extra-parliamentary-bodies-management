using Bk.APG.Business.Dtos;

namespace Bk.APG.Business.Services;

public interface ICompareListService
{
    Task<(string fileName, Stream content)> GenerateDocument(ReportFilterParametersDto filterDto);
}
