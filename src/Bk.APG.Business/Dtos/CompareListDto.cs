namespace Bk.APG.Business.Dtos;

public class CompareListDto
{
    public string? StartYear { get; set; }
    public string? EndYear { get; set; }
    public IEnumerable<CompareListCommitteeTypeDto>? CommitteeTypes { get; set; }
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

    public int MemberCountOld { get; init; }
    public int MemberCountNew { get; init; }
    public bool FederalDutyBothDisplay { get; init; }
    public bool FederalDutyOldDisplay { get; init; }
    public bool FederalDutyNewDisplay { get; init; }
    public int FederalDutyCountOld { get; init; }
    public int FederalDutyCountNew { get; init; }
    public bool FederalAssemblyBothDisplay { get; init; }
    public bool FederalAssemblyOldDisplay { get; init; }
    public bool FederalAssemblyNewDisplay { get; init; }
    public int FederalAssemblyCountOld { get; init; }
    public int FederalAssemblyCountNew { get; init; }
    public bool GermanBothDisplay { get; init; }
    public bool GermanOldDisplay { get; init; }
    public bool GermanNewDisplay { get; init; }
    public string? GermanTextOld { get; init; }
    public string? GermanTextNew { get; init; }
    public bool FrenchBothDisplay { get; init; }
    public bool FrenchOldDisplay { get; init; }
    public bool FrenchNewDisplay { get; init; }
    public string? FrenchTextOld { get; init; }
    public string? FrenchTextNew { get; init; }
    public bool ItalianBothDisplay { get; init; }
    public bool ItalianOldDisplay { get; init; }
    public bool ItalianNewDisplay { get; init; }
    public string? ItalianTextOld { get; init; }
    public string? ItalianTextNew { get; init; }
    public bool GenderBothDisplay { get; init; }
    public bool GenderOldDisplay { get; init; }
    public bool GenderNewDisplay { get; init; }
    public string? GenderTextOld { get; init; }
    public string? GenderTextNew { get; init; }
    public string? Justification { get; init; }
}
