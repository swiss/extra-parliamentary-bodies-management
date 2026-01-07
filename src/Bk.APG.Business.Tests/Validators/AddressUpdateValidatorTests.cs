using Bk.APG.Business.Dtos;
using Bk.APG.Business.Validators;
using FluentValidation.TestHelper;

namespace Bk.APG.Business.Tests.Validators;

[TestFixture]
internal class AddressUpdateValidatorTests
{
    private AddressUpdateValidator _validator;

    [SetUp]
    public void SetUp()
    {
        _validator = new AddressUpdateValidator();
    }

    [Test]
    public void Validate_WithValidModel_ShouldNotHaveAnyValidationErrors()
    {
        var model = BuildValidModel();

        var result = _validator.TestValidate(model);
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Test]
    public void Validate_WithInvalidMaxLength_ShouldAddErrorForFields()
    {
        var model = new AddressUpdateDto
        {
            CompanyName = new string('a', 151),
            Street = new string('a', 101),
            PoBox = new string('a', 51),
            CountryCode = new string('a', 11),
            Zip = new string('a', 11),
            City = new string('a', 101),
            Phone = new string('a', 21),
            Mobile = new string('a', 21),
            Email = new string('a', 151)
        };

        var result = _validator.TestValidate(model);

        result.ShouldHaveValidationErrorFor(x => x.CompanyName);
        result.ShouldHaveValidationErrorFor(x => x.Street);
        result.ShouldHaveValidationErrorFor(x => x.PoBox);
        result.ShouldHaveValidationErrorFor(x => x.CountryCode);
        result.ShouldHaveValidationErrorFor(x => x.Zip);
        result.ShouldHaveValidationErrorFor(x => x.City);
        result.ShouldHaveValidationErrorFor(x => x.Phone);
        result.ShouldHaveValidationErrorFor(x => x.Mobile);
        result.ShouldHaveValidationErrorFor(x => x.Email);
    }

    [Test]
    public void Validate_WithStreetButNoZip_ShouldAddErrorForZip()
    {
        var model = new AddressUpdateDto
        {
            Street = "Test Street 123",
            Zip = string.Empty
        };

        var result = _validator.TestValidate(model);

        result.ShouldHaveValidationErrorFor(x => x.Zip);
    }

    [Test]
    public void Validate_WithoutStreetAndNoZip_ShouldNotAddErrorForZip()
    {
        var model = new AddressUpdateDto
        {
            Street = string.Empty,
            Zip = string.Empty
        };

        var result = _validator.TestValidate(model);

        result.ShouldNotHaveValidationErrorFor(x => x.Zip);
    }

    [Test]
    public void Validate_WithInvalidEmail_ShouldAddErrorForEmail()
    {
        var model = new AddressUpdateDto
        {
            Email = "invalid-email"
        };

        var result = _validator.TestValidate(model);

        result.ShouldHaveValidationErrorFor(x => x.Email);
    }

    [Test]
    public void Validate_WithEmptyEmail_ShouldNotAddErrorForEmail()
    {
        var model = new AddressUpdateDto
        {
            Email = string.Empty
        };

        var result = _validator.TestValidate(model);

        result.ShouldNotHaveValidationErrorFor(x => x.Email);
    }

    [Test]
    public void Validate_WithValidEmail_ShouldNotAddErrorForEmail()
    {
        var model = new AddressUpdateDto
        {
            Email = "test@example.com"
        };

        var result = _validator.TestValidate(model);

        result.ShouldNotHaveValidationErrorFor(x => x.Email);
    }

    private static AddressUpdateDto BuildValidModel()
    {
        return new AddressUpdateDto
        {
            CompanyName = "Test Company",
            Street = "Test Street 123",
            PoBox = "PO Box 123",
            CountryCode = "CH",
            Zip = "1234",
            City = "Test City",
            Phone = "+41 12 345 67 89",
            Mobile = "+41 79 123 45 67",
            Email = "test@example.com"
        };
    }
}

