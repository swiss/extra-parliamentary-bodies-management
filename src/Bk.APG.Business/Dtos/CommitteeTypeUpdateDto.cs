namespace Bk.APG.Business.Dtos;

public class CommitteeTypeUpdateDto
{
    public Guid Id { get; init; }
    public string? Text { get; set; }
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
    public uint RowVersion { get; set; }
}
