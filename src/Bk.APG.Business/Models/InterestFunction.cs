namespace Bk.APG.Business.Models;

public class InterestFunction : MasterDataBase
{
    public ICollection<Interest> Interests { get; set; } = new List<Interest>();
}
