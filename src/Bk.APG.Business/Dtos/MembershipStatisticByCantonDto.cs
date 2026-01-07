namespace Bk.APG.Business.Dtos;

public class MembershipStatisticByCantonDto
{
    public required Guid CommitteeId { get; init; }
    public required int CommitteeOgdId { get; init; }
    public required Guid CantonId { get; init; }
    public required int CantonOgdId { get; init; }
    public required int CantonCount { get; init; }
    public int FemaleCount { get; set; }
    public decimal FemalePercentage { get; set; }
    public int MaleCount { get; set; }
    public decimal MalePercentage { get; set; }
    public int GermanCount { get; set; }
    public decimal GermanPercentage { get; set; }
    public int FrenchCount { get; set; }
    public decimal FrenchPercentage { get; set; }
    public int ItalianCount { get; set; }
    public decimal ItalianPercentage { get; set; }
    public int RomanshCount { get; set; }
    public decimal RomanshPercentage { get; set; }
    public int FederalDutyCount { get; set; }
    public int FederalAssemblyCount { get; set; }
    public int Over40Count { get; set; }
    public decimal Over40Percentage { get; set; }
    public int UnderOr40Count { get; set; }
    public decimal UnderOr40Percentage { get; set; }
}
