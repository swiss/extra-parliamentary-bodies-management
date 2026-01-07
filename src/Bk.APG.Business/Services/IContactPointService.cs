using Bk.APG.Business.Dtos;

namespace Bk.APG.Business.Services;

public interface IContactPointService
{
    Task<IEnumerable<ContactPointListDto>> GetContactPointListByCommitteeId(Guid committeeId);

    Task<ContactPointUpdateDto> GetByIdForUpdate(Guid contactPointId);

    Task<ContactPointCreateDto> GetEmpty(Guid committeeId);

    Task Update(Guid id, ContactPointUpdateDto updateDto);

    Task<ContactPointDetailDto> Create(ContactPointCreateDto createDto);

    Task Delete(Guid id);
}
