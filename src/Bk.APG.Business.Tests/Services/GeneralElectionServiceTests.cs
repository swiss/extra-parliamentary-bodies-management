using Bk.APG.Business.Dtos;
using Bk.APG.Business.Models;
using Bk.APG.Business.Repositories;
using Bk.APG.Business.Services;
using Bk.APG.CrossCutting.Exception;
using Bk.APG.CrossCutting.Tests.Builders;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace Bk.APG.Business.Tests.Services;

[TestFixture]
public class GeneralElectionServiceTests
{
    private GeneralElectionService _generalElectionService = null!;
    private readonly IAuthorizationService _authorizationService = Substitute.For<IAuthorizationService>();
    private readonly IWorklistTaskService _worklistTaskService = Substitute.For<IWorklistTaskService>();
    private readonly IMasterDataRepository _masterDataRepository = Substitute.For<IMasterDataRepository>();
    private readonly ITermOfOfficeDateService _termOfOfficeDateService = Substitute.For<ITermOfOfficeDateService>();
    private readonly IEiamAssignmentService _eiamAssignmentService = Substitute.For<IEiamAssignmentService>();
    private readonly IGeneralElectionCommitteeService _generalElectionCommitteeService = Substitute.For<IGeneralElectionCommitteeService>();
    private readonly IGeneralElectionCommitteeRepository _generalElectionCommitteeRepository = Substitute.For<IGeneralElectionCommitteeRepository>();
    private readonly ICommitteeRepository _committeeRepository = Substitute.For<ICommitteeRepository>();
    private readonly IMembershipRepository _membershipRepository = Substitute.For<IMembershipRepository>();
    private readonly IMembershipCandidateRepository _membershipCandidateRepository = Substitute.For<IMembershipCandidateRepository>();
    private readonly IMembershipCandidateLogMessageRepository _membershipCandidateLogMessageRepository = Substitute.For<IMembershipCandidateLogMessageRepository>();
    private readonly ILogger<GeneralElectionService> _logger = NullLogger<GeneralElectionService>.Instance;

    private readonly Guid _zeroGuid = Guid.Empty;

    [SetUp]
    public void SetUp()
    {
        _termOfOfficeDateService.CheckForRunningGeneralElection().Returns(true);

        _generalElectionService = new GeneralElectionService(
            _authorizationService,
            _worklistTaskService,
            _eiamAssignmentService,
            _masterDataRepository,
            _termOfOfficeDateService,
            _generalElectionCommitteeService,
            _generalElectionCommitteeRepository,
            _committeeRepository,
            _membershipRepository,
            _membershipCandidateRepository,
            _membershipCandidateLogMessageRepository,
            _logger);
    }

    [TearDown]
    public void TearDown()
    {
        _authorizationService.ClearSubstitute();
        _worklistTaskService.ClearSubstitute();
        _termOfOfficeDateService.ClearSubstitute();
        _generalElectionCommitteeRepository.ClearSubstitute();
        _committeeRepository.ClearSubstitute();
        _membershipRepository.ClearSubstitute();
        _membershipCandidateRepository.ClearSubstitute();
        _membershipCandidateLogMessageRepository.ClearSubstitute();
    }

    [Test]
    public async Task IsGeneralElectionAvailable_WhenGeneralElectionIsClosed_ReturnsFalse()
    {
        _termOfOfficeDateService.CheckForRunningGeneralElection().Returns(false);

        var result = await _generalElectionService.IsGeneralElectionToggleAvailable();

        Assert.That(result, Is.False);
    }

    [Test]
    public async Task IsGeneralElectionAvailable_WhenUserIsAdmin_ReturnsTrue()
    {
        _authorizationService.IsAdmin.Returns(true);

        var result = await _generalElectionService.IsGeneralElectionToggleAvailable();

        Assert.That(result, Is.True);
    }

    [Test]
    public async Task IsGeneralElectionAvailable_WhenUserIsObserver_ReturnsTrue()
    {
        _authorizationService.IsAdmin.Returns(false);
        _authorizationService.IsObserver.Returns(true);

        var result = await _generalElectionService.IsGeneralElectionToggleAvailable();

        Assert.That(result, Is.True);
    }

