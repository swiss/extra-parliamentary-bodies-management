using Bk.APG.Business.Models;

namespace Bk.APG.Business.Services;

public interface IMembershipMirrorService
{
    Task MirrorOrDeleteMembershipForGeneralElection(Membership membership, bool deleteCandidate);
}
