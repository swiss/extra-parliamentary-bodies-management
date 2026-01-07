namespace Bk.APG.Business.Models;

public class MembershipCandidateLogMessage : EntityBase
{
    public GeneralElectionCommittee? GeneralElectionCommittee { get; set; }
    public Guid GeneralElectionCommitteeId { get; set; }
    public Person? Person { get; set; }
    public Guid PersonId { get; set; }
    public string? LogMessage { get; set; }
}
