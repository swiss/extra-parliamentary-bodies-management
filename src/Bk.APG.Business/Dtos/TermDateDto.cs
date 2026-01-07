namespace Bk.APG.Business.Dtos;

public class TermDateDto : MasterDataDtoBase
{
    public DateOnly BeginDate { get; set; }
    public DateOnly? EndDate { get; set; }
    public bool? IsGeneralElection { get; set; }
}
