using Bk.APG.Business.Models;

namespace Bk.APG.Business.Repositories;

public interface IMembershipCandidateRepository
{
    Task<MembershipCandidate> GetById(Guid id);
    Task<MembershipCandidate> Create(MembershipCandidate membershipCandidate);
    Task<MembershipCandidate> GetByIdForUpdate(Guid id, uint? updateDtoRowVersion = null);
    Task<MembershipCandidate?> GetByMembershipIdForUpdate(Guid membershipId, uint? updateDtoRowVersion = null);
    Task<IEnumerable<MembershipCandidate>> GetByPersonIdForUpdate(Guid personId);
    Task CommitChanges();
    Task Delete(MembershipCandidate membershipCandidate);
    void DeleteRange(IEnumerable<MembershipCandidate> membershipCandidates);
}
