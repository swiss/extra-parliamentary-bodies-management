namespace Bk.APG.Business.Models;

public class InterestLegalForm : MasterDataBase
{
    // TODO: this old masterdata table has to be removed, when final migration is done. 
    public ICollection<Interest> Interests { get; set; } = new List<Interest>();
}
