using Bk.APG.Business.Dtos;
using Bk.APG.Business.Validators;
using FluentValidation.TestHelper;

namespace Bk.APG.Business.Tests.Validators;

[TestFixture]
internal class PersonUpdateValidatorTests
{
    private PersonUpdateValidator _validator;

    [SetUp]
    public void SetUp()
    {
        _validator = new PersonUpdateValidator();
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
        model.Surname = string.Empty;
        model.GivenName = string.Empty;
        model.LanguageId = Guid.Empty;
        model.CorrespondenceLanguageId = Guid.Empty;
        model.GenderId = Guid.Empty;
        model.PrivateAddress = null;
        model.OfficeAddress = null;

        var result = _validator.TestValidate(model);

        result.ShouldHaveValidationErrorFor(x => x.Surname);
        result.ShouldHaveValidationErrorFor(x => x.GivenName);
        result.ShouldHaveValidationErrorFor(x => x.LanguageId);
        result.ShouldHaveValidationErrorFor(x => x.CorrespondenceLanguageId);
        result.ShouldHaveValidationErrorFor(x => x.GenderId);
        result.ShouldHaveValidationErrorFor(x => x.PrivateAddress);
        result.ShouldHaveValidationErrorFor(x => x.OfficeAddress);
    }

    [Test]
    public void Validate_WithInvalidMaxLength_ShouldAddErrorForFields()
    {
        var model = BuildValidModel();
        model.Surname = new string('a', 151);
        model.GivenName = new string('a', 151);

        var result = _validator.TestValidate(model);

        result.ShouldHaveValidationErrorFor(x => x.Surname);
        result.ShouldHaveValidationErrorFor(x => x.GivenName);
    }

    [TestCase(1899)]
    [TestCase(2101)]
    public void Validate_WithInvalidBirthYear_ShouldAddErrorForBirthYear(int birthYear)
    {
        var model = BuildValidModel();
        model.BirthYear = birthYear;

        var result = _validator.TestValidate(model);

        result.ShouldHaveValidationErrorFor(x => x.BirthYear);
    }

    [Test]
    public void Validate_WhenFederalAssemblyAndEmptyLegislaturePeriodIds_ShouldHaveError()
    {
        var model = BuildValidModel();
        model.FederalAssembly = true;
        model.LegislaturePeriodIds.Clear();

        var result = _validator.TestValidate(model);

        result.ShouldHaveValidationErrorFor(x => x.LegislaturePeriodIds);
    }

    [Test]
    public void Validate_WhenFederalAssemblyAndNotEmptyLegislaturePeriodIds_ShouldNotHaveError()
    {
        var model = BuildValidModel();
        model.FederalAssembly = false;

        var result = _validator.TestValidate(model);

        result.ShouldNotHaveValidationErrorFor(x => x.LegislaturePeriodIds);
    }

    [Test]
    public void Validate_WhenNotFederalAssemblyAndEmptyLegislaturePeriodIds_ShouldNotHaveError()
    {
        var model = BuildValidModel();
        model.FederalAssembly = false;
        model.LegislaturePeriodIds.Clear();

        var result = _validator.TestValidate(model);

        result.ShouldNotHaveValidationErrorFor(x => x.LegislaturePeriodIds);
    }

    [Test]
    public void Validate_WithMaskedAddress_ShouldNotHaveErrorForMissingAddress()
    {
        var model = BuildValidModel();
        model.MaskAddress = true;
        model.PrivateAddress = null;
        model.OfficeAddress = null;

        var result = _validator.TestValidate(model);

        result.ShouldNotHaveValidationErrorFor(x => x.PrivateAddress);
        result.ShouldNotHaveValidationErrorFor(x => x.OfficeAddress);
    }

    private static PersonUpdateDto BuildValidModel()
    {
        return new PersonUpdateDto
        {
            Id = Guid.NewGuid(),
            Surname = "surname",
            BirthYear = 2000,
            CorrespondenceLanguageId = Guid.NewGuid(),
            GenderId = Guid.NewGuid(),
            GivenName = "givenName",
            LanguageId = Guid.NewGuid(),
            SalutationId = Guid.NewGuid(),
            MaskAddress = false,
            PrivateAddress = new AddressUpdateDto(),
            OfficeAddress = new AddressUpdateDto(),
            FederalAssembly = true,
            CouncilId = Guid.NewGuid(),
            LegislaturePeriodIds = [Guid.NewGuid()],
            RowVersion = 666
        };
    }
}
