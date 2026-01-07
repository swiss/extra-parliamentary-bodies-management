using Bk.APG.Business.Dtos;
using Bk.APG.Business.Validators;
using FluentValidation.TestHelper;

namespace Bk.APG.Business.Tests.Validators;

[TestFixture]
internal class AppointmentDecisionUpdateValidatorTests
{
    private AppointmentDecisionUpdateValidator _validator;

    [SetUp]
    public void SetUp()
    {
        _validator = new AppointmentDecisionUpdateValidator();
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
        var model = BuildValidModel();
        model.AppointmentDecisionTypeId = null;

        var result = _validator.TestValidate(model);

        result.ShouldHaveValidationErrorFor(x => x.AppointmentDecisionTypeId);
    }

    [Test]
    public void Validate_WithInvalidMaxLength_ShouldAddErrorForFields()
    {
        var model = BuildValidModel();
        model.Link = new string('a', 251);
        model.Text = new string('a', 2001);

        var result = _validator.TestValidate(model);

        result.ShouldHaveValidationErrorFor(x => x.Link);
        result.ShouldHaveValidationErrorFor(x => x.Text);
    }

    [Test]
    public void Validate_WithValidMaxLength_ShouldNotAddErrorForFields()
    {
        var model = BuildValidModel();
        model.Link = new string('a', 250);
        model.Text = new string('a', 2000);

        var result = _validator.TestValidate(model);

        result.ShouldNotHaveValidationErrorFor(x => x.Link);
        result.ShouldNotHaveValidationErrorFor(x => x.Text);
    }

    [Test]
    public void Validate_WithNullOptionalFields_ShouldNotAddError()
    {
        var model = BuildValidModel();
        model.Link = null;
        model.Text = null;

        var result = _validator.TestValidate(model);

        result.ShouldNotHaveValidationErrorFor(x => x.Link);
        result.ShouldNotHaveValidationErrorFor(x => x.Text);
    }

    [Test]
    public void Validate_WithEmptyOptionalFields_ShouldNotAddError()
    {
        var model = BuildValidModel();
        model.Link = string.Empty;
        model.Text = string.Empty;

        var result = _validator.TestValidate(model);

        result.ShouldNotHaveValidationErrorFor(x => x.Link);
        result.ShouldNotHaveValidationErrorFor(x => x.Text);
    }

    private static AppointmentDecisionUpdateDto BuildValidModel()
    {
        return new AppointmentDecisionUpdateDto
        {
            AppointmentDecisionTypeId = Guid.NewGuid(),
            Link = "https://example.com/decision",
            Text = "Appointment decision text"
        };
    }
}

