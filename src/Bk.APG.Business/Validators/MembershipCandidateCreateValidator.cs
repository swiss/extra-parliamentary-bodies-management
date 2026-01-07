using Bk.APG.Business.Dtos;
using FluentValidation;

namespace Bk.APG.Business.Validators;

public class MembershipCandidateCreateValidator : AbstractValidator<MembershipCandidateCreateDto>
{
    public MembershipCandidateCreateValidator()
    {
        RuleFor(x => x.FunctionId)
            .NotEmpty();

        // Conditional validation: when PersonId is not provided, person details are required
        RuleFor(x => x.Surname)
            .NotNull()
            .NotEmpty()
            .MaximumLength(150)
            .When(x => !x.PersonId.HasValue);

        RuleFor(x => x.GivenName)
            .NotNull()
            .NotEmpty()
            .MaximumLength(150)
            .When(x => !x.PersonId.HasValue);

        RuleFor(x => x.GenderId)
            .NotNull()
            .NotEmpty()
            .When(x => !x.PersonId.HasValue);

        RuleFor(x => x.LanguageId)
            .NotNull()
            .NotEmpty()
            .When(x => !x.PersonId.HasValue);

        RuleFor(x => x.BirthYear)
            .InclusiveBetween(1900, 2100)
            .When(x => !x.PersonId.HasValue);

        RuleFor(x => x.Remarks)
            .MaximumLength(1000);

        RuleFor(x => x.RemarksStatus)
            .MaximumLength(1000);
    }
}
