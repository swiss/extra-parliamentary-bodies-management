using Bk.APG.Business.Dtos;
using Bk.APG.Business.Models;
using Bk.APG.Business.Repositories;
using Bk.APG.Business.Services;
using Bk.APG.CrossCutting.Tests.Builders;
using Microsoft.Extensions.Logging.Abstractions;

namespace Bk.APG.Business.Tests.Services;

[TestFixture]
internal class GeneralMeasureServiceTests
{
    private GeneralMeasureService _service = null!;

    private readonly IGeneralMeasureRepository _generalMeasureRepository = Substitute.For<IGeneralMeasureRepository>();
    private readonly IAuthorizationService _authorizationService = Substitute.For<IAuthorizationService>();
    private readonly IMasterDataRepository _masterDataRepository = Substitute.For<IMasterDataRepository>();

    [SetUp]
    public void Setup()
    {
        _service = new GeneralMeasureService(_generalMeasureRepository, _authorizationService, _masterDataRepository, NullLogger<GeneralMeasureService>.Instance);
    }

    [TearDown]
    public void TearDown()
    {
        _generalMeasureRepository.ClearSubstitute();
        _authorizationService.ClearSubstitute();
        _masterDataRepository.ClearSubstitute();
    }

    [Test]
    public async Task GetGeneralMeasures_WhenUserIsDepartment_ShouldReturnOnlyDepartmentData()
    {
        var departmentId = Guid.NewGuid();
        var department = new DepartmentBuilder().WithId(departmentId).WithGermanText("Test Department").Build();

        var genderMeasure = new GeneralGenderMeasureBuilder()
            .WithDepartmentId(departmentId)
            .WithDescription("Gender Description")
            .Build();
        var languageMeasure = new GeneralLanguageMeasureBuilder()
            .WithDepartmentId(departmentId)
            .WithDescription("Language Description")
            .Build();

        _authorizationService.IsDepartment.Returns(true);
        _authorizationService.IsAdmin.Returns(false);
        _authorizationService.GetDepartment().Returns(department);
        _generalMeasureRepository.GetGeneralGenderMeasures().Returns(new List<GeneralGenderMeasure> { genderMeasure });
        _generalMeasureRepository.GetGeneralLanguageMeasures().Returns(new List<GeneralLanguageMeasure> { languageMeasure });
        _masterDataRepository.GetDepartments().Returns(new List<Department> { department });

        var result = (await _service.GetGeneralMeasures()).ToList();

        Assert.That(result, Has.Count.EqualTo(1));
        Assert.That(result.First().DepartmentId, Is.EqualTo(departmentId));
        Assert.That(result.First().JustificationGenders, Is.EqualTo("Gender Description"));
        Assert.That(result.First().JustificationLanguages, Is.EqualTo("Language Description"));
    }

    [Test]
    public async Task GetGeneralMeasures_WhenUserIsAdmin_ShouldReturnAllDepartments()
    {
        var dept1Id = Guid.NewGuid();
        var dept2Id = Guid.NewGuid();
        var department1 = new DepartmentBuilder().WithId(dept1Id).WithGermanText("Department 1").Build();
        var department2 = new DepartmentBuilder().WithId(dept2Id).WithGermanText("Department 2").Build();

        var genderMeasure1 = new GeneralGenderMeasureBuilder()
            .WithDepartmentId(dept1Id)
            .WithDescription("Gender 1")
            .Build();
        var languageMeasure2 = new GeneralLanguageMeasureBuilder()
            .WithDepartmentId(dept2Id)
            .WithDescription("Language 2")
            .Build();

        _authorizationService.IsDepartment.Returns(false);
        _authorizationService.IsAdmin.Returns(true);
        _generalMeasureRepository.GetGeneralGenderMeasures().Returns(new List<GeneralGenderMeasure> { genderMeasure1 });
        _generalMeasureRepository.GetGeneralLanguageMeasures().Returns(new List<GeneralLanguageMeasure> { languageMeasure2 });
        _masterDataRepository.GetDepartments().Returns(new List<Department> { department1, department2 });

        var result = (await _service.GetGeneralMeasures()).ToList();

        Assert.That(result, Has.Count.EqualTo(2));
        Assert.That(result.Any(r => r.DepartmentId == dept1Id && r.JustificationGenders == "Gender 1"), Is.True);
        Assert.That(result.Any(r => r.DepartmentId == dept2Id && r.JustificationLanguages == "Language 2"), Is.True);
    }

