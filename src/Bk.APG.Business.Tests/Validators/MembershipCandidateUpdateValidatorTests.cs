using Bk.APG.Business.Dtos;
using Bk.APG.Business.Validators;
using FluentValidation.TestHelper;

namespace Bk.APG.Business.Tests.Validators;

[TestFixture]
internal class MembershipCandidateUpdateValidatorTests
{
    private MembershipCandidateUpdateValidator _validator;

    [SetUp]
    public void SetUp()
    {
        _validator = new MembershipCandidateUpdateValidator();
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
        var model = new MembershipCandidateUpdateDto
        {
            Id = Guid.Empty,
            BeginDate = default,
            EndDate = default,
            ElectionTypeId = Guid.Empty,
            FunctionId = Guid.Empty,
            ElectionOfficeId = Guid.Empty,
            RowVersion = 1
        };

        var result = _validator.TestValidate(model);

        result.ShouldHaveValidationErrorFor(x => x.Id);
        result.ShouldHaveValidationErrorFor(x => x.BeginDate);
        result.ShouldHaveValidationErrorFor(x => x.EndDate);
        result.ShouldHaveValidationErrorFor(x => x.ElectionTypeId);
        result.ShouldHaveValidationErrorFor(x => x.FunctionId);
        result.ShouldHaveValidationErrorFor(x => x.ElectionOfficeId);
    }

    [Test]
    public void Validate_WithoutPersonIdAndMissingPersonDetails_ShouldAddErrorForPersonFields()
    {
        var model = BuildValidModelWithoutPersonId();
        model.Surname = string.Empty;
        model.GivenName = string.Empty;
        model.GenderId = null;
        model.LanguageId = null;

        var result = _validator.TestValidate(model);

        result.ShouldHaveValidationErrorFor(x => x.Surname);
        result.ShouldHaveValidationErrorFor(x => x.GivenName);
        result.ShouldHaveValidationErrorFor(x => x.GenderId);
        result.ShouldHaveValidationErrorFor(x => x.LanguageId);
    }

    [Test]
    public void Validate_WithPersonIdAndMissingPersonDetails_ShouldNotAddErrorForPersonFields()
    {
        var model = BuildValidModelWithPersonId();
        model.Surname = string.Empty;
        model.GivenName = string.Empty;
        model.GenderId = null;
        model.LanguageId = null;

        var result = _validator.TestValidate(model);

        result.ShouldNotHaveValidationErrorFor(x => x.Surname);
        result.ShouldNotHaveValidationErrorFor(x => x.GivenName);
        result.ShouldNotHaveValidationErrorFor(x => x.GenderId);
        result.ShouldNotHaveValidationErrorFor(x => x.LanguageId);
    }

    [Test]
    public void Validate_WithEndDateBeforeBeginDate_ShouldAddErrorForEndDate()
    {
        var model = BuildValidModelWithPersonId();
        model.BeginDate = DateOnly.FromDateTime(DateTime.Today);
        model.EndDate = DateOnly.FromDateTime(DateTime.Today).AddDays(-1);

        var result = _validator.TestValidate(model);

        result.ShouldHaveValidationErrorFor(x => x.EndDate);
    }

    [Test]
    public void Validate_WithEndDateAfterBeginDate_ShouldNotAddErrorForEndDate()
    {
        var model = BuildValidModelWithPersonId();
        model.BeginDate = DateOnly.FromDateTime(DateTime.Today);
        model.EndDate = DateOnly.FromDateTime(DateTime.Today).AddDays(1);

        var result = _validator.TestValidate(model);

        result.ShouldNotHaveValidationErrorFor(x => x.EndDate);
    }

    [Test]
    public void Validate_WithInvalidMaxLength_ShouldAddErrorForFields()
    {
        var model = BuildValidModelWithoutPersonId();
        model.Surname = new string('a', 151);
        model.GivenName = new string('a', 151);

        var result = _validator.TestValidate(model);

        result.ShouldHaveValidationErrorFor(x => x.Surname);
        result.ShouldHaveValidationErrorFor(x => x.GivenName);
    }

    [TestCase(0)]
    [TestCase(101)]
    public void Validate_WithInvalidMaximumEmploymentLevel_ShouldAddError(int level)
    {
        var model = BuildValidModelWithPersonId();
        model.MaximumEmploymentLevel = level;

        var result = _validator.TestValidate(model);

        result.ShouldHaveValidationErrorFor(x => x.MaximumEmploymentLevel);
    }

    [TestCase(1)]
    [TestCase(50)]
    [TestCase(100)]
    public void Validate_WithValidMaximumEmploymentLevel_ShouldNotAddError(int level)
    {
        var model = BuildValidModelWithPersonId();
        model.MaximumEmploymentLevel = level;

        var result = _validator.TestValidate(model);

        result.ShouldNotHaveValidationErrorFor(x => x.MaximumEmploymentLevel);
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

    private static MembershipCandidateUpdateDto BuildValidModelWithPersonId()
    {
        return new MembershipCandidateUpdateDto
        {
            Id = Guid.NewGuid(),
            BeginDate = DateOnly.FromDateTime(DateTime.Today),
            EndDate = DateOnly.FromDateTime(DateTime.Today).AddYears(1),
            ElectionTypeId = Guid.NewGuid(),
            FunctionId = Guid.NewGuid(),
            ElectionOfficeId = Guid.NewGuid(),
            PersonId = Guid.NewGuid(),
            MaximumEmploymentLevel = 50,
            RowVersion = 1
        };
    }

    private static MembershipCandidateUpdateDto BuildValidModelWithoutPersonId()
    {
        return new MembershipCandidateUpdateDto
        {
            Id = Guid.NewGuid(),
            BeginDate = DateOnly.FromDateTime(DateTime.Today),
            EndDate = DateOnly.FromDateTime(DateTime.Today).AddYears(1),
            ElectionTypeId = Guid.NewGuid(),
            FunctionId = Guid.NewGuid(),
            ElectionOfficeId = Guid.NewGuid(),
            PersonId = null,
            Surname = "TestSurname",
            GivenName = "TestGivenName",
            GenderId = Guid.NewGuid(),
            LanguageId = Guid.NewGuid(),
            BirthYear = 1990,
            MaximumEmploymentLevel = 50,
            RowVersion = 1
        };
    }
}

