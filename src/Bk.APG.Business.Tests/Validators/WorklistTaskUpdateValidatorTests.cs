using Bk.APG.Business.Dtos;
using Bk.APG.Business.Validators;
using FluentValidation.TestHelper;

namespace Bk.APG.Business.Tests.Validators;

[TestFixture]
internal class WorklistTaskUpdateValidatorTests
{
    private WorklistTaskUpdateValidator _validator;

    [SetUp]
    public void SetUp()
    {
        _validator = new WorklistTaskUpdateValidator();
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
        var model = new WorklistTaskUpdateDto
        {
            WorklistTaskType = string.Empty,
            WorklistTaskState = string.Empty,
            AssignedBy = string.Empty,
            AssignedTo = string.Empty,
            DueDate = default,
            Id = Guid.NewGuid()
        };

        var result = _validator.TestValidate(model);

        result.ShouldHaveValidationErrorFor(x => x.WorklistTaskType);
        result.ShouldHaveValidationErrorFor(x => x.WorklistTaskState);
        result.ShouldHaveValidationErrorFor(x => x.AssignedBy);
        result.ShouldHaveValidationErrorFor(x => x.AssignedTo);
    }

    private static WorklistTaskUpdateDto BuildValidModel()
    {
        return new WorklistTaskUpdateDto
        {
            WorklistTaskType = "Test task type",
            Description = "Test task description",
            DueDate = DateOnly.FromDateTime(DateTime.Today.AddDays(7)),
            Id = Guid.NewGuid(),
            WorklistTaskState = "Test task state",
            AssignedTo = "Test assigned to",
            AssignedBy = "Test assigned by"
        };
    }
}