    [Test]
    public async Task GetGeneralMeasures_WhenDepartmentHasNoMeasures_ShouldReturnNullJustifications()
    {
        var departmentId = Guid.NewGuid();
        var department = new DepartmentBuilder().WithId(departmentId).WithGermanText("Test Department").Build();

        _authorizationService.IsDepartment.Returns(false);
        _authorizationService.IsAdmin.Returns(true);
        _generalMeasureRepository.GetGeneralGenderMeasures().Returns(Array.Empty<GeneralGenderMeasure>());
        _generalMeasureRepository.GetGeneralLanguageMeasures().Returns(Array.Empty<GeneralLanguageMeasure>());
        _masterDataRepository.GetDepartments().Returns(new List<Department> { department });

        var result = (await _service.GetGeneralMeasures()).ToList();

        Assert.That(result, Has.Count.EqualTo(1));
        Assert.That(result.First().JustificationGenders, Is.Null);
        Assert.That(result.First().JustificationLanguages, Is.Null);
    }

    [Test]
    public async Task GetGeneralMeasures_WhenMultipleDepartmentsWithMixedMeasures_ShouldCombineCorrectly()
    {
        var dept1Id = Guid.NewGuid();
        var dept2Id = Guid.NewGuid();
        var dept3Id = Guid.NewGuid();
        var department1 = new DepartmentBuilder().WithId(dept1Id).Build();
        var department2 = new DepartmentBuilder().WithId(dept2Id).Build();
        var department3 = new DepartmentBuilder().WithId(dept3Id).Build();

        var genderMeasure1 = new GeneralGenderMeasureBuilder().WithDepartmentId(dept1Id).WithDescription("G1").Build();
        var genderMeasure3 = new GeneralGenderMeasureBuilder().WithDepartmentId(dept3Id).WithDescription("G3").Build();
        var languageMeasure1 = new GeneralLanguageMeasureBuilder().WithDepartmentId(dept1Id).WithDescription("L1").Build();
        var languageMeasure2 = new GeneralLanguageMeasureBuilder().WithDepartmentId(dept2Id).WithDescription("L2").Build();

        _authorizationService.IsDepartment.Returns(false);
        _authorizationService.IsAdmin.Returns(true);
        _generalMeasureRepository.GetGeneralGenderMeasures().Returns(new List<GeneralGenderMeasure> { genderMeasure1, genderMeasure3 });
        _generalMeasureRepository.GetGeneralLanguageMeasures().Returns(new List<GeneralLanguageMeasure> { languageMeasure1, languageMeasure2 });
        _masterDataRepository.GetDepartments().Returns(new List<Department> { department1, department2, department3 });

        var result = (await _service.GetGeneralMeasures()).ToList();

        Assert.That(result, Has.Count.EqualTo(3));

        var dept1Result = result.First(r => r.DepartmentId == dept1Id);
        Assert.That(dept1Result.JustificationGenders, Is.EqualTo("G1"));
        Assert.That(dept1Result.JustificationLanguages, Is.EqualTo("L1"));

        var dept2Result = result.First(r => r.DepartmentId == dept2Id);
        Assert.That(dept2Result.JustificationGenders, Is.Null);
        Assert.That(dept2Result.JustificationLanguages, Is.EqualTo("L2"));

        var dept3Result = result.First(r => r.DepartmentId == dept3Id);
        Assert.That(dept3Result.JustificationGenders, Is.EqualTo("G3"));
        Assert.That(dept3Result.JustificationLanguages, Is.Null);
    }

