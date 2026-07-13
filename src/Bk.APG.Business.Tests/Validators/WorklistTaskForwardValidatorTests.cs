using Bk.APG.Business.Dtos;
using Bk.APG.Business.Validators;
using FluentValidation.TestHelper;

namespace Bk.APG.Business.Tests.Validators;

[TestFixture]
internal class WorklistTaskForwardValidatorTests
{
    private WorklistTaskForwardValidator _validator;

    [SetUp]
    public void SetUp()
    {
        _validator = new WorklistTaskForwardValidator();
    }

    [Test]
    public void Validate_WithValidModel_ShouldNotHaveAnyValidationErrors()
    {
        var model = BuildValidModel();

        var result = _validator.TestValidate(model);
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Test]
    public void Validate_WithValidDescriptions_ShouldNotAddErrorForDescriptions()
    {
        var model = BuildValidModel();
        model.CandidateListDescription = "Valid candidate list description";
        model.CommitteeDescription = "Valid committee description";

        var result = _validator.TestValidate(model);

        result.ShouldNotHaveValidationErrorFor(x => x.CandidateListDescription);
        result.ShouldNotHaveValidationErrorFor(x => x.CommitteeDescription);
    }

    private static WorklistTaskForwardDto BuildValidModel()
    {
        return new WorklistTaskForwardDto
        {
            CommitteeDueDate = DateOnly.FromDateTime(DateTime.Today).AddDays(14),
            CandidateListDueDate = DateOnly.FromDateTime(DateTime.Today).AddDays(7),
            CandidateListDescription = "Candidate list task description",
            CommitteeDescription = "Committee task description"
        };
    }
}

