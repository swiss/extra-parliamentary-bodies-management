namespace Bk.APG.Business.Dtos;

public class CommitteeTypeListDto
{
    public Guid Id { get; init; }
    public required string Text { get; set; }
    public required string Description { get; set; }
    public double? FemaleThreshold { get; set; }
    public double? MaleThreshold { get; set; }
    public int? GermanMinimalThreshold { get; set; }
    public int? FrenchMinimalThreshold { get; set; }
    public int? ItalianMinimalThreshold { get; set; }
    public int? RomanshMinimalThreshold { get; set; }
    public double? GermanThresholdPercentage { get; set; }
    public double? FrenchThresholdPercentage { get; set; }
    public double? ItalianThresholdPercentage { get; set; }
    public double? RomanshThresholdPercentage { get; set; }
}
