using Bk.APG.Business.Dtos;
using FluentValidation;

namespace Bk.APG.Business.Validators;

public class ContactPointCreateValidator : AbstractValidator<ContactPointCreateDto>
{
    public ContactPointCreateValidator()
    {
        RuleFor(x => x.ContactPointTypeUri)
            .NotNull()
            .NotEmpty();

        RuleFor(x => x.BeginDate)
            .NotNull()
            .NotEmpty();

        RuleFor(x => x.EndDate)
            .GreaterThanOrEqualTo(x => x.BeginDate)
            .When(x => x.EndDate.HasValue);

        RuleFor(x => x.Zip)
            .NotNull()
            .NotEmpty()
            .MaximumLength(10);

        RuleFor(x => x.City)
            .NotNull()
            .NotEmpty()
            .MaximumLength(100);

        RuleFor(x => x.CompanyName)
            .MaximumLength(150);

        RuleFor(x => x.Section)
            .MaximumLength(150);

        RuleFor(x => x.Street)
            .MaximumLength(100);

        RuleFor(x => x.Email)
            .MaximumLength(150);

        RuleFor(x => x.Email)
            .NotNull()
            .NotEmpty()
            .EmailAddress()
            .When(x => !string.IsNullOrWhiteSpace(x.CompanyName) && string.IsNullOrWhiteSpace(x.PersonalEmail));

        RuleFor(x => x.Phone)
            .MaximumLength(20);

        RuleFor(x => x.PoBox)
            .MaximumLength(50);

        RuleFor(x => x.Surname)
            .MaximumLength(150);

        RuleFor(x => x.GivenName)
            .MaximumLength(150);

        RuleFor(x => x.GivenName)
            .NotNull()
            .NotEmpty()
            .When(x => !string.IsNullOrWhiteSpace(x.Surname));

        RuleFor(x => x.PersonalEmail)
            .MaximumLength(150);

        RuleFor(x => x.PersonalEmail)
            .NotNull()
            .NotEmpty()
            .EmailAddress()
            .When(x => !string.IsNullOrWhiteSpace(x.Surname) && string.IsNullOrWhiteSpace(x.Email));

        RuleFor(x => x.PersonalPhone)
            .MaximumLength(20);

        RuleFor(x => x.PersonalMobile)
            .MaximumLength(20);

        RuleFor(x => x.Title)
            .MaximumLength(100);
    }
}
