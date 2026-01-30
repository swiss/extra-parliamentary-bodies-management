using Bk.APG.Business.Dtos;
using Bk.APG.Business.Models;

namespace Bk.APG.Business.Services;

public interface IMembershipService
{
    Task<MembershipUpdateDto> GetMembershipForUpdate(Guid id);
    Task<IEnumerable<PersonMembershipDto>> GetAllByPersonId(Guid personId);
    Task<MembershipListDto> GetAllByCommitteeId(Guid committeeId);
    Task<IEnumerable<Membership>> GetAllActiveByCommitteeId(Guid committeeId);
    Task<MembershipDetailDto> CreateMembership(MembershipCreateDto createDto);
    Task<MembershipUpdateDto> UpdateMembership(Guid id, MembershipUpdateDto updateDto);
    IEnumerable<MembershipGenderLanguageStatisticDto> GetMembershipsForGenderLanguageStatistic(IEnumerable<Membership> memberships);
    IEnumerable<MembershipGenderLanguageStatisticDto> GetMembershipsForCommitteeTypeAndDepartmentGenderLanguageStatistic(IEnumerable<Membership> memberships);
    IEnumerable<MembershipGenderLanguageStatisticDto> GetExtraAndNonExtraParliamentaryCommitteesStatistic(IEnumerable<Membership> memberships);
    Task<IEnumerable<MembershipCantonStatisticDto>> GetMembershipsForCantonStatistic(IEnumerable<Membership> memberships);
    Task<IEnumerable<MembershipStatisticByCantonDto>> GetMembershipsForDetailedCantonStatistic(IEnumerable<Membership> memberships);
    Task DeleteMembership(Guid id);
}
