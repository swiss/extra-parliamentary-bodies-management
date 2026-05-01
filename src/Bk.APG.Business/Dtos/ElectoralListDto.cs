namespace Bk.APG.Business.Dtos;

public class ElectoralListDto
{
    public required IEnumerable<ElectoralListDepartmentDto> Departments { get; set; }

    public required ElectoralListFunctionHeadersDto FunctionHeaders
    {
        get;
        set;
    }

    public required string InterestsLabel { get; set; }
    public required string EvaluationDateLabel { get; set; }
    public DateOnly EvaluationDate { get; set; }
    public required string TitleLabel { get; set; }
    public required string OtherMembershipsLabel { get; set; }
}

public class ElectoralListFunctionHeadersDto
{
    public required string SalutationSurnameGivenName { get; set; }
    public required string TitleOccupation { get; set; }
    public required string Canton { get; set; }
    public required string BirthYear { get; set; }
    public required string BeginDate { get; set; }
    public required string EndDate { get; set; }
    public required string ElectionType { get; set; }
    public required string Language { get; set; }
    public required string FederalAssembly { get; set; }
    public required string FederalDuty { get; set; }
}

public class ElectoralListDepartmentDto
{
    public required string Name { get; set; }
    public required IEnumerable<ElectoralListCommitteeDto> Committees { get; set; }
}

public class ElectoralListCommitteeDto
{
    public required string Name { get; set; }
    public required string CommitteeType { get; set; }
    public string? SelfOrganized { get; set; }
    public required IEnumerable<ElectoralListFunctionDto> Functions { get; set; }
}

public class ElectoralListFunctionDto
{
    public required string Name { get; set; }
    public required IEnumerable<ElectoralListMemberDto> Members { get; set; }
}

public class ElectoralListMemberDto
{
    public string? TitleOccupation { get; set; }
    public required string Salutation { get; set; }
    public required string Surname { get; set; }
    public required string GivenName { get; set; }
    public required string Canton { get; set; }
    public int BirthYear { get; set; }
    public DateOnly BeginDate { get; set; }
    public DateOnly EndDate { get; set; }
    public required string ElectionType { get; set; }
    public required string Language { get; set; }
    public required string FederalAssembly { get; set; }
    public required string FederalDuty { get; set; }
    public required string Interests { get; set; }
    public string? OtherMemberships { get; set; }
    public bool HasOtherMemberships => !string.IsNullOrWhiteSpace(OtherMemberships);
}