    [Test]
    public async Task IsGeneralElectionAvailable_WhenUserIsSecretariatAndHasCommitteeInGeneralElection_ReturnsTrue()
    {
        _authorizationService.IsAdmin.Returns(false);
        _authorizationService.IsObserver.Returns(false);

        var committees = new List<Committee>
        {
            new CommitteeBuilder()
                .WithCommitteeLevelId(CommitteeLevel.FederalCouncilGuid)
                .WithTermOfOfficeId(TermOfOffice.Period4YearsInGeneralElectionGuid)
                .Build()
        };

        _authorizationService.LoadCommittees().Returns(committees);

        var result = await _generalElectionService.IsGeneralElectionToggleAvailable();

        Assert.That(result, Is.True);
    }

    [Test]
    public async Task IsGeneralElectionAvailable_WhenUserIsSecretariatAndHasNoCommitteeInGeneralElection_ReturnsFalse()
    {
        _authorizationService.IsAdmin.Returns(false);
        _authorizationService.IsObserver.Returns(false);

        var committees = new List<Committee>
        {
            new CommitteeBuilder()
                .WithCommitteeLevelId(Guid.NewGuid())
                .WithTermOfOfficeId(Guid.NewGuid())
                .Build()
        };

        _authorizationService.LoadCommittees().Returns(committees);

        var result = await _generalElectionService.IsGeneralElectionToggleAvailable();

        Assert.That(result, Is.False);
    }

    [Test]
    public async Task IsGeneralElectionAvailable_WhenUserIsSecretariatAndHasEmptyCommitteeList_ReturnsFalse()
    {
        _authorizationService.IsAdmin.Returns(false);
        _authorizationService.IsObserver.Returns(false);
        _authorizationService.LoadCommittees().Returns(new List<Committee>());

        var result = await _generalElectionService.IsGeneralElectionToggleAvailable();

        Assert.That(result, Is.False);
    }

    [Test]
    public void PrepareGeneralElection_WhenAlreadyRunning_ThrowsException()
    {
        _termOfOfficeDateService.CheckForRunningGeneralElection().Returns(true);

        Assert.That(async () => await _generalElectionService.PrepareGeneralElection(CreateTestData()), Throws.Exception.InstanceOf<BusinessValidationException>());
    }

    [Test]
    public async Task PrepareGeneralElection_WithRecordsInDatabase_CallsServices()
    {
        var nextTermOfOfficeDate = new TermOfOfficeDateBuilder().Build();

        _termOfOfficeDateService.CheckForRunningGeneralElection().Returns(false);
        _termOfOfficeDateService.GetNextTermOfOfficeDate().Returns(nextTermOfOfficeDate);
        _worklistTaskService.CreateWorklistTaskByAdmin(Arg.Any<WorklistTaskCreateDto>()).Returns(new WorklistTaskBuilder().Build());

        await _generalElectionService.PrepareGeneralElection(CreateTestData());

        await _termOfOfficeDateService.Received(1).Update(nextTermOfOfficeDate);
    }

