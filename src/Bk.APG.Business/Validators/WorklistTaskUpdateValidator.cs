using Bk.APG.Business.Dtos;
using FluentValidation;

namespace Bk.APG.Business.Validators;

public class WorklistTaskUpdateValidator : AbstractValidator<WorklistTaskUpdateDto>
{
    public WorklistTaskUpdateValidator()
    {
        RuleFor(x => x.WorklistTaskType)
            .NotNull()
            .NotEmpty();

        RuleFor(x => x.WorklistTaskState)
            .NotNull()
            .NotEmpty();

        RuleFor(x => x.AssignedTo)
            .NotNull()
            .NotEmpty();

        RuleFor(x => x.AssignedBy)
            .NotNull()
            .NotEmpty();

        RuleFor(x => x.DueDate)
            .NotNull();
    }
}
