namespace Bk.APG.Business.Dtos;

public class ParliamentaryReportDto
{
    public required string TermOfOfficeDateRange { get; set; }
    public required bool OnlyReleased { get; set; }
    public required int NumberOfMembers { get; set; }
    public required int NumberOfCommittees { get; set; }
    // Behördenkommission
    public required int NumberOfAuthoritiesCommissions { get; set; }
    // Verwaltungskommission
    public required int NumberOfAdministrationCommissions { get; set; }
    // Leitungsorgane
    public required int NumberOfManagementCommittees { get; set; }
    // Vertretungen des Bundes
    public required int NumberOfFederalAgenciesCommittees { get; set; }
    public required int NumberOfVacancies { get; set; }
    public required int MoreThan15Members { get; set; }
    public required int NumberOfExtraParliamentaryCommissions { get; set; }
    public required decimal FemalePercentage { get; set; }
    public required decimal MalePercentage { get; set; }
    public required int FemaleUnderStuffed { get; set; }
    public required int MaleUnderStuffed { get; set; }
    public required int FrenchMissing { get; set; }
    public required int ItalianMissing { get; set; }
    public required int FrenchAndItalianMissing { get; set; }
    public required decimal GermanPercentage { get; set; }
    public required decimal FrenchPercentage { get; set; }
    public required decimal ItalianPercentage { get; set; }
    public required decimal RomanshPercentage { get; set; }
    public required int LongerThan12Years { get; set; }
    public required int AdminCommWithFederalAssemblyMembers { get; set; }
    public required int AdminCommMembersWithFederalAssembly { get; set; }
    public required int CommissionsWithFederalDutyMembers { get; set; }
    public required int CommissionsMembersWithFederalDuty { get; set; }
    public required int OtherCommitteeTypes { get; set; }

    public IEnumerable<ReportDepartmentWithCommitteeTypeDto>? ReleasedCommittees { get; set; }
    public IEnumerable<ReportDepartmentWithCommitteeTypeDto>? UnreleasedCommittees { get; set; }
    public IEnumerable<ReportDepartmentDto>? DisbandedCommittees { get; set; }
    public IEnumerable<ReportDepartmentWithCommitteesDto>? FinancialImpactsCommittees { get; set; }
    // will be filled separately, needs other DTO
    public IEnumerable<ReportDepartmentWithCommitteesDto>? VacanciesCommittees { get; set; }
    public IEnumerable<ReportDepartmentWithCommitteesDto>? MoreThan15MembersCommittees { get; set; }
    public IEnumerable<ReportDepartmentWithCommitteesAndGendersDto>? MissingGenderMembersCommittees { get; set; }
    public IEnumerable<ReportDepartmentWithCommitteesAndLanguagesDto>? MissingLanguageMembersCommittees { get; set; }
    public IEnumerable<ReportDepartmentWithCommitteesAndMembersDto>? LongerDutyMembersCommittees { get; set; }
    public IEnumerable<ReportDepartmentWithCommitteesAndMembersDto>? FederalAssemblyMembersCommittees { get; set; }
    public IEnumerable<ReportDepartmentWithCommitteesAndMembersDto>? FederalDutyMembersCommittees { get; set; }
    public IEnumerable<ReportDepartmentWithCommitteesDto>? DifferentTermOfOfficeCommittees { get; set; }
}
