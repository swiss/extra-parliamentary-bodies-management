using Bk.APG.Business.Dtos;
using FluentValidation;

namespace Bk.APG.Business.Validators;

public class FormLetterSenderUpdateValidator : FormLetterSenderValidatorBase<FormLetterSenderUpdateDto>
{
    public FormLetterSenderUpdateValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty();
    }
}
