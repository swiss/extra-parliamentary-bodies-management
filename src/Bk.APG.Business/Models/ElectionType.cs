namespace Bk.APG.Business.Models;

public class ElectionType : MasterDataBase
{
    public const string NewElection = "https://politics.ld.admin.ch/fch/apg/vocabulary/election-type/6";
    public const string ReElection = "https://politics.ld.admin.ch/fch/apg/vocabulary/election-type/4";
    public const string Permanent = "https://politics.ld.admin.ch/fch/apg/vocabulary/election-type/7";

    // Neuwahl
    public const string NewElectionGuidAsString = "2cf75ac7-e06c-44ca-94cb-7d1cdadc4c95";
    public static readonly Guid NewElectionGuid = Guid.Parse(NewElectionGuidAsString);

    // Wiederwahl
    public const string ReElectionGuidAsString = "e6f5295c-d452-4c7d-9458-500b73581b12";
    public static readonly Guid ReElectionGuid = Guid.Parse(ReElectionGuidAsString);

    // Rücktritt
    public const string RetirementGuidAsString = "48fe8955-86f5-4eef-90c6-749100d4a2db";
    public static readonly Guid RetirementGuid = Guid.Parse(RetirementGuidAsString);

    // Ausscheiden wegen maximaler Amtszeit
    public const string MaximumMembershipDurationGuidAsString = "bd814190-0667-4e74-8779-e581390629d9";
    public static readonly Guid MaximumMembershipDurationGuid = Guid.Parse(MaximumMembershipDurationGuidAsString);

    // Todesfall
    public const string MembershipEndedBecauseOfDeathGuidAsString = "c5d01ed1-4a61-41de-ba01-0c415c4b87a0";
    public static readonly Guid MembershipEndedBecauseOfDeathGuid = Guid.Parse(MembershipEndedBecauseOfDeathGuidAsString);

    // anderer Ausscheidungsgrund
    public const string OtherRetirementReasonGuidAsString = "e2149808-c317-4ebf-b60c-287e162342b2";
    public static readonly Guid OtherRetirementReasonGuid = Guid.Parse(OtherRetirementReasonGuidAsString);

    public ICollection<Membership> Memberships { get; set; } = new List<Membership>();
    public ICollection<MembershipCandidate> MembershipCandidates { get; set; } = new List<MembershipCandidate>();
}
