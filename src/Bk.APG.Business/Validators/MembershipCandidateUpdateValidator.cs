using Bk.APG.Business.Dtos;
using FluentValidation;

namespace Bk.APG.Business.Validators;

public class MembershipCandidateUpdateValidator : AbstractValidator<MembershipCandidateUpdateDto>
{
    public MembershipCandidateUpdateValidator()
    {
        RuleFor(x => x.Id)
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
            .When(x => x.BirthYear.HasValue);
    }
}
