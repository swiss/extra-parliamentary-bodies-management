using Bk.APG.Business.Dtos;
using FluentValidation;

namespace Bk.APG.Business.Validators;

public class GeneralElectionCommitteeUpdateValidator : AbstractValidator<GeneralElectionCommitteeUpdateDto>
{
    public GeneralElectionCommitteeUpdateValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty();

        RuleFor(x => x.CommitteeId)
            .NotEmpty();

        RuleFor(x => x.DescriptionGerman)
            .NotNull()
            .NotEmpty()
            .MinimumLength(1)
            .MaximumLength(500);

        RuleFor(x => x.DescriptionFrench)
            .NotNull()
            .NotEmpty()
            .MinimumLength(1)
            .MaximumLength(500);

        RuleFor(x => x.DescriptionItalian)
            .NotNull()
            .NotEmpty()
            .MinimumLength(1)
            .MaximumLength(500);

        RuleFor(x => x.DescriptionRomansh)
            .NotNull()
            .NotEmpty()
            .MinimumLength(1)
            .MaximumLength(500);

        RuleFor(x => x.LevelId)
            .NotEmpty();

        RuleFor(x => x.OfficeId)
            .NotEmpty();

        RuleFor(x => x.DepartmentId)
            .NotEmpty();

        RuleFor(x => x.CommitteeTypeId)
            .NotEmpty();

        RuleFor(x => x.TermOfOfficeId)
            .NotEmpty();

        RuleFor(x => x.BeginDate)
            .NotEmpty();

        RuleFor(x => x.EndDate)
            .GreaterThanOrEqualTo(x => x.BeginDate)
            .When(x => x.EndDate.HasValue);

        RuleFor(x => x.LegalBase)
            .NotNull()
            .NotEmpty()
            .When(x => x.FederalLawEstablishment == true);

        RuleFor(x => x.LegalBase)
            .MaximumLength(2000);

        RuleFor(x => x.MinimalMembers)
            .GreaterThanOrEqualTo(0)
            .When(x => x.MinimalMembers.HasValue);

        RuleFor(x => x.MaximalMembers)
            .GreaterThanOrEqualTo(0)
            .When(x => x.MaximalMembers.HasValue);

        RuleFor(x => x.MaximalMembers)
            .GreaterThanOrEqualTo(x => x.MinimalMembers)
            .When(x => x.MinimalMembers.HasValue && x.MaximalMembers.HasValue);

        RuleFor(x => x.LinkAuthorityWebsite)
            .MaximumLength(500);

        RuleFor(x => x.LinkHomepageGerman)
            .MaximumLength(500);

        RuleFor(x => x.LinkHomepageFrench)
            .MaximumLength(500);

        RuleFor(x => x.LinkHomepageItalian)
            .MaximumLength(500);

        RuleFor(x => x.LinkHomepageRomansh)
            .MaximumLength(500);

        RuleFor(x => x.AdditionalAuthorityMembers)
            .NotNull();
    }
}
