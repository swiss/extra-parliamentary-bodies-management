using Bk.APG.Business.Dtos;
using Bk.APG.Business.Validators;
using FluentValidation.TestHelper;

namespace Bk.APG.Business.Tests.Validators;

[TestFixture]
internal class GeneralMeasureUpdateValidatorTests
{
    private GeneralMeasureUpdateValidator _validator;

    [SetUp]
    public void SetUp()
    {
        _validator = new GeneralMeasureUpdateValidator();
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
        var model = new GeneralMeasureUpdateDto
        {
            DepartmentId = Guid.Empty
        };

        var result = _validator.TestValidate(model);

        result.ShouldHaveValidationErrorFor(x => x.DepartmentId);
    }

    [Test]
    public void Validate_WithNullOptionalFields_ShouldNotAddError()
    {
        var model = new GeneralMeasureUpdateDto
        {
            DepartmentId = Guid.NewGuid(),
            JustificationGenders = null,
            JustificationLanguages = null
        };

        var result = _validator.TestValidate(model);

        result.ShouldNotHaveValidationErrorFor(x => x.JustificationGenders);
        result.ShouldNotHaveValidationErrorFor(x => x.JustificationLanguages);
    }

    [Test]
    public void Validate_WithEmptyOptionalFields_ShouldNotAddError()
    {
        var model = new GeneralMeasureUpdateDto
        {
            DepartmentId = Guid.NewGuid(),
            JustificationGenders = string.Empty,
            JustificationLanguages = string.Empty
        };

        var result = _validator.TestValidate(model);

        result.ShouldNotHaveValidationErrorFor(x => x.JustificationGenders);
        result.ShouldNotHaveValidationErrorFor(x => x.JustificationLanguages);
    }

    private static GeneralMeasureUpdateDto BuildValidModel()
    {
        return new GeneralMeasureUpdateDto
        {
            DepartmentId = Guid.NewGuid(),
            JustificationGenders = "Justification for gender measures",
            JustificationLanguages = "Justification for language measures"
        };
    }
}

