using Bk.APG.Business.Dtos;
using FluentValidation;

namespace Bk.APG.Business.Validators;

public class PersonUpdateValidator : AbstractValidator<PersonUpdateDto>
{
    public PersonUpdateValidator()
    {
        // Nested validator for addresses
        RuleFor(x => x.PrivateAddress)
            .SetValidator(new AddressUpdateValidator()!)
            .When(x => x.PrivateAddress != null);

        RuleFor(x => x.OfficeAddress)
            .SetValidator(new AddressUpdateValidator()!)
            .When(x => x.OfficeAddress != null);

        RuleFor(x => x.Surname)
            .NotNull()
            .NotEmpty()
            .MaximumLength(150);

        RuleFor(x => x.GivenName)
            .NotNull()
            .NotEmpty()
            .MaximumLength(150);

        RuleFor(x => x.BirthYear)
            .InclusiveBetween(1900, 2100);

        RuleFor(x => x.CorrespondenceLanguageId)
            .NotNull()
            .NotEmpty();

        RuleFor(x => x.LanguageId)
            .NotNull()
            .NotEmpty();

        RuleFor(x => x.GenderId)
            .NotNull()
            .NotEmpty();

        RuleFor(x => x.PrivateAddress)
            .NotNull()
            .When(x => !x.MaskAddress && x.OfficeAddress is null);

        RuleFor(x => x.OfficeAddress)
            .NotNull()
            .When(x => !x.MaskAddress && x.PrivateAddress is null);

        RuleFor(x => x.LegislaturePeriodIds)
            .NotEmpty()
            .When(x => x.FederalAssembly);

        RuleFor(x => x.Title)
            .MaximumLength(100);

        RuleFor(x => x.Occupation)
            .MaximumLength(150);

        RuleFor(x => x.Employer)
            .MaximumLength(150);

        RuleFor(x => x.SalutationText)
            .MaximumLength(200);

        RuleFor(x => x.CouncilId)
            .NotNull()
            .NotEmpty()
            .When(x => x.FederalAssembly);

        RuleFor(x => x.OfficeId)
            .NotNull()
            .NotEmpty()
            .When(x => x.FederalDuty);
    }
}
