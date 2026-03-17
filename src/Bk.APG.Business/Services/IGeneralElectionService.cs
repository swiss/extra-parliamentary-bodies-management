using Bk.APG.Business.Dtos;
using Bk.APG.Business.Models;

namespace Bk.APG.Business.Services;

public interface IGeneralElectionService
{
    Task<bool> IsGeneralElectionToggleAvailable();
    Task<bool> PrepareGeneralElection(WorklistTaskCreateDto worklistTaskCreateDto);
    Task<bool> StartGeneralElection(Guid termOfOfficeDateId, DateOnly termOfOfficeBeginDate, DateOnly termOfOfficeEndDate, DateOnly dueDate, string description);
    Task CreateNewMembershipCandidate(Membership newMembership, bool isSelected = false);
    Task UpdateCandidatesFromPerson(Person person);
}
