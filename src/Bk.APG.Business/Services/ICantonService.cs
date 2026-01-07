using Bk.APG.Business.Dtos;
using Bk.APG.Business.Models;

namespace Bk.APG.Business.Services;

public interface ICantonService
{
    Task<IEnumerable<CantonDto>> GetAll();
    Task<Canton> CreateOrUpdate(Canton canton);
}
