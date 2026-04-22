namespace Bk.APG.Business.Dtos;

public class InformationNoteDto
{
    public required string TermOfOfficeDateRange { get; set; }
    public required string LastTermOfOfficeDateRange { get; set; }
    public required string CurrentYear { get; set; }
    public required int NumberOfMembers { get; set; }
    public required int ReleasedCommittees { get; set; }
    public required int TotalCommittees { get; set; }
    public required int UnreleasedCommittees { get; set; }
    public required int OneVacanciesTotal { get; set; }
    public required int TwoVacanciesTotal { get; set; }
    public required int ThreeVacanciesTotal { get; set; }
    public required int FourVacanciesTotal { get; set; }

    // Behördenkommission
    public required int ReleasedAuthoritiesCommittees { get; set; }
    public required int TotalAuthoritiesCommittees { get; set; }
    public required int UnreleasedAuthoritiesCommittees { get; set; }
    public required int OneVacanciesAuthoritiesCommittees { get; set; }
    public required int TwoVacanciesAuthoritiesCommittees { get; set; }
    public required int ThreeVacanciesAuthoritiesCommittees { get; set; }
    public required int FourVacanciesAuthoritiesCommittees { get; set; }

    // Verwaltungskommission
    public required int ReleasedAdministrationCommittees { get; set; }
    public required int TotalAdministrationCommittees { get; set; }
    public required int UnreleasedAdministrationCommittees { get; set; }
    public required int OneVacanciesAdministrationCommittees { get; set; }
    public required int TwoVacanciesAdministrationCommittees { get; set; }
    public required int ThreeVacanciesAdministrationCommittees { get; set; }
    public required int FourVacanciesAdministrationCommittees { get; set; }

    // Leitungsorgane
    public required int ReleasedManagementCommittees { get; set; }
    public required int TotalManagementCommittees { get; set; }
    public required int UnreleasedManagementCommittees { get; set; }
    public required int OneVacanciesManagementCommittees { get; set; }
    public required int TwoVacanciesManagementCommittees { get; set; }
    public required int ThreeVacanciesManagementCommittees { get; set; }
    public required int FourVacanciesManagementCommittees { get; set; }

    // Vertretungen des Bundes
    public required int ReleasedFederalAgenciesCommittees { get; set; }
    public required int TotalFederalAgenciesCommittees { get; set; }
    public required int UnreleasedFederalAgenciesCommittees { get; set; }
    public required int OneVacanciesFederalAgenciesCommittees { get; set; }
    public required int TwoVacanciesFederalAgenciesCommittees { get; set; }
    public required int ThreeVacanciesFederalAgenciesCommittees { get; set; }
    public required int FourVacanciesFederalAgenciesCommittees { get; set; }

    // APK
    public required int TotalExtraParliamentaryCommittees { get; set; }
    public required int UnreleasedExtraParliamentaryCommittees { get; set; }
    public required int PreviousTotalExtraParliamentaryCommittees { get; set; }
    public decimal? ExpectedGenderPercentage { get; set; }
    public decimal? PreviousExpectedGenderPercentage { get; set; }
    public decimal? CurrentFemalePercentage { get; set; }
    public decimal? PreviousFemalePercentage { get; set; }
    public required int UnderstuffedFemaleCommittees { get; set; }
    public required int HeavyUnderstuffedFemaleCommittees { get; set; }
    public required int PreviousHeavyUnderstuffedFemaleCommittees { get; set; }

    public decimal? CurrentMalePercentage { get; set; }
    public decimal? PreviousMalePercentage { get; set; }
    public required int UnderstuffedMaleCommittees { get; set; }
    public required int HeavyUnderstuffedMaleCommittees { get; set; }
    public required int PreviousHeavyUnderstuffedMaleCommittees { get; set; }

    public decimal? CurrentGermanPercentage { get; set; }
    public decimal? PreviousGermanPercentage { get; set; }
    public decimal? CurrentFrenchPercentage { get; set; }
    public decimal? PreviousFrenchPercentage { get; set; }
    public decimal? CurrentItalianPercentage { get; set; }
    public decimal? PreviousItalianPercentage { get; set; }
    public decimal? CurrentRomanshPercentage { get; set; }
    public decimal? PreviousRomanshPercentage { get; set; }

    // we also include missing german committees, even though currently it will most likely be 0. But it will be included in the template.
    public required int MissingGermanCommittees { get; set; }
    public required int PreviousMissingGermanCommittees { get; set; }
    public required int MissingFrenchCommittees { get; set; }
    public required int PreviousMissingFrenchCommittees { get; set; }
    public required int MissingItalianCommittees { get; set; }
    public required int PreviousMissingItalianCommittees { get; set; }
    // currently, this might not be used? Logicially it is needed.
    public required int MissingFrenchItalianCommittees { get; set; }
    public required int PreviousMissingFrenchItalianCommittees { get; set; }

    public required int TotalMembersExtraParliamentaryCommittees { get; set; }
    public required int MoreThan12Years { get; set; }
    public required int MoreThan12YearsFederalDuty { get; set; }

