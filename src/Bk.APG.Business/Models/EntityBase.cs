namespace Bk.APG.Business.Models;

public class EntityBase : IChangeTracking
{
    public Guid Id { get; init; }
    public required DateTime Created { get; set; }
    public required string CreatedBy { get; set; }
    public required DateTime Modified { get; set; }
    public required string ModifiedBy { get; set; }
}