    [Test]
    public async Task AddOrUpdateGeneralMeasure_WhenMeasuresExist_ShouldUpdateThem()
    {
        var departmentId = Guid.NewGuid();
        const string userName = "test.user@example.com";
        var existingGenderMeasure = new GeneralGenderMeasureBuilder()
            .WithDepartmentId(departmentId)
            .WithDescription("Old Gender")
            .Build();
        var existingLanguageMeasure = new GeneralLanguageMeasureBuilder()
            .WithDepartmentId(departmentId)
            .WithDescription("Old Language")
            .Build();
        var updateDto = new GeneralMeasureUpdateDto
        {
            DepartmentId = departmentId,
            JustificationGenders = "Updated Gender",
            JustificationLanguages = "Updated Language"
        };

        _authorizationService.GetCurrentUserName().Returns(userName);
        _generalMeasureRepository.GetGeneralGenderMeasureForUpdate(departmentId).Returns(Task.FromResult<GeneralGenderMeasure?>(existingGenderMeasure));
        _generalMeasureRepository.GetGeneralLanguageMeasureForUpdate(departmentId).Returns(Task.FromResult<GeneralLanguageMeasure?>(existingLanguageMeasure));

        await _service.AddOrUpdateGeneralMeasure(updateDto);

        Assert.Multiple(() =>
        {
            Assert.That(existingGenderMeasure.Description, Is.EqualTo("Updated Gender"));
            Assert.That(existingGenderMeasure.ModifiedBy, Is.EqualTo(userName));
            Assert.That(existingLanguageMeasure.Description, Is.EqualTo("Updated Language"));
            Assert.That(existingLanguageMeasure.ModifiedBy, Is.EqualTo(userName));
        });
        await _generalMeasureRepository.Received(2).CommitChanges();
        await _generalMeasureRepository.DidNotReceive().AddGeneralGenderMeasure(Arg.Any<GeneralGenderMeasure>());
        await _generalMeasureRepository.DidNotReceive().AddGeneralLanguageMeasure(Arg.Any<GeneralLanguageMeasure>());
    }

    [Test]
    public async Task AddOrUpdateGeneralMeasure_WhenJustificationsAreNull_ShouldSetEmptyString()
    {
        var departmentId = Guid.NewGuid();
        const string userName = "test.user@example.com";
        var updateDto = new GeneralMeasureUpdateDto
        {
            DepartmentId = departmentId,
            JustificationGenders = null,
            JustificationLanguages = null
        };

        _authorizationService.GetCurrentUserName().Returns(userName);
        _generalMeasureRepository.GetGeneralGenderMeasureForUpdate(departmentId).Returns(Task.FromResult<GeneralGenderMeasure?>(null));
        _generalMeasureRepository.GetGeneralLanguageMeasureForUpdate(departmentId).Returns(Task.FromResult<GeneralLanguageMeasure?>(null));

        await _service.AddOrUpdateGeneralMeasure(updateDto);

        await _generalMeasureRepository.Received(1).AddGeneralGenderMeasure(Arg.Is<GeneralGenderMeasure>(
            m => m.Description == string.Empty));
        await _generalMeasureRepository.Received(1).AddGeneralLanguageMeasure(Arg.Is<GeneralLanguageMeasure>(
            m => m.Description == string.Empty));
    }

    [Test]
    public async Task AddOrUpdateGeneralMeasure_WhenOnlyGenderMeasureExists_ShouldUpdateGenderAndAddLanguage()
    {
        var departmentId = Guid.NewGuid();
        const string userName = "test.user@example.com";
        var existingGenderMeasure = new GeneralGenderMeasureBuilder()
            .WithDepartmentId(departmentId)
            .Build();
        var updateDto = new GeneralMeasureUpdateDto
        {
            DepartmentId = departmentId,
            JustificationGenders = "Updated Gender",
            JustificationLanguages = "New Language"
        };

        _authorizationService.GetCurrentUserName().Returns(userName);
        _generalMeasureRepository.GetGeneralGenderMeasureForUpdate(departmentId).Returns(Task.FromResult<GeneralGenderMeasure?>(existingGenderMeasure));
        _generalMeasureRepository.GetGeneralLanguageMeasureForUpdate(departmentId).Returns(Task.FromResult<GeneralLanguageMeasure?>(null));

        await _service.AddOrUpdateGeneralMeasure(updateDto);

        Assert.That(existingGenderMeasure.Description, Is.EqualTo("Updated Gender"));
        await _generalMeasureRepository.Received(1).CommitChanges();
        await _generalMeasureRepository.Received(1).AddGeneralLanguageMeasure(Arg.Is<GeneralLanguageMeasure>(
            m => m.DepartmentId == departmentId && m.Description == "New Language"));
    }

