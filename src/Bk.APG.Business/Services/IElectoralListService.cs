using Bk.APG.Business.Dtos;

namespace Bk.APG.Business.Services;

public interface IElectoralListService
{
    Task<(string fileName, Stream content)> GenerateDocument(ReportFilterParametersDto filterDto, string listType);
}
