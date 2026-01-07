using Bk.APG.Business.Models;

namespace Bk.APG.Business.Repositories;

public interface IMembershipCandidateLogMessageRepository
{
    Task<MembershipCandidateLogMessage> Create(MembershipCandidateLogMessage logMessage);
    Task<IEnumerable<MembershipCandidateLogMessage>> GetByPersonId(Guid personId);
    void DeleteRange(IEnumerable<MembershipCandidateLogMessage> logMessages);
}
