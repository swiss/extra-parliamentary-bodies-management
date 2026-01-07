using Bk.APG.Business.Dtos;

namespace Bk.APG.Business.Services;

public interface IMembershipCandidateService
{
    Task PartialUpdateMembershipCandidate(Guid id, MembershipCandidatePartialUpdateDto membershipCandidatePartialUpdate);
    Task UpdateMembershipCandidate(Guid id, MembershipCandidateUpdateDto membershipCandidateUpdate);
    Task<MembershipCandidateUpdateDto> GetMembershipCandidateForUpdate(Guid id);
    Task<MembershipCandidateDetailDto> CreateMembershipCandidate(MembershipCandidateCreateDto membershipCandidateCreate);
    Task<MembershipListDto> GetMembers(Guid generalElectionCommitteeId);
    Task DeleteMembershipCandidate(Guid id);
    Task ForwardCandidateList(Guid committeeId, CandidateListForwardDto forwardDto);
    Task<CandidateListValidationResultDto> ValidateCandidateList(Guid committeeId, IEnumerable<Guid> selectedCandidateIds, bool duplicateCheckDone);
    Task SaveCandidateList(Guid committeeId, IEnumerable<Guid> candidateIds);
    Task<MembershipCandidateDetailDto?> GetDuplicateMembershipCandidateForList(Guid committeeId, string surname, string givenName, int birthYear, Guid genderId, Guid languageId);
}
