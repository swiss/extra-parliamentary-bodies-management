using Bk.APG.Business.Models;

namespace Bk.APG.Business.Services;

public interface IGeneralElectionHelperService
{
    Task CreateNewMembershipCandidate(Membership newMembership, string userName);
    Task<DateOnly> CheckMembershipCandidateSpecialCases(MembershipCandidate membershipCandidate, GeneralElectionCommittee committee, bool isPersonInFederalDuty, DateOnly termOfOfficeEndDate);
    Task MirrorOrDeleteMembershipForGeneralElection(Membership membership, bool deleteCandidate);
}
