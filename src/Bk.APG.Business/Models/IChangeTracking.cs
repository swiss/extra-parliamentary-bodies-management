namespace Bk.APG.Business.Models;

public interface IChangeTracking
{
    DateTime Created { get; set; }
    string CreatedBy { get; set; }
    DateTime Modified { get; set; }
    string ModifiedBy { get; set; }
}
