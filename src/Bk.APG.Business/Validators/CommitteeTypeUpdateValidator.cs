using Bk.APG.Business.Dtos;
using FluentValidation;

namespace Bk.APG.Business.Validators;

public class CommitteeTypeUpdateValidator : AbstractValidator<CommitteeTypeUpdateDto>
{
    public CommitteeTypeUpdateValidator()
    {
        RuleFor(x => x.FemaleThreshold)
            .InclusiveBetween(0, 100)
            .When(x => x.FemaleThreshold.HasValue);

        RuleFor(x => x.MaleThreshold)
            .InclusiveBetween(0, 100)
            .When(x => x.MaleThreshold.HasValue);

        RuleFor(x => x.GermanMinimalThreshold)
            .GreaterThanOrEqualTo(0)
            .When(x => x.GermanMinimalThreshold.HasValue);

        RuleFor(x => x.FrenchMinimalThreshold)
            .GreaterThanOrEqualTo(0)
            .When(x => x.FrenchMinimalThreshold.HasValue);

        RuleFor(x => x.ItalianMinimalThreshold)
            .GreaterThanOrEqualTo(0)
            .When(x => x.ItalianMinimalThreshold.HasValue);

        RuleFor(x => x.RomanshMinimalThreshold)
            .GreaterThanOrEqualTo(0)
            .When(x => x.RomanshMinimalThreshold.HasValue);

        RuleFor(x => x.GermanThresholdPercentage)
            .InclusiveBetween(0, 100)
            .When(x => x.GermanThresholdPercentage.HasValue);

        RuleFor(x => x.FrenchThresholdPercentage)
            .InclusiveBetween(0, 100)
            .When(x => x.FrenchThresholdPercentage.HasValue);

        RuleFor(x => x.ItalianThresholdPercentage)
            .InclusiveBetween(0, 100)
            .When(x => x.ItalianThresholdPercentage.HasValue);

        RuleFor(x => x.RomanshThresholdPercentage)
            .InclusiveBetween(0, 100)
            .When(x => x.RomanshThresholdPercentage.HasValue);

        // Custom validation: sum of gender percentages should not exceed 100
        RuleFor(x => x)
            .Must(x =>
            {
                var sum = (x.FemaleThreshold ?? 0) + (x.MaleThreshold ?? 0);
                return sum <= 100;
            })
            .WithMessage("The sum of FemaleThreshold and MaleThreshold cannot exceed 100")
            .When(x => x.FemaleThreshold.HasValue || x.MaleThreshold.HasValue);

        // Custom validation: sum of language percentages should not exceed 100
        RuleFor(x => x)
            .Must(x =>
            {
                var sum = (x.GermanThresholdPercentage ?? 0) +
                         (x.FrenchThresholdPercentage ?? 0) +
                         (x.ItalianThresholdPercentage ?? 0) +
                         (x.RomanshThresholdPercentage ?? 0);
                return sum <= 100;
            })
            .WithMessage("The sum of all language threshold percentages cannot exceed 100")
            .When(x => x.GermanThresholdPercentage.HasValue ||
                      x.FrenchThresholdPercentage.HasValue ||
                      x.ItalianThresholdPercentage.HasValue ||
                      x.RomanshThresholdPercentage.HasValue);
    }
}
