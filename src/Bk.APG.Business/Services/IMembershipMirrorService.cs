using Bk.APG.Business.Dtos;
using Bk.APG.Business.Models;

namespace Bk.APG.Business.Services;

public interface IMembershipMirrorService
{
    Task MirrorOrDeleteMembershipForGeneralElection(Membership membership, bool deleteCandidate, bool wasMetadataChanged);
    Task CreateNewMembershipFromCandidate(MembershipCreateDto createDto, string userName);
    Task UpdateMembershipFromCandidate(Guid id, MembershipUpdateDto mappedMembership, string userName);
}
