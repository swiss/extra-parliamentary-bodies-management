using System.Globalization;
using Bk.APG.Business.Dtos;
using Bk.APG.Business.Models;
using Bk.APG.Business.Repositories;
using Bk.APG.Business.Services;
using Bk.APG.CrossCutting.Tests.Builders;
using IAuthorizationService = Bk.APG.Business.Services.IAuthorizationService;

namespace Bk.APG.Business.Tests.Services;

[TestFixture]
internal class MasterDataServiceTests
{
    private MasterDataService _service = null!;

    private readonly IMasterDataRepository _masterDataRepository = Substitute.For<IMasterDataRepository>();
    private readonly ICultureService _cultureService = Substitute.For<ICultureService>();
    private readonly ITermOfOfficeDateService _termOfOfficeDateService = Substitute.For<ITermOfOfficeDateService>();
    private readonly IAuthorizationService _authorizationService = Substitute.For<IAuthorizationService>();

    [SetUp]
    public void Setup()
    {
        _service = new MasterDataService(_masterDataRepository, _cultureService, _termOfOfficeDateService, _authorizationService);

        _masterDataRepository.GetSalutations().Returns(
        [
            new SalutationBuilder().WithGender(Guid.Parse("8663f178-6309-4ce9-a2a4-e6f4220e4c47")).Build(),
            new SalutationBuilder().WithGender(Guid.Parse("8663f178-6309-4ce9-a2a4-e6f4220e4c47")).Build(),
            new SalutationBuilder().WithGender(Guid.Parse("9cfaff1f-ec56-42f0-8367-ab2ccd7f6cb7")).Build()
        ]);
    }

    [TearDown]
    public void TearDown()
    {
        _masterDataRepository.ClearSubstitute();
        _cultureService.ClearSubstitute();
    }

    [Test]
    public async Task GetLanguages_WhenCalled_ShouldCallRepository()
    {
        var result = (await _service.GetLanguages()).ToList();

        await _masterDataRepository.Received(1).GetLanguages();
        Assert.That(result, Is.Not.Null);
        Assert.That(result, Is.Empty);
    }

    [Test]
    public async Task GetGenders_WhenCalled_ShouldCallRepository()
    {
        var genders = (await _service.GetGenders()).ToList();

        await _masterDataRepository.Received(1).GetGenders();
        Assert.That(genders, Is.Not.Null);
        Assert.That(genders, Is.Empty);
    }

    [Test]
    public async Task GetGenders_WhenCalled_ShouldCallServiceAndReturnSortedResult()
    {
        var gender1 = new GenderBuilder().Build();
        var gender2 = new GenderBuilder().Build();
        var gender3 = new GenderBuilder().Build();

        gender1.Sort = 0;
        gender1.TextDe = "male";
        gender2.Sort = 0;
        gender2.TextDe = "female";
        gender3.Sort = 1;
        gender3.TextDe = "other";

        var genderList = new List<Gender>
        {
            gender1,
            gender2,
            gender3
        };

        _masterDataRepository.GetGenders().Returns(genderList);
        var result = (await _service.GetGenders()).ToList();

        await _masterDataRepository.Received(1).GetGenders();
        Assert.That(result, Is.Not.Null);
        Assert.That(result, Has.Count.EqualTo(3));

        Assert.Multiple(() =>
        {
            Assert.That(result.First().Text, Is.EqualTo("female"));
            Assert.That(result.Last().Text, Is.EqualTo("other"));

            Assert.That(result, Is.Ordered.By("Sort").Then.By("Text"));
        });
    }

    [Test]
    public async Task GetSalutations_WhenCalled_ShouldCallRepository()
    {
        var salutations = (await _service.GetSalutations()).ToList();

        await _masterDataRepository.Received(1).GetSalutations();
        Assert.That(salutations, Is.Not.Null);
        Assert.That(salutations, Has.Count.EqualTo(3));
    }

