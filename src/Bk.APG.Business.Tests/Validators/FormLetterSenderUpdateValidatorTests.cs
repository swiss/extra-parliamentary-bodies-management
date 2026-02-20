using Bk.APG.Business.Dtos;
using Bk.APG.Business.Validators;
using FluentValidation.TestHelper;
using Microsoft.AspNetCore.Http;

namespace Bk.APG.Business.Tests.Validators;

[TestFixture]
internal class FormLetterSenderUpdateValidatorTests
{
    private FormLetterSenderUpdateValidator _validator;

    [SetUp]
    public void SetUp()
    {
        _validator = new FormLetterSenderUpdateValidator();
    }

    [Test]
    public void Validate_WithValidModel_ShouldNotHaveAnyValidationErrors()
    {
        var model = BuildValidModel();

        var result = _validator.TestValidate(model);
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Test]
    public void Validate_WithEmptyId_ShouldHaveValidationError()
    {
        var model = BuildValidModel();
        model.Id = Guid.Empty;

        var result = _validator.TestValidate(model);

        result.ShouldHaveValidationErrorFor(x => x.Id);
    }

    [Test]
    public void Validate_WithEmptyDescription_ShouldHaveValidationError()
    {
        var model = BuildValidModel();
        model.Description = string.Empty;

        var result = _validator.TestValidate(model);

        result.ShouldHaveValidationErrorFor(x => x.Description);
    }

    [Test]
    public void Validate_WithDescriptionExceedingMaxLength_ShouldHaveValidationError()
    {
        var model = BuildValidModel();
        model.Description = new string('a', 151);

        var result = _validator.TestValidate(model);

        result.ShouldHaveValidationErrorFor(x => x.Description);
    }

    [Test]
    public void Validate_WithDescriptionAtMaxLength_ShouldNotHaveValidationError()
    {
        var model = BuildValidModel();
        model.Description = new string('a', 150);

        var result = _validator.TestValidate(model);

        result.ShouldNotHaveValidationErrorFor(x => x.Description);
    }

    [Test]
    public void Validate_WithEmptySurname_ShouldHaveValidationError()
    {
        var model = BuildValidModel();
        model.Surname = string.Empty;

        var result = _validator.TestValidate(model);

        result.ShouldHaveValidationErrorFor(x => x.Surname);
    }

    [Test]
    public void Validate_WithSurnameExceedingMaxLength_ShouldHaveValidationError()
    {
        var model = BuildValidModel();
        model.Surname = new string('a', 151);

        var result = _validator.TestValidate(model);

        result.ShouldHaveValidationErrorFor(x => x.Surname);
    }

    [Test]
    public void Validate_WithEmptyGivenName_ShouldHaveValidationError()
    {
        var model = BuildValidModel();
        model.GivenName = string.Empty;

        var result = _validator.TestValidate(model);

        result.ShouldHaveValidationErrorFor(x => x.GivenName);
    }

    [Test]
    public void Validate_WithGivenNameExceedingMaxLength_ShouldHaveValidationError()
    {
        var model = BuildValidModel();
        model.GivenName = new string('a', 151);

        var result = _validator.TestValidate(model);

        result.ShouldHaveValidationErrorFor(x => x.GivenName);
    }

    [Test]
    public void Validate_WithEmptyStreetGerman_ShouldHaveValidationError()
    {
        var model = BuildValidModel();
        model.StreetGerman = string.Empty;

        var result = _validator.TestValidate(model);

        result.ShouldHaveValidationErrorFor(x => x.StreetGerman);
    }

    [Test]
    public void Validate_WithStreetExceedingMaxLength_ShouldHaveValidationError()
    {
        var model = BuildValidModel();
        model.StreetGerman = new string('a', 101);
        model.StreetFrench = new string('a', 101);
        model.StreetItalian = new string('a', 101);
        model.StreetRomansh = new string('a', 101);

        var result = _validator.TestValidate(model);

        result.ShouldHaveValidationErrorFor(x => x.StreetGerman);
        result.ShouldHaveValidationErrorFor(x => x.StreetFrench);
        result.ShouldHaveValidationErrorFor(x => x.StreetItalian);
        result.ShouldHaveValidationErrorFor(x => x.StreetRomansh);
    }

    [Test]
    public void Validate_WithEmptyStreetFrench_ShouldHaveValidationError()
    {
        var model = BuildValidModel();
        model.StreetFrench = string.Empty;

        var result = _validator.TestValidate(model);

        result.ShouldHaveValidationErrorFor(x => x.StreetFrench);
    }

