using Bk.APG.Business.Dtos;

namespace Bk.APG.Business.Services;

public interface IInterestService
{
    Task<IEnumerable<InterestUpdateDto>> GetInterestsForUpdateByPersonId(Guid personId);
    Task<IEnumerable<InterestUpdateDto>> UpdateInterests(Guid personId, InterestUpdateDto[] updateDtos);
}