    [Test]
    public async Task StartGeneralElection_WhenCalled_CallsServicesAndCreateData()
    {
        var termOfOfficeDateId = Guid.NewGuid();
        var beginDate = new DateOnly(2028, 1, 1);
        var endDate = new DateOnly(2031, 12, 31);
        var dueDate = new DateOnly(207, 4, 14);
        const string description = "Start";

        var committees = new List<Committee>();

        var committee1 = new CommitteeBuilder().Build();
        var committee2 = new CommitteeBuilder().Build();
        committees.Add(committee1);
        committees.Add(committee2);

        var person1 = new PersonBuilder().Build();
        var person2 = new PersonBuilder().Build();

        var memberships = new List<Membership>
        {
            new MembershipBuilder().WithPerson(person1).Build(),
            new MembershipBuilder().WithPerson(person2).Build()
        };

        var parentId = Guid.NewGuid();
        var departments = new List<Department>
        {
            new DepartmentBuilder().Build(),
            new DepartmentBuilder().Build(),
        };

        _committeeRepository.GetAllForGeneralElection(_zeroGuid, _zeroGuid, _zeroGuid).Returns(committees);
        _worklistTaskService.CreateWorklistTaskByAdmin(Arg.Any<WorklistTaskCreateDto>()).Returns(new WorklistTaskBuilder().WithId(parentId).Build());
        _generalElectionCommitteeRepository.Create(Arg.Any<GeneralElectionCommittee>()).Returns(new GeneralElectionCommitteeBuilder().Build());
        _membershipRepository.GetAllActiveMembershipsForCommittee(Arg.Any<Guid>()).Returns(memberships);
        _masterDataRepository.GetDepartments().Returns(departments);

        await _generalElectionService.StartGeneralElection(termOfOfficeDateId, beginDate, endDate, dueDate, description);

        await _generalElectionCommitteeRepository.Received(1).DeleteAll();
        await _generalElectionCommitteeRepository.Received(2).Create(Arg.Any<GeneralElectionCommittee>());
        await _membershipCandidateRepository.Received(4).Create(Arg.Any<MembershipCandidate>());
        await _worklistTaskService.Received(1 + (departments.Count * 3)).CreateWorklistTaskByAdmin(Arg.Any<WorklistTaskCreateDto>());
        await _worklistTaskService.Received(departments.Count).CreateWorklistTaskByAdmin(Arg.Is<WorklistTaskCreateDto>(
            x => x.WorklistTaskTypeId == WorklistTaskType.GeneralMeasureCheck && x.WorklistTaskStateId == WorklistTaskState.Active));
        await _worklistTaskService.Received(departments.Count).CreateWorklistTaskByAdmin(Arg.Is<WorklistTaskCreateDto>(
            x => x.WorklistTaskTypeId == WorklistTaskType.GeneralMeasureValidate && x.WorklistTaskStateId == WorklistTaskState.Inactive));
    }

    [Test]
    public async Task CreateNewMembershipCandidate_WhenCalled_ShouldCallCreate()
    {
        var membershipId = Guid.NewGuid();
        var committeeId = Guid.NewGuid();
        var personId = Guid.NewGuid();

        var termOfOfficeDate = new TermOfOfficeDateBuilder().WithBeginDate(new DateOnly(2025, 1, 1)).WithEndDate(new DateOnly(2029, 12, 31)).Build();
        var person = new PersonBuilder().WithId(personId).Build();
        var membership = new MembershipBuilder().WithId(membershipId).WithCommitteeId(committeeId).WithPerson(person).Build();
        var generalElectionCommittee = new GeneralElectionCommitteeBuilder().WithCommitteeId(committeeId).WithTermOfOfficeDate(termOfOfficeDate).Build();

        _generalElectionCommitteeRepository.GetByCommitteeId(membership.CommitteeId).Returns(generalElectionCommittee);
        _authorizationService.GetCurrentUserName().Returns("FritzTester");

        await _generalElectionService.CreateNewMembershipCandidate(membership, "username");

        await _membershipCandidateRepository.Received(1).Create(Arg.Any<MembershipCandidate>());
        await _membershipRepository.Received(0).GetAllMembershipsForCommitteeAndPerson(committeeId, personId);
    }

