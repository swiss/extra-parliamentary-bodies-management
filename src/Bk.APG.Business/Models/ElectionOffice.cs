namespace Bk.APG.Business.Models;

public class ElectionOffice : MasterDataBase
{
    // Departement
    public const string DepartmentAsString = "9890fce5-4331-4e96-8894-04b51acaedfb";
    public static readonly Guid DepartmentAsGuid = Guid.Parse(DepartmentAsString);

    /// Andere
    public const string OtherAsString = "b4ec71be-3174-4624-83cc-110da132d699";
    /// Andere
    public static readonly Guid OtherGuid = Guid.Parse(OtherAsString);

    public ICollection<Membership> Memberships { get; set; } = new List<Membership>();
    public ICollection<MembershipCandidate> MembershipCandidates { get; set; } = new List<MembershipCandidate>();
}
