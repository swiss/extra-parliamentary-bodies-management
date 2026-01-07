namespace Bk.APG.Business.Dtos;

public enum DuplicateReason { NoDuplicateFound = 0, FullMatch = 1, LanguageMismatch = 2, InBirthYearRange = 3, GivenNameMismatch = 4 }

public class CandidateListDuplicateCheckResultDto
{
    public MembershipCandidateDetailDto MembershipCandidateToCheck { get; set; } = null!;
    public PersonDetailDto PersonFound { get; set; } = null!;
    public DuplicateReason DuplicateReason { get; set; }
}
