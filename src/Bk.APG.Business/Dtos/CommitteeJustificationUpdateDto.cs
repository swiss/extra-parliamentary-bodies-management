namespace Bk.APG.Business.Dtos;

public class CommitteeJustificationUpdateDto
{
    public Guid Id { get; init; }
    public string? JustificationMembers { get; set; }
    public string? JustificationGenders { get; set; }
    public string? MeasuresGenders { get; set; }
    public string? JustificationLanguages { get; set; }
    public string? MeasuresLanguages { get; set; }
    public int CurrentMemberCount { get; set; }
    public string? CurrentGenderQuota { get; set; }
    public string? CurrentLanguageQuota { get; set; }
    public required uint RowVersion { get; init; }
}
