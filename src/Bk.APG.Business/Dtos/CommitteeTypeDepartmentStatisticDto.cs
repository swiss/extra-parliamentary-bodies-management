namespace Bk.APG.Business.Dtos;

public class CommitteeTypeDepartmentStatisticDto
{
    public required Guid CommitteeTypeId { get; init; }
    public required int CommitteeTypeOdgId { get; init; }
    public required int DepartmentOdgId { get; init; }
    public required int CommitteeCount { get; init; }
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
}
