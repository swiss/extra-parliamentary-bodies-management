namespace Bk.APG.Business.Dtos;

public class MembershipGenderLanguageStatisticDto
{
    public required int MembershipCount { get; set; }
    public Guid? CommitteeId { get; set; }
    public int? CommitteeOgdId { get; set; }
    public string? CommitteeType { get; set; }
    public Guid? CommitteeTypeId { get; set; }
    public int? CommitteeTypeOgdId { get; set; }
    public string? Department { get; set; }
    public string? DepartmentUri { get; set; }

    public int FemaleCount { get; set; }
    public decimal FemalePercentage { get; set; }
    public int MaleCount { get; set; }
    public decimal MalePercentage { get; set; }
    public int GermanCount { get; set; }
    public decimal GermanPercentage { get; set; }
    public int FrenchCount { get; set; }
    public decimal FrenchPercentage { get; set; }
    public int ItalianCount { get; set; }
    public decimal ItalianPercentage { get; set; }
    public int RomanshCount { get; set; }
    public decimal RomanshPercentage { get; set; }
    public int FederalDutyCount { get; set; }
    public int FederalAssemblyCount { get; set; }
    public int UpTo30Count { get; set; }
    public decimal UpTo30Percentage { get; set; }
    public int From31To40Count { get; set; }
    public decimal From31To40Percentage { get; set; }
    public int From41To50Count { get; set; }
    public decimal From41To50Percentage { get; set; }
    public int From51To60Count { get; set; }
    public decimal From51To60Percentage { get; set; }
    public int From61To70Count { get; set; }
    public decimal From61To70Percentage { get; set; }
    public int Over70Count { get; set; }
    public decimal Over70Percentage { get; set; }
}
