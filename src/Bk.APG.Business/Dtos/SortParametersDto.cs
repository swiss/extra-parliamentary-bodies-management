using Bk.APG.CrossCutting;

namespace Bk.APG.Business.Dtos;

public class SortParametersDto
{
    public string? Sort { get; init; }
    public SortDirection? Direction { get; init; }
}
