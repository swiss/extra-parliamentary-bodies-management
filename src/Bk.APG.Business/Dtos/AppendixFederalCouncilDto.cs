namespace Bk.APG.Business.Dtos;

public class AppendixFederalCouncilDto
{
    public required string TermOfOfficeDateRange { get; set; }
    public required int NumberOfMembers { get; set; }
    public required int NumberOfCommittees { get; set; }
    public required int MoreThan15Members { get; set; }
    public required int NumberOfExtraParliamentaryCommissions { get; set; }
    public required int MissingGender { get; set; }
    public required int MissingLanguage { get; set; }
    public required int NumberOfMultipleMembershipsPersons { get; set; }
    public IEnumerable<ReportDepartmentDto>? DisbandedCommittees { get; set; }
    public IEnumerable<ReportDepartmentWithCommitteesDto>? ReleasedCommittees { get; set; }
    public IEnumerable<ReportDepartmentWithCommitteesDto>? UnreleasedCommittees { get; set; }
    public IEnumerable<ReportDepartmentWithCommitteesDto>? VacanciesCommittees { get; set; }
    public IEnumerable<ReportDepartmentWithCommitteesAndMembersDto>? ShorterDutyMembersCommittees { get; set; }
    public IEnumerable<ReportDepartmentWithCommitteesAndMembersDto>? MarketOrientatedMembersCommittees { get; set; }
    public IEnumerable<ReportDepartmentWithCommitteesDto>? MoreThan15MembersCommittees { get; set; }
    public IEnumerable<ReportDepartmentWithCommitteesAndGendersDto>? MissingGenderMembersCommittees { get; set; }
    public IEnumerable<ReportDepartmentWithCommitteesAndLanguagesDto>? MissingLanguageMembersCommittees { get; set; }
    public IEnumerable<ReportDepartmentWithCommitteesAndMembersDto>? LongerDutyMembersCommittees { get; set; }
    public IEnumerable<ReportDepartmentWithCommitteesAndMembersDto>? FederalAssemblyMembersCommittees { get; set; }
    public IEnumerable<ReportDepartmentWithCommitteesAndMembersDto>? FederalDutyMembersCommittees { get; set; }
    public IEnumerable<ReportDepartmentWithCommitteesAndGendersDto>? MissingGenderMembersManagementCommittees { get; set; }
    public IEnumerable<ReportDepartmentWithCommitteesAndLanguagesDto>? MissingLanguageMembersManagementCommittees { get; set; }
    public IEnumerable<ReportDepartmentWithCommitteesAndGendersDto>? MissingGenderMembersFederalAgenciesCommittees { get; set; }
    public IEnumerable<ReportDepartmentWithCommitteesAndLanguagesDto>? MissingLanguageMembersFederalAgenciesCommittees { get; set; }
    public IEnumerable<ReportDepartmentWithCommitteesAndMembersDto>? FormerFederalAssemblyMembersCommittees { get; set; }
    public IEnumerable<MultipleMembershipsDto>? MultipleMembershipsPersons { get; set; }
    public IEnumerable<ReportDepartmentWithCommitteesDto>? SelectionProcedureCommittees { get; set; }
    public IEnumerable<ReportDepartmentWithCommitteesAndMembersDto>? RequirementsProfileMembersCommittees { get; set; }
}

public class MultipleMembershipsDto
{
    public required string Surname { get; set; }
    public required string GivenName { get; set; }
    public int NumberOfMemberships { get; set; }
    public string? Committees { get; set; }
}
