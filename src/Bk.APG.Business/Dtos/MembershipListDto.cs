namespace Bk.APG.Business.Dtos;

public class MembershipListDto
{
    public CommitteeQuotasDto? CommitteeQuotas { get; set; }
    public IEnumerable<CommitteeMemberDto> ActiveMemberships { get; set; } = new List<CommitteeMemberDto>();
    public IEnumerable<CommitteeMemberDto> InactiveMemberships { get; set; } = new List<CommitteeMemberDto>();
}