    [Test]
    public async Task CreateNewMembershipCandidate_When16YearsDurationIsReached_ShouldWriteLogEntryAndNotCallCreate()
    {
        var membershipId = Guid.NewGuid();
        var committeeId = Guid.NewGuid();
        var personId = Guid.NewGuid();

        var firstMembership = new MembershipBuilder().WithBeginDate(new DateOnly(2000, 1, 1)).WithEndDate(new DateOnly(2008, 12, 31)).Build();
        var secondMembership = new MembershipBuilder().WithBeginDate(new DateOnly(2010, 1, 1)).WithEndDate(new DateOnly(2020, 12, 31)).Build();
        var oldMemberships = new List<Membership>
        {
            firstMembership,
            secondMembership
        };

        var termOfOfficeDate = new TermOfOfficeDateBuilder().WithBeginDate(new DateOnly(2025, 1, 1)).WithEndDate(new DateOnly(2029, 12, 31)).Build();
        var person = new PersonBuilder().WithId(personId).Build();
        var membership = new MembershipBuilder().WithId(membershipId).WithCommitteeId(committeeId).WithPerson(person).WithInCorrelationWithFederalDuty(false).Build();
        var generalElectionCommittee = new GeneralElectionCommitteeBuilder()
            .WithCommitteeId(committeeId)
            .WithTermOfOfficeDate(termOfOfficeDate)
            .WithCommitteeTypeId(CommitteeType.AuthoritiesCommissionGuid)
            .Build();

        _generalElectionCommitteeRepository.GetByCommitteeId(membership.CommitteeId).Returns(generalElectionCommittee);
        _membershipRepository.GetAllMembershipsForCommitteeAndPerson(committeeId, personId).Returns(oldMemberships);
        _authorizationService.GetCurrentUserName().Returns("FritzTester");

        await _generalElectionService.CreateNewMembershipCandidate(membership, "username");

        await _membershipCandidateRepository.Received(0).Create(Arg.Any<MembershipCandidate>());
        await _membershipCandidateLogMessageRepository.Received(1).Create(Arg.Any<MembershipCandidateLogMessage>());
    }

    [Test]
    public async Task CreateNewMembershipCandidate_WhenBetween12And15YearsDuration_ShouldCallCreate()
    {
        var membershipId = Guid.NewGuid();
        var committeeId = Guid.NewGuid();
        var personId = Guid.NewGuid();

        var firstMembership = new MembershipBuilder().WithBeginDate(new DateOnly(2000, 1, 1)).WithEndDate(new DateOnly(2008, 12, 31)).Build();
        var secondMembership = new MembershipBuilder().WithBeginDate(new DateOnly(2010, 1, 1)).WithEndDate(new DateOnly(2015, 12, 31)).Build();
        var oldMemberships = new List<Membership>
        {
            firstMembership,
            secondMembership
        };

        var termOfOfficeDate = new TermOfOfficeDateBuilder().WithBeginDate(new DateOnly(2025, 1, 1)).WithEndDate(new DateOnly(2029, 12, 31)).Build();
        var person = new PersonBuilder().WithId(personId).Build();
        var membership = new MembershipBuilder().WithId(membershipId).WithCommitteeId(committeeId).WithPerson(person).WithInCorrelationWithFederalDuty(false).Build();
        var generalElectionCommittee = new GeneralElectionCommitteeBuilder()
            .WithCommitteeId(committeeId)
            .WithTermOfOfficeDate(termOfOfficeDate)
            .WithCommitteeTypeId(CommitteeType.AuthoritiesCommissionGuid)
            .Build();

        _generalElectionCommitteeRepository.GetByCommitteeId(membership.CommitteeId).Returns(generalElectionCommittee);
        _membershipRepository.GetAllMembershipsForCommitteeAndPerson(committeeId, personId).Returns(oldMemberships);
        _authorizationService.GetCurrentUserName().Returns("FritzTester");

        await _generalElectionService.CreateNewMembershipCandidate(membership, "username");

        await _membershipCandidateRepository.Received(1).Create(Arg.Any<MembershipCandidate>());
        await _membershipCandidateLogMessageRepository.Received(0).Create(Arg.Any<MembershipCandidateLogMessage>());
    }

    private static WorklistTaskCreateDto CreateTestData()
    {
        return new WorklistTaskCreateDto
        {
            AssignedToId = Guid.NewGuid(),
            WorklistTaskTypeId = Guid.NewGuid(),
            WorklistTaskStateId = Guid.NewGuid(),
            Description = "description",
            DepartmentId = Guid.NewGuid(),
            OfficeId = Guid.NewGuid(),
            ParentTaskId = Guid.NewGuid(),
            CommitteeId = Guid.NewGuid(),
            MembershipId = Guid.NewGuid(),
            PersonId = Guid.NewGuid(),
            GeneralElectionCommitteeId = Guid.NewGuid(),
            MembershipCandidateId = Guid.NewGuid(),
            TermOfOfficeDateId = Guid.NewGuid(),
            DueDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(-300))
        };
    }
}