    [Test]
    public async Task GetSalutations_WhenCalled_ShouldCallServiceAndReturnSortedResult()
    {
        var salutation1 = new SalutationBuilder().Build();
        var salutation2 = new SalutationBuilder().Build();
        var salutation3 = new SalutationBuilder().Build();

        salutation1.Sort = 0;
        salutation1.TextDe = "Herr";
        salutation2.Sort = 0;
        salutation2.TextDe = "Frau";
        salutation3.Sort = 1;
        salutation3.TextDe = "";

        var salutationList = new List<Salutation>
        {
            salutation1,
            salutation2,
            salutation3
        };

        _masterDataRepository.GetSalutations().Returns(salutationList);
        var result = (await _service.GetSalutations()).ToList();

        await _masterDataRepository.Received(1).GetSalutations();
        Assert.That(result, Is.Not.Null);
        Assert.That(result, Has.Count.EqualTo(3));

        Assert.Multiple(() =>
        {
            Assert.That(result.First().Text, Is.EqualTo("Frau"));
            Assert.That(result.Last().Text, Is.EqualTo(""));

            Assert.That(result, Is.Ordered.By("Sort").Then.By("Text"));
        });
    }

    [Test]
    public async Task GetInterestCommittees_WhenCalled_ShouldCallRepository()
    {
        var interestCommittees = (await _service.GetInterestCommittees()).ToList();

        await _masterDataRepository.Received(1).GetInterestCommittees();
        Assert.That(interestCommittees, Is.Not.Null);
        Assert.That(interestCommittees, Is.Empty);
    }

    [Test]
    public async Task GetInterestFunctions_WhenCalled_ShouldCallRepository()
    {
        var interestFunctions = (await _service.GetInterestFunctions()).ToList();

        await _masterDataRepository.Received(1).GetInterestFunctions();
        Assert.That(interestFunctions, Is.Not.Null);
        Assert.That(interestFunctions, Is.Empty);
    }

    [Test]
    public async Task GetInterestLegalForms_WhenCalled_ShouldCallRepository()
    {
        var interestLegalForms = (await _service.GetInterestLegalForms()).ToList();

        await _masterDataRepository.Received(1).GetInterestLegalForms();
        Assert.That(interestLegalForms, Is.Not.Null);
        Assert.That(interestLegalForms, Is.Empty);
    }

    [Test]
    public async Task GetLegalForms_WhenCalled_ShouldCallRepository()
    {
        var legalForms = (await _service.GetLegalForms()).ToList();

        await _masterDataRepository.Received(1).GetLegalForms();
        Assert.That(legalForms, Is.Not.Null);
        Assert.That(legalForms, Is.Empty);
    }

    [Test]
    public void GetLegalFormTextByLegalFormId_WhenCalled_ShouldCallRepository()
    {
        var legalFormText = _service.GetLegalFormTextByLegalFormId("abc");

        _masterDataRepository.Received(1).GetLegalFormByLegalFormId("abc");
        Assert.That(legalFormText, Is.Not.Null);
        Assert.That(legalFormText, Is.Empty);
    }

    [Test]
    public void GetLegalFormGuidByLegalFormId_WhenCalled_ShouldCallRepository()
    {
        var legalFormGuid = _service.GetLegalFormGuidByLegalFormId("abc");

        _masterDataRepository.Received(1).GetLegalFormByLegalFormId("abc");
        Assert.That(legalFormGuid, Is.Empty);
    }

    [Test]
    public async Task GetLevels_WhenCalled_ShouldCallRepository()
    {
        _cultureService.GetCurrentUiCulture().Returns(new CultureInfo("de"));
        _masterDataRepository.GetLevels().Returns([new CommitteeLevelBuilder().WithGermanText("Foo").WithGermanDescription("Bar").Build()]);

        var levels = (await _service.GetLevels()).ToList();

        await _masterDataRepository.Received(1).GetLevels();
        Assert.That(levels, Has.Count.EqualTo(1));
        Assert.That(levels, Is.Ordered.By(nameof(LevelDto.Text)));
    }

    [Test]
    public async Task GetDepartments_WhenCalled_ShouldCallRepository()
    {
        _cultureService.GetCurrentUiCulture().Returns(new CultureInfo("de"));
        _masterDataRepository.GetDepartments().Returns([new DepartmentBuilder().WithUri("uri1").WithGermanText("Foo").WithGermanDescription("Bar").Build()]);

        var departments = (await _service.GetDepartments()).ToList();

        await _masterDataRepository.Received(1).GetDepartments();
        Assert.That(departments, Has.Count.EqualTo(1));
        Assert.That(departments, Is.Ordered.By(nameof(DepartmentDto.Text)));
    }

