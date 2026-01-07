using Bk.APG.Business.Dtos;
using FluentValidation;

namespace Bk.APG.Business.Validators;

public class DocumentStorageModificationValidator : AbstractValidator<DocumentStorageModificationDto>
{
    public DocumentStorageModificationValidator()
    {
        RuleFor(x => x.DisplayName)
            .NotNull()
            .NotEmpty();

        RuleFor(x => x.LanguageId)
            .NotNull()
            .NotEmpty();
    }
}
