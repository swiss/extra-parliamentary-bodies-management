using Bk.APG.Business.Dtos;
using Bk.APG.Business.Models;

namespace Bk.APG.Business.Services;

public interface IGeneralElectionService
{
    Task<bool> IsGeneralElectionToggleAvailable();
    Task<bool> PrepareGeneralElection(WorklistTaskCreateDto worklistTaskCreateDto);
    Task<bool> StartGeneralElection(Guid termOfOfficeDateId, DateOnly termOfOfficeBeginDate, DateOnly termOfOfficeEndDate, DateOnly dueDate, string description);
    Task MirrorOrDeleteMembershipForGeneralElection(Membership membership, bool deleteCandidate);
    Task CreateNewMembershipCandidate(Membership newMembership);
    Task UpdateCandidatesFromPerson(Person person);
}
