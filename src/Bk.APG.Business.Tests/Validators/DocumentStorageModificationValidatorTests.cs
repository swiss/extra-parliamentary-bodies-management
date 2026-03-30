using Bk.APG.Business.Dtos;
using Bk.APG.Business.Models;
using Bk.APG.Business.Validators;
using FluentValidation.TestHelper;
using Microsoft.AspNetCore.Http;

namespace Bk.APG.Business.Tests.Validators;

[TestFixture]
internal class DocumentStorageModificationValidatorTests
{
    private DocumentStorageModificationValidator _validator;

    [SetUp]
    public void SetUp()
    {
        _validator = new DocumentStorageModificationValidator();
    }

    [Test]
    public void Validate_WithValidModel_ShouldNotHaveAnyValidationErrors()
    {
        var model = BuildValidModel();

        var result = _validator.TestValidate(model);
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Test]
    public void Validate_WithEmptyDescription_ShouldThrowBusinessValidationException()
    {
        var model = new DocumentStorageModificationDto() { Id = Guid.NewGuid(), DisplayName = string.Empty, IsOriginal = true, File = null, LanguageId = new Guid(Language.GermanId) };

        var result = _validator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.DisplayName);
    }

    private static DocumentStorageModificationDto BuildValidModel()
    {
        var stream = new MemoryStream("Hello world!"u8.ToArray());
        IFormFile file = new FormFile(stream, 0, stream.Length, "id_from_form1", "fileName1");

        return new DocumentStorageModificationDto
        {
            Id = Guid.NewGuid(),
            DisplayName = "Name",
            File = file,
            IsOriginal = true,
            LanguageId = new Guid(Language.GermanId)
        };
    }
}
