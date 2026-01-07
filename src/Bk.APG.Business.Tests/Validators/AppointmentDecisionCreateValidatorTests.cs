using Bk.APG.Business.Dtos;
using Bk.APG.Business.Models;
using Bk.APG.Business.Validators;
using FluentValidation.TestHelper;
using Microsoft.AspNetCore.Http;

namespace Bk.APG.Business.Tests.Validators;

[TestFixture]
internal class AppointmentDecisionCreateValidatorTests
{
    private AppointmentDecisionCreateValidator _validator;

    [SetUp]
    public void SetUp()
    {
        _validator = new AppointmentDecisionCreateValidator();
    }

    [Test]
    public void Validate_WithValidModel_ShouldNotHaveAnyValidationErrors()
    {
        var model = BuildValidModel();

        var result = _validator.TestValidate(model);
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Test]
    public void Validate_WithInvalidMaxLength_ShouldAddErrorForFields()
    {
        var model = BuildValidModel();
        model.Link = new string('a', 252);
        model.Text = new string('a', 2001);

        var result = _validator.TestValidate(model);

        result.ShouldHaveValidationErrorFor(x => x.Link);
        result.ShouldHaveValidationErrorFor(x => x.Text);
    }

    [Test]
    public void Validate_WithEmptyAppointmentDecisionTypeId_ShouldThrowBusinessValidationException()
    {
        var stream = new MemoryStream("Hello world!"u8.ToArray());
        IFormFile file = new FormFile(stream, 0, stream.Length, "id_from_form1", "fileName1");

        var model = new AppointmentDecisionCreateDto
        {
            CommitteeId = Guid.NewGuid(),
            AppointmentDecisionTypeId = null,
            AppointmentDecisionDate = DateOnly.FromDateTime(DateTime.UtcNow),
            Text = "Text",
            Link = "Link",
            Documents = [new DocumentStorageModificationDto { DisplayName = "displayName1", IsOriginal = true, File = file, LanguageId = new Guid(Language.GermanId) }]
        };
        var result = _validator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.AppointmentDecisionTypeId);
    }

    [Test]
    public void Validate_WithEmptyCommitteeId_ShouldThrowBusinessValidationException()
    {
        var stream = new MemoryStream("Hello world!"u8.ToArray());
        IFormFile file = new FormFile(stream, 0, stream.Length, "id_from_form1", "fileName1");

        var model = new AppointmentDecisionCreateDto
        {
            CommitteeId = Guid.Empty,
            AppointmentDecisionTypeId = Guid.NewGuid(),
            AppointmentDecisionDate = DateOnly.FromDateTime(DateTime.UtcNow),
            Text = "Text",
            Link = "Link",
            Documents = [new DocumentStorageModificationDto { DisplayName = "displayName1", IsOriginal = true, File = file, LanguageId = new Guid(Language.GermanId) }]
        };
        var result = _validator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.CommitteeId);
    }

    private static AppointmentDecisionCreateDto BuildValidModel()
    {
        var stream = new MemoryStream("Hello world!"u8.ToArray());
        IFormFile file = new FormFile(stream, 0, stream.Length, "id_from_form1", "fileName1");

        return new AppointmentDecisionCreateDto
        {
            CommitteeId = Guid.NewGuid(),
            AppointmentDecisionTypeId = Guid.NewGuid(),
            AppointmentDecisionDate = DateOnly.FromDateTime(DateTime.UtcNow),
            Text = "Text",
            Link = "Link",
            Documents = [new DocumentStorageModificationDto { DisplayName = "displayName1", IsOriginal = true, File = file, LanguageId = new Guid(Language.GermanId) }]
        };
    }
}
