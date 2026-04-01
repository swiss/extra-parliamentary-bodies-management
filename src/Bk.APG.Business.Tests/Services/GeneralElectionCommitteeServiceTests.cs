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
using Swiss.FCh.DocumentService.Client.Models;

namespace Bk.APG.Business.Tests.Services;

[TestFixture]
public class GeneralElectionCommitteeServiceTests
{
    private GeneralElectionCommitteeService _generalElectionCommitteeService = null!;
    private readonly IGeneralElectionCommitteeRepository _generalElectionCommitteeRepository = Substitute.For<IGeneralElectionCommitteeRepository>();
    private readonly IAuthorizationService _authorizationService = Substitute.For<IAuthorizationService>();
    private readonly ICultureService _cultureService = Substitute.For<ICultureService>();
    private readonly ICommitteeService _committeeService = Substitute.For<ICommitteeService>();
    private readonly IGeneralMeasureRepository _generalMeasureRepository = Substitute.For<IGeneralMeasureRepository>();
    private readonly IWorklistTaskRepository _worklistTaskRepository = Substitute.For<IWorklistTaskRepository>();
    private readonly IMasterDataRepository _masterDataRepository = Substitute.For<IMasterDataRepository>();
    private readonly Swiss.FCh.DocumentService.Client.IDocumentService _documentService = Substitute.For<Swiss.FCh.DocumentService.Client.IDocumentService>();
    private readonly ILogger<GeneralElectionCommitteeService> _logger = NullLogger<GeneralElectionCommitteeService>.Instance;

    private readonly List<GeneralElectionCommittee> _generalElectionCommittees = new();
    private GeneralElectionCommittee _generalElectionCommittee1;
    private GeneralElectionCommittee _generalElectionCommittee2;
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

        _generalElectionCommittee1 = new GeneralElectionCommitteeBuilder()
            .WithId(_generalElectionCommitteeId)
            .WithCommitteeId(_committeeId)
            .WithCommittee(_committee)
            .WithBeginDate(new DateOnly(1976, 1, 1))
            .WithEndDate(new DateOnly(2030, 12, 31))
            .WithMaximalMember(5)
            .WithDepartment(_department)
            .WithIsValidated(true)
            .WithTermOfOfficeDate(new TermOfOfficeDateBuilder().Build())
            .WithCandidateListStateId(CandidateListState.Draft)
            .Build();

        _generalElectionCommittee2 = new GeneralElectionCommitteeBuilder()
            .WithId(Guid.NewGuid())
            .WithCommitteeId(_committeeId)
            .WithCommittee(_committee)
            .WithBeginDate(new DateOnly(1976, 1, 1))
            .WithEndDate(new DateOnly(2030, 12, 31))
            .WithMaximalMember(5)
            .WithDepartment(_department)
            .WithIsValidated(false)
            .WithTermOfOfficeDate(new TermOfOfficeDateBuilder().Build())
            .WithCandidateListStateId(CandidateListState.ReadyForFederalCouncilProposalFinalized)
            .Build();

        _generalElectionCommittees.Add(_generalElectionCommittee1);
        _generalElectionCommittees.Add(_generalElectionCommittee2);

