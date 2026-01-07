using Bk.APG.Business.Models;

namespace Bk.APG.Business.Mapper;

public static class MembershipCandidateLogMessageMapper
{
    public static MembershipCandidateLogMessage ToMembershipCandidateLogMessage(Guid generalElectionCommitteeId, Guid personId, string logMessage, string currentUserName)
    {
        return new MembershipCandidateLogMessage
        {
            PersonId = personId,
            GeneralElectionCommitteeId = generalElectionCommitteeId,
            LogMessage = logMessage,
            Created = DateTime.UtcNow,
            CreatedBy = currentUserName,
            Modified = DateTime.UtcNow,
            ModifiedBy = currentUserName
        };
    }
}
