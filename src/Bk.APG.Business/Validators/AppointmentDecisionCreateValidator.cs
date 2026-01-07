using Bk.APG.Business.Dtos;
using FluentValidation;

namespace Bk.APG.Business.Validators;

public class AppointmentDecisionCreateValidator : AbstractValidator<AppointmentDecisionCreateDto>
{
    public AppointmentDecisionCreateValidator()
    {
        RuleFor(x => x.CommitteeId)
            .NotEmpty();

        RuleFor(x => x.AppointmentDecisionTypeId)
            .NotNull();

        RuleFor(x => x.Link)
            .MaximumLength(250);

        RuleFor(x => x.Text)
            .MaximumLength(2000);
    }
}
