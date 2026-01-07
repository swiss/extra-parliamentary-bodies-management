using Bk.APG.Business.Dtos;
using FluentValidation;

namespace Bk.APG.Business.Validators;

public class AppointmentDecisionUpdateValidator : AbstractValidator<AppointmentDecisionUpdateDto>
{
    public AppointmentDecisionUpdateValidator()
    {
        RuleFor(x => x.AppointmentDecisionTypeId)
            .NotNull();

        RuleFor(x => x.Link)
            .MaximumLength(250);

        RuleFor(x => x.Text)
            .MaximumLength(2000);
    }
}
