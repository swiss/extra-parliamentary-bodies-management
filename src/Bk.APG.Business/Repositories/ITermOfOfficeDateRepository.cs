using Bk.APG.Business.Models;

namespace Bk.APG.Business.Repositories;

public interface ITermOfOfficeDateRepository
{
    Task<IEnumerable<TermOfOfficeDate>> GetAll();
    Task<TermOfOfficeDate> GetById(Guid id);
    Task<TermOfOfficeDate> Update(TermOfOfficeDate existing, TermOfOfficeDate update);
}
