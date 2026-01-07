using Bk.APG.Business.Dtos;
using FluentValidation;

namespace Bk.APG.Business.Validators;

public class InterestCreateValidator : AbstractValidator<InterestCreateDto>
{
    public InterestCreateValidator()
    {
        RuleFor(x => x.PersonId)
            .NotEmpty();

        RuleFor(x => x.InterestText)
            .NotNull()
            .NotEmpty();

        RuleFor(x => x.InterestCommitteeId)
            .NotEmpty();

        RuleFor(x => x.InterestFunctionId)
            .NotEmpty();

        RuleFor(x => x.LegalFormId)
            .NotNull()
            .NotEmpty()
            .When(x => x.LegalFormId.HasValue);

        RuleFor(x => x.EndDate)
            .GreaterThanOrEqualTo(x => x.BeginDate)
            .When(x => x.BeginDate.HasValue && x.EndDate.HasValue);
    }
}
