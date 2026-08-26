namespace Bk.APG.Business.Dtos;

public class CompareListDto
{
    public string? StartYear { get; set; }
    public string? EndYear { get; set; }
    public IEnumerable<CompareListCommitteeTypeDto>? CommitteeTypes { get; set; }
    public IEnumerable<CompareListNewCommitteeDto>? NewCommittees { get; set; }
    public IEnumerable<CompareListFormerCommitteeDto>? FormerCommittees { get; set; }
}

public class CompareListCommitteeTypeDto
{
    public required Guid CommitteeTypeId { get; init; }
    public required string Name { get; init; }
    public IEnumerable<CompareListDepartmentDto>? Departments { get; set; }
}

public class CompareListDepartmentDto
{
    public required Guid DepartmentId { get; init; }
    public required string Name { get; init; }
    public IEnumerable<CompareListCommitteeDto>? Committees { get; set; }
}

public class CompareListCommitteeDto
{
    public required Guid Id { get; init; }
    public required int CommitteeNumber { get; init; }
    public required string Name { get; init; }
    public string? Department { get; init; }
    public required Guid DepartmentId { get; init; }
    public string? Office { get; init; }
    public string? CommitteeType { get; init; }
    public required Guid CommitteeTypeId { get; init; }

    public int MemberCountOld { get; set; }
    public int MemberCountNew { get; set; }
    public bool FederalDutyBothDisplay { get; set; }
    public bool FederalDutyOldDisplay { get; set; }
    public bool FederalDutyNewDisplay { get; set; }
    public int FederalDutyCountOld { get; set; }
    public int FederalDutyCountNew { get; set; }
    public bool FederalAssemblyBothDisplay { get; set; }
    public bool FederalAssemblyOldDisplay { get; set; }
    public bool FederalAssemblyNewDisplay { get; set; }
    public int FederalAssemblyCountOld { get; set; }
    public int FederalAssemblyCountNew { get; set; }
    public bool GermanBothDisplay { get; set; }
    public bool GermanOldDisplay { get; set; }
    public bool GermanNewDisplay { get; set; }
    public string? GermanTextOld { get; set; }
    public string? GermanTextNew { get; set; }
    public bool FrenchBothDisplay { get; set; }
    public bool FrenchOldDisplay { get; set; }
    public bool FrenchNewDisplay { get; set; }
    public string? FrenchTextOld { get; set; }
    public string? FrenchTextNew { get; set; }
    public bool ItalianBothDisplay { get; set; }
    public bool ItalianOldDisplay { get; set; }
    public bool ItalianNewDisplay { get; set; }
    public string? ItalianTextOld { get; set; }
    public string? ItalianTextNew { get; set; }
    public bool GenderBothDisplay { get; set; }
    public bool GenderOldDisplay { get; set; }
    public bool GenderNewDisplay { get; set; }
    public string? GenderTextOld { get; set; }
    public string? GenderTextNew { get; set; }
    public string? Justification { get; set; }
}

public class CompareListNewCommitteeDto
{
    public required Guid Id { get; init; }
    public required string Name { get; init; }
    public required DateOnly StartDate { get; init; }
    public string? MemberCount { get; init; }
    public string? GenderQuota { get; init; }
    public string? LanguageQuota { get; init; }
}

public class CompareListFormerCommitteeDto
{
    public required Guid Id { get; init; }
    public required string Name { get; init; }
    public required DateOnly EndDate { get; init; }
}