    [Test]
    public async Task GetPermittedDepartments_WhenCalledAsAdmin_ShouldReturnAllData()
    {
        _authorizationService.GetDepartment().Returns((Department?)null);
        _authorizationService.IsAdmin.Returns(true);

        var departmentId = Guid.Parse("08531C40-093D-4C38-A276-0E7A6A1F5026");
        var department1 = new DepartmentBuilder().WithId(departmentId).WithUri("uri1").WithGermanText("Foo").WithGermanDescription("Bar").Build();
        var department2 = new DepartmentBuilder().WithId(Guid.NewGuid()).WithUri("uri2").WithGermanText("Food").WithGermanDescription("Barn").Build();

        _cultureService.GetCurrentUiCulture().Returns(new CultureInfo("de"));
        _masterDataRepository.GetDepartments().Returns([department1, department2]);

        var departments = (await _service.GetPermittedDepartments()).ToList();

        await _masterDataRepository.Received(1).GetDepartments();
        Assert.That(departments, Has.Count.EqualTo(2));
        Assert.That(departments, Is.Ordered.By(nameof(DepartmentDto.Text)));
    }

    [Test]
    public async Task GetPermittedDepartments_WhenCalledAsDepartment_ShouldReturnPermittedData()
    {
        var departmentId = Guid.Parse("08531C40-093D-4C38-A276-0E7A6A1F5026");
        var department1 = new DepartmentBuilder().WithId(departmentId).WithUri("uri1").WithGermanText("Foo").WithGermanDescription("Bar").Build();
        var department2 = new DepartmentBuilder().WithId(Guid.NewGuid()).WithUri("uri2").WithGermanText("Food").WithGermanDescription("Barn").Build();

        _authorizationService.GetDepartment().Returns(department1);
        _authorizationService.IsAdmin.Returns(false);
        _authorizationService.IsDepartment.Returns(true);

        _cultureService.GetCurrentUiCulture().Returns(new CultureInfo("de"));
        _masterDataRepository.GetDepartments().Returns([department1, department2]);

        var departments = (await _service.GetPermittedDepartments()).ToList();

        await _masterDataRepository.Received(1).GetDepartments();
        Assert.That(departments, Has.Count.EqualTo(1));
    }

    [Test]
    public async Task GetOffices_WhenCalled_ShouldCallRepository()
    {
        _cultureService.GetCurrentUiCulture().Returns(new CultureInfo("de"));
        _masterDataRepository.GetOffices().Returns([new OfficeBuilder().WithGermanText("Foo").WithGermanDescription("Bar").Build()]);

        var offices = (await _service.GetOffices()).ToList();

        await _masterDataRepository.Received(1).GetOffices();
        Assert.That(offices, Has.Count.EqualTo(1));
        Assert.That(offices, Is.Ordered.By(nameof(OfficeDto.Text)));
    }

    [Test]
    public async Task GetPermittedOffices_WhenCalledAsAdmin_ShouldReturnAllData()
    {
        _authorizationService.GetDepartment().Returns((Department?)null);
        _authorizationService.IsAdmin.Returns(true);

        var officeId = Guid.Parse("08531C40-093D-4C38-A276-0E7A6A1F5026");
        var office1 = new OfficeBuilder().WithId(officeId).WithUri("uri1").WithGermanText("Foo").WithGermanDescription("Bar").Build();
        var office2 = new OfficeBuilder().WithId(Guid.NewGuid()).WithUri("uri2").WithGermanText("Food").WithGermanDescription("Barn").Build();

        _cultureService.GetCurrentUiCulture().Returns(new CultureInfo("de"));
        _masterDataRepository.GetOffices().Returns([office1, office2]);

        var offices = (await _service.GetPermittedOffices()).ToList();

        await _masterDataRepository.Received(1).GetOffices();
        Assert.That(offices, Has.Count.EqualTo(2));
        Assert.That(offices, Is.Ordered.By(nameof(OfficeDto.Text)));
    }

