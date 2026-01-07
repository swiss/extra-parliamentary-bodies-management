namespace Bk.APG.Business.Models;

public class LegislaturePeriod : MasterDataBase
{
    public required DateOnly ElectionDate { get; init; }
    public required DateOnly StartDate { get; init; }
    public required DateOnly EndDate { get; init; }

    public ICollection<Person> Persons { get; set; } = new List<Person>();
}