    [Test]
    public void Validate_WithEmptyStreetItalian_ShouldHaveValidationError()
    {
        var model = BuildValidModel();
        model.StreetItalian = string.Empty;

        var result = _validator.TestValidate(model);

        result.ShouldHaveValidationErrorFor(x => x.StreetItalian);
    }

    [Test]
    public void Validate_WithEmptyStreetRomansh_ShouldHaveValidationError()
    {
        var model = BuildValidModel();
        model.StreetRomansh = string.Empty;

        var result = _validator.TestValidate(model);

        result.ShouldHaveValidationErrorFor(x => x.StreetRomansh);
    }

    [Test]
    public void Validate_WithEmptyCityGerman_ShouldHaveValidationError()
    {
        var model = BuildValidModel();
        model.CityGerman = string.Empty;

        var result = _validator.TestValidate(model);

        result.ShouldHaveValidationErrorFor(x => x.CityGerman);
    }

    [Test]
    public void Validate_WithCityExceedingMaxLength_ShouldHaveValidationError()
    {
        var model = BuildValidModel();
        model.CityGerman = new string('a', 101);
        model.CityFrench = new string('a', 101);
        model.CityItalian = new string('a', 101);
        model.CityRomansh = new string('a', 101);

        var result = _validator.TestValidate(model);

        result.ShouldHaveValidationErrorFor(x => x.CityGerman);
        result.ShouldHaveValidationErrorFor(x => x.CityFrench);
        result.ShouldHaveValidationErrorFor(x => x.CityItalian);
        result.ShouldHaveValidationErrorFor(x => x.CityRomansh);
    }

    [Test]
    public void Validate_WithEmptyCityFrench_ShouldHaveValidationError()
    {
        var model = BuildValidModel();
        model.CityFrench = string.Empty;

        var result = _validator.TestValidate(model);

        result.ShouldHaveValidationErrorFor(x => x.CityFrench);
    }

    [Test]
    public void Validate_WithEmptyCityItalian_ShouldHaveValidationError()
    {
        var model = BuildValidModel();
        model.CityItalian = string.Empty;

        var result = _validator.TestValidate(model);

        result.ShouldHaveValidationErrorFor(x => x.CityItalian);
    }

    [Test]
    public void Validate_WithEmptyCityRomansh_ShouldHaveValidationError()
    {
        var model = BuildValidModel();
        model.CityRomansh = string.Empty;

        var result = _validator.TestValidate(model);

        result.ShouldHaveValidationErrorFor(x => x.CityRomansh);
    }

    [Test]
    public void Validate_WithEmptyZip_ShouldHaveValidationError()
    {
        var model = BuildValidModel();
        model.Zip = string.Empty;

        var result = _validator.TestValidate(model);

        result.ShouldHaveValidationErrorFor(x => x.Zip);
    }

    [Test]
    public void Validate_WithZipExceedingMaxLength_ShouldHaveValidationError()
    {
        var model = BuildValidModel();
        model.Zip = new string('1', 11);

        var result = _validator.TestValidate(model);

        result.ShouldHaveValidationErrorFor(x => x.Zip);
    }

    [Test]
    public void Validate_WithZipAtMaxLength_ShouldNotHaveValidationError()
    {
        var model = BuildValidModel();
        model.Zip = new string('1', 10);

        var result = _validator.TestValidate(model);

        result.ShouldNotHaveValidationErrorFor(x => x.Zip);
    }

    [Test]
    public void Validate_WithPhoneExceedingMaxLength_ShouldHaveValidationError()
    {
        var model = BuildValidModel();
        model.Phone = new string('1', 21);

        var result = _validator.TestValidate(model);

        result.ShouldHaveValidationErrorFor(x => x.Phone);
    }

    [Test]
    public void Validate_WithValidPhone_ShouldNotHaveValidationError()
    {
        var model = BuildValidModel();
        model.Phone = "+41 79 123 45 67";

        var result = _validator.TestValidate(model);

        result.ShouldNotHaveValidationErrorFor(x => x.Phone);
    }

    [Test]
    public void Validate_WithEmptyPhone_ShouldNotHaveValidationError()
    {
        var model = BuildValidModel();
        model.Phone = string.Empty;

        var result = _validator.TestValidate(model);

        result.ShouldNotHaveValidationErrorFor(x => x.Phone);
    }