    [Test]
    public async Task GetPermittedOffices_WhenCalledAsOffice_ShouldReturnPermittedData()
    {
        var officeId = Guid.Parse("08531C40-093D-4C38-A276-0E7A6A1F5026");
        var office1 = new OfficeBuilder().WithId(officeId).WithUri("uri1").WithGermanText("Foo").WithGermanDescription("Bar").Build();
        var office2 = new OfficeBuilder().WithId(Guid.NewGuid()).WithUri("uri2").WithGermanText("Food").WithGermanDescription("Barn").Build();

        _authorizationService.GetOffice().Returns(office1);
        _authorizationService.IsAdmin.Returns(false);
        _authorizationService.IsOffice.Returns(true);

        _cultureService.GetCurrentUiCulture().Returns(new CultureInfo("de"));
        _masterDataRepository.GetOffices().Returns([office1, office2]);

        var offices = (await _service.GetPermittedOffices()).ToList();

        await _masterDataRepository.Received(1).GetOffices();
        Assert.That(offices, Has.Count.EqualTo(1));
    }

    [Test]
    public async Task GetOfficesByName_WhenCalled_ShouldCallRepositoryAndDeliverHitsInAllLanguages()
    {
        _cultureService.GetCurrentUiCulture().Returns(new CultureInfo("de"));
        _masterDataRepository.GetOffices().Returns([
            new OfficeBuilder().WithGermanText("Foo").WithGermanDescription("Bar").WithFrenchText("Bar").WithFrenchDescription("Bar").WithItalianText("Bar").WithItalianDescription("Bar").Build(),
            new OfficeBuilder().WithGermanText("Bar").WithGermanDescription("FOO").WithFrenchText("Bar").WithFrenchDescription("Bar").WithItalianText("Bar").WithItalianDescription("Bar").Build(),
            new OfficeBuilder().WithGermanText("Bar").WithGermanDescription("Bar").WithFrenchText("foo").WithFrenchDescription("Bar").WithItalianText("Bar").WithItalianDescription("Bar").Build(),
            new OfficeBuilder().WithGermanText("Bar").WithGermanDescription("Bar").WithFrenchText("Bar").WithFrenchDescription("FoO").WithItalianText("Bar").WithItalianDescription("Bar").Build(),
            new OfficeBuilder().WithGermanText("Bar").WithGermanDescription("Bar").WithFrenchText("Bar").WithFrenchDescription("Bar").WithItalianText("fOo").WithItalianDescription("Bar").Build(),
            new OfficeBuilder().WithGermanText("Bar").WithGermanDescription("Bar").WithFrenchText("Bar").WithFrenchDescription("Bar").WithItalianText("Bar").WithItalianDescription("fOO").Build(),
            new OfficeBuilder().WithGermanText("Bar").WithGermanDescription("Bar").WithFrenchText("Bar").WithFrenchDescription("Bar").WithItalianText("Bar").WithItalianDescription("Bar").Build(),
        ]);

        var offices = (await _service.GetOfficesByName("Foo")).ToList();

        await _masterDataRepository.Received(1).GetOffices();
        Assert.That(offices, Has.Count.EqualTo(6));
    }

    [Test]
    public async Task GetGeneralSecretariatOffices_WhenCalled_ShouldCallRepository()
    {
        _cultureService.GetCurrentUiCulture().Returns(new CultureInfo("de"));
        _masterDataRepository.GetOffices().Returns([
            new OfficeBuilder().WithGermanText("Foo1").WithGermanDescription("Bar1").WithIsGeneralSecretariat(true).Build(),
            new OfficeBuilder().WithGermanText("Foo2").WithGermanDescription("Bar2").WithIsGeneralSecretariat(false).Build()
        ]);

        var offices = (await _service.GetGeneralSecretariatOffices()).ToList();

        await _masterDataRepository.Received(1).GetOffices();
        Assert.That(offices, Has.Count.EqualTo(1));
    }

    [Test]
    public async Task GetCommitteeTypes_WhenCalled_ShouldCallRepository()
    {
        _cultureService.GetCurrentUiCulture().Returns(new CultureInfo("de"));
        _masterDataRepository.GetCommitteeTypes().Returns([new CommitteeTypeBuilder().WithGermanText("Foo").WithGermanDescription("Bar").Build()]);

        var committeeTypes = (await _service.GetCommitteeTypes()).ToList();

        await _masterDataRepository.Received(1).GetCommitteeTypes();
        Assert.That(committeeTypes, Has.Count.EqualTo(1));
        Assert.That(committeeTypes, Is.Ordered.By(nameof(CommitteeTypeDto.Text)));
    }

