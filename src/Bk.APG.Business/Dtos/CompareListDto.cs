namespace Bk.APG.Business.Dtos;

public class CompareListDto
{
    public IEnumerable<CompareListCommitteeTypeDto>? CommitteeTypes { get; set; }
}

public class CompareListCommitteeTypeDto
{
    public required string Name { get; init; }
    public IEnumerable<CompareListDepartmentDto>? Departments { get; set; }
}

public class CompareListDepartmentDto
{
    public required string Name { get; init; }
    public IEnumerable<CompareListCommitteeDto>? Committees { get; set; }
}

public class CompareListCommitteeDto
{
    public required Guid Id { get; init; }
    public required int CommitteeId { get; init; }
    public required string Name { get; init; }
    public required string Department { get; init; }
    public required string Office { get; init; }
    public required string CommitteeType { get; init; }

    public int MemberCountOld { get; init; }
    public int MemberCountNew { get; init; }
    public bool FederalDutyDisplay { get; init; }
    public int FederalDutyCountOld { get; init; }
    public int FederalDutyCountNew { get; init; }
    public bool FederalAssemblyDisplay { get; init; }
    public int FederalAssemblyCountOld { get; init; }
    public int FederalAssemblyCountNew { get; init; }
    public bool GermanDisplay { get; init; }
    public int GermanCountOld { get; init; }
    public int GermanCountNew { get; init; }
    public bool FrenchDisplay { get; init; }
    public int FrenchCountOld { get; init; }
    public int FrenchCountNew { get; init; }
    public bool ItalianDisplay { get; init; }
    public int ItalianCountOld { get; init; }
    public int ItalianCountNew { get; init; }
    public bool MaleDisplay { get; init; }
    public int MaleCountOld { get; init; }
    public int MaleCountNew { get; init; }
    public bool FemaleDisplay { get; init; }
    public int FemaleCountOld { get; init; }
    public int FemaleCountNew { get; init; }
    public string? Justification { get; init; }
}
