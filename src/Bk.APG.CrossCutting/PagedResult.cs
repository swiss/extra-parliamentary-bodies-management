namespace Bk.APG.CrossCutting;

public class PagedResult<T> where T : class
{
    public required int Index { get; init; }
    public required int Total { get; init; }
    public required IEnumerable<T> Items { get; init; } = [];
}
