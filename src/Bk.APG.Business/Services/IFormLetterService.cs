using Bk.APG.Business.Dtos;

namespace Bk.APG.Business.Services;

public interface IFormLetterService
{
    Task<(string fileName, Stream content)> CreateFormLetterAsZipFile(FormLetterFilterParameters filterDto);
}
