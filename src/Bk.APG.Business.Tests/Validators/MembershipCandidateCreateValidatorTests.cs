using Bk.APG.Business.Dtos;
using Bk.APG.Business.Validators;
using FluentValidation.TestHelper;

namespace Bk.APG.Business.Tests.Validators;

[TestFixture]
internal class MembershipCandidateCreateValidatorTests
{
    private MembershipCandidateCreateValidator _validator;

    [SetUp]
    public void SetUp()
    {
        _validator = new MembershipCandidateCreateValidator();
    }

    [Test]
    public void Validate_WithValidModelWithPersonId_ShouldNotHaveAnyValidationErrors()
    {
        var model = BuildValidModelWithPersonId();

        var result = _validator.TestValidate(model);
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Test]
    public void Validate_WithValidModelWithoutPersonId_ShouldNotHaveAnyValidationErrors()
    {
        var model = BuildValidModelWithoutPersonId();

        var result = _validator.TestValidate(model);
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Test]
    public void Validate_WithoutRequiredFields_ShouldAddErrorForRequiredFields()
    {
        var model = BuildValidModelWithPersonId();
        model.FunctionId = Guid.Empty;

        var result = _validator.TestValidate(model);

        result.ShouldHaveValidationErrorFor(x => x.FunctionId);
    }

    [Test]
    public void Validate_WithoutPersonIdAndMissingPersonDetails_ShouldAddErrorForPersonFields()
    {
        var model = new MembershipCandidateCreateDto
        {
            CommitteeId = Guid.NewGuid(),
            FunctionId = Guid.NewGuid(),
            PersonId = null,
            Surname = string.Empty,
            GivenName = string.Empty,
            GenderId = Guid.Empty,
            LanguageId = Guid.Empty
        };

        var result = _validator.TestValidate(model);

        result.ShouldHaveValidationErrorFor(x => x.Surname);
        result.ShouldHaveValidationErrorFor(x => x.GivenName);
        result.ShouldHaveValidationErrorFor(x => x.GenderId);
        result.ShouldHaveValidationErrorFor(x => x.LanguageId);
    }

    [Test]
    public void Validate_WithPersonIdAndMissingPersonDetails_ShouldNotAddErrorForPersonFields()
    {
        var model = new MembershipCandidateCreateDto
        {
            CommitteeId = Guid.NewGuid(),
            FunctionId = Guid.NewGuid(),
            PersonId = Guid.NewGuid(),
            Surname = string.Empty,
            GivenName = string.Empty,
            GenderId = Guid.Empty,
            LanguageId = Guid.Empty
        };

        var result = _validator.TestValidate(model);

        result.ShouldNotHaveValidationErrorFor(x => x.Surname);
        result.ShouldNotHaveValidationErrorFor(x => x.GivenName);
        result.ShouldNotHaveValidationErrorFor(x => x.GenderId);
        result.ShouldNotHaveValidationErrorFor(x => x.LanguageId);
    }

    [Test]
    public void Validate_WithInvalidMaxLength_ShouldAddErrorForFields()
    {
        var model = BuildValidModelWithoutPersonId();
        model.Surname = new string('a', 151);
        model.GivenName = new string('a', 151);
        model.Remarks = new string('a', 1001);
        model.RemarksStatus = new string('a', 1001);

        var result = _validator.TestValidate(model);

        result.ShouldHaveValidationErrorFor(x => x.Surname);
        result.ShouldHaveValidationErrorFor(x => x.GivenName);
        result.ShouldHaveValidationErrorFor(x => x.Remarks);
        result.ShouldHaveValidationErrorFor(x => x.RemarksStatus);
    }

    [TestCase(1899)]
    [TestCase(2101)]
    public void Validate_WithInvalidBirthYear_ShouldAddErrorForBirthYear(int birthYear)
    {
        var model = BuildValidModelWithoutPersonId();
        model.BirthYear = birthYear;

        var result = _validator.TestValidate(model);

        result.ShouldHaveValidationErrorFor(x => x.BirthYear);
    }

    [TestCase(1900)]
    [TestCase(2000)]
    [TestCase(2100)]
    public void Validate_WithValidBirthYear_ShouldNotAddErrorForBirthYear(int birthYear)
    {
        var model = BuildValidModelWithoutPersonId();
        model.BirthYear = birthYear;

        var result = _validator.TestValidate(model);

        result.ShouldNotHaveValidationErrorFor(x => x.BirthYear);
    }

    private static MembershipCandidateCreateDto BuildValidModelWithPersonId()
    {
        return new MembershipCandidateCreateDto
        {
            CommitteeId = Guid.NewGuid(),
            FunctionId = Guid.NewGuid(),
            PersonId = Guid.NewGuid(),
            Surname = "TestSurname",
            GivenName = "TestGivenName",
            GenderId = Guid.NewGuid(),
            LanguageId = Guid.NewGuid(),
            Remarks = "Test remarks",
            RemarksStatus = "Test status"
        };
    }

    private static MembershipCandidateCreateDto BuildValidModelWithoutPersonId()
    {
        return new MembershipCandidateCreateDto
        {
            CommitteeId = Guid.NewGuid(),
            FunctionId = Guid.NewGuid(),
            PersonId = null,
            Surname = "TestSurname",
            GivenName = "TestGivenName",
            GenderId = Guid.NewGuid(),
            LanguageId = Guid.NewGuid(),
            BirthYear = 1990,
            Remarks = "Test remarks",
            RemarksStatus = "Test status"
        };
    }
}

