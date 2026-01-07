using Bk.APG.Business.Dtos;
using FluentValidation;

namespace Bk.APG.Business.Validators;

public class MembershipUpdateValidator : AbstractValidator<MembershipUpdateDto>
{
    public MembershipUpdateValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty();

        RuleFor(x => x.CommitteeId)
            .NotEmpty();

        RuleFor(x => x.PersonId)
            .NotEmpty();

        RuleFor(x => x.BeginDate)
            .NotEmpty();

        RuleFor(x => x.EndDate)
            .NotEmpty()
            .GreaterThanOrEqualTo(x => x.BeginDate);

        RuleFor(x => x.ElectionTypeId)
            .NotEmpty();

        RuleFor(x => x.FunctionId)
            .NotEmpty();

        RuleFor(x => x.ElectionOfficeId)
            .NotEmpty();

        RuleFor(x => x.MaximumEmploymentLevel)
            .InclusiveBetween(1, 100)
            .When(x => x.MaximumEmploymentLevel.HasValue);
    }
}
