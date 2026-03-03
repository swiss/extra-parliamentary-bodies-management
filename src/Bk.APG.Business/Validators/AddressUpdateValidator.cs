using Bk.APG.Business.Dtos;
using FluentValidation;

namespace Bk.APG.Business.Validators;

public class AddressUpdateValidator : AbstractValidator<AddressUpdateDto>
{
    public AddressUpdateValidator()
    {
        RuleFor(x => x.CompanyName)
            .MaximumLength(150);

        RuleFor(x => x.Street)
            .MaximumLength(100);

        RuleFor(x => x.PoBox)
            .MaximumLength(50);

        RuleFor(x => x.Zip)
            .MaximumLength(10)
            .NotEmpty()
            .When(x => !string.IsNullOrWhiteSpace(x.Street), ApplyConditionTo.CurrentValidator);

        RuleFor(x => x.City)
            .MaximumLength(100);

        RuleFor(x => x.Phone)
            .MaximumLength(20);

        RuleFor(x => x.Mobile)
            .MaximumLength(20);

        RuleFor(x => x.Email)
            .MaximumLength(150)
            .EmailAddress()
            .When(x => !string.IsNullOrWhiteSpace(x.Email));
    }
}
