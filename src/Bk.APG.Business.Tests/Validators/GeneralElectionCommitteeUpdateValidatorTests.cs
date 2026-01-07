using Bk.APG.Business.Dtos;
using Bk.APG.Business.Validators;
using FluentValidation.TestHelper;

namespace Bk.APG.Business.Tests.Validators;

[TestFixture]
internal class GeneralElectionCommitteeUpdateValidatorTests
{
    private GeneralElectionCommitteeUpdateValidator _validator;

    [SetUp]
    public void SetUp()
    {
        _validator = new GeneralElectionCommitteeUpdateValidator();
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
        var model = new GeneralElectionCommitteeUpdateDto
        {
            Id = Guid.Empty,
            CommitteeId = Guid.Empty,
            DescriptionGerman = string.Empty,
            DescriptionFrench = string.Empty,
            DescriptionItalian = string.Empty,
            DescriptionRomansh = string.Empty,
            LevelId = Guid.Empty,
            OfficeId = Guid.Empty,
            DepartmentId = Guid.Empty,
            CommitteeTypeId = Guid.Empty,
            TermOfOfficeId = Guid.Empty,
            BeginDate = default,
            AdditionalAuthorityMembers = false,
            RowVersion = 1
        };

        var result = _validator.TestValidate(model);

        result.ShouldHaveValidationErrorFor(x => x.DescriptionGerman);
        result.ShouldHaveValidationErrorFor(x => x.DescriptionFrench);
        result.ShouldHaveValidationErrorFor(x => x.DescriptionItalian);
        result.ShouldHaveValidationErrorFor(x => x.DescriptionRomansh);
        result.ShouldHaveValidationErrorFor(x => x.LevelId);
        result.ShouldHaveValidationErrorFor(x => x.OfficeId);
        result.ShouldHaveValidationErrorFor(x => x.DepartmentId);
        result.ShouldHaveValidationErrorFor(x => x.CommitteeTypeId);
        result.ShouldHaveValidationErrorFor(x => x.TermOfOfficeId);
        result.ShouldHaveValidationErrorFor(x => x.BeginDate);
    }

