using Bk.APG.Api.Controllers;
using Bk.APG.Business.Dtos;
using Bk.APG.Business.Services;
using Bogus;
using Microsoft.AspNetCore.Mvc;

namespace Bk.APG.Api.Tests.Controllers;

[TestFixture]
internal class AppointmentDecisionsControllerTests
{
    private readonly IAppointmentDecisionService _appointmentDecisionService = Substitute.For<IAppointmentDecisionService>();
    private readonly IDocumentService _documentService = Substitute.For<IDocumentService>();

    private AppointmentDecisionsController _controller = null!;

    [SetUp]
    public void SetUp()
    {
        _controller = new AppointmentDecisionsController(_appointmentDecisionService, _documentService);
    }

    [TearDown]
    public void TearDown()
    {
        _appointmentDecisionService.ClearSubstitute();
    }

    [Test]
    public async Task GetAllByCommitteeId_WhenCalled_ShouldCallServiceAndReturnResult()
    {
        var appointmentDecisionListDto = new AppointmentDecisionListDto { Id = Guid.NewGuid() };

        var appointmentDecisions = new List<AppointmentDecisionListDto>
        {
            appointmentDecisionListDto
        };

        _appointmentDecisionService
            .GetAppointmentDecisionListByCommitteeId(Arg.Any<Guid>())
            .Returns(appointmentDecisions);

        var response = await _controller.GetAllByCommitteeId(Guid.NewGuid());

        Assert.That(response, Is.Not.Null);
        var responseObject = response as OkObjectResult;

        Assert.That(responseObject, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(responseObject.StatusCode, Is.EqualTo(200));
            Assert.That(responseObject.Value, Is.EqualTo(appointmentDecisions));
        });
    }

    [Test]
    public async Task CreateAppointmentDecision_WhenCalled_ShouldCallServiceAndReturnResult()
    {
        var createDto = new Faker<AppointmentDecisionCreateDto>().Generate();

        var result = await _controller.CreateAppointmentDecision(createDto) as OkObjectResult;

        await _appointmentDecisionService.Received(1).CreateAppointmentDecision(createDto);
        Assert.That(result, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(result.StatusCode, Is.EqualTo(200));
        });
    }

    [Test]
    public void GetEmpty_WhenCalled_ShouldCallServiceAndReturnResult()
    {
        var response = _controller.GetEmpty();

        _appointmentDecisionService.ReceivedWithAnyArgs().GetEmpty();

        Assert.That(response, Is.Not.Null);
        var responseObject = response as OkObjectResult;

        Assert.That(responseObject, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(responseObject.StatusCode, Is.EqualTo(200));
        });
    }

    [Test]
    public async Task GetAppointmentDecisionForUpdate_WhenCalled_ShouldCallServiceAndReturnResult()
    {
        var appointmentDecisionUpdateDto = new AppointmentDecisionUpdateDto { Id = Guid.NewGuid() };

        _appointmentDecisionService
            .GetByIdForUpdate(Arg.Any<Guid>())
            .Returns(appointmentDecisionUpdateDto);

        var response = await _controller.GetAppointmentDecisionForUpdate(Guid.NewGuid());

        Assert.That(response, Is.Not.Null);
        var responseObject = response as OkObjectResult;

        Assert.That(responseObject, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(responseObject.StatusCode, Is.EqualTo(200));
            Assert.That(responseObject.Value, Is.EqualTo(appointmentDecisionUpdateDto));
        });
    }

    [Test]
    public async Task UpdateAppointmentDecision_WhenCalledWithInvalidId_ShouldReturnBadRequest()
    {
        var updateDto = new Faker<AppointmentDecisionUpdateDto>().Generate();
        var id = Guid.NewGuid();

        var result = await _controller.UpdateAppointmentDecision(id, updateDto) as BadRequestResult;

        await _appointmentDecisionService.DidNotReceiveWithAnyArgs().UpdateAppointmentDecision(id, updateDto);
        Assert.That(result, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(result.StatusCode, Is.EqualTo(400));
        });
    }

    [Test]
    public async Task UpdateAppointmentDecision_WhenCalled_ShouldCallServiceAndReturnResult()
    {
        var updateDto = new Faker<AppointmentDecisionUpdateDto>().Generate();

        var result = await _controller.UpdateAppointmentDecision(updateDto.Id, updateDto) as OkObjectResult;

        await _appointmentDecisionService.Received(1).UpdateAppointmentDecision(updateDto.Id, updateDto);
        Assert.That(result, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(result.StatusCode, Is.EqualTo(200));
        });
    }

    [Test]
    public async Task DeleteAppointmentDecision_WhenCalled_ShouldCallServiceAndReturnResult()
    {
        var id = Guid.NewGuid();

        var result = await _controller.DeleteAppointmentDecision(id) as OkResult;

        await _appointmentDecisionService.Received(1).DeleteAppointmentDecision(id);

        Assert.That(result, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(result.StatusCode, Is.EqualTo(200));
        });
    }
}
