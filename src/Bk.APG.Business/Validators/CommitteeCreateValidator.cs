using Bk.APG.Business.Dtos;
using FluentValidation;

namespace Bk.APG.Business.Validators;

public class CommitteeCreateValidator : AbstractValidator<CommitteeCreateDto>
{
    public CommitteeCreateValidator()
    {
        RuleFor(x => x.DescriptionGerman)
            .NotNull()
            .NotEmpty()
            .MaximumLength(500);

        RuleFor(x => x.DescriptionFrench)
            .NotNull()
            .NotEmpty()
            .MaximumLength(500);

        RuleFor(x => x.DescriptionItalian)
            .NotNull()
            .NotEmpty()
            .MaximumLength(500);

        RuleFor(x => x.DescriptionRomansh)
            .NotNull()
            .NotEmpty()
            .MaximumLength(500);

        RuleFor(x => x.LevelId)
            .NotNull()
            .NotEmpty();

        RuleFor(x => x.OfficeId)
            .NotNull()
            .NotEmpty();

        RuleFor(x => x.DepartmentId)
            .NotNull()
            .NotEmpty();

        RuleFor(x => x.CommitteeTypeId)
            .NotNull()
            .NotEmpty();

        RuleFor(x => x.TermOfOfficeId)
            .NotNull()
            .NotEmpty();

        RuleFor(x => x.BeginDate)
            .NotNull();

        RuleFor(x => x.EndDate)
            .GreaterThan(x => x.BeginDate)
            .When(x => x.EndDate.HasValue);

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

        RuleFor(x => x.VacanciesInGeneralElection)
            .InclusiveBetween(0, 99)
            .When(x => x.VacanciesInGeneralElection.HasValue);
    }
}