    [Test]
    public void Validate_WithInvalidMaxLength_ShouldAddErrorForFields()
    {
        var model = BuildValidModel();
        model.DescriptionGerman = new string('a', 501);
        model.DescriptionFrench = new string('a', 501);
        model.DescriptionItalian = new string('a', 501);
        model.DescriptionRomansh = new string('a', 501);
        model.LegalBase = new string('a', 2001);
        model.LinkAuthorityWebsite = new string('a', 501);
        model.LinkHomepageGerman = new string('a', 501);
        model.LinkHomepageFrench = new string('a', 501);
        model.LinkHomepageItalian = new string('a', 501);
        model.LinkHomepageRomansh = new string('a', 501);

        var result = _validator.TestValidate(model);

        result.ShouldHaveValidationErrorFor(x => x.DescriptionGerman);
        result.ShouldHaveValidationErrorFor(x => x.DescriptionFrench);
        result.ShouldHaveValidationErrorFor(x => x.DescriptionItalian);
        result.ShouldHaveValidationErrorFor(x => x.DescriptionRomansh);
        result.ShouldHaveValidationErrorFor(x => x.LegalBase);
        result.ShouldHaveValidationErrorFor(x => x.LinkAuthorityWebsite);
        result.ShouldHaveValidationErrorFor(x => x.LinkHomepageGerman);
        result.ShouldHaveValidationErrorFor(x => x.LinkHomepageFrench);
        result.ShouldHaveValidationErrorFor(x => x.LinkHomepageItalian);
        result.ShouldHaveValidationErrorFor(x => x.LinkHomepageRomansh);
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
    public void Validate_WithFederalLawEstablishmentTrueAndNoLegalBase_ShouldAddErrorForLegalBase()
    {
        var model = new GeneralElectionCommitteeUpdateDto
        {
            Id = Guid.NewGuid(),
            CommitteeId = Guid.NewGuid(),
            DescriptionGerman = "German Description",
            DescriptionFrench = "French Description",
            DescriptionItalian = "Italian Description",
            DescriptionRomansh = "Romansh Description",
            LevelId = Guid.NewGuid(),
            OfficeId = Guid.NewGuid(),
            DepartmentId = Guid.NewGuid(),
            CommitteeTypeId = Guid.NewGuid(),
            TermOfOfficeId = Guid.NewGuid(),
            BeginDate = DateOnly.FromDateTime(DateTime.Today),
            FederalLawEstablishment = true,
            LegalBase = string.Empty,
            AdditionalAuthorityMembers = false,
            RowVersion = 1
        };

        var result = _validator.TestValidate(model);

        result.ShouldHaveValidationErrorFor(x => x.LegalBase);
    }

    [Test]
    public void Validate_WithFederalLawEstablishmentTrueAndLegalBase_ShouldNotAddErrorForLegalBase()
    {
        var model = new GeneralElectionCommitteeUpdateDto
        {
            Id = Guid.NewGuid(),
            CommitteeId = Guid.NewGuid(),
            DescriptionGerman = "German Description",
            DescriptionFrench = "French Description",
            DescriptionItalian = "Italian Description",
            DescriptionRomansh = "Romansh Description",
            LevelId = Guid.NewGuid(),
            OfficeId = Guid.NewGuid(),
            DepartmentId = Guid.NewGuid(),
            CommitteeTypeId = Guid.NewGuid(),
            TermOfOfficeId = Guid.NewGuid(),
            BeginDate = DateOnly.FromDateTime(DateTime.Today),
            FederalLawEstablishment = true,
            LegalBase = "Legal base text",
            AdditionalAuthorityMembers = false,
            RowVersion = 1
        };

        var result = _validator.TestValidate(model);

        result.ShouldNotHaveValidationErrorFor(x => x.LegalBase);
    }

    [Test]
    public void Validate_WithFederalLawEstablishmentFalseAndNoLegalBase_ShouldNotAddErrorForLegalBase()
    {
        var model = new GeneralElectionCommitteeUpdateDto
        {
            Id = Guid.NewGuid(),
            CommitteeId = Guid.NewGuid(),
            DescriptionGerman = "German Description",
            DescriptionFrench = "French Description",
            DescriptionItalian = "Italian Description",
            DescriptionRomansh = "Romansh Description",
            LevelId = Guid.NewGuid(),
            OfficeId = Guid.NewGuid(),
            DepartmentId = Guid.NewGuid(),
            CommitteeTypeId = Guid.NewGuid(),
            TermOfOfficeId = Guid.NewGuid(),
            BeginDate = DateOnly.FromDateTime(DateTime.Today),
            FederalLawEstablishment = false,
            LegalBase = string.Empty,
            AdditionalAuthorityMembers = false,
            RowVersion = 1
        };

        var result = _validator.TestValidate(model);

        result.ShouldNotHaveValidationErrorFor(x => x.LegalBase);
    }

    [TestCase(-1)]
    public void Validate_WithInvalidMinimalMembers_ShouldAddError(int members)
    {
        var model = BuildValidModel();
        model.MinimalMembers = members;

        var result = _validator.TestValidate(model);

        result.ShouldHaveValidationErrorFor(x => x.MinimalMembers);
    }

    [TestCase(0)]
    [TestCase(10)]
    public void Validate_WithValidMinimalMembers_ShouldNotAddError(int members)
    {
        var model = BuildValidModel();
        model.MinimalMembers = members;

        var result = _validator.TestValidate(model);

        result.ShouldNotHaveValidationErrorFor(x => x.MinimalMembers);
    }

    [Test]
    public void Validate_WithMaximalMembersLessThanMinimalMembers_ShouldAddErrorForMaximalMembers()
    {
        var model = BuildValidModel();
        model.MinimalMembers = 10;
        model.MaximalMembers = 5;

        var result = _validator.TestValidate(model);

        result.ShouldHaveValidationErrorFor(x => x.MaximalMembers);
    }

    [Test]
    public void Validate_WithMaximalMembersGreaterThanMinimalMembers_ShouldNotAddErrorForMaximalMembers()
    {
        var model = BuildValidModel();
        model.MinimalMembers = 5;
        model.MaximalMembers = 10;

        var result = _validator.TestValidate(model);

        result.ShouldNotHaveValidationErrorFor(x => x.MaximalMembers);
    }

    [Test]
    public void Validate_WithMaximalMembersEqualsMinimalMembers_ShouldNotAddErrorForMaximalMembers()
    {
        var model = BuildValidModel();
        model.MinimalMembers = 10;
        model.MaximalMembers = 10;

        var result = _validator.TestValidate(model);

        result.ShouldNotHaveValidationErrorFor(x => x.MaximalMembers);
    }

    private static GeneralElectionCommitteeUpdateDto BuildValidModel()
    {
        return new GeneralElectionCommitteeUpdateDto
        {
            Id = Guid.NewGuid(),
            CommitteeId = Guid.NewGuid(),
            DescriptionGerman = "German Description",
            DescriptionFrench = "French Description",
            DescriptionItalian = "Italian Description",
            DescriptionRomansh = "Romansh Description",
            LevelId = Guid.NewGuid(),
            OfficeId = Guid.NewGuid(),
            DepartmentId = Guid.NewGuid(),
            CommitteeTypeId = Guid.NewGuid(),
            TermOfOfficeId = Guid.NewGuid(),
            BeginDate = DateOnly.FromDateTime(DateTime.Today),
            EndDate = DateOnly.FromDateTime(DateTime.Today.AddYears(4)),
            FederalLawEstablishment = false,
            LegalBase = "Legal base text",
            MinimalMembers = 5,
            MaximalMembers = 20,
            LinkAuthorityWebsite = "https://example.com",
            LinkHomepageGerman = "https://example.com/de",
            LinkHomepageFrench = "https://example.com/fr",
            LinkHomepageItalian = "https://example.com/it",
            LinkHomepageRomansh = "https://example.com/rm",
            AdditionalAuthorityMembers = false,
            RowVersion = 1
        };
    }
}

