using Bk.APG.Business.Dtos;
using Bk.APG.Business.Models;

namespace Bk.APG.Business.Services;

public interface IGeneralElectionService
{
    Task<bool> IsGeneralElectionToggleAvailable();
    Task<bool> PrepareGeneralElection(WorklistTaskCreateDto worklistTaskCreateDto);
    Task<bool> StartGeneralElection(Guid termOfOfficeDateId, DateOnly termOfOfficeBeginDate, DateOnly termOfOfficeEndDate, DateOnly dueDate, string description);
    Task MirrorOrDeleteMembershipForGeneralElection(Membership membership, bool deleteCandidate);
    Task CreateNewMembershipCandidate(Membership newMembership, string userName);
    Task UpdateCandidatesFromPerson(Person person);
    Task<DateOnly> CheckMembershipCandidateSpecialCases(MembershipCandidate membershipCandidate, GeneralElectionCommittee committee, bool isPersonInFederalDuty, DateOnly termOfOfficeEndDate);
    Task<bool> PrepareEndGeneralElection(WorklistTaskCreateDto worklistTaskCreateDto);
    Task<bool> EndGeneralElection();
}
