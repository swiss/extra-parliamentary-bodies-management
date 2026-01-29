namespace Bk.APG.Business.Dtos;

public class CommitteeTypeDepartmentStatisticDto
{
    // can be real committeeType or combined types (e.g. APK = all Behördenkommissionen & Verwaltungskommissionen)
    public required int OgdId { get; init; }
    public string? CommitteeType { get; set; }
    public int? CommitteeTypeOgdId { get; set; }
    // can be department or Total/Federal for sums
    public string? Organisation { get; set; }
    public string? DepartmentUri { get; set; }
    public int CommitteeCount { get; set; }
}
