using Bk.APG.Business.Dtos;
using Bk.APG.Business.Validators;
using FluentValidation.TestHelper;

namespace Bk.APG.Business.Tests.Validators;

[TestFixture]
internal class ContactPointCreateValidatorTests
{
    private ContactPointCreateValidator _validator;

    [SetUp]
    public void SetUp()
    {
        _validator = new ContactPointCreateValidator();
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
        var model = BuildValidModel();
        model.CompanyName = new string('a', 151);
        model.Section = new string('a', 151);
        model.Street = new string('a', 101);
        model.Zip = new string('a', 11);
        model.City = new string('a', 101);
        model.Email = new string('a', 151);
        model.Phone = new string('a', 21);
        model.PersonalEmail = new string('a', 151);
        model.PersonalPhone = new string('a', 21);
        model.PersonalMobile = new string('a', 21);
        model.Surname = new string('a', 151);
        model.GivenName = new string('a', 151);
        model.Title = new string('a', 101);

        var result = _validator.TestValidate(model);

        result.ShouldHaveValidationErrorFor(x => x.CompanyName);
        result.ShouldHaveValidationErrorFor(x => x.Street);
        result.ShouldHaveValidationErrorFor(x => x.Zip);
        result.ShouldHaveValidationErrorFor(x => x.City);
        result.ShouldHaveValidationErrorFor(x => x.Email);
        result.ShouldHaveValidationErrorFor(x => x.Phone);
        result.ShouldHaveValidationErrorFor(x => x.PersonalEmail);
        result.ShouldHaveValidationErrorFor(x => x.PersonalPhone);
        result.ShouldHaveValidationErrorFor(x => x.PersonalMobile);
        result.ShouldHaveValidationErrorFor(x => x.Surname);
        result.ShouldHaveValidationErrorFor(x => x.GivenName);
    }

    [Test]
    public void Validate_WithoutEndDateSmallerThanBeginDate_ShouldAddError()
    {
        var model = BuildValidModel();
        model.BeginDate = DateOnly.FromDateTime(DateTime.Today.AddDays(1));
        model.EndDate = DateOnly.FromDateTime(DateTime.Today.AddDays(-1));

        var result = _validator.TestValidate(model);

        result.ShouldHaveValidationErrorFor(x => x.EndDate);
    }

    [Test]
    public void Validate_WithoutGivenNameAndWithSurname_ShouldAddError()
    {
        var model = BuildValidModel();
        model.Surname = "MyName";
        model.GivenName = string.Empty;

        var result = _validator.TestValidate(model);

        result.ShouldHaveValidationErrorFor(x => x.GivenName);
    }

    [Test]
    public void Validate_WithoutRequiredFields_ShouldAddErrorForRequiredFields()
    {
        var model = BuildValidModel();
        model.ContactPointTypeId = Guid.Empty;
        model.ContactPointTypeUri = string.Empty;

        var result = _validator.TestValidate(model);

        result.ShouldHaveValidationErrorFor(x => x.ContactPointTypeUri);
    }

    [Test]
    public void Validate_WithoutEmailButWithCompanyName_ShouldAddErrorForRequiredFields()
    {
        var model = BuildValidModel();
        model.CompanyName = "Test AG";
        model.Email = string.Empty;
        model.PersonalEmail = string.Empty;

        var result = _validator.TestValidate(model);

        result.ShouldHaveValidationErrorFor(x => x.Email);
    }

    [Test]
    public void Validate_WithoutEmailButWithCompanyNameAndPersonalEmail_ShouldNotAddError()
    {
        var model = BuildValidModel();
        model.CompanyName = "Test AG";
        model.Email = string.Empty;
        model.PersonalEmail = "test@test.ch";

        var result = _validator.TestValidate(model);

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Test]
    public void Validate_WithoutPersonalEmailButWithSurname_ShouldAddErrorForRequiredFields()
    {
        var model = BuildValidModel();
        model.Surname = "Haueter";
        model.GivenName = string.Empty;
        model.PersonalEmail = string.Empty;
        model.Email = string.Empty;

        var result = _validator.TestValidate(model);

        result.ShouldHaveValidationErrorFor(x => x.PersonalEmail);
        result.ShouldHaveValidationErrorFor(x => x.GivenName);
    }

    [Test]
    public void Validate_WithoutPersonalEmailButWithCompanyEmail_ShouldNotAddError()
    {
        var model = BuildValidModel();
        model.Surname = "Haueter";
        model.GivenName = "Vreni";
        model.PersonalEmail = string.Empty;
        model.Email = "test@test.ch";

        var result = _validator.TestValidate(model);

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Test]
    public void Validate_WithInvalidEmail_ShouldAddError()
    {
        var model = BuildValidModel();
        model.Surname = "Haueter";
        model.GivenName = "Vreni";
        model.PersonalEmail = string.Empty;
        model.Email = "test";

        var result = _validator.TestValidate(model);

        result.ShouldHaveValidationErrorFor(x => x.Email);
    }

    private static ContactPointCreateDto BuildValidModel()
    {
        return new ContactPointCreateDto
        {
            CommitteeId = Guid.NewGuid(),
            ContactPointTypeId = Guid.NewGuid(),
            ContactPointTypeUri = "my.uri.admin.ch",
            CompanyName = "Test Amt",
            Section = "Sektion Nr. 1",
            BeginDate = new DateOnly(2020, 1, 1),
            EndDate = new DateOnly(2027, 12, 31),
            Street = "Teststrasse 2",
            PoBox = "Postfach",
            Zip = "3000",
            City = "Bern 12",
            Phone = "+41 77 444 33 22",
            Email = "test@testamt.ch",
            Surname = "Muster",
            GivenName = "Rita",
            Title = "Datenschutzberaterin",
            PersonalPhone = "+41 78 777 66 55",
            PersonalMobile = "+41 78 999 88 77",
            PersonalEmail = "r.muster@test.ch",
            ReleasePersonData = false,
            OldId = 123123,
            GenderId = Guid.NewGuid(),
            LanguageId = Guid.NewGuid(),
        };
    }
}
