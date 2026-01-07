namespace Bk.APG.Business.Models;

public class MembershipAddition : MasterDataBase
{
    public ICollection<Membership> Memberships { get; set; } = new List<Membership>();
    public ICollection<MembershipCandidate> MembershipCandidates { get; set; } = new List<MembershipCandidate>();
    public ICollection<Committee> Committees { get; set; } = new List<Committee>();
}