        _generalElectionCommitteeService = new GeneralElectionCommitteeService(
            _generalElectionCommitteeRepository,
            _authorizationService,
            _cultureService,
            _committeeService,
            _generalMeasureRepository,
            _worklistTaskRepository,
            _masterDataRepository,
            _documentService,
            _logger);
    }

    [TearDown]
    public void TearDown()
    {
        _generalElectionCommitteeRepository.ClearSubstitute();
        _authorizationService.ClearSubstitute();
        _cultureService.ClearSubstitute();
        _committeeService.ClearSubstitute();
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
    public async Task GetGeneralElectionCommittee_WithCandidateListTasks_ShouldEnableCandidateListActions()
    {
        var candidateListState = new CandidateListStateBuilder()
            .WithId(CandidateListState.Draft)
            .Build();
        var assignmentId = Guid.NewGuid();
        var currentAssignment = new EiamAssignmentBuilder()
            .WithId(assignmentId)
            .WithRole(Role.Department)
            .Build();
        var activeCandidateListTask = new WorklistTaskBuilder()
            .WithWorklistTaskTypeId(WorklistTaskType.CandidateListCreate)
            .WithWorklistTaskStateId(WorklistTaskState.Active)
            .WithAssignedTo(currentAssignment)
            .WithGeneralElectionCommitteeId(_generalElectionCommittee1.Id)
            .Build();

        _generalElectionCommitteeRepository.GetByCommitteeId(_committeeId).Returns(_generalElectionCommittee1);
        _generalElectionCommittee1.CandidateListState = candidateListState;
        _generalElectionCommittee1.CandidateListStateId = candidateListState.Id;
        _authorizationService.HasAccessToCommittee(_generalElectionCommittee1.Committee!).Returns(true);
        _authorizationService.GetCurrentEiamAssignment().Returns(currentAssignment);
        _generalMeasureRepository.GetGeneralGenderMeasure(_generalElectionCommittee1.DepartmentId).Returns(Task.FromResult<GeneralGenderMeasure?>(null));
        _generalMeasureRepository.GetGeneralLanguageMeasure(_generalElectionCommittee1.DepartmentId).Returns(Task.FromResult<GeneralLanguageMeasure?>(null));
        _worklistTaskRepository.GetAllByGeneralElectionCommitteeId(_generalElectionCommittee1.Id).Returns([activeCandidateListTask]);

        var result = await _generalElectionCommitteeService.GetGeneralElectionCommittee(_committeeId);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(result.WasGeneralElectionStartedForCommittee, Is.True);
            Assert.That(result.CanSaveCandidateList, Is.True);
            Assert.That(result.CanValidateCandidateList, Is.True);
            Assert.That(result.CanForwardCandidateList, Is.True);
        }
    }

    [Test]
    public async Task GetGeneralElectionCommittee_WithoutCandidateListTasks_ShouldDisableCandidateListActions()
    {
        var candidateListState = new CandidateListStateBuilder()
            .WithId(CandidateListState.Draft)
            .Build();
        var currentAssignment = new EiamAssignmentBuilder()
            .WithRole(Role.Admin)
            .Build();

        _generalElectionCommitteeRepository.GetByCommitteeId(_committeeId).Returns(_generalElectionCommittee1);
        _generalElectionCommittee1.CandidateListState = candidateListState;
        _generalElectionCommittee1.CandidateListStateId = candidateListState.Id;
        _authorizationService.HasAccessToCommittee(_generalElectionCommittee1.Committee!).Returns(true);
        _authorizationService.GetCurrentEiamAssignment().Returns(currentAssignment);
        _generalMeasureRepository.GetGeneralGenderMeasure(_generalElectionCommittee1.DepartmentId).Returns(Task.FromResult<GeneralGenderMeasure?>(null));
        _generalMeasureRepository.GetGeneralLanguageMeasure(_generalElectionCommittee1.DepartmentId).Returns(Task.FromResult<GeneralLanguageMeasure?>(null));
        _worklistTaskRepository.GetAllByGeneralElectionCommitteeId(_generalElectionCommittee1.Id).Returns(new List<WorklistTask>());

        var result = await _generalElectionCommitteeService.GetGeneralElectionCommittee(_committeeId);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(result.WasGeneralElectionStartedForCommittee, Is.False);
            Assert.That(result.CanSaveCandidateList, Is.False);
            Assert.That(result.CanValidateCandidateList, Is.False);
            Assert.That(result.CanForwardCandidateList, Is.False);
        }
    }

    [Test]
    public async Task GetGeneralElectionCommittee_WithReadyForFederalCouncilProposalState_ShouldSetIsReadyForProposal()
    {
        var candidateListState = new CandidateListStateBuilder()
            .WithId(CandidateListState.ReadyForFederalCouncilProposalFinalized)
            .Build();
        var currentAssignment = new EiamAssignmentBuilder()
            .WithRole(Role.Department)
            .Build();

        _generalElectionCommitteeRepository.GetByCommitteeId(_committeeId).Returns(_generalElectionCommittee1);
        _generalElectionCommittee1.CandidateListState = candidateListState;
        _generalElectionCommittee1.CandidateListStateId = candidateListState.Id;
        _authorizationService.HasAccessToCommittee(_generalElectionCommittee1.Committee!).Returns(true);
        _authorizationService.GetCurrentEiamAssignment().Returns(currentAssignment);
        _generalMeasureRepository.GetGeneralGenderMeasure(_generalElectionCommittee1.DepartmentId).Returns(Task.FromResult<GeneralGenderMeasure?>(null));
        _generalMeasureRepository.GetGeneralLanguageMeasure(_generalElectionCommittee1.DepartmentId).Returns(Task.FromResult<GeneralLanguageMeasure?>(null));
        _worklistTaskRepository.GetAllByGeneralElectionCommitteeId(_generalElectionCommittee1.Id).Returns([]);

        var result = await _generalElectionCommitteeService.GetGeneralElectionCommittee(_committeeId);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(result.IsReadyForProposalFinalized, Is.True);
            Assert.That(result.CanForwardReadyForProposal, Is.False);
            Assert.That(result.CanFinalizeReadyForProposal, Is.False);
        }
    }

    [Test]
    public async Task GetGeneralElectionCommittee_WithActiveReadyForProposalTaskForCurrentNonAdmin_ShouldSetCanForwardReadyForProposal()
    {
        var candidateListState = new CandidateListStateBuilder()
            .WithId(CandidateListState.Draft)
            .Build();
        var currentAssignment = new EiamAssignmentBuilder()
            .WithRole(Role.Secretariat)
            .Build();
        var activeReadyForProposalTask = new WorklistTaskBuilder()
            .WithWorklistTaskTypeId(WorklistTaskType.ReadyForFederalCouncilProposal)
            .WithWorklistTaskStateId(WorklistTaskState.Active)
            .WithAssignedTo(currentAssignment)
            .WithGeneralElectionCommitteeId(_generalElectionCommittee1.Id)
            .Build();

        _generalElectionCommitteeRepository.GetByCommitteeId(_committeeId).Returns(_generalElectionCommittee1);
        _generalElectionCommittee1.CandidateListState = candidateListState;
        _generalElectionCommittee1.CandidateListStateId = candidateListState.Id;
        _authorizationService.HasAccessToCommittee(_generalElectionCommittee1.Committee!).Returns(true);
        _authorizationService.GetCurrentEiamAssignment().Returns(currentAssignment);
        _generalMeasureRepository.GetGeneralGenderMeasure(_generalElectionCommittee1.DepartmentId).Returns(Task.FromResult<GeneralGenderMeasure?>(null));
        _generalMeasureRepository.GetGeneralLanguageMeasure(_generalElectionCommittee1.DepartmentId).Returns(Task.FromResult<GeneralLanguageMeasure?>(null));
        _worklistTaskRepository.GetAllByGeneralElectionCommitteeId(_generalElectionCommittee1.Id).Returns([activeReadyForProposalTask]);

        var result = await _generalElectionCommitteeService.GetGeneralElectionCommittee(_committeeId);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(result.ReadyForProposalAssignedTo, Is.EqualTo(currentAssignment.GetText()));
            Assert.That(result.CanForwardReadyForProposal, Is.True);
            Assert.That(result.CanFinalizeReadyForProposal, Is.False);
        }
    }

    [Test]
    public async Task GetGeneralElectionCommittee_WithAdminAndProposalForwarded_ShouldSetCanFinalizeReadyForProposal()
    {
        var candidateListState = new CandidateListStateBuilder()
            .WithId(CandidateListState.ReadyForFederalCouncilProposalForwarded)
            .Build();
        var currentAssignment = new EiamAssignment
        {
            Id = Guid.NewGuid(),
            ExternalId = "admin-external-id",
            Role = Role.Admin
        };
        _generalElectionCommitteeRepository.GetByCommitteeId(_committeeId).Returns(_generalElectionCommittee1);
        _generalElectionCommittee1.CandidateListState = candidateListState;
        _generalElectionCommittee1.CandidateListStateId = candidateListState.Id;
        _authorizationService.HasAccessToCommittee(_generalElectionCommittee1.Committee!).Returns(true);
        _authorizationService.GetCurrentEiamAssignment().Returns(currentAssignment);
        _generalMeasureRepository.GetGeneralGenderMeasure(_generalElectionCommittee1.DepartmentId).Returns(Task.FromResult<GeneralGenderMeasure?>(null));
        _generalMeasureRepository.GetGeneralLanguageMeasure(_generalElectionCommittee1.DepartmentId).Returns(Task.FromResult<GeneralLanguageMeasure?>(null));
        _worklistTaskRepository.GetAllByGeneralElectionCommitteeId(_generalElectionCommittee1.Id).Returns([]);

        var result = await _generalElectionCommitteeService.GetGeneralElectionCommittee(_committeeId);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(result.IsReadyForProposalFinalized, Is.False);
            Assert.That(result.CanForwardReadyForProposal, Is.False);
            Assert.That(result.CanFinalizeReadyForProposal, Is.True);
        }
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
        _generalElectionCommitteeRepository.GetByIdForUpdate(updateDto.Id, updateDto.RowVersion).Returns(_generalElectionCommittee1);
        _generalElectionCommitteeRepository.GetById(updateDto.Id).Returns(_generalElectionCommittee1);
        _worklistTaskRepository.GetAllByGeneralElectionCommitteeId(_generalElectionCommittee1.Id).Returns([missingJustificationTask]);

        await _generalElectionCommitteeService.UpdateGeneralElectionCommitteeJustifications(updateDto.Id, updateDto);

        await _generalElectionCommitteeRepository.Received(1).GetByIdForUpdate(updateDto.Id, updateDto.RowVersion);
        await _generalElectionCommitteeRepository.Received(1).CommitChanges();
        await _worklistTaskRepository.Received(1).GetAllByGeneralElectionCommitteeId(_generalElectionCommittee1.Id);
        Assert.Multiple(() =>
        {
            Assert.That(_generalElectionCommittee1.JustificationMembers, Is.EqualTo(updateDto.JustificationMembers));
            Assert.That(_generalElectionCommittee1.JustificationGenders, Is.EqualTo(updateDto.JustificationGenders));
            Assert.That(_generalElectionCommittee1.MeasuresGenders, Is.EqualTo(updateDto.MeasuresGenders));
            Assert.That(_generalElectionCommittee1.JustificationLanguages, Is.EqualTo(updateDto.JustificationLanguages));
            Assert.That(_generalElectionCommittee1.MeasuresLanguages, Is.EqualTo(updateDto.MeasuresLanguages));
            Assert.That(_generalElectionCommittee1.SelectionProcedure, Is.EqualTo(updateDto.SelectionProcedure));
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
        _generalElectionCommitteeRepository.GetByIdForUpdate(updateDto.Id, updateDto.RowVersion).Returns(_generalElectionCommittee1);
        _generalElectionCommitteeRepository.GetById(updateDto.Id).Returns(_generalElectionCommittee1);
        _authorizationService.GetDepartment().Returns(_department);
        _worklistTaskRepository.GetAllByGeneralElectionCommitteeId(_generalElectionCommittee1.Id).Returns([missingJustificationTask]);

        await _generalElectionCommitteeService.UpdateGeneralElectionCommitteeJustifications(updateDto.Id, updateDto);

        await _generalElectionCommitteeRepository.Received(1).GetByIdForUpdate(updateDto.Id, updateDto.RowVersion);
        await _generalElectionCommitteeRepository.Received(1).CommitChanges();
        Assert.Multiple(() =>
        {
            Assert.That(_generalElectionCommittee1.JustificationMembers, Is.EqualTo(updateDto.JustificationMembers));
            Assert.That(_generalElectionCommittee1.JustificationGenders, Is.EqualTo(updateDto.JustificationGenders));
            Assert.That(_generalElectionCommittee1.MeasuresGenders, Is.EqualTo(updateDto.MeasuresGenders));
            Assert.That(_generalElectionCommittee1.JustificationLanguages, Is.EqualTo(updateDto.JustificationLanguages));
            Assert.That(_generalElectionCommittee1.MeasuresLanguages, Is.EqualTo(updateDto.MeasuresLanguages));
        });
    }

    [Test]
    public async Task UpdateGeneralElectionCommitteeJustification_ForCommitteeNotInOwnDepartment_ShouldThrowAuthorizationException()
    {
        _authorizationService.IsAdmin.Returns(false);
        _authorizationService.IsDepartment.Returns(true);

        var updateDto = BuildJustificationUpdateDto();
        _generalElectionCommitteeRepository.GetByIdForUpdate(updateDto.Id, updateDto.RowVersion).Returns(_generalElectionCommittee1);
        _generalElectionCommitteeRepository.GetById(updateDto.Id).Returns(_generalElectionCommittee1);
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

        _generalElectionCommitteeRepository.GetByCommitteeIdForUpdate(committeeId).Returns(_generalElectionCommittee1);

        await _generalElectionCommitteeService.UpdateGeneralElectionCommitteeVacancies(committeeId, vacancies);

        await _generalElectionCommitteeRepository.Received(1).GetByCommitteeIdForUpdate(committeeId);
        await _generalElectionCommitteeRepository.Received(1).CommitChanges();
        Assert.Multiple(() =>
        {
            Assert.That(_generalElectionCommittee1.VacanciesGeneralElection, Is.EqualTo(vacancies));
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
        _generalElectionCommitteeRepository.GetByCommitteeIdForUpdate(committeeId).Returns(_generalElectionCommittee1);

        await _generalElectionCommitteeService.UpdateGeneralElectionCommitteeVacancies(committeeId, vacancies);

        await _generalElectionCommitteeRepository.Received(1).GetByCommitteeIdForUpdate(committeeId);
        await _generalElectionCommitteeRepository.Received(1).CommitChanges();
        Assert.Multiple(() =>
        {
            Assert.That(_generalElectionCommittee1.VacanciesGeneralElection, Is.EqualTo(vacancies));
        });
    }

    [Test]
    public async Task UpdateGeneralElectionCommitteeVacancies_ForCommitteeNotInOwnDepartment_ShouldThrowAuthorizationException()
    {
        _authorizationService.IsAdmin.Returns(false);
        _authorizationService.IsDepartment.Returns(true);
        var committeeId = Guid.NewGuid();
        var vacancies = 13;

        _generalElectionCommitteeRepository.GetByCommitteeIdForUpdate(committeeId).Returns(_generalElectionCommittee1);
        _authorizationService.GetDepartment().Returns(new DepartmentBuilder().Build());

        Assert.That(async () => await _generalElectionCommitteeService.UpdateGeneralElectionCommitteeVacancies(committeeId, vacancies), Throws.Exception.InstanceOf<AuthorizationException>());

        await _generalElectionCommitteeRepository.Received(1).GetByCommitteeIdForUpdate(committeeId);
        await _generalElectionCommitteeRepository.DidNotReceiveWithAnyArgs().CommitChanges();
    }

    [Test]
    public async Task GenerateCandidateListExport_WithPersonEntity_ShouldExportCorrectly()
    {
        var committeeId = Guid.NewGuid();
        var membershipCandidateId = Guid.NewGuid();

        var geCommittee = new GeneralElectionCommitteeBuilder()
            .WithGermanDescription("Gremium DE")
            .WithOffice(new OfficeBuilder().WithGermanDescription("Office DE").Build())
            .WithMembershipCandidates(
            [
                new MembershipCandidateBuilder()
                    .WithId(membershipCandidateId)
                    .WithBeginDate(new DateOnly(2025, 1, 1))
                    .WithEndDate(new DateOnly(2025, 12, 31))
                    .WithFunction(new FunctionBuilder().Build())
                    .WithPerson(
                        new PersonBuilder()
                            .WithGender(new GenderBuilder().Build())
                            .WithLanguage(new LanguageBuilder().Build())
                            .WithBirthYear(2000)
                            .WithCorrespondenceAddress(new AddressBuilder()
                                .WithCity("Zurich")
                                .WithEmail("test@test.ch")
                                .WithPhone("+4111223344")
                                .Build())
                            .WithOccupations([
                                new OccupationBuilder()
                                    .WithGermanDescription("JobDE").Build()
                            ])
                            .WithInterests([
                                new InterestBuilder()
                                    .Build()
                            ]).Build())
                    .WithRemarks("remarks")
                    .WithRemarksStatus("remarksStatus")
                    .Build()
            ])
            .Build();

        _generalElectionCommitteeRepository
            .GetForCandidateListExport(Arg.Is<Guid>(x => x == committeeId),
                Arg.Is<IEnumerable<Guid>>(list => list.SequenceEqual(new[] { membershipCandidateId })))
            .Returns(geCommittee);

        Spreadsheet? capturedSpreadsheet = null;
        _documentService
            .CreateExcel(Arg.Do<Spreadsheet>(spreadsheet => capturedSpreadsheet = spreadsheet), Arg.Any<SpreadsheetOptions?>())
            .Returns(new MemoryStream());

        await _generalElectionCommitteeService.GenerateCandidateListExport(committeeId, [membershipCandidateId]);

        Assert.That(capturedSpreadsheet, Is.Not.Null);

        var dataRow = capturedSpreadsheet.BodyCells[0];
        Assert.Multiple(() =>
        {
            Assert.That(dataRow[0].Text, Is.EqualTo(geCommittee.DescriptionGerman));
            Assert.That(dataRow[1].Text, Is.EqualTo(geCommittee.Office!.GetDescription()));
            Assert.That(dataRow[2].Text, Is.EqualTo(geCommittee.MembershipCandidates.First().Person!.Title));
            Assert.That(dataRow[3].Text, Is.EqualTo(geCommittee.MembershipCandidates.First().Person!.Surname));
            Assert.That(dataRow[4].Text, Is.EqualTo(geCommittee.MembershipCandidates.First().Person!.GivenName));
            Assert.That(dataRow[5].Text, Is.EqualTo(geCommittee.MembershipCandidates.First().Person!.BirthYear.ToString(CultureInfo.InvariantCulture)));
            Assert.That(dataRow[6].Text, Is.EqualTo(geCommittee.MembershipCandidates.First().Person!.Gender!.GetText()));
            Assert.That(dataRow[7].Text, Is.EqualTo(geCommittee.MembershipCandidates.First().Person!.Language!.GetText()));
            Assert.That(dataRow[8].Text, Is.EqualTo(geCommittee.MembershipCandidates.First().RemarksStatus));
            Assert.That(dataRow[9].Text, Is.EqualTo(geCommittee.MembershipCandidates.First().Function!.GetText()));
            Assert.That(dataRow[10].Text, Is.EqualTo(geCommittee.MembershipCandidates.First().BeginDate.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture)));
            Assert.That(dataRow[11].Text, Is.EqualTo(geCommittee.MembershipCandidates.First().EndDate.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture)));
            Assert.That(dataRow[12].Text, Is.EqualTo(geCommittee.MembershipCandidates.First().ElectionType!.GetText()));
            Assert.That(dataRow[13].Text, Is.EqualTo(geCommittee.MembershipCandidates.First().MembershipAddition!.GetText()));
            Assert.That(dataRow[14].Text, Is.EqualTo(geCommittee.MembershipCandidates.First().Remarks));
            Assert.That(dataRow[15].Text, Is.EqualTo(geCommittee.MembershipCandidates.First().Person!.Occupations.First().GetText()));
            Assert.That(dataRow[16].Text, Is.EqualTo(geCommittee.MembershipCandidates.First().Person!.CorrespondenceAddress!.City));
            Assert.That(dataRow[17].Text, Is.EqualTo(geCommittee.MembershipCandidates.First().Person!.CorrespondenceAddress!.Phone));
            Assert.That(dataRow[18].Text, Is.EqualTo(geCommittee.MembershipCandidates.First().Person!.CorrespondenceAddress!.Email));
            Assert.That(dataRow[19].Text, Is.EqualTo(string.Join(";", geCommittee.MembershipCandidates.First().Person!.Interests.Select(y => y.InterestText))));
        });
    }

    [Test]
    public async Task GenerateCandidateListExport_WithoutPersonEntity_ShouldExportCorrectly()
    {
        var committeeId = Guid.NewGuid();
        var membershipCandidateId = Guid.NewGuid();

        var geCommittee = new GeneralElectionCommitteeBuilder()
            .WithGermanDescription("Gremium DE")
            .WithOffice(new OfficeBuilder().WithGermanDescription("Office DE").Build())
            .WithMembershipCandidates(
            [
                new MembershipCandidateBuilder()
                    .WithId(membershipCandidateId)
                    .WithBeginDate(new DateOnly(2025, 1, 1))
                    .WithEndDate(new DateOnly(2025, 12, 31))
                    .WithFunction(new FunctionBuilder().Build())
                    .WithSurname("clark")
                    .WithGivenName("jim")
                    .WithGender(new GenderBuilder().Build())
                    .WithLanguage(new LanguageBuilder().Build())
                    .WithBirthYear(2000)
                    .WithRemarks("remarks")
                    .WithRemarksStatus("remarksStatus")
                    .Build()
            ])
            .Build();

        _generalElectionCommitteeRepository
            .GetForCandidateListExport(Arg.Is<Guid>(x => x == committeeId),
                Arg.Is<IEnumerable<Guid>>(list => list.SequenceEqual(new[] { membershipCandidateId })))
            .Returns(geCommittee);

        var stream = new MemoryStream();

        Spreadsheet? capturedSpreadsheet = null;
        _documentService
            .CreateExcel(Arg.Do<Spreadsheet>(spreadsheet => capturedSpreadsheet = spreadsheet), Arg.Any<SpreadsheetOptions?>())
            .Returns(stream);

        await _generalElectionCommitteeService.GenerateCandidateListExport(committeeId, [membershipCandidateId]);

        Assert.That(capturedSpreadsheet, Is.Not.Null);

        var dataRow = capturedSpreadsheet.BodyCells[0];
        Assert.Multiple(() =>
        {
            Assert.That(dataRow[0].Text, Is.EqualTo(geCommittee.DescriptionGerman));
            Assert.That(dataRow[1].Text, Is.EqualTo(geCommittee.Office!.GetDescription()));
            Assert.That(dataRow[2].Text, Is.Empty);
            Assert.That(dataRow[3].Text, Is.EqualTo(geCommittee.MembershipCandidates.First().Surname));
            Assert.That(dataRow[4].Text, Is.EqualTo(geCommittee.MembershipCandidates.First().GivenName));
            Assert.That(dataRow[5].Text, Is.EqualTo(geCommittee.MembershipCandidates.First().BirthYear.ToString(CultureInfo.InvariantCulture)));
            Assert.That(dataRow[6].Text, Is.EqualTo(geCommittee.MembershipCandidates.First().Gender!.GetText()));
            Assert.That(dataRow[7].Text, Is.EqualTo(geCommittee.MembershipCandidates.First().Language!.GetText()));
            Assert.That(dataRow[8].Text, Is.EqualTo(geCommittee.MembershipCandidates.First().RemarksStatus));
            Assert.That(dataRow[9].Text, Is.EqualTo(geCommittee.MembershipCandidates.First().Function!.GetText()));
            Assert.That(dataRow[10].Text, Is.EqualTo(geCommittee.MembershipCandidates.First().BeginDate.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture)));
            Assert.That(dataRow[11].Text, Is.EqualTo(geCommittee.MembershipCandidates.First().EndDate.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture)));
            Assert.That(dataRow[12].Text, Is.EqualTo(geCommittee.MembershipCandidates.First().ElectionType!.GetText()));
            Assert.That(dataRow[13].Text, Is.EqualTo(geCommittee.MembershipCandidates.First().MembershipAddition!.GetText()));
            Assert.That(dataRow[14].Text, Is.EqualTo(geCommittee.MembershipCandidates.First().Remarks));
            Assert.That(dataRow[15].Text, Is.Empty);
            Assert.That(dataRow[16].Text, Is.Empty);
            Assert.That(dataRow[17].Text, Is.Empty);
            Assert.That(dataRow[18].Text, Is.Empty);
            Assert.That(dataRow[19].Text, Is.Empty);
        });

        await stream.DisposeAsync();
    }

    [Test]
    public async Task InvalidateMembershipCandidateList_WhenCalled_ShouldInvalidateTasks()
    {
        var candidateListState = new CandidateListStateBuilder()
            .WithId(CandidateListState.Draft)
            .Build();
        var currentAssignment = new EiamAssignmentBuilder()
            .WithId(Guid.NewGuid())
            .WithRole(Role.Department)
            .Build();
        var currentAssignmentSecretariat = new EiamAssignmentBuilder()
            .WithId(Guid.NewGuid())
            .WithRole(Role.Secretariat)
            .Build();
        var currentAssignmentOffice = new EiamAssignmentBuilder()
            .WithId(Guid.NewGuid())
            .WithRole(Role.Office)
            .Build();
        var currentAssignmentDepartment = new EiamAssignmentBuilder()
            .WithId(Guid.NewGuid())
            .WithRole(Role.Department)
            .Build();
        var currentAssignmentAdmin = new EiamAssignmentBuilder()
            .WithId(Guid.NewGuid())
            .WithRole(Role.Admin)
            .Build();
        var completedCandidateListApprovalTask = new WorklistTaskBuilder()
            .WithWorklistTaskTypeId(WorklistTaskType.CandidateListApprove)
            .WithWorklistTaskStateId(WorklistTaskState.Completed)
            .WithAssignedTo(currentAssignment)
            .WithGeneralElectionCommitteeId(_generalElectionCommittee1.Id)
            .Build();
        var activeCandidateListTaskSecretariat = new WorklistTaskBuilder()
            .WithWorklistTaskTypeId(WorklistTaskType.GeneralElectionPersonInterests)
            .WithWorklistTaskStateId(WorklistTaskState.Active)
            .WithAssignedTo(currentAssignmentSecretariat)
            .WithGeneralElectionCommitteeId(_generalElectionCommittee1.Id)
            .Build();
        var activeReadyForProposalTasksForSecretariat = new WorklistTaskBuilder()
            .WithWorklistTaskTypeId(WorklistTaskType.ReadyForFederalCouncilProposal)
            .WithWorklistTaskStateId(WorklistTaskState.Active)
            .WithAssignedTo(currentAssignmentSecretariat)
            .WithGeneralElectionCommitteeId(_generalElectionCommittee1.Id)
            .Build();
        var activeReadyForProposalTasksForOffice = new WorklistTaskBuilder()
            .WithWorklistTaskTypeId(WorklistTaskType.ReadyForFederalCouncilProposal)
            .WithWorklistTaskStateId(WorklistTaskState.Active)
            .WithAssignedTo(currentAssignmentOffice)
            .WithGeneralElectionCommitteeId(_generalElectionCommittee1.Id)
            .Build();
        var activeReadyForProposalTasksForDepartment = new WorklistTaskBuilder()
            .WithWorklistTaskTypeId(WorklistTaskType.ReadyForFederalCouncilProposal)
            .WithWorklistTaskStateId(WorklistTaskState.Active)
            .WithAssignedTo(currentAssignmentDepartment)
            .WithGeneralElectionCommitteeId(_generalElectionCommittee1.Id)
            .Build();
        var activeReadyForProposalTasksForAdmin = new WorklistTaskBuilder()
            .WithWorklistTaskTypeId(WorklistTaskType.ReadyForFederalCouncilProposal)
            .WithWorklistTaskStateId(WorklistTaskState.Active)
            .WithAssignedTo(currentAssignmentAdmin)
            .WithGeneralElectionCommitteeId(_generalElectionCommittee1.Id)
            .Build();

        _generalElectionCommittee1.IsValidated = true;
        _generalElectionCommitteeRepository.GetByCommitteeIdForUpdate(_committeeId).Returns(_generalElectionCommittee1);
        _generalElectionCommittee1.CandidateListState = candidateListState;
        _generalElectionCommittee1.CandidateListStateId = candidateListState.Id;
        _worklistTaskRepository.GetAllByGeneralElectionCommitteeId(_generalElectionCommittee1.Id).Returns([
            completedCandidateListApprovalTask,
            activeCandidateListTaskSecretariat,
            activeReadyForProposalTasksForSecretariat,
            activeReadyForProposalTasksForOffice,
            activeReadyForProposalTasksForDepartment,
            activeReadyForProposalTasksForAdmin]);

        await _generalElectionCommitteeService.InvalidateMembershipCandidateList(_committeeId);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(_generalElectionCommittee1.IsValidated, Is.False);
            Assert.That(activeCandidateListTaskSecretariat.WorklistTaskStateId == WorklistTaskState.Inactive, Is.True);
            Assert.That(completedCandidateListApprovalTask.WorklistTaskStateId == WorklistTaskState.Active, Is.True);
            Assert.That(activeReadyForProposalTasksForSecretariat.WorklistTaskStateId == WorklistTaskState.Inactive, Is.True);
            Assert.That(activeReadyForProposalTasksForOffice.WorklistTaskStateId == WorklistTaskState.Inactive, Is.True);
            Assert.That(activeReadyForProposalTasksForDepartment.WorklistTaskStateId == WorklistTaskState.Inactive, Is.True);
            Assert.That(activeReadyForProposalTasksForAdmin.WorklistTaskStateId == WorklistTaskState.Inactive, Is.True);
        }
    }

    [Test]
    public async Task SetFederalCouncilProposalToDirty_WhenCalled_ShouldInvalidateTasks()
    {
        var candidateListState = new CandidateListStateBuilder()
            .WithId(CandidateListState.Draft)
            .Build();
        var assignmentId = Guid.NewGuid();
        var currentAssignmentSecretariat = new EiamAssignmentBuilder()
            .WithId(assignmentId)
            .WithRole(Role.Secretariat)
            .Build();
        var activeCandidateListTaskSecretariat = new WorklistTaskBuilder()
            .WithWorklistTaskTypeId(WorklistTaskType.ReadyForFederalCouncilProposal)
            .WithWorklistTaskStateId(WorklistTaskState.Completed)
            .WithAssignedTo(currentAssignmentSecretariat)
            .WithGeneralElectionCommitteeId(_generalElectionCommittee1.Id)
            .Build();
        _generalElectionCommittee1.IsValidated = true;
        _generalElectionCommitteeRepository.GetByCommitteeIdForUpdate(_committeeId).Returns(_generalElectionCommittee1);
        _generalElectionCommittee1.CandidateListState = candidateListState;
        _generalElectionCommittee1.CandidateListStateId = candidateListState.Id;
        _worklistTaskRepository.GetAllByGeneralElectionCommitteeId(_generalElectionCommittee1.Id).Returns([activeCandidateListTaskSecretariat]);

        await _generalElectionCommitteeService.SetFederalCouncilProposalToDirty(_committeeId);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(_generalElectionCommittee1.IsValidated, Is.True);
            Assert.That(_generalElectionCommittee1.IsFederalCouncilProposalDirty, Is.True);
            Assert.That(activeCandidateListTaskSecretariat.WorklistTaskStateId == WorklistTaskState.Active, Is.True);
        }
    }

    [Test]
    public async Task GetAllUnfinishedCommittees_WhenCalled_ShouldLoadData()
    {
        var list = new List<GeneralElectionCommittee>() { _generalElectionCommittee1, _generalElectionCommittee2 };

        _generalElectionCommitteeRepository.GetAll().Returns(list);

        var result = await _generalElectionCommitteeService.GetAllUnfinishedCommittees();

        Assert.That(result, Is.Not.Null);
        Assert.That(result.Count, Is.EqualTo(1));
        Assert.That(result.First().CommitteeId, Is.EqualTo(_committeeId));
    }

    [Test]
    public async Task EndGeneralElectionForCommittee_WhenCalled_ShouldCallService()
    {
        var result = await _generalElectionCommitteeService.EndGeneralElectionForCommittee(_generalElectionCommittee1);

        await _committeeService.Received(1).UpdateCommitteeAfterGeneralElection(Arg.Any<Guid>(), Arg.Any<CommitteeUpdateDto>(), Arg.Any<List<MembershipCandidate>>());
        Assert.That(result, Is.True);
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
