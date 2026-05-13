namespace Bk.APG.Business.Dtos;

public class ReportDepartmentDto
{
    public required string Name { get; set; }
}

public class ReportDepartmentWithCommitteeTypeDto : ReportDepartmentDto
{
    public IEnumerable<ReportCommitteeTypeDto>? CommitteeTypes { get; set; }
}

public class ReportCommitteeTypeDto
{
    public required string CommitteeType { get; set; }
    public IEnumerable<ReportCommitteeDto>? Committees { get; set; }
}

public class ReportCommitteeDto
{
    public required string Name { get; set; }
    public int MemberCount { get; set; }
    public string? Justification { get; set; }
    public string? FreeText { get; set; }
    public string? FreeText2 { get; set; }
    public bool HasOpenJustificationChanges { get; set; }
    public string? JustificationUrl { get; set; }
}

public class ReportDepartmentWithCommitteesDto : ReportDepartmentDto
{
    public IEnumerable<ReportCommitteeDto>? Committees { get; set; }
}

public class ReportDepartmentWithCommitteesAndGendersDto : ReportDepartmentDto
{
    public string? Measure { get; set; }
    public bool HasOpenMeasureChanges { get; set; }
    public string? MeasureUrl { get; set; }
    public IEnumerable<ReportCommitteeGenderMissingDto>? Committees { get; set; }
}

public class ReportDepartmentWithCommitteesAndLanguagesDto : ReportDepartmentDto
{
    public string? Measure { get; set; }
    public bool HasOpenMeasureChanges { get; set; }
    public string? MeasureUrl { get; set; }
    public IEnumerable<ReportCommitteeLanguageMissingDto>? Committees { get; set; }
}

public class ReportDepartmentWithCommitteesAndMembersDto : ReportDepartmentDto
{
    public IEnumerable<ReportCommitteeWithMemberDetailDto>? Committees { get; set; }
}

public class ReportCommitteeGenderMissingDto
{
    public required string Name { get; set; }
    public int MemberCount { get; set; }
    public string? Measure { get; set; }
    public string? Justification { get; set; }
    public decimal? FemaleMissingPercentage { get; set; }
    public decimal? MaleMissingPercentage { get; set; }
    public bool HasOpenJustificationChanges { get; set; }
    public bool HasOpenMeasureChanges { get; set; }
    public string? JustificationUrl { get; set; }
}

public class ReportCommitteeLanguageMissingDto
{
    public required string Name { get; set; }
    public int MemberCount { get; set; }
    public string? Measure { get; set; }
    public string? Justification { get; set; }
    public bool? FrenchMissing { get; set; }
    public bool? ItalianMissing { get; set; }
    public decimal? GermanPercentage { get; set; }
    public decimal? FrenchPercentage { get; set; }
    public decimal? ItalianPercentage { get; set; }
    public decimal? RomanshPercentage { get; set; }
    public bool HasOpenJustificationChanges { get; set; }
    public bool HasOpenMeasureChanges { get; set; }
    public string? JustificationUrl { get; set; }
}

public class ReportCommitteeWithMemberDetailDto
{
    public required string Name { get; set; }
    public IEnumerable<ReportMembershipDto>? Members { get; set; }
    public int MemberCount { get; set; }
}

public class ReportMembershipDto
{
    public required string Surname { get; set; }
    public required string GivenName { get; set; }
    public string? Function { get; set; }
    // can be used for any form of text information regarding memberships or person
    public string? FreeText { get; set; }
    public string? Justification { get; set; }
    public string? FreeText2 { get; set; }
    public string? FreeText3 { get; set; }
    public bool HasOpenJustificationChanges { get; set; }
    public ReportMembershipType Type { get; set; }
    public string? JustificationUrl { get; set; }
}

public class ReportCommitteeWithFreeTextDto
{
    public required string Name { get; set; }
    public string? FreeText { get; set; }
}

public class ReportVacanciesDetailDto
{
    public required string Name { get; set; }
    public int Vacancies { get; set; }
    public string? MembershipAdditionsString { get; set; }
}

public enum ReportMembershipType
{
    ShorterDuty,
    FederalAssembly,
    FederalDuty,
    MarketOrientated,
    CompetenceProfile,
    MoreThan15Members
}

public enum ReportCommitteeType
{
    StandardBehaviour,
    Vacancies,
    SelectionProcedure
}
