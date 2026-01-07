using Bk.APG.Business.Dtos;
using Bk.APG.Business.Validators;
using FluentValidation.TestHelper;

namespace Bk.APG.Business.Tests.Validators;

[TestFixture]
internal class CommitteeTypeUpdateValidatorTests
{
    private CommitteeTypeUpdateValidator _validator;

    [SetUp]
    public void SetUp()
    {
        _validator = new CommitteeTypeUpdateValidator();
    }

    [Test]
    public void Validate_WithValidModel_ShouldNotHaveAnyValidationErrors()
    {
        var model = BuildValidModel();

        var result = _validator.TestValidate(model);
        result.ShouldNotHaveAnyValidationErrors();
    }

    [TestCase(-1)]
    [TestCase(101)]
    public void Validate_WithInvalidFemaleThreshold_ShouldAddError(int threshold)
    {
        var model = BuildValidModel();
        model.FemaleThreshold = threshold;

        var result = _validator.TestValidate(model);

        result.ShouldHaveValidationErrorFor(x => x.FemaleThreshold);
    }

    [TestCase(0)]
    [TestCase(50)]
    [TestCase(100)]
    public void Validate_WithValidFemaleThreshold_ShouldNotAddError(int threshold)
    {
        var model = BuildValidModel();
        model.FemaleThreshold = threshold;

        var result = _validator.TestValidate(model);

        result.ShouldNotHaveValidationErrorFor(x => x.FemaleThreshold);
    }

    [TestCase(-1)]
    [TestCase(101)]
    public void Validate_WithInvalidMaleThreshold_ShouldAddError(int threshold)
    {
        var model = BuildValidModel();
        model.MaleThreshold = threshold;

        var result = _validator.TestValidate(model);

        result.ShouldHaveValidationErrorFor(x => x.MaleThreshold);
    }

    [Test]
    public void Validate_WithGenderThresholdsSumExceeding100_ShouldAddError()
    {
        var model = BuildValidModel();
        model.FemaleThreshold = 60;
        model.MaleThreshold = 50;

        var result = _validator.TestValidate(model);

        result.ShouldHaveValidationErrorFor(x => x);
    }

    [Test]
    public void Validate_WithGenderThresholdsSumEquals100_ShouldNotAddError()
    {
        var model = BuildValidModel();
        model.FemaleThreshold = 60;
        model.MaleThreshold = 40;

        var result = _validator.TestValidate(model);

        result.ShouldNotHaveAnyValidationErrors();
    }

    [TestCase(-1)]
    public void Validate_WithInvalidGermanMinimalThreshold_ShouldAddError(int threshold)
    {
        var model = BuildValidModel();
        model.GermanMinimalThreshold = threshold;

        var result = _validator.TestValidate(model);

        result.ShouldHaveValidationErrorFor(x => x.GermanMinimalThreshold);
    }

    [TestCase(0)]
    [TestCase(10)]
    public void Validate_WithValidGermanMinimalThreshold_ShouldNotAddError(int threshold)
    {
        var model = BuildValidModel();
        model.GermanMinimalThreshold = threshold;

        var result = _validator.TestValidate(model);

        result.ShouldNotHaveValidationErrorFor(x => x.GermanMinimalThreshold);
    }

    [TestCase(-1)]
    [TestCase(101)]
    public void Validate_WithInvalidGermanThresholdPercentage_ShouldAddError(int percentage)
    {
        var model = BuildValidModel();
        model.GermanThresholdPercentage = percentage;

        var result = _validator.TestValidate(model);

        result.ShouldHaveValidationErrorFor(x => x.GermanThresholdPercentage);
    }

    [Test]
    public void Validate_WithLanguageThresholdsSumExceeding100_ShouldAddError()
    {
        var model = BuildValidModel();
        model.GermanThresholdPercentage = 60;
        model.FrenchThresholdPercentage = 30;
        model.ItalianThresholdPercentage = 20;
        model.RomanshThresholdPercentage = 10;

        var result = _validator.TestValidate(model);

        result.ShouldHaveValidationErrorFor(x => x);
    }

    [Test]
    public void Validate_WithLanguageThresholdsSumEquals100_ShouldNotAddError()
    {
        var model = BuildValidModel();
        model.GermanThresholdPercentage = 60;
        model.FrenchThresholdPercentage = 25;
        model.ItalianThresholdPercentage = 10;
        model.RomanshThresholdPercentage = 5;

        var result = _validator.TestValidate(model);

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Test]
    public void Validate_WithAllNullThresholds_ShouldNotAddError()
    {
        var model = BuildValidModel();
        model.FemaleThreshold = null;
        model.MaleThreshold = null;
        model.GermanMinimalThreshold = null;
        model.FrenchMinimalThreshold = null;
        model.ItalianMinimalThreshold = null;
        model.RomanshMinimalThreshold = null;
        model.GermanThresholdPercentage = null;
        model.FrenchThresholdPercentage = null;
        model.ItalianThresholdPercentage = null;
        model.RomanshThresholdPercentage = null;

        var result = _validator.TestValidate(model);

        result.ShouldNotHaveAnyValidationErrors();
    }

    private static CommitteeTypeUpdateDto BuildValidModel()
    {
        return new CommitteeTypeUpdateDto
        {
            FemaleThreshold = 40,
            MaleThreshold = 40,
            GermanMinimalThreshold = 5,
            FrenchMinimalThreshold = 3,
            ItalianMinimalThreshold = 2,
            RomanshMinimalThreshold = 1,
            GermanThresholdPercentage = 60,
            FrenchThresholdPercentage = 25,
            ItalianThresholdPercentage = 10,
            RomanshThresholdPercentage = 5
        };
    }
}

