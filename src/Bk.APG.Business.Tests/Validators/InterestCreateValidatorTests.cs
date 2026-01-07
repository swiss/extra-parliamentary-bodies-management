using Bk.APG.Business.Dtos;
using Bk.APG.Business.Validators;
using FluentValidation.TestHelper;

namespace Bk.APG.Business.Tests.Validators;

[TestFixture]
internal class InterestCreateValidatorTests
{
    private InterestCreateValidator _validator;

    [SetUp]
    public void SetUp()
    {
        _validator = new InterestCreateValidator();
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
        var model = new InterestCreateDto
        {
            Id = Guid.NewGuid(),
            PersonId = Guid.Empty,
            InterestText = string.Empty,
            InterestCommitteeId = Guid.Empty,
            InterestFunctionId = Guid.Empty
        };

        var result = _validator.TestValidate(model);

        result.ShouldHaveValidationErrorFor(x => x.PersonId);
        result.ShouldHaveValidationErrorFor(x => x.InterestText);
        result.ShouldHaveValidationErrorFor(x => x.InterestCommitteeId);
        result.ShouldHaveValidationErrorFor(x => x.InterestFunctionId);
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

    [Test]
    public void Validate_WithoutBeginDate_ShouldNotAddErrorForEndDate()
    {
        var model = BuildValidModel();
        model.BeginDate = null;
        model.EndDate = DateOnly.FromDateTime(DateTime.Today);

        var result = _validator.TestValidate(model);

        result.ShouldNotHaveValidationErrorFor(x => x.EndDate);
    }

    [Test]
    public void Validate_WithEmptyLegalFormId_ShouldNotAddError()
    {
        var model = new InterestCreateDto
        {
            Id = Guid.NewGuid(),
            PersonId = Guid.NewGuid(),
            InterestText = "Test Interest",
            InterestCommitteeId = Guid.NewGuid(),
            InterestFunctionId = Guid.NewGuid(),
            LegalFormId = Guid.Empty
        };

        var result = _validator.TestValidate(model);

        result.ShouldNotHaveValidationErrorFor(x => x.LegalFormId);
    }

    [Test]
    public void Validate_WithNullLegalFormId_ShouldNotAddError()
    {
        var model = new InterestCreateDto
        {
            Id = Guid.NewGuid(),
            PersonId = Guid.NewGuid(),
            InterestText = "Test Interest",
            InterestCommitteeId = Guid.NewGuid(),
            InterestFunctionId = Guid.NewGuid(),
            LegalFormId = null
        };

        var result = _validator.TestValidate(model);

        result.ShouldNotHaveValidationErrorFor(x => x.LegalFormId);
    }

    private static InterestCreateDto BuildValidModel()
    {
        return new InterestCreateDto
        {
            Id = Guid.NewGuid(),
            PersonId = Guid.NewGuid(),
            InterestText = "Test Interest",
            InterestCommitteeId = Guid.NewGuid(),
            InterestFunctionId = Guid.NewGuid(),
            LegalFormId = Guid.NewGuid(),
            BeginDate = DateOnly.FromDateTime(DateTime.Today).AddDays(-30),
            EndDate = DateOnly.FromDateTime(DateTime.Today)
        };
    }
}

