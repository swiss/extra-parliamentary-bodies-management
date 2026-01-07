using Bk.APG.Business.Dtos;
using Bk.APG.Business.Validators;
using FluentValidation.TestHelper;

namespace Bk.APG.Business.Tests.Validators;

[TestFixture]
internal class MembershipUpdateValidatorTests
{
    private MembershipUpdateValidator _validator;

    [SetUp]
    public void SetUp()
    {
        _validator = new MembershipUpdateValidator();
    }

    [Test]
    public void Validate_WithValidModel_ShouldNotHaveAnyValidationErrors()
    {
        var model = BuildValidModel();

        var result = _validator.TestValidate(model);
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Test]
    public void Validate_WithoutRequiredFields_ShouldAddErrorForRequiredFields()
    {
        var model = new MembershipUpdateDto
        {
            Id = Guid.Empty,
            CommitteeId = Guid.Empty,
            PersonId = Guid.Empty,
            BeginDate = default,
            EndDate = default,
            ElectionTypeId = Guid.Empty,
            FunctionId = Guid.Empty,
            ElectionOfficeId = Guid.Empty,
            RowVersion = 1
        };

        var result = _validator.TestValidate(model);

        result.ShouldHaveValidationErrorFor(x => x.Id);
        result.ShouldHaveValidationErrorFor(x => x.CommitteeId);
        result.ShouldHaveValidationErrorFor(x => x.PersonId);
        result.ShouldHaveValidationErrorFor(x => x.BeginDate);
        result.ShouldHaveValidationErrorFor(x => x.EndDate);
        result.ShouldHaveValidationErrorFor(x => x.ElectionTypeId);
        result.ShouldHaveValidationErrorFor(x => x.FunctionId);
        result.ShouldHaveValidationErrorFor(x => x.ElectionOfficeId);
    }

    [Test]
    public void Validate_WithEndDateBeforeBeginDate_ShouldAddErrorForEndDate()
    {
        var model = BuildValidModel();
        model.BeginDate = DateOnly.FromDateTime(DateTime.Today);
        model.EndDate = DateOnly.FromDateTime(DateTime.Today.AddDays(-1));

        var result = _validator.TestValidate(model);

        result.ShouldHaveValidationErrorFor(x => x.EndDate);
    }

    [Test]
    public void Validate_WithEndDateAfterBeginDate_ShouldNotAddErrorForEndDate()
    {
        var model = BuildValidModel();
        model.BeginDate = DateOnly.FromDateTime(DateTime.Today);
        model.EndDate = DateOnly.FromDateTime(DateTime.Today.AddDays(1));

        var result = _validator.TestValidate(model);

        result.ShouldNotHaveValidationErrorFor(x => x.EndDate);
    }

    [TestCase(0)]
    [TestCase(101)]
    public void Validate_WithInvalidMaximumEmploymentLevel_ShouldAddError(int level)
    {
        var model = BuildValidModel();
        model.MaximumEmploymentLevel = level;

        var result = _validator.TestValidate(model);

        result.ShouldHaveValidationErrorFor(x => x.MaximumEmploymentLevel);
    }

    [TestCase(1)]
    [TestCase(50)]
    [TestCase(100)]
    public void Validate_WithValidMaximumEmploymentLevel_ShouldNotAddError(int level)
    {
        var model = BuildValidModel();
        model.MaximumEmploymentLevel = level;

        var result = _validator.TestValidate(model);

        result.ShouldNotHaveValidationErrorFor(x => x.MaximumEmploymentLevel);
    }

    [Test]
    public void Validate_WithNullMaximumEmploymentLevel_ShouldNotAddError()
    {
        var model = BuildValidModel();
        model.MaximumEmploymentLevel = null;

        var result = _validator.TestValidate(model);

        result.ShouldNotHaveValidationErrorFor(x => x.MaximumEmploymentLevel);
    }

    private static MembershipUpdateDto BuildValidModel()
    {
        return new MembershipUpdateDto
        {
            Id = Guid.NewGuid(),
            CommitteeId = Guid.NewGuid(),
            PersonId = Guid.NewGuid(),
            BeginDate = DateOnly.FromDateTime(DateTime.Today),
            EndDate = DateOnly.FromDateTime(DateTime.Today.AddYears(1)),
            ElectionTypeId = Guid.NewGuid(),
            FunctionId = Guid.NewGuid(),
            ElectionOfficeId = Guid.NewGuid(),
            MaximumEmploymentLevel = 50,
            RowVersion = 1
        };
    }
}

