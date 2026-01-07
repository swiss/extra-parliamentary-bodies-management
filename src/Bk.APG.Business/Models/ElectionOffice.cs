namespace Bk.APG.Business.Models;

public class ElectionOffice : MasterDataBase
{
    // Departement
    public const string DepartmentAsString = "9890fce5-4331-4e96-8894-04b51acaedfb";
    public static readonly Guid DepartmentAsGuid = Guid.Parse(DepartmentAsString);

    public ICollection<Membership> Memberships { get; set; } = new List<Membership>();
    public ICollection<MembershipCandidate> MembershipCandidates { get; set; } = new List<MembershipCandidate>();
}
