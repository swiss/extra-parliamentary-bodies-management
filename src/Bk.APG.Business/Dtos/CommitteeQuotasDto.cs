namespace Bk.APG.Business.Dtos;

public class CommitteeQuotasDto
{
    public required int MembersCount { get; set; }

    public required double FemaleThreshold { get; set; }
    public required double FemaleQuota { get; set; }
    public required double MaleThreshold { get; set; }
    public required double MaleQuota { get; set; }

    public required bool IsPercentageBased { get; set; }
    public required string PercentageLabel { get; set; }
    public required double GermanThreshold { get; set; }
    public required double GermanQuota { get; set; }
    public required double FrenchThreshold { get; set; }
    public required double FrenchQuota { get; set; }
    public required double ItalianThreshold { get; set; }
    public required double ItalianQuota { get; set; }
    public required double RomanshThreshold { get; set; }
    public required double RomanshQuota { get; set; }
}
