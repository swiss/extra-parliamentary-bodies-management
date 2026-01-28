namespace Bk.APG.Business.Dtos;

public class CommitteeTypeDepartmentStatisticDto
{
    // can be real committeeType or combined types (e.g. APK = all Behördenkommissionen & Verwaltungskommissionen)
    public required int CommitteeTypeOgdId { get; init; }
    public string? CommitteeType { get; set; }
    // can be department or Total/Federal for sums
    public string? Organisation { get; set; }
    public int CommitteeCount { get; set; }
}