    [Test]
    public void Validate_WithEmailExceedingMaxLength_ShouldHaveValidationError()
    {
        var model = BuildValidModel();
        model.Email = new string('a', 140) + "@example.com";

        var result = _validator.TestValidate(model);

        result.ShouldHaveValidationErrorFor(x => x.Email);
    }

    [Test]
    public void Validate_WithInvalidEmail_ShouldHaveValidationError()
    {
        var model = BuildValidModel();
        model.Email = "invalid-email";

        var result = _validator.TestValidate(model);

        result.ShouldHaveValidationErrorFor(x => x.Email);
    }

    [Test]
    public void Validate_WithValidEmail_ShouldNotHaveValidationError()
    {
        var model = BuildValidModel();
        model.Email = "test@example.com";

        var result = _validator.TestValidate(model);

        result.ShouldNotHaveValidationErrorFor(x => x.Email);
    }

    [Test]
    public void Validate_WithEmptyEmail_ShouldNotHaveValidationError()
    {
        var model = BuildValidModel();
        model.Email = string.Empty;

        var result = _validator.TestValidate(model);

        result.ShouldNotHaveValidationErrorFor(x => x.Email);
    }

    [Test]
    public void Validate_WithWebsiteExceedingMaxLength_ShouldHaveValidationError()
    {
        var model = BuildValidModel();
        model.Website = new string('a', 501);

        var result = _validator.TestValidate(model);

        result.ShouldHaveValidationErrorFor(x => x.Website);
    }

    [Test]
    public void Validate_WithValidWebsite_ShouldNotHaveValidationError()
    {
        var model = BuildValidModel();
        model.Website = "https://www.example.com";

        var result = _validator.TestValidate(model);

        result.ShouldNotHaveValidationErrorFor(x => x.Website);
    }

    [Test]
    public void Validate_WithEmptyWebsite_ShouldNotHaveValidationError()
    {
        var model = BuildValidModel();
        model.Website = string.Empty;

        var result = _validator.TestValidate(model);

        result.ShouldNotHaveValidationErrorFor(x => x.Website);
    }

    [Test]
    public void Validate_WithEmptySenderFunctionId_ShouldHaveValidationError()
    {
        var model = BuildValidModel();
        model.SenderFunctionId = Guid.Empty;

        var result = _validator.TestValidate(model);

        result.ShouldHaveValidationErrorFor(x => x.SenderFunctionId);
    }

    [Test]
    public void Validate_WithEmptyDepartmentId_ShouldHaveValidationError()
    {
        var model = BuildValidModel();
        model.DepartmentId = Guid.Empty;

        var result = _validator.TestValidate(model);

        result.ShouldHaveValidationErrorFor(x => x.DepartmentId);
    }

    [Test]
    public void Validate_WithValidSignatureJpg_ShouldNotHaveValidationError()
    {
        var model = BuildValidModel();
        var file = CreateFormFile("test.jpg", 1024 * 100);
        model.Signature = file;

        var result = _validator.TestValidate(model);

        result.ShouldNotHaveValidationErrorFor(x => x.Signature);
    }

    [Test]
    public void Validate_WithValidSignaturePng_ShouldNotHaveValidationError()
    {
        var model = BuildValidModel();
        var file = CreateFormFile("test.png", 1024 * 100);
        model.Signature = file;

        var result = _validator.TestValidate(model);

        result.ShouldNotHaveValidationErrorFor(x => x.Signature);
    }

    [Test]
    public void Validate_WithValidSignatureGif_ShouldNotHaveValidationError()
    {
        var model = BuildValidModel();
        var file = CreateFormFile("test.gif", 1024 * 100);
        model.Signature = file;

        var result = _validator.TestValidate(model);

        result.ShouldNotHaveValidationErrorFor(x => x.Signature);
    }

    [Test]
    public void Validate_WithValidSignatureBmp_ShouldNotHaveValidationError()
    {
        var model = BuildValidModel();
        var file = CreateFormFile("test.bmp", 1024 * 100);
        model.Signature = file;

        var result = _validator.TestValidate(model);

        result.ShouldNotHaveValidationErrorFor(x => x.Signature);
    }

    [Test]
    public void Validate_WithValidSignatureWebp_ShouldNotHaveValidationError()
    {
        var model = BuildValidModel();
        var file = CreateFormFile("test.webp", 1024 * 100);
        model.Signature = file;

        var result = _validator.TestValidate(model);

        result.ShouldNotHaveValidationErrorFor(x => x.Signature);
    }

