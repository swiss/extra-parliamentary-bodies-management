namespace Bk.APG.Business.Dtos;

public class PagedResultDto<T>
{
    public int Index { get; init; }
    public int Total { get; init; }
    public IEnumerable<T> Items { get; init; } = [];
}
