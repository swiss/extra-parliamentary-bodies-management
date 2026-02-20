using Bk.APG.Business.Dtos;

namespace Bk.APG.Business.Services;

public interface IFormLetterSenderService
{
    Task<IEnumerable<FormLetterSenderListDto>> GetFormLetterSenderList();
    Task<FormLetterSenderCreateDto> GetEmpty();
    Task<FormLetterSenderUpdateDto> CreateFormLetterSender(FormLetterSenderCreateDto formLetterSenderCreateDto);
    Task<FormLetterSenderUpdateDto> GetFormLetterSenderForUpdate(Guid id);
    Task<FormLetterSenderUpdateDto> UpdateFormLetterSender(Guid id, FormLetterSenderUpdateDto formLetterSenderUpdateDto);
    Task DeleteFormLetterSender(Guid id);
}
