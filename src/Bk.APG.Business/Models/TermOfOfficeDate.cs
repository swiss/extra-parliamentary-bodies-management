namespace Bk.APG.Business.Models;

public class TermOfOfficeDate : MasterDataBase
{
    // Amtsperioden-Daten/Term of office date, indefinite for all not General election Committees! Used in DataMigration!
    // TODO PP, to make this completly correct, we have to get the values for current and next period by source, not as constants -> important in 4 years!
    public static readonly Guid IndefiniteDurationGuid = new("009863e8-5ebc-4465-bb6e-418256a06fd4");
    public static readonly Guid CurrentGeneralElectionGuid = new("cc7d5836-19fb-4d39-b189-c57a28a68454");
    public static readonly Guid NextGeneralElectionGuid = new("d881f0e4-912a-4335-b550-9b955ca39614");

    public required DateOnly BeginDate { get; set; }
    public DateOnly? EndDate { get; set; }
    public bool? IsGeneralElection { get; set; }

    public DateOnly? PlannedPublicationDate { get; set; }
    public DateTime? PublicationDate { get; set; }

    public ICollection<Committee> Committees { get; set; } = new List<Committee>();
    public ICollection<GeneralElectionCommittee> GeneralElectionCommittees { get; set; } = new List<GeneralElectionCommittee>();
}
