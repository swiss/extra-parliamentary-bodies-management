namespace Bk.APG.Business.Models;

public class InterestCommittee : MasterDataBase
{
    public ICollection<Interest> Interests { get; set; } = new List<Interest>();
}
