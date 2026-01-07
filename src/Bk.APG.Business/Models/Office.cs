namespace Bk.APG.Business.Models;

public class Office : MasterDataBase
{
    public EiamAssignment? EiamAssignment { get; set; }
    public Guid? EiamAssignmentId { get; set; }

    public required Guid DepartmentId { get; set; }
    public Department? Department { get; set; }

    public bool IsCentralFederalAdministration { get; set; }
    public bool IsGeneralSecretariat { get; set; }

    public ICollection<Committee> Committees { get; set; } = new List<Committee>();
    public ICollection<GeneralElectionCommittee> GeneralElectionCommittees { get; set; } = new List<GeneralElectionCommittee>();
}