    public decimal? MinimalFemaleThreshold { get; set; }
    public decimal? MinimalMaleThreshold { get; set; }
    public decimal? CurrentFemaleThresholdManagementCommittees { get; set; }
    public decimal? CurrentMaleThresholdManagementCommittees { get; set; }
    public decimal? CurrentFemaleThresholdFederalAgenciesCommittees { get; set; }
    public decimal? CurrentMaleThresholdFederalAgenciesCommittees { get; set; }

    public decimal? MinimalGermanThreshold { get; set; }
    public decimal? MinimalFrenchThreshold { get; set; }
    public decimal? MinimalItalianThreshold { get; set; }
    public decimal? MinimalRomanshThreshold { get; set; }
    public decimal? CurrentGermanThresholdManagementCommittees { get; set; }
    public decimal? CurrentFrenchThresholdManagementCommittees { get; set; }
    public decimal? CurrentItalianThresholdManagementCommittees { get; set; }
    public decimal? CurrentRomanshThresholdManagementCommittees { get; set; }
    public decimal? CurrentGermanThresholdFederalAgenciesCommittees { get; set; }
    public decimal? CurrentFrenchThresholdFederalAgenciesCommittees { get; set; }
    public decimal? CurrentItalianThresholdFederalAgenciesCommittees { get; set; }
    public decimal? CurrentRomanshThresholdFederalAgenciesCommittees { get; set; }

    public required int UnderstuffedFemaleManagementCommittees { get; set; }
    public required int UnderstuffedMaleManagementCommittees { get; set; }

    public required int TotalMembersWith3Memberships { get; set; }
    public required int FemaleMembersWith3Memberships { get; set; }
    public required int MaleMembersWith3Memberships { get; set; }
    public required int TotalMembersWith4Memberships { get; set; }
    public required int FemaleMembersWith4Memberships { get; set; }
    public required int MaleMembersWith4Memberships { get; set; }
    public required int TotalMembersWith5Memberships { get; set; }
    public required int FemaleMembersWith5Memberships { get; set; }
    public required int MaleMembersWith5Memberships { get; set; }
    public required int TotalMembersWith6Memberships { get; set; }
    public required int FemaleMembersWith6Memberships { get; set; }
    public required int MaleMembersWith6Memberships { get; set; }
    public required int TotalMultipleMembers { get; set; }
    public required int FemaleMultipleMembers { get; set; }
    public required int MaleMultipleMembers { get; set; }

    public IEnumerable<ReportDepartmentWithCommitteeTypeDto>? ReleasedCommitteesByDepartmentAndType { get; set; }
    public IEnumerable<ReportDepartmentWithCommitteeTypeDto>? UnreleasedCommitteesByDepartmentAndType { get; set; }
    public IEnumerable<ReportDepartmentWithCommitteeTypeDto>? GenderUnderstuffedCommitteesByDepartmentAndType { get; set; }
    public IEnumerable<ReportDepartmentWithCommitteeTypeDto>? LanguageUnderstuffedCommitteesByDepartmentAndType { get; set; }
    public IEnumerable<ReportDepartmentWithCommitteesDto>? LongerDutyCommitteesByDepartmentAndType { get; set; }

    public IEnumerable<InformationNoteNonExtraParliamentaryCommitteeDepartmentDto>? NonExtraParliamentCommitteesByDepartmentAndType { get; set; }
    public IEnumerable<InformationNotePersonMembershipCountDto>? PersonWithMultipleMemberships { get; set; }
}

public class InformationNotePersonMembershipCountDto
{
    public required Guid PersonId { get; set; }
    public required int MembershipCount { get; set; }
    public required Guid GenderId { get; set; }
    public required string GivenName { get; set; }
    public required string Surname { get; set; }
    public IEnumerable<InformationNoteCommitteeNameDto>? CommitteeNames { get; set; }
}

public class InformationNoteNonExtraParliamentaryCommitteeDepartmentDto
{
    public required string Name { get; set; }
    public IEnumerable<InformationNoteNonExtraParliamentaryCommitteeTypeDto>? CommitteeTypes { get; set; }
}

public class InformationNoteNonExtraParliamentaryCommitteeTypeDto
{
    public required string CommitteeType { get; set; }
    public IEnumerable<InformationNoteNonExtraParliamentaryCommitteeData>? Committees { get; set; }
}

public class InformationNoteNonExtraParliamentaryCommitteeData
{
    public required string Name { get; set; }
    public string? GermanText { get; set; }
    public string? FrenchText { get; set; }
    public string? ItalianText { get; set; }
    public string? RomanshText { get; set; }
    public string? FemaleText { get; set; }
    public string? MaleText { get; set; }
}

public class InformationNoteCommitteeNameDto
{
    public required string Name { get; set; }
}

public class CommitteeNameLongerDutyDto
{
    public required string Name { get; set; }
    public required int MemberCount { get; set; }
    public required int FederalMemberCount { get; set; }
}

public enum InformationNoteData
{
    Vacancies,
    Genders,
    Languages
}
