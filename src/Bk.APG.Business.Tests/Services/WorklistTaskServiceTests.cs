using Bk.APG.Business.Dtos;
using Bk.APG.Business.Models;
using Bk.APG.Business.Repositories;
using Bk.APG.Business.Services;
using Bk.APG.CrossCutting;
using Bk.APG.CrossCutting.Tests.Builders;
using Bogus;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace Bk.APG.Business.Tests.Services;

[TestFixture]
internal class WorklistTaskServiceTests
{
    private readonly IWorklistTaskRepository _worklistTaskRepository = Substitute.For<IWorklistTaskRepository>();
    private readonly IAuthorizationService _authorizationService = Substitute.For<IAuthorizationService>();
    private readonly ICommitteeRepository _committeeRepository = Substitute.For<ICommitteeRepository>();
    private readonly ILogger<WorklistTaskService> _logger = NullLogger<WorklistTaskService>.Instance;
    private readonly IEiamAssignmentRepository _eiamAssignmentRepository = Substitute.For<IEiamAssignmentRepository>();
    private readonly IGeneralElectionCommitteeRepository _generalElectionCommitteeRepository = Substitute.For<IGeneralElectionCommitteeRepository>();

    private WorklistTaskService _service = null!;

    [SetUp]
    public void SetUp()
    {
        _service = new WorklistTaskService(
            _worklistTaskRepository,
            _authorizationService,
            _committeeRepository,
            _eiamAssignmentRepository,
            _generalElectionCommitteeRepository,
            _logger);
    }

    [TearDown]
    public void TearDown()
    {
        _worklistTaskRepository.ClearSubstitute();
        _authorizationService.ClearSubstitute();
        _committeeRepository.ClearSubstitute();
        _eiamAssignmentRepository.ClearSubstitute();
        _generalElectionCommitteeRepository.ClearSubstitute();
    }

