namespace Bk.APG.Business.Dtos;

public class DecisionFederalCouncilReportDto
{
    public required string TermOfOfficeDateRange { get; set; }

    public IEnumerable<ReportCommitteeWithFreeTextDto>? NonReleasedCommissions { get; set; }
    public IEnumerable<ReportCommitteeWithFreeTextDto>? MarketOrientatedCommissions { get; set; }
    public IEnumerable<ReportCommitteeWithMemberDetailDto>? MoreThan15MembersCommittees { get; set; }
    public IEnumerable<ReportCommitteeGenderMissingDto>? MissingGenderMembersCommittees { get; set; }
    public IEnumerable<ReportCommitteeLanguageMissingDto>? MissingLanguageMembersCommittees { get; set; }
    public IEnumerable<ReportCommitteeWithFreeTextDto>? LongerDutyMembersCommittees { get; set; }
    public IEnumerable<ReportCommitteeWithFreeTextDto>? FederalDutyMembersCommittees { get; set; }
}
