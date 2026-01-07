using Bk.APG.Business.Dtos;
using Bk.APG.Business.Mapper;
using Bk.APG.Business.Repositories;

namespace Bk.APG.Business.Services;

public class InterestService : IInterestService
{
    private readonly IInterestRepository _interestRepository;
    private readonly IAuthorizationService _authorizationService;

    public InterestService(
        IInterestRepository interestRepository,
        IAuthorizationService authorizationService)
    {
        _interestRepository = interestRepository;
        _authorizationService = authorizationService;
    }

    public async Task<IEnumerable<InterestUpdateDto>> GetInterestsForUpdateByPersonId(Guid personId)
    {
        var interests = await _interestRepository.GetAllByPersonId(personId);

        var interestList = interests.Select(InterestMapper.ToInterestUpdateDto).ToList().OrderBy(i => i.BeginDate).ThenBy(i => i.Text);

        return interestList;
    }

    public async Task<IEnumerable<InterestUpdateDto>> UpdateInterests(Guid personId, InterestUpdateDto[] updateDtos)
    {
        var returnList = new List<InterestUpdateDto>();

        // get all saved interests for this person, to find out, which ones have been deleted
        var interests = await _interestRepository.GetAllByPersonId(personId);
        var interestIds = interests.Select(i => i.Id).ToList();

        var currentUserName = _authorizationService.GetCurrentUserName();
        foreach (var update in updateDtos)
        {
            var mappedEntry = InterestMapper.FromInterestUpdateDto(update);
            if (update.Id == Guid.Empty || update.Id is null)
            {
                mappedEntry.CreatedBy = currentUserName;
                mappedEntry.Created = DateTime.UtcNow;
                mappedEntry.ModifiedBy = currentUserName;
                mappedEntry.Modified = DateTime.UtcNow;

                var addedEntry = await _interestRepository.Create(mappedEntry);
                var entry = InterestMapper.ToInterestUpdateDto(addedEntry);
                returnList.Add(entry);
            }
            else
            {
                var existingEntry = await _interestRepository.GetByIdForUpdate(update.Id.Value, update.RowVersion);

                mappedEntry.ModifiedBy = currentUserName;
                mappedEntry.Modified = DateTime.UtcNow;

                var addedEntry = _interestRepository.Update(existingEntry, mappedEntry);
                var entry = InterestMapper.ToInterestUpdateDto(addedEntry);
                returnList.Add(entry);

                interestIds.Remove((Guid)update.Id);
            }
        }

        foreach (var interestId in interestIds)
        {
            var existingEntry = await _interestRepository.GetByIdForUpdate(interestId);
            _interestRepository.Delete(existingEntry);
        }

        await _interestRepository.CommitAsync();

        return returnList;
    }
}
