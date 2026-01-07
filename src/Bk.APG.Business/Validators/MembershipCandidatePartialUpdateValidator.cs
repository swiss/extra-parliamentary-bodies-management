using Bk.APG.Business.Dtos;
using FluentValidation;

namespace Bk.APG.Business.Validators;

public class MembershipCandidatePartialUpdateValidator : AbstractValidator<MembershipCandidatePartialUpdateDto>
{
    public MembershipCandidatePartialUpdateValidator()
    {
        RuleFor(x => x.FunctionId)
            .NotEmpty();

        RuleFor(x => x.Remarks)
            .MaximumLength(1000);

        RuleFor(x => x.RemarksStatus)
            .MaximumLength(1000);
    }
}
