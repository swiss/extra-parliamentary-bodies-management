namespace Bk.APG.Business.Dtos;

public class CandidateListValidationResultDto
{
    public List<string> Errors { get; } = [];
    public bool IsValid => Errors.Count == 0;
    public List<CandidateListDuplicateCheckResultDto> DuplicateCheckResults { get; } = [];
    public List<PersonDetailDto> CreatedPersons { get; } = [];
    public List<PersonDetailDto> ExistingPersons { get; } = [];
    public bool AreJustificationsMissing { get; set; }
    public bool AreContactPointsMissing { get; set; }
    public bool IsAdditionalMembershipValidationRequired => PersonsWithMissingInterests.Count > 0 || PersonsWithMissingBaseData.Count > 0;
    public List<PersonMinimalDto> PersonsWithMissingInterests { get; } = [];
    public List<PersonMinimalDto> PersonsWithMissingBaseData { get; } = [];
    public List<PersonMinimalDto> PersonsWithMembershipValidationIssues { get; } = [];

    public bool AllValidationsPassed => !AreJustificationsMissing && !AreContactPointsMissing && !IsAdditionalMembershipValidationRequired &&
        Errors.Count == 0 && PersonsWithMembershipValidationIssues.Count == 0;

    public bool IsReadyForProposalActivated { get; set; }
}