    [Test]
    public async Task GetTerms_WhenCalled_ShouldCallRepository()
    {
        _cultureService.GetCurrentUiCulture().Returns(new CultureInfo("de"));
        _masterDataRepository.GetTerms().Returns([new TermOfOfficeBuilder().WithGermanText("Foo").WithGermanDescription("Bar").Build()]);

        var terms = (await _service.GetTerms()).ToList();

        await _masterDataRepository.Received(1).GetTerms();
        Assert.That(terms, Has.Count.EqualTo(1));
        Assert.That(terms, Is.Ordered.By(nameof(TermDto.Text)));
    }

    [Test]
    public async Task GetElectionTypes_WhenCalled_ShouldCallRepository()
    {
        _cultureService.GetCurrentUiCulture().Returns(new CultureInfo("de"));
        _masterDataRepository.GetElectionTypes().Returns([new ElectionTypeBuilder().WithGermanText("Foo").WithGermanDescription("Bar").Build()]);

        var electionTypes = (await _service.GetElectionTypes()).ToList();

        await _masterDataRepository.Received(1).GetElectionTypes();
        Assert.That(electionTypes, Has.Count.EqualTo(1));
        Assert.That(electionTypes, Is.Ordered.By(nameof(ElectionTypeDto.Text)));
    }

    [Test]
    public async Task GetElectionOffices_WhenCalled_ShouldCallRepository()
    {
        _cultureService.GetCurrentUiCulture().Returns(new CultureInfo("de"));
        _masterDataRepository.GetElectionOffices().Returns([new ElectionOfficeBuilder().WithGermanText("Foo").WithGermanDescription("Bar").Build()]);

        var electionOffices = (await _service.GetElectionOffices()).ToList();

        await _masterDataRepository.Received(1).GetElectionOffices();
        Assert.That(electionOffices, Has.Count.EqualTo(1));
        Assert.That(electionOffices, Is.Ordered.By(nameof(ElectionOfficeDto.Text)));
    }

    [Test]
    public async Task GetFunctions_WhenCalled_ShouldCallRepository()
    {
        _cultureService.GetCurrentUiCulture().Returns(new CultureInfo("de"));
        _masterDataRepository.GetFunctions().Returns([new FunctionBuilder().WithGermanText("Foo").WithGermanDescription("Bar").Build()]);

        var functions = (await _service.GetFunctions()).ToList();

        await _masterDataRepository.Received(1).GetFunctions();
        Assert.That(functions, Has.Count.EqualTo(1));
        Assert.That(functions, Is.Ordered.By(nameof(FunctionDto.Text)));
    }

    [Test]
    public async Task GetContactPointGuidFromContactPointUri_WhenCalled_ShouldCallRepository()
    {
        var guid = Guid.NewGuid();
        var uri = "my.uri.com";

        _cultureService.GetCurrentUiCulture().Returns(new CultureInfo("de"));
        _masterDataRepository.GetContactPointTypes().Returns([new ContactPointTypeBuilder().WithId(guid).WithUri(uri).Build()]);

        var contactPointTypeGuid = await _service.GetContactPointGuidFromContactPointUri(uri);

        await _masterDataRepository.Received(1).GetContactPointTypes();
        Assert.That(contactPointTypeGuid, Is.EqualTo(guid));
    }

    [Test]
    public async Task GetMembershipAdditions_WhenCalled_ShouldCallRepository()
    {
        _cultureService.GetCurrentUiCulture().Returns(new CultureInfo("de"));
        _masterDataRepository.GetMembershipAdditions().Returns([new MembershipAdditionBuilder().WithGermanText("Foo").WithGermanDescription("Bar").Build()]);

        var membershipAddition = (await _service.GetMembershipAdditions()).ToList();

        await _masterDataRepository.Received(1).GetMembershipAdditions();
        Assert.That(membershipAddition, Has.Count.EqualTo(1));
        Assert.That(membershipAddition, Is.Ordered.By(nameof(MembershipAdditionDto.Text)));
    }

