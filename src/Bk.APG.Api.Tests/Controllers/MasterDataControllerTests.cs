using Bk.APG.Api.Controllers;
using Bk.APG.Business.Dtos;
using Bk.APG.Business.Services;
using Bogus;
using Microsoft.AspNetCore.Mvc;

namespace Bk.APG.Api.Tests.Controllers;

[TestFixture]
internal class MasterDataControllerTests
{
    private readonly IMasterDataService _masterDataService = Substitute.For<IMasterDataService>();
    private readonly ICantonService _cantonService = Substitute.For<ICantonService>();
    private readonly ICountryService _countryService = Substitute.For<ICountryService>();

    private MasterDataController _controller = null!;

    [SetUp]
    public void SetUp()
    {
        _controller = new MasterDataController(_masterDataService, _cantonService, _countryService);
    }

    [TearDown]
    public void TearDown()
    {
        _masterDataService.ClearSubstitute();
    }

    [Test]
    public async Task GetMasterData_WhenCalled_ShouldReturnMasterData()
    {
        _masterDataService.GetLanguages().Returns([]);
        _cantonService.GetAll().Returns([]);
        _masterDataService.GetGenders().Returns([]);
        _masterDataService.GetSalutations().Returns([]);
        _masterDataService.GetInterestCommittees().Returns([]);
        _masterDataService.GetInterestFunctions().Returns([]);
        _masterDataService.GetInterestLegalForms().Returns([]);
        _masterDataService.GetLegalForms().Returns([]);
        _masterDataService.GetLevels().Returns([]);
        _masterDataService.GetDepartments().Returns([]);
        _masterDataService.GetOffices().Returns([]);
        _masterDataService.GetCommitteeTypes().Returns([]);
        _masterDataService.GetTerms().Returns([]);
        _masterDataService.GetTermDates().Returns([]);
        _masterDataService.GetElectionTypes().Returns([]);
        _masterDataService.GetElectionOffices().Returns([]);
        _masterDataService.GetFunctions().Returns([]);
        _masterDataService.GetMembershipAdditions().Returns([]);
        _masterDataService.GetAppointmentDecisionLinkTypes().Returns([]);
        _masterDataService.GetAppointmentDecisionTypes().Returns([]);
        _masterDataService.GetLegislaturePeriods().Returns([]);
        _masterDataService.GetCouncils().Returns([]);
        _masterDataService.GetWorklistTaskTypes().Returns([]);
        _masterDataService.GetWorklistTaskStates().Returns([]);

        var result = await _controller.GetMasterData();

        await _masterDataService.Received(1).GetLanguages();
        await _cantonService.Received(1).GetAll();
        await _masterDataService.Received(1).GetGenders();
        await _masterDataService.Received(1).GetSalutations();
        await _masterDataService.Received(1).GetInterestCommittees();
        await _masterDataService.Received(1).GetInterestFunctions();
        await _masterDataService.Received(1).GetInterestLegalForms();
        await _masterDataService.Received(1).GetLegalForms();
        await _masterDataService.Received(1).GetLevels();
        await _masterDataService.Received(1).GetDepartments();
        await _masterDataService.Received(1).GetOffices();
        await _masterDataService.Received(1).GetCommitteeTypes();
        await _masterDataService.Received(1).GetTerms();
        await _masterDataService.Received(1).GetTermDates();
        await _masterDataService.Received(1).GetElectionTypes();
        await _masterDataService.Received(1).GetElectionOffices();
        await _masterDataService.Received(1).GetFunctions();
        await _masterDataService.Received(1).GetMembershipAdditions();
        await _masterDataService.Received(1).GetAppointmentDecisionLinkTypes();
        await _masterDataService.Received(1).GetAppointmentDecisionTypes();
        await _masterDataService.Received(1).GetLegislaturePeriods();
        await _masterDataService.Received(1).GetCouncils();
        await _masterDataService.Received(1).GetWorklistTaskTypes();
        await _masterDataService.Received(1).GetWorklistTaskStates();

        Assert.That(result, Is.Not.Null);
        var response = result as OkObjectResult;
        Assert.That(response, Is.Not.Null);
        Assert.That(response!.StatusCode, Is.EqualTo(200));
    }

    [Test]
    public async Task GetOfficesByName_WhenCalled_ShouldCallServiceAndReturnResult()
    {
        var officeName = "searchText";
        var office = new Faker<OfficeDto>().Generate();
        _masterDataService.GetOfficesByName(officeName).Returns([office]);

        var result = await _controller.GetOfficesByName(officeName);

        await _masterDataService.Received(1).GetOfficesByName(officeName);
        Assert.That(result, Is.Not.Null);

        var response = result as OkObjectResult;
        Assert.That(response, Is.Not.Null);
        Assert.That(response.StatusCode, Is.EqualTo(200));
    }
}