    [Test]
    public void Validate_WithValidSignatureSvg_ShouldNotHaveValidationError()
    {
        var model = BuildValidModel();
        var file = CreateFormFile("test.svg", 1024 * 100);
        model.Signature = file;

        var result = _validator.TestValidate(model);

        result.ShouldNotHaveValidationErrorFor(x => x.Signature);
    }

    [Test]
    public void Validate_WithValidSignatureTif_ShouldNotHaveValidationError()
    {
        var model = BuildValidModel();
        var file = CreateFormFile("test.tif", 1024 * 100);
        model.Signature = file;

        var result = _validator.TestValidate(model);

        result.ShouldNotHaveValidationErrorFor(x => x.Signature);
    }

    [Test]
    public void Validate_WithValidSignatureTiff_ShouldNotHaveValidationError()
    {
        var model = BuildValidModel();
        var file = CreateFormFile("test.tiff", 1024 * 100);
        model.Signature = file;

        var result = _validator.TestValidate(model);

        result.ShouldNotHaveValidationErrorFor(x => x.Signature);
    }

    [Test]
    public void Validate_WithValidSignatureJpeg_ShouldNotHaveValidationError()
    {
        var model = BuildValidModel();
        var file = CreateFormFile("test.jpeg", 1024 * 100);
        model.Signature = file;

        var result = _validator.TestValidate(model);

        result.ShouldNotHaveValidationErrorFor(x => x.Signature);
    }

    [Test]
    public void Validate_WithSignatureInvalidFormat_ShouldHaveValidationError()
    {
        var model = BuildValidModel();
        var file = CreateFormFile("test.pdf", 1024 * 100);
        model.Signature = file;

        var result = _validator.TestValidate(model);

        result.ShouldHaveValidationErrorFor(x => x.Signature);
    }

    [Test]
    public void Validate_WithSignatureInvalidFormatDOC_ShouldHaveValidationError()
    {
        var model = BuildValidModel();
        var file = CreateFormFile("test.doc", 1024 * 100);
        model.Signature = file;

        var result = _validator.TestValidate(model);

        result.ShouldHaveValidationErrorFor(x => x.Signature);
    }

    [Test]
    public void Validate_WithSignatureExceedingMaxSize_ShouldHaveValidationError()
    {
        var model = BuildValidModel();
        var file = CreateFormFile("test.jpg", 1024 * 1024 * 6);
        model.Signature = file;

        var result = _validator.TestValidate(model);

        result.ShouldHaveValidationErrorFor(x => x.Signature);
    }

    [Test]
    public void Validate_WithSignatureAtMaxSize_ShouldNotHaveValidationError()
    {
        var model = BuildValidModel();
        var file = CreateFormFile("test.jpg", 1024 * 1024 * 5);
        model.Signature = file;

        var result = _validator.TestValidate(model);

        result.ShouldNotHaveValidationErrorFor(x => x.Signature);
    }

    [Test]
    public void Validate_WithoutSignature_ShouldNotHaveValidationError()
    {
        var model = BuildValidModel();
        model.Signature = null;

        var result = _validator.TestValidate(model);

        result.ShouldNotHaveValidationErrorFor(x => x.Signature);
    }

    [Test]
    public void Validate_WithSignatureUppercaseExtension_ShouldNotHaveValidationError()
    {
        var model = BuildValidModel();
        var file = CreateFormFile("test.JPG", 1024 * 100);
        model.Signature = file;

        var result = _validator.TestValidate(model);

        result.ShouldNotHaveValidationErrorFor(x => x.Signature);
    }

    private static FormLetterSenderUpdateDto BuildValidModel()
    {
        return new FormLetterSenderUpdateDto
        {
            Id = Guid.NewGuid(),
            Description = "Test Description",
            Surname = "Müller",
            GivenName = "John",
            StreetGerman = "Teststrasse 123",
            StreetFrench = "Rue de Test 123",
            StreetItalian = "Via del Test 123",
            StreetRomansh = "Via dal Test 123",
            Zip = "3011",
            CityGerman = "Bern",
            CityFrench = "Berne",
            CityItalian = "Berna",
            CityRomansh = "Berna",
            SenderFunctionId = Guid.NewGuid(),
            DepartmentId = Guid.NewGuid(),
            OfficeId = Guid.NewGuid(),
            Phone = "+41 79 123 45 67",
            Email = "test@example.com",
            Website = "https://www.example.com",
            Signature = null
        };
    }

    private static FormFile CreateFormFile(string fileName, long fileSize)
    {
        using var stream = new MemoryStream(new byte[fileSize]);
        return new FormFile(stream, 0, fileSize, "signature", fileName);
    }
}

