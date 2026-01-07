using Bk.APG.Business.Dtos;
using Bk.APG.Business.Validators;
using FluentValidation.TestHelper;

namespace Bk.APG.Business.Tests.Validators;

[TestFixture]
internal class MembershipCandidatePartialUpdateValidatorTests
{
    private MembershipCandidatePartialUpdateValidator _validator;

    [SetUp]
    public void SetUp()
    {
        _validator = new MembershipCandidatePartialUpdateValidator();
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
        model.FunctionId = Guid.Empty;

        var result = _validator.TestValidate(model);

        result.ShouldHaveValidationErrorFor(x => x.FunctionId);
    }

    [Test]
    public void Validate_WithInvalidMaxLength_ShouldAddErrorForFields()
    {
        var model = BuildValidModel();
        model.Remarks = new string('a', 1001);
        model.RemarksStatus = new string('a', 1001);

        var result = _validator.TestValidate(model);

        result.ShouldHaveValidationErrorFor(x => x.Remarks);
        result.ShouldHaveValidationErrorFor(x => x.RemarksStatus);
    }

    [Test]
    public void Validate_WithValidMaxLength_ShouldNotAddErrorForFields()
    {
        var model = BuildValidModel();
        model.Remarks = new string('a', 1000);
        model.RemarksStatus = new string('a', 1000);

        var result = _validator.TestValidate(model);

        result.ShouldNotHaveValidationErrorFor(x => x.Remarks);
        result.ShouldNotHaveValidationErrorFor(x => x.RemarksStatus);
    }

    [Test]
    public void Validate_WithNullOptionalFields_ShouldNotAddError()
    {
        var model = BuildValidModel();
        model.Remarks = null;
        model.RemarksStatus = null;

        var result = _validator.TestValidate(model);

        result.ShouldNotHaveValidationErrorFor(x => x.Remarks);
        result.ShouldNotHaveValidationErrorFor(x => x.RemarksStatus);
    }

    private static MembershipCandidatePartialUpdateDto BuildValidModel()
    {
        return new MembershipCandidatePartialUpdateDto
        {
            FunctionId = Guid.NewGuid(),
            Remarks = "Test remarks",
            RemarksStatus = "Test status"
        };
    }
}

