using Bk.APG.Business.Dtos;

namespace Bk.APG.Business.Services;

public interface ICommitteeTypeService
{
    Task<List<CommitteeTypeListDto>> GetCommitteeTypeList();
    Task<CommitteeTypeUpdateDto> GetCommitteeTypeForUpdate(Guid id);
    Task<CommitteeTypeUpdateDto> UpdateCommitteeType(Guid id, CommitteeTypeUpdateDto updateDto);
}
