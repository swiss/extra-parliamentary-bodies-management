using Bk.APG.Business.Dtos;
using Bk.APG.Business.Validators;
using FluentValidation.TestHelper;

namespace Bk.APG.Business.Tests.Validators;

[TestFixture]
internal class CommitteeCreateValidatorTests
{
    private CommitteeCreateValidator _validator;

    [SetUp]
    public void SetUp()
    {
        _validator = new CommitteeCreateValidator();
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
        model.DescriptionGerman = string.Empty;
        model.DescriptionFrench = string.Empty;
        model.DescriptionItalian = string.Empty;
        model.DescriptionRomansh = string.Empty;
        model.DepartmentId = Guid.Empty;
        model.OfficeId = Guid.Empty;
        model.LevelId = Guid.Empty;
        model.CommitteeTypeId = Guid.Empty;
        model.TermOfOfficeId = Guid.Empty;

        var result = _validator.TestValidate(model);

        result.ShouldHaveValidationErrorFor(x => x.DescriptionGerman);
        result.ShouldHaveValidationErrorFor(x => x.DescriptionFrench);
        result.ShouldHaveValidationErrorFor(x => x.DescriptionItalian);
        result.ShouldHaveValidationErrorFor(x => x.DescriptionRomansh);
        result.ShouldHaveValidationErrorFor(x => x.DepartmentId);
        result.ShouldHaveValidationErrorFor(x => x.OfficeId);
        result.ShouldHaveValidationErrorFor(x => x.LevelId);
        result.ShouldHaveValidationErrorFor(x => x.CommitteeTypeId);
        result.ShouldHaveValidationErrorFor(x => x.TermOfOfficeId);
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
        model.LinkHomepageGerman = new string('a', count: 501);
        model.LinkHomepageFrench = new string('a', count: 501);
        model.LinkHomepageItalian = new string('a', count: 501);
        model.LinkHomepageRomansh = new string('a', count: 501);

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
    public void Validate_WithInvalidEndDate_ShouldAddErrorForField()
    {
        var model = BuildValidModel();
        model.BeginDate = DateOnly.FromDateTime(DateTime.Now);
        model.EndDate = DateOnly.FromDateTime(DateTime.Now.AddDays(-1));

        var result = _validator.TestValidate(model);

        result.ShouldHaveValidationErrorFor(x => x.EndDate);
    }

    [Test]
    public void Validate_WithMinimalMembersBelowZero_ShouldAddErrorForField()
    {
        var model = BuildValidModel();
        model.MinimalMembers = -1;

        var result = _validator.TestValidate(model);

        result.ShouldHaveValidationErrorFor(x => x.MinimalMembers);
    }

    [TestCase(null, -1)]
    [TestCase(2, 1)]
    public void Validate_WithInvalidMaximalMembers_ShouldAddErrorForField(int? min, int? max)
    {
        var model = BuildValidModel();
        model.MinimalMembers = min;
        model.MaximalMembers = max;

        var result = _validator.TestValidate(model);

        result.ShouldHaveValidationErrorFor(x => x.MaximalMembers);
    }

    [TestCase(-1)]
    [TestCase(100)]
    public void Validate_WithVacanciesInGeneralElectionAndInvalidValue_ShouldAddErrorForField(int vacancies)
    {
        var model = BuildValidModel();
        model.VacanciesInGeneralElection = vacancies;

        var result = _validator.TestValidate(model);

        result.ShouldHaveValidationErrorFor(x => x.VacanciesInGeneralElection);
    }

    private static CommitteeCreateDto BuildValidModel()
    {
        return new CommitteeCreateDto
        {
            DescriptionGerman = "Foo",
            DescriptionFrench = "Foo",
            DescriptionItalian = "Foo",
            DescriptionRomansh = "Foo",
            DepartmentId = Guid.NewGuid(),
            LevelId = Guid.NewGuid(),
            OfficeId = Guid.NewGuid(),
            CommitteeTypeId = Guid.NewGuid(),
            TermOfOfficeId = Guid.NewGuid(),
            TermOfOfficeDateId = Guid.NewGuid(),
            BeginDate = DateOnly.FromDateTime(DateTime.Now),
            EndDate = DateOnly.FromDateTime(DateTime.Now.AddDays(4)),
            MinimalMembers = 1,
            MaximalMembers = 2,
            SupervisionDuty = true,
            FederalLawEstablishment = true,
            MarketOrientated = true
        };
    }
}
