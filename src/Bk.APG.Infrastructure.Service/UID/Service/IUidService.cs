using Bk.APG.Business.Dtos;

namespace Bk.APG.Infrastructure.Service.UID.Service;

public interface IUidService
{
    Task<IEnumerable<UidDto>> Search(string organizationName);
}