    [Test]
    public async Task GetAppointmentDecisionTypes_WhenCalled_ShouldCallRepository()
    {
        _cultureService.GetCurrentUiCulture().Returns(new CultureInfo("de"));
        _masterDataRepository.GetAppointmentDecisionTypes().Returns([new AppointmentDecisionTypeBuilder().WithUri("uri").Build(), new AppointmentDecisionTypeBuilder().WithUri("uri2").Build()]);

        var appointmentDecisionTypes = (await _service.GetAppointmentDecisionTypes()).ToList();

        await _masterDataRepository.Received(1).GetAppointmentDecisionTypes();
        Assert.That(appointmentDecisionTypes, Has.Count.EqualTo(2));
        Assert.That(appointmentDecisionTypes, Is.Ordered.By(nameof(AppointmentDecisionTypeDto.Text)));
    }

    [Test]
    public async Task GetAppointmentDecisionLinkTypes_WhenCalled_ShouldCallRepository()
    {
        _cultureService.GetCurrentUiCulture().Returns(new CultureInfo("de"));
        _masterDataRepository.GetAppointmentDecisionLinkTypes().Returns([new AppointmentDecisionLinkTypeBuilder().WithUri("uri").Build(), new AppointmentDecisionLinkTypeBuilder().WithUri("uri2").Build()]);

        var appointmentDecisionLinkTypes = (await _service.GetAppointmentDecisionLinkTypes()).ToList();

        await _masterDataRepository.Received(1).GetAppointmentDecisionLinkTypes();
        Assert.That(appointmentDecisionLinkTypes, Has.Count.EqualTo(2));
        Assert.That(appointmentDecisionLinkTypes, Is.Ordered.By(nameof(AppointmentDecisionLinkTypeDto.Text)));
    }

    [Test]
    public async Task GetLegislaturePeriods_WhenCalled_ShouldCallRepository()
    {
        _masterDataRepository.GetLegislaturePeriods().Returns([new LegislaturePeriodBuilder().WithText("Foo").Build()]);

        var legislaturePeriods = (await _service.GetLegislaturePeriods()).ToList();

        await _masterDataRepository.Received(1).GetLegislaturePeriods();
        Assert.That(legislaturePeriods, Has.Count.EqualTo(1));
        Assert.That(legislaturePeriods, Is.Ordered.By(nameof(LegislaturePeriodDto.Text)));
    }

    [Test]
    public async Task GetCouncils_WhenCalled_ShouldCallRepository()
    {
        _masterDataRepository.GetCouncils().Returns([new CouncilBuilder().WithText("Ständerat").Build()]);

        var councils = (await _service.GetCouncils()).ToList();

        await _masterDataRepository.Received(1).GetCouncils();
        Assert.That(councils, Has.Count.EqualTo(1));
        Assert.That(councils, Is.Ordered.By(nameof(Council.Sort)));
    }

    [Test]
    public async Task GetWorklistTaskTypes_WhenCalled_ShouldCallRepository()
    {
        _masterDataRepository.GetWorklistTaskTypes().Returns([new WorklistTaskTypeBuilder().WithGermanText("Type1").Build()]);
        _termOfOfficeDateService.CheckForRunningGeneralElection().Returns(true);
        var worklistTaskTypes = (await _service.GetWorklistTaskTypes()).ToList();
        await _masterDataRepository.Received(1).GetWorklistTaskTypes();
        await _termOfOfficeDateService.Received(1).CheckForRunningGeneralElection();
        Assert.That(worklistTaskTypes, Has.Count.EqualTo(1));
        Assert.That(worklistTaskTypes, Is.Ordered.By(nameof(WorklistTaskTypeDto.Text)));
    }

    [Test]
    public async Task GetWorklistTaskStates_WhenCalled_ShouldCallRepository()
    {
        _masterDataRepository.GetWorklistTaskStates().Returns([new WorklistTaskStateBuilder().WithGermanText("State1").Build()]);
        var worklistTaskStates = (await _service.GetWorklistTaskStates()).ToList();
        await _masterDataRepository.Received(1).GetWorklistTaskStates();
        Assert.That(worklistTaskStates, Has.Count.EqualTo(1));
        Assert.That(worklistTaskStates, Is.Ordered.By(nameof(WorklistTaskStateDto.Text)));
    }
}
