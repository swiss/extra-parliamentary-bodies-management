using System.Globalization;
using Bk.APG.Business.Dtos;
using Bk.APG.Business.Models;
using Bk.APG.Business.Repositories;
using Bk.APG.Business.Services;
using Bk.APG.CrossCutting;
using Bk.APG.CrossCutting.Exception;
using Bk.APG.CrossCutting.Tests.Builders;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace Bk.APG.Business.Tests.Services;

[TestFixture]
public class GeneralElectionCommitteeServiceTests
{
    private GeneralElectionCommitteeService _generalElectionCommitteeService = null!;
    private readonly IGeneralElectionCommitteeRepository _generalElectionCommitteeRepository = Substitute.For<IGeneralElectionCommitteeRepository>();
    private readonly IAuthorizationService _authorizationService = Substitute.For<IAuthorizationService>();
    private readonly ICultureService _cultureService = Substitute.For<ICultureService>();
    private readonly IGeneralMeasureRepository _generalMeasureRepository = Substitute.For<IGeneralMeasureRepository>();
    private readonly IWorklistTaskRepository _worklistTaskRepository = Substitute.For<IWorklistTaskRepository>();
    private readonly ILogger<GeneralElectionCommitteeService> _logger = NullLogger<GeneralElectionCommitteeService>.Instance;

    private GeneralElectionCommittee _generalElectionCommittee;
    private Committee _committee;
    private Guid _committeeId;
    private Guid _generalElectionCommitteeId;
    private Guid _departmentId;
    private Department _department;

    [SetUp]
    public void SetUp()
    {
        _cultureService.GetCurrentUiCulture().Returns(new CultureInfo("de"));

        _generalElectionCommitteeId = Guid.NewGuid();
        _departmentId = Guid.NewGuid();
        _committeeId = Guid.Parse("17FEBC36-0837-4AD3-AB92-0594777FBC1E");

        _department = new DepartmentBuilder().WithId(_departmentId).Build();

        _committee = new CommitteeBuilder().WithId(_committeeId).WithDepartment(_department).Build();

        _generalElectionCommittee = new GeneralElectionCommitteeBuilder()
            .WithId(_generalElectionCommitteeId)
            .WithCommitteeId(_committeeId)
            .WithCommittee(_committee)
            .WithBeginDate(new DateOnly(1976, 1, 1))
            .WithEndDate(new DateOnly(2030, 12, 31))
            .WithMaximalMember(5)
            .WithDepartment(_department)
            .Build();

        _generalElectionCommitteeService = new GeneralElectionCommitteeService(
            _generalElectionCommitteeRepository,
            _authorizationService,
            _cultureService,
            _generalMeasureRepository,
            _worklistTaskRepository,
            _logger);
    }

    [TearDown]
    public void TearDown()
    {
        _generalElectionCommitteeRepository.ClearSubstitute();
        _authorizationService.ClearSubstitute();
        _cultureService.ClearSubstitute();
        _generalMeasureRepository.ClearSubstitute();
        _worklistTaskRepository.ClearSubstitute();
    }

