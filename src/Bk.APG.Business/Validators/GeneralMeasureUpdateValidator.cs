using Bk.APG.Business.Dtos;
using FluentValidation;

namespace Bk.APG.Business.Validators;

public class GeneralMeasureUpdateValidator : AbstractValidator<GeneralMeasureUpdateDto>
{
    public GeneralMeasureUpdateValidator()
    {
        RuleFor(x => x.DepartmentId)
            .NotEmpty();
    }
}
