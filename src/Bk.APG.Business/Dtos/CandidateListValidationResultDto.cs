namespace Bk.APG.Business.Dtos;

public class CandidateListValidationResultDto
{
    public List<string> Errors { get; } = [];
    public bool IsValid => Errors.Count == 0;
    public List<CandidateListDuplicateCheckResultDto> DuplicateCheckResults { get; } = [];
    public List<PersonDetailDto> CreatedPersons { get; } = [];
    public List<PersonDetailDto> ExistingPersons { get; } = [];
    public bool AreJustificationsMissing { get; set; }
}
