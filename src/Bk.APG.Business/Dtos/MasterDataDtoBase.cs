namespace Bk.APG.Business.Dtos;

public abstract class MasterDataDtoBase
{
    public Guid Id { get; init; }
    public string Text { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
    public string Uri { get; init; } = string.Empty;
    public bool IsDeleted { get; init; }
}
