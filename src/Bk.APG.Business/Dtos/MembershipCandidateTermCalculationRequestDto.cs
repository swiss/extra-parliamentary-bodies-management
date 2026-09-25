namespace Bk.APG.Business.Dtos;

public class MembershipCandidateTermCalculationRequestDto
{
    public required DateOnly BeginDate { get; init; }
    public required DateOnly EndDate { get; init; }
}
