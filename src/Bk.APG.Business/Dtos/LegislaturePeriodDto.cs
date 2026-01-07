namespace Bk.APG.Business.Dtos;

public class LegislaturePeriodDto : MasterDataDtoBase
{
    public required DateOnly ElectionDate { get; init; }
    public required DateOnly StartDate { get; init; }
    public required DateOnly EndDate { get; init; }
}
