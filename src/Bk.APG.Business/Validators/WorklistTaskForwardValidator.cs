using Bk.APG.Business.Dtos;
using FluentValidation;

namespace Bk.APG.Business.Validators;

public class WorklistTaskForwardValidator : AbstractValidator<WorklistTaskForwardDto>
{
    public WorklistTaskForwardValidator()
    {
        RuleFor(x => x.CommitteeDueDate)
            .NotNull();

        RuleFor(x => x.CandidateListDueDate)
            .NotNull();

        RuleFor(x => x.CandidateListDescription)
            .NotNull()
            .NotEmpty();

        RuleFor(x => x.CommitteeDescription)
            .NotNull()
            .NotEmpty();
    }
}