    [Test]
    public async Task GetGeneralElectionCommitteeList_ShouldReturnCommitteeList()
    {
        var pagingDto = new PagingParametersDto { PageIndex = 42, PageSize = 2 };
        const string sortKey = "description";
        const SortDirection sortDirection = SortDirection.Asc;
        var resultFromRepository = new PagedResult<GeneralElectionCommittee>
        {
            Total = 276,
            Index = 42,
            Items =
            [
                new GeneralElectionCommitteeBuilder()
                    .Build()
            ]
        };
        _generalElectionCommitteeRepository
            .GetAll(Arg.Any<PagingParameters>(), Arg.Any<GeneralElectionCommitteeFilterParameters>(), Arg.Any<string?>(), Arg.Any<SortDirection?>())
            .Returns(resultFromRepository);
        _authorizationService.IsAdmin.Returns(true);

        var committees = await _generalElectionCommitteeService.GetGeneralElectionCommitteeList(pagingDto, null, sortKey, sortDirection);

        await _generalElectionCommitteeRepository.Received(1).GetAll(
            Arg.Is<PagingParameters>(p => p.PageIndex == pagingDto.PageIndex && p.PageSize == pagingDto.PageSize),
            Arg.Any<GeneralElectionCommitteeFilterParameters>(),
            Arg.Is(sortKey),
            Arg.Is(sortDirection));

        Assert.That(committees, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(committees.Index, Is.EqualTo(resultFromRepository.Index));
            Assert.That(committees.Total, Is.EqualTo(resultFromRepository.Total));
            Assert.That(committees.Items.Count(), Is.EqualTo(resultFromRepository.Items.Count()));
        });
    }

    [Test]
    public async Task GetGeneralElectionCommitteeList_ForNonAdminsOrObservers_ShouldSetCommitteeFilter()
    {
        _authorizationService.IsAdmin.Returns(false);
        _authorizationService.LoadCommittees().Returns([new CommitteeBuilder().WithId(_committeeId).Build()]);
        _generalElectionCommitteeRepository.GetAll(Arg.Any<PagingParameters>(), Arg.Any<GeneralElectionCommitteeFilterParameters>(), null, null).Returns(new PagedResult<GeneralElectionCommittee> { Index = 0, Total = 0, Items = [] });

        await _generalElectionCommitteeService.GetGeneralElectionCommitteeList(new PagingParametersDto { PageIndex = 0, PageSize = 0 }, new GeneralElectionCommitteeFilterParametersDto(), null, null);

        await _generalElectionCommitteeRepository.Received(1).GetAll(
            Arg.Any<PagingParameters>(),
            Arg.Is<GeneralElectionCommitteeFilterParameters>(x => x.CommitteeIds != null && x.CommitteeIds.Contains(_committeeId)),
            null,
            null);
    }

    [Test]
    public async Task UpdateGeneralElectionCommitteeJustifications_ForAdmin_ShouldUpdatePropertiesAndCommitChanges()
    {
        _authorizationService.IsAdmin.Returns(true);

        var updateDto = BuildJustificationUpdateDto();
        var missingJustificationTask = new WorklistTaskBuilder()
            .WithWorklistTaskTypeId(WorklistTaskType.GeneralElectionMissingJustifications)
            .WithWorklistTaskStateId(WorklistTaskState.Active)
            .Build();
        _generalElectionCommitteeRepository.GetByIdForUpdate(updateDto.Id, updateDto.RowVersion).Returns(_generalElectionCommittee);
        _generalElectionCommitteeRepository.GetById(updateDto.Id).Returns(_generalElectionCommittee);
        _worklistTaskRepository.GetAllByGeneralElectionCommitteeId(_generalElectionCommittee.Id).Returns([missingJustificationTask]);

        await _generalElectionCommitteeService.UpdateGeneralElectionCommitteeJustifications(updateDto.Id, updateDto);

        await _generalElectionCommitteeRepository.Received(1).GetByIdForUpdate(updateDto.Id, updateDto.RowVersion);
        await _generalElectionCommitteeRepository.Received(1).CommitChanges();
        await _worklistTaskRepository.Received(1).GetAllByGeneralElectionCommitteeId(_generalElectionCommittee.Id);
        Assert.Multiple(() =>
        {
            Assert.That(_generalElectionCommittee.JustificationMembers, Is.EqualTo(updateDto.JustificationMembers));
            Assert.That(_generalElectionCommittee.JustificationGenders, Is.EqualTo(updateDto.JustificationGenders));
            Assert.That(_generalElectionCommittee.MeasuresGenders, Is.EqualTo(updateDto.MeasuresGenders));
            Assert.That(_generalElectionCommittee.JustificationLanguages, Is.EqualTo(updateDto.JustificationLanguages));
            Assert.That(_generalElectionCommittee.MeasuresLanguages, Is.EqualTo(updateDto.MeasuresLanguages));
            Assert.That(_generalElectionCommittee.SelectionProcedure, Is.EqualTo(updateDto.SelectionProcedure));
            Assert.That(missingJustificationTask.WorklistTaskStateId, Is.EqualTo(WorklistTaskState.Completed));
        });
    }

    [Test]
    public async Task UpdateGeneralElectionCommitteeJustification_ForDepartmentInOwnDepartment_ShouldUpdatePropertiesAndCommitChanges()
    {
        _authorizationService.IsAdmin.Returns(false);
        _authorizationService.IsDepartment.Returns(true);

        var updateDto = BuildJustificationUpdateDto();
        var missingJustificationTask = new WorklistTaskBuilder()
            .WithWorklistTaskTypeId(WorklistTaskType.GeneralElectionMissingJustifications)
            .WithWorklistTaskStateId(WorklistTaskState.Active)
            .Build();
        _generalElectionCommitteeRepository.GetByIdForUpdate(updateDto.Id, updateDto.RowVersion).Returns(_generalElectionCommittee);
        _generalElectionCommitteeRepository.GetById(updateDto.Id).Returns(_generalElectionCommittee);
        _authorizationService.GetDepartment().Returns(_department);
        _worklistTaskRepository.GetAllByGeneralElectionCommitteeId(_generalElectionCommittee.Id).Returns([missingJustificationTask]);

        await _generalElectionCommitteeService.UpdateGeneralElectionCommitteeJustifications(updateDto.Id, updateDto);

        await _generalElectionCommitteeRepository.Received(1).GetByIdForUpdate(updateDto.Id, updateDto.RowVersion);
        await _generalElectionCommitteeRepository.Received(1).CommitChanges();
        Assert.Multiple(() =>
        {
            Assert.That(_generalElectionCommittee.JustificationMembers, Is.EqualTo(updateDto.JustificationMembers));
            Assert.That(_generalElectionCommittee.JustificationGenders, Is.EqualTo(updateDto.JustificationGenders));
            Assert.That(_generalElectionCommittee.MeasuresGenders, Is.EqualTo(updateDto.MeasuresGenders));
            Assert.That(_generalElectionCommittee.JustificationLanguages, Is.EqualTo(updateDto.JustificationLanguages));
            Assert.That(_generalElectionCommittee.MeasuresLanguages, Is.EqualTo(updateDto.MeasuresLanguages));
        });
    }

    [Test]
    public async Task UpdateGeneralElectionCommitteeJustification_ForCommitteeNotInOwnDepartment_ShouldThrowAuthorizationException()
    {
        _authorizationService.IsAdmin.Returns(false);
        _authorizationService.IsDepartment.Returns(true);

        var updateDto = BuildJustificationUpdateDto();
        _generalElectionCommitteeRepository.GetByIdForUpdate(updateDto.Id, updateDto.RowVersion).Returns(_generalElectionCommittee);
        _generalElectionCommitteeRepository.GetById(updateDto.Id).Returns(_generalElectionCommittee);
        _authorizationService.GetDepartment().Returns(new DepartmentBuilder().Build());

        Assert.That(async () => await _generalElectionCommitteeService.UpdateGeneralElectionCommitteeJustifications(updateDto.Id, updateDto), Throws.Exception.InstanceOf<AuthorizationException>());

        await _generalElectionCommitteeRepository.Received(1).GetByIdForUpdate(updateDto.Id, updateDto.RowVersion);
        await _generalElectionCommitteeRepository.DidNotReceiveWithAnyArgs().CommitChanges();
    }

    [Test]
    public async Task UpdateGeneralElectionCommitteeVacancies_ForAdmin_ShouldUpdatePropertiesAndCommitChanges()
    {
        _authorizationService.IsAdmin.Returns(true);
        var committeeId = Guid.NewGuid();
        var vacancies = 13;

        _generalElectionCommitteeRepository.GetByCommitteeIdForUpdate(committeeId).Returns(_generalElectionCommittee);

        await _generalElectionCommitteeService.UpdateGeneralElectionCommitteeVacancies(committeeId, vacancies);

        await _generalElectionCommitteeRepository.Received(1).GetByCommitteeIdForUpdate(committeeId);
        await _generalElectionCommitteeRepository.Received(1).CommitChanges();
        Assert.Multiple(() =>
        {
            Assert.That(_generalElectionCommittee.VacanciesGeneralElection, Is.EqualTo(vacancies));
        });
    }

    [Test]
    public async Task UpdateGeneralElectionCommitteeVacancies_ForDepartmentInOwnDepartment_ShouldUpdatePropertiesAndCommitChanges()
    {
        _authorizationService.IsAdmin.Returns(false);
        _authorizationService.IsDepartment.Returns(true);
        var committeeId = Guid.NewGuid();
        var vacancies = 13;

        _authorizationService.GetDepartment().Returns(_department);
        _generalElectionCommitteeRepository.GetByCommitteeIdForUpdate(committeeId).Returns(_generalElectionCommittee);

        await _generalElectionCommitteeService.UpdateGeneralElectionCommitteeVacancies(committeeId, vacancies);

        await _generalElectionCommitteeRepository.Received(1).GetByCommitteeIdForUpdate(committeeId);
        await _generalElectionCommitteeRepository.Received(1).CommitChanges();
        Assert.Multiple(() =>
        {
            Assert.That(_generalElectionCommittee.VacanciesGeneralElection, Is.EqualTo(vacancies));
        });
    }

    [Test]
    public async Task UpdateGeneralElectionCommitteeVacancies_ForCommitteeNotInOwnDepartment_ShouldThrowAuthorizationException()
    {
        _authorizationService.IsAdmin.Returns(false);
        _authorizationService.IsDepartment.Returns(true);
        var committeeId = Guid.NewGuid();
        var vacancies = 13;

        _generalElectionCommitteeRepository.GetByCommitteeIdForUpdate(committeeId).Returns(_generalElectionCommittee);
        _authorizationService.GetDepartment().Returns(new DepartmentBuilder().Build());

        Assert.That(async () => await _generalElectionCommitteeService.UpdateGeneralElectionCommitteeVacancies(committeeId, vacancies), Throws.Exception.InstanceOf<AuthorizationException>());

        await _generalElectionCommitteeRepository.Received(1).GetByCommitteeIdForUpdate(committeeId);
        await _generalElectionCommitteeRepository.DidNotReceiveWithAnyArgs().CommitChanges();
    }

    private static GeneralElectionCommitteeJustificationUpdateDto BuildJustificationUpdateDto()
    {
        return new GeneralElectionCommitteeJustificationUpdateDto
        {
            JustificationMembers = "member",
            JustificationGenders = "gender",
            MeasuresGenders = "measureGender",
            JustificationLanguages = "justificationLanguage",
            MeasuresLanguages = "measureLanguages",
            SelectionProcedure = "selectionProcedure",
            RowVersion = 666
        };
    }
}
