namespace Bk.APG.Business.Dtos;

public class GeneralElectionCommitteeJustificationUpdateDto
{
    public Guid Id { get; init; }
    public string? JustificationMembers { get; set; }
    public bool IsJustificationGendersRequired { get; set; }
    public string? JustificationGenders { get; set; }
    public string? MeasuresGenders { get; set; }
    public bool IsJustificationLanguagesRequired { get; set; }
    public string? JustificationLanguages { get; set; }
    public string? MeasuresLanguages { get; set; }
    public int CurrentMemberCount { get; set; }
    public string? CurrentGenderQuota { get; set; }
    public string? CurrentLanguageQuota { get; set; }
    public string? SelectionProcedure { get; set; }
    public required uint RowVersion { get; init; }
}