    [Test]
    public async Task AddOrUpdateGeneralMeasure_WhenOnlyLanguageMeasureExists_ShouldAddGenderAndUpdateLanguage()
    {
        var departmentId = Guid.NewGuid();
        const string userName = "test.user@example.com";
        var existingLanguageMeasure = new GeneralLanguageMeasureBuilder()
            .WithDepartmentId(departmentId)
            .Build();
        var updateDto = new GeneralMeasureUpdateDto
        {
            DepartmentId = departmentId,
            JustificationGenders = "New Gender",
            JustificationLanguages = "Updated Language"
        };

        _authorizationService.IsDepartment.Returns(false);
        _authorizationService.GetCurrentUserName().Returns(userName);
        _generalMeasureRepository.GetGeneralGenderMeasureForUpdate(departmentId).Returns(Task.FromResult<GeneralGenderMeasure?>(null));
        _generalMeasureRepository.GetGeneralLanguageMeasureForUpdate(departmentId).Returns(Task.FromResult<GeneralLanguageMeasure?>(existingLanguageMeasure));

        await _service.AddOrUpdateGeneralMeasure(updateDto);

        Assert.That(existingLanguageMeasure.Description, Is.EqualTo("Updated Language"));
        await _generalMeasureRepository.Received(1).CommitChanges();
        await _generalMeasureRepository.Received(1).AddGeneralGenderMeasure(Arg.Is<GeneralGenderMeasure>(
            m => m.DepartmentId == departmentId && m.Description == "New Gender"));
    }

    [Test]
    public async Task AddOrUpdateGeneralMeasure_WhenAdding_ShouldSetCreatedAndModifiedFields()
    {
        var departmentId = Guid.NewGuid();
        const string userName = "test.user@example.com";
        var beforeTime = DateTime.UtcNow.AddSeconds(-1);
        var updateDto = new GeneralMeasureUpdateDto
        {
            DepartmentId = departmentId,
            JustificationGenders = "Gender",
            JustificationLanguages = "Language"
        };

        _authorizationService.GetCurrentUserName().Returns(userName);
        _generalMeasureRepository.GetGeneralGenderMeasureForUpdate(departmentId).Returns(Task.FromResult<GeneralGenderMeasure?>(null));
        _generalMeasureRepository.GetGeneralLanguageMeasureForUpdate(departmentId).Returns(Task.FromResult<GeneralLanguageMeasure?>(null));

        await _service.AddOrUpdateGeneralMeasure(updateDto);
        var afterTime = DateTime.UtcNow.AddSeconds(1);

        await _generalMeasureRepository.Received(1).AddGeneralGenderMeasure(Arg.Is<GeneralGenderMeasure>(m =>
            m.CreatedBy == userName &&
            m.ModifiedBy == userName &&
            m.Created >= beforeTime && m.Created <= afterTime &&
            m.Modified >= beforeTime && m.Modified <= afterTime));

        await _generalMeasureRepository.Received(1).AddGeneralLanguageMeasure(Arg.Is<GeneralLanguageMeasure>(m =>
            m.CreatedBy == userName &&
            m.ModifiedBy == userName &&
            m.Created >= beforeTime && m.Created <= afterTime &&
            m.Modified >= beforeTime && m.Modified <= afterTime));
    }
}
