namespace Bk.APG.Business.Models;

public class PersonOccupation
{
    public Guid OccupationsId { get; set; }
    public Occupation? Occupation { get; set; }

    public Guid PersonsId { get; set; }
    public Person? Person { get; set; }
}