    [Test]
    public async Task GetWorklistTasks_ShouldReturnResult()
    {
        var pagingDto = new PagingParametersDto { PageIndex = 26, PageSize = 2 };
        var filtersDto = new WorklistFilterParametersDto();
        const string sortKey = "assignedToRole";
        const SortDirection sortDirection = SortDirection.Asc;
        var resultFromRepository = new PagedResult<WorklistTask>
        {
            Total = 276,
            Index = 25,
            Items =
            [
                new WorklistTaskBuilder().Build(),
                new WorklistTaskBuilder().Build()
            ]
        };
        _worklistTaskRepository
            .GetAll(Arg.Any<PagingParameters>(), Arg.Any<WorklistFilterParameters?>(), Arg.Any<string?>(), Arg.Any<SortDirection?>())
            .Returns(resultFromRepository);

        var worklistTasks = await _service.GetWorklistTasks(pagingDto, filtersDto, sortKey, sortDirection);

        await _worklistTaskRepository.Received(1).GetAll(
            Arg.Is<PagingParameters>(p => p.PageIndex == pagingDto.PageIndex && p.PageSize == pagingDto.PageSize),
            Arg.Any<WorklistFilterParameters?>(),
            Arg.Is(sortKey),
            Arg.Is(sortDirection));

        Assert.That(worklistTasks, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(worklistTasks.Index, Is.EqualTo(resultFromRepository.Index));
            Assert.That(worklistTasks.Total, Is.EqualTo(resultFromRepository.Total));
            Assert.That(worklistTasks.Items.Count(), Is.EqualTo(resultFromRepository.Items.Count()));
        });
    }

    [Test]
    public async Task GetWorklistTasks_ForObserver_ShouldReturnEmptyResult()
    {
        _authorizationService.IsObserver.Returns(true);

        var worklistTasks = await _service.GetWorklistTasks(null!, null!, null!, null!);

        await _worklistTaskRepository.DidNotReceiveWithAnyArgs().GetAll(Arg.Any<PagingParameters>(), Arg.Any<WorklistFilterParameters>(), Arg.Any<string>(), Arg.Any<SortDirection>());

        Assert.That(worklistTasks, Is.Not.Null);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(worklistTasks.Index, Is.Zero);
            Assert.That(worklistTasks.Total, Is.Zero);
            Assert.That(worklistTasks.Items, Is.Empty);
        }
    }

    [Test]
    public async Task CreateWorklistTask_WithDto_ShouldReturnResult()
    {
        _authorizationService.GetCurrentUserName().Returns("userName");
        var createDto = new Faker<WorklistTaskCreateDto>().RuleForType(typeof(Guid?), x => new Guid?(x.Random.Guid()));
        _worklistTaskRepository.Create(Arg.Any<WorklistTask>()).Returns(new WorklistTaskBuilder().Build());

        await _service.CreateWorklistTaskByAdmin(createDto);

        _authorizationService.Received(1).GetCurrentUserName();
        await _worklistTaskRepository.Received(1).Create(Arg.Any<WorklistTask>());
    }

    [Test]
    public async Task GetWorklistTaskForUpdate_ShouldReturnResult()
    {
        var worklistTask = new WorklistTaskBuilder().Build();
        var id = Guid.NewGuid();
        _worklistTaskRepository.GetByIdForUpdate(id).Returns(worklistTask);

        var result = await _service.GetWorklistTaskForUpdate(id);

        await _worklistTaskRepository.Received(1).GetByIdForUpdate(id);
        Assert.That(result, Is.Not.Null);
    }

    [Test]
    public async Task UpdateWorklistTask_WithDto_ShouldReturnResult()
    {
        var updateDto = new Faker<WorklistTaskUpdateDto>().Generate();
        var id = updateDto.Id;
        var worklistTask = new WorklistTaskBuilder().Build();
        _worklistTaskRepository.GetByIdForUpdate(id).Returns(worklistTask);

        await _service.UpdateWorklistTask(id, updateDto);

        await _worklistTaskRepository.Received(1).GetByIdForUpdate(id);
        await _worklistTaskRepository.Received(1).Update(Arg.Any<WorklistTask>());
    }

    [Test]
    public async Task RemoveAllGeneralElectionTasks_ShouldDeleteTasksAndLog()
    {
        var termOfOfficeDateId = Guid.NewGuid();
        var tasks = new List<WorklistTask>
        {
            new WorklistTaskBuilder().Build(),
            new WorklistTaskBuilder().Build()
        };
        _worklistTaskRepository.GetByTermOfOfficeDateId(termOfOfficeDateId).Returns(tasks);

        await _service.RemoveAllGeneralElectionTasks(termOfOfficeDateId);

        await _worklistTaskRepository.Received(1).GetByTermOfOfficeDateId(termOfOfficeDateId);
        _worklistTaskRepository.Received(1).DeleteRange(tasks);
    }

    [Test]
    public async Task ForwardWorklistTask_WithGeneralElectionDispatchForSmallDepartment_ShouldCreateTasksAndCompleteParent()
    {
        var worklistTask = new WorklistTaskBuilder()
            .WithWorklistTaskTypeId(WorklistTaskType.GeneralElectionDispatch)
            .WithWorklistTaskStateId(WorklistTaskState.Active)
            .WithAssignedTo(new EiamAssignmentBuilder().WithRole(Role.Department).Build())
            .WithDepartment(new DepartmentBuilder().WithIsBigDepartment(false).Build())
            .Build();
        var forwardDto = new WorklistTaskForwardDto
        {
            CandidateListDueDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(10)),
            CandidateListDescription = "Test description",
            CommitteeDueDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(10)),
            CommitteeDescription = "Test"
        };
        var committees = new List<Committee>
        {
            new CommitteeBuilder()
                .WithGeneralElectionCommittee(new GeneralElectionCommitteeBuilder().Build())
                .Build()
        };

        _worklistTaskRepository.GetByIdForForward(worklistTask.Id).Returns(worklistTask);
        _authorizationService.GetCurrentExternalId().Returns(worklistTask.AssignedTo!.ExternalId);
        _committeeRepository.GetForGeneralElectionByDepartmentId(worklistTask.DepartmentId!.Value).Returns(committees);
        _authorizationService.GetCurrentUserName().Returns("testUser");

        await _service.ForwardWorklistTask(worklistTask.Id, forwardDto);

        await _worklistTaskRepository.Received(1).CreateRange(Arg.Is<List<WorklistTask>>(x => x.Count == 2));
        await _worklistTaskRepository.Received(1).Update(Arg.Is<WorklistTask>(x => x.WorklistTaskStateId == WorklistTaskState.Completed));
        await _generalElectionCommitteeRepository.Received(1).GetByDepartmentId(worklistTask.DepartmentId!.Value);
        await _generalElectionCommitteeRepository.Received(1).CommitChanges();
    }

    [Test]
    public async Task ForwardWorklistTask_WithGeneralElectionDispatchForBigDepartmentToOffice_ShouldCreateTasksAndCompleteParent()
    {
        var officeAssignment = new EiamAssignmentBuilder().WithRole(Role.Office).Build();
        var departmentAssignment = new EiamAssignmentBuilder()
            .WithRole(Role.Department)
            .WithChildren([officeAssignment])
            .Build();
        var worklistTask = new WorklistTaskBuilder()
            .WithWorklistTaskTypeId(WorklistTaskType.GeneralElectionDispatch)
            .WithWorklistTaskStateId(WorklistTaskState.Active)
            .WithAssignedTo(departmentAssignment)
            .WithDepartment(new DepartmentBuilder().WithIsBigDepartment(true).Build())
            .Build();
        var forwardDto = new WorklistTaskForwardDto
        {
            CandidateListDueDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(10)),
            CandidateListDescription = "Test description",
            CommitteeDueDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(10)),
            CommitteeDescription = "Test"
        };

        _worklistTaskRepository.GetByIdForForward(worklistTask.Id).Returns(worklistTask);
        _authorizationService.GetCurrentExternalId().Returns(worklistTask.AssignedTo!.ExternalId);
        _authorizationService.GetCurrentUserName().Returns("testUser");
        await _service.ForwardWorklistTask(worklistTask.Id, forwardDto);

        await _worklistTaskRepository.Received(1).CreateRange(Arg.Is<List<WorklistTask>>(x => x.Count == 1));
        await _worklistTaskRepository.Received(1).Update(Arg.Is<WorklistTask>(x => x.WorklistTaskStateId == WorklistTaskState.Completed));
        await _generalElectionCommitteeRepository.Received(1).GetByDepartmentId(worklistTask.AssignedTo.DepartmentId!.Value);
        await _generalElectionCommitteeRepository.Received(1).CommitChanges();
    }

    [Test]
    public async Task ForwardWorklistTask_WithGeneralElectionDispatchForBigDepartmentFromOffice_ShouldCreateTasksAndCompleteParent()
    {
        var officeAssignment = new EiamAssignmentBuilder().WithRole(Role.Office).Build();
        var worklistTask = new WorklistTaskBuilder()
            .WithWorklistTaskTypeId(WorklistTaskType.GeneralElectionDispatch)
            .WithWorklistTaskStateId(WorklistTaskState.Active)
            .WithParentWorklistTask(new WorklistTaskBuilder().Build())
            .WithAssignedTo(officeAssignment)
            .WithDepartment(new DepartmentBuilder().WithIsBigDepartment(true).Build())
            .WithOffice(new OfficeBuilder().WithDepartment(new DepartmentBuilder().WithIsBigDepartment(true).Build()).Build())
            .Build();
        var forwardDto = new WorklistTaskForwardDto
        {
            CandidateListDueDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(10)),
            CandidateListDescription = "Test description",
            CommitteeDueDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(10)),
            CommitteeDescription = "Test"
        };
        var committees = new List<Committee>
        {
            new CommitteeBuilder()
                .WithGeneralElectionCommittee(new GeneralElectionCommitteeBuilder().Build())
                .Build()
        };

        _worklistTaskRepository.GetByIdForForward(worklistTask.Id).Returns(worklistTask);
        _authorizationService.GetCurrentExternalId().Returns(worklistTask.AssignedTo!.ExternalId);
        _committeeRepository.GetForGeneralElectionByOfficeId(Arg.Any<Guid>()).Returns(committees);
        _authorizationService.GetCurrentUserName().Returns("testUser");

        await _service.ForwardWorklistTask(worklistTask.Id, forwardDto);

        await _worklistTaskRepository.Received(1).CreateRange(Arg.Is<List<WorklistTask>>(x => x.Count == 3));
        await _worklistTaskRepository.Received(1).Update(Arg.Is<WorklistTask>(x => x.WorklistTaskStateId == WorklistTaskState.Completed));
        await _generalElectionCommitteeRepository.Received(1).GetByOfficeId(worklistTask.OfficeId!.Value);
        await _generalElectionCommitteeRepository.Received(1).CommitChanges();
    }

    [Test]
    public void ForwardWorklistTask_WithUnsupportedTaskType_ShouldThrowNotSupportedException()
    {
        var worklistTask = new WorklistTaskBuilder()
            .WithWorklistTaskTypeId(Guid.NewGuid())
            .Build();
        var forwardDto = new Faker<WorklistTaskForwardDto>().Generate();

        _worklistTaskRepository.GetByIdForForward(worklistTask.Id).Returns(worklistTask);
        _authorizationService.GetCurrentExternalId().Returns(worklistTask.AssignedTo!.ExternalId);

        Assert.ThrowsAsync<NotSupportedException>(async () => await _service.ForwardWorklistTask(worklistTask.Id, forwardDto));
    }

    [Test]
    public void ForwardWorklistTask_WhenCannotBeForwarded_ShouldThrowNotSupportedException()
    {
        var worklistTask = new WorklistTaskBuilder()
            .WithWorklistTaskTypeId(WorklistTaskType.GeneralElectionDispatch)
            .Build();
        var forwardDto = new Faker<WorklistTaskForwardDto>().Generate();

        _worklistTaskRepository.GetByIdForForward(worklistTask.Id).Returns(worklistTask);
        _authorizationService.GetCurrentExternalId().Returns("externalId");

        Assert.ThrowsAsync<NotSupportedException>(async () => await _service.ForwardWorklistTask(worklistTask.Id, forwardDto));
    }

    [Test]
    public async Task CreateWorklistTasksForSingleCommitteeInBigDepartment_WithDto_ShouldReturnResult()
    {
        _authorizationService.GetCurrentUserName().Returns("userName");

        var department = new DepartmentBuilder().WithIsBigDepartment(true).Build();
        var geCommittee = new GeneralElectionCommitteeBuilder().Build();
        var parentTask = new WorklistTaskBuilder().Build();
        var worklistTask = new WorklistTaskBuilder().WithParentWorklistTask(parentTask).Build();
        var committee = new CommitteeBuilder().WithDepartment(department).WithGeneralElectionCommittee(geCommittee).Build();

        await _service.CreateWorklistTasksForSingleCommittee(worklistTask, committee);

        _authorizationService.Received(1).GetCurrentUserName();
        await _worklistTaskRepository.Received(1).CreateRange(Arg.Is<List<WorklistTask>>(x => x.Count == 3));
    }

    [Test]
    public async Task CreateWorklistTasksForSingleCommitteeInSmallDepartment_WithDto_ShouldReturnResult()
    {
        _authorizationService.GetCurrentUserName().Returns("userName");

        var department = new DepartmentBuilder().WithIsBigDepartment(false).Build();
        var geCommittee = new GeneralElectionCommitteeBuilder().Build();
        var parentTask = new WorklistTaskBuilder().Build();
        var worklistTask = new WorklistTaskBuilder().WithParentWorklistTask(parentTask).Build();
        var committee = new CommitteeBuilder().WithDepartment(department).WithGeneralElectionCommittee(geCommittee).Build();

        await _service.CreateWorklistTasksForSingleCommittee(worklistTask, committee);

        _authorizationService.Received(1).GetCurrentUserName();
        await _worklistTaskRepository.Received(1).CreateRange(Arg.Is<List<WorklistTask>>(x => x.Count == 2));
    }
}
