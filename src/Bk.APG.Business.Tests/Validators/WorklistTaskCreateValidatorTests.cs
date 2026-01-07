using Bk.APG.Business.Dtos;
using Bk.APG.Business.Validators;
using FluentValidation.TestHelper;

namespace Bk.APG.Business.Tests.Validators;

[TestFixture]
internal class WorklistTaskCreateValidatorTests
{
    private WorklistTaskCreateValidator _validator;

    [SetUp]
    public void SetUp()
    {
        _validator = new WorklistTaskCreateValidator();
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
        var model = new WorklistTaskCreateDto
        {
            WorklistTaskTypeId = Guid.Empty,
            DueDate = default
        };

        var result = _validator.TestValidate(model);

        result.ShouldHaveValidationErrorFor(x => x.WorklistTaskTypeId);
    }

    private static WorklistTaskCreateDto BuildValidModel()
    {
        return new WorklistTaskCreateDto
        {
            WorklistTaskTypeId = Guid.NewGuid(),
            Description = "Test task description",
            DueDate = DateOnly.FromDateTime(DateTime.Today.AddDays(7))
        };
    }
}

