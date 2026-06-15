using Bk.APG.Api.Controllers;
using Bk.APG.Business.Dtos;
using Bk.APG.Business.Services;
using Bk.APG.CrossCutting;
using Bogus;
using Microsoft.AspNetCore.Mvc;

namespace Bk.APG.Api.Tests.Controllers;

[TestFixture]
internal class WorklistTasksControllerTests
{
    private readonly IWorklistTaskService _worklistTaskService = Substitute.For<IWorklistTaskService>();
    private readonly IGeneralElectionService _generalElectionService = Substitute.For<IGeneralElectionService>();

    private WorklistTasksController _controller = null!;

    [SetUp]
    public void SetUp()
    {
        _controller = new WorklistTasksController(_worklistTaskService, _generalElectionService);
    }

    [TearDown]
    public void TearDown()
    {
        _worklistTaskService.ClearSubstitute();
    }

    [Test]
    public async Task GetAll_WhenCalled_ShouldCallServiceAndReturnResult()
    {
        var worklistTasks = new PagedResultDto<WorklistTaskDto>
        {
            Index = 0,
            Total = 100,
            Items = new Faker<WorklistTaskDto>().Generate(10)
        };
        _worklistTaskService
            .GetWorklistTasks(Arg.Any<PagingParametersDto>(), Arg.Any<WorklistFilterParametersDto>(), Arg.Any<string?>(), Arg.Any<SortDirection?>())
            .Returns(worklistTasks);

        var response = await _controller.GetAll(new PagingParametersDto(), new WorklistFilterParametersDto(), new SortParametersDto());

        Assert.That(response, Is.Not.Null);
        var responseObject = response as OkObjectResult;

        Assert.That(responseObject, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(responseObject.StatusCode, Is.EqualTo(200));
            Assert.That(responseObject.Value, Is.EqualTo(worklistTasks));
        });
    }

    [Test]
    public async Task CreateWorklistTask_WhenCalled_ShouldCallServiceAndReturnResult()
    {
        var createDto = new Faker<WorklistTaskCreateDto>().Generate();
        var result = await _controller.CreateWorklistTask(createDto) as OkResult;
        await _worklistTaskService.Received(1).CreateWorklistTaskByAdmin(createDto);
        Assert.That(result, Is.Not.Null);
        Assert.That(result.StatusCode, Is.EqualTo(200));
    }

    [Test]
    public async Task GetById_WhenCalled_ShouldCallServiceAndReturnResult()
    {
        var worklistTask = new Faker<WorklistTaskUpdateDto>().Generate();
        var id = Guid.NewGuid();
        _worklistTaskService.GetWorklistTaskForUpdate(id).Returns(worklistTask);

        var response = await _controller.GetByIdForUpdate(id);

        Assert.That(response, Is.Not.Null);
        var responseObject = response as OkObjectResult;
        Assert.That(responseObject, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(responseObject.StatusCode, Is.EqualTo(200));
            Assert.That(responseObject.Value, Is.EqualTo(worklistTask));
        });
    }

    [Test]
    public async Task Update_WhenCalled_ShouldCallServiceAndReturnResult()
    {
        var updateDto = new Faker<WorklistTaskUpdateDto>().Generate();
        var id = updateDto.Id;
        _worklistTaskService.UpdateWorklistTask(id, updateDto).Returns(updateDto);

        var response = await _controller.Update(id, updateDto);

        Assert.That(response, Is.Not.Null);
        var responseObject = response as OkObjectResult;
        Assert.That(responseObject, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(responseObject.StatusCode, Is.EqualTo(200));
            Assert.That(responseObject.Value, Is.EqualTo(updateDto));
        });
    }
}
