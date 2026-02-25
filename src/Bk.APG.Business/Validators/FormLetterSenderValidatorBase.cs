using Bk.APG.Business.Dtos;
using FluentValidation;
using Microsoft.AspNetCore.Http;

namespace Bk.APG.Business.Validators;

public abstract class FormLetterSenderValidatorBase<T> : AbstractValidator<T> where T : FormLetterSenderModificationDto
{
    private const int MaxSignatureFileSizeInBytes = 5 * 1024 * 1024;
    private readonly string[] _allowedSignatureExtensions = [".jpg", ".jpeg", ".png", ".gif", ".bmp", ".webp", ".svg", ".tif", ".tiff"];

    protected FormLetterSenderValidatorBase()
    {
        RuleFor(x => x.Description)
            .NotEmpty()
            .MaximumLength(150);

        RuleFor(x => x.Surname)
            .NotEmpty()
            .MaximumLength(150);

        RuleFor(x => x.GivenName)
            .NotEmpty()
            .MaximumLength(150);

        RuleFor(x => x.StreetGerman)
            .NotEmpty()
            .MaximumLength(100);

        RuleFor(x => x.CityGerman)
            .NotEmpty()
            .MaximumLength(100);

        RuleFor(x => x.StreetFrench)
            .NotEmpty()
            .MaximumLength(100);

        RuleFor(x => x.CityFrench)
            .NotEmpty()
            .MaximumLength(100);

        RuleFor(x => x.StreetItalian)
            .NotEmpty()
            .MaximumLength(100);

        RuleFor(x => x.CityItalian)
            .NotEmpty()
            .MaximumLength(100);

        RuleFor(x => x.StreetRomansh)
            .NotEmpty()
            .MaximumLength(100);

        RuleFor(x => x.CityRomansh)
            .NotEmpty()
            .MaximumLength(100);

        RuleFor(x => x.Zip)
            .NotEmpty()
            .MaximumLength(10);

        RuleFor(x => x.Phone)
            .MaximumLength(20)
            .When(x => !string.IsNullOrEmpty(x.Phone));

        RuleFor(x => x.Email)
            .MaximumLength(150)
            .EmailAddress()
            .When(x => !string.IsNullOrEmpty(x.Email));

        RuleFor(x => x.Website)
            .MaximumLength(500)
            .When(x => !string.IsNullOrEmpty(x.Website));

        RuleFor(x => x.SenderFunctionId)
            .NotEmpty();

        RuleFor(x => x.DepartmentId)
            .NotEmpty();

        RuleFor(x => x.Signature)
            .Custom((signature, context) =>
            {
                if (signature is not null)
                {
                    ValidateSignatureFormat(signature, context);
                    ValidateSignatureSize(signature, context);
                }
            });
    }

    private void ValidateSignatureFormat(IFormFile signature, ValidationContext<T> context)
    {
        var extension = Path.GetExtension(signature.FileName).ToLowerInvariant();
        if (!_allowedSignatureExtensions.Contains(extension))
        {
            context.AddFailure(
                nameof(FormLetterSenderModificationDto.Signature),
                $"Signature file format is not allowed. Allowed formats: {string.Join(", ", _allowedSignatureExtensions)}");
        }
    }

    private static void ValidateSignatureSize(IFormFile signature, ValidationContext<T> context)
    {
        if (signature.Length > MaxSignatureFileSizeInBytes)
        {
            context.AddFailure(
                nameof(FormLetterSenderModificationDto.Signature),
                $"Signature file size must not exceed 5 MB.");
        }
    }
}

