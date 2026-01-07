namespace Bk.APG.Business.Models;

public class Salutation : MasterDataBase
{
    public Guid? GenderId { get; set; }
    public Gender? Gender { get; set; }
}
