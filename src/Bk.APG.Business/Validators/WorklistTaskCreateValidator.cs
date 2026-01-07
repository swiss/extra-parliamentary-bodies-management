using Bk.APG.Business.Dtos;
using FluentValidation;

namespace Bk.APG.Business.Validators;

public class WorklistTaskCreateValidator : AbstractValidator<WorklistTaskCreateDto>
{
    public WorklistTaskCreateValidator()
    {
        RuleFor(x => x.WorklistTaskTypeId)
            .NotNull()
            .NotEmpty();

        RuleFor(x => x.DueDate)
            .NotNull();
    }
}
