using Bk.APG.Business.Dtos;
using Bk.APG.Business.Models;
using Bk.APG.Business.Repositories;
using Bk.APG.Business.Services;
using Bk.APG.CrossCutting.Tests.Builders;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace Bk.APG.Business.Tests.Services;

[TestFixture]
public class MembershipMirrorServiceTests
{
    private MembershipMirrorService _membershipMirrorService = null!;
    private readonly IMembershipCandidateRepository _membershipCandidateRepository = Substitute.For<IMembershipCandidateRepository>();
    private readonly IMembershipRepository _membershipRepository = Substitute.For<IMembershipRepository>();
    private readonly IAuthorizationService _authorizationService = Substitute.For<IAuthorizationService>();
    private readonly IWorklistTaskRepository _worklistTaskRepository = Substitute.For<IWorklistTaskRepository>();
    private readonly IGeneralElectionCommitteeRepository _generalElectionCommitteeRepository = Substitute.For<IGeneralElectionCommitteeRepository>();
    private readonly ILogger<MembershipMirrorService> _logger = NullLogger<MembershipMirrorService>.Instance;

    private GeneralElectionCommittee _generalElectionCommittee1;
    private Committee _committee;
    private Guid _committeeId;
    private Guid _generalElectionCommitteeId;
    private Guid _departmentId;
    private Department _department;

    [SetUp]
    public void SetUp()
    {
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

        _membershipMirrorService = new MembershipMirrorService(
            _membershipCandidateRepository,
            _membershipRepository,
            _authorizationService,
            _worklistTaskRepository,
            _generalElectionCommitteeRepository,
            _logger);
    }

    [TearDown]
    public void TearDown()
    {
        _membershipCandidateRepository.ClearSubstitute();
    }

    [Test]
    public async Task MirrorOrDeleteMembershipForGeneralElection_WithoutDeleteCandidateWithMetadataChanges_ShouldCommitChangesWithoutJustifications()
    {
        var membershipId = Guid.NewGuid();
        var membership = new MembershipBuilder()
            .WithId(membershipId)
            .WithInCorrelationWithFederalDuty(true)
            .WithJustificationLongerDuty("A")
            .WithJustificationShorterDuty("B")
            .WithJustificationMemberInFederalDuty("C")
            .WithJustificationMemberInFederalAssembly("D")
            .WithRequirementsProfile("E").Build();
        var membershipCandidate = new MembershipCandidateBuilder()
          .WithMembership(membership)
          .WithGeneralElectionCommittee(new GeneralElectionCommitteeBuilder().WithIsValidated(true).WithCandidateListStateId(CandidateListState.ReadyForFederalCouncilProposalForwarded)
          .Build()).Build();

        _membershipCandidateRepository.GetByMembershipIdForUpdate(membershipId).Returns(membershipCandidate);

        await _membershipMirrorService.MirrorOrDeleteMembershipForGeneralElection(membership, false, true);

        Assert.Multiple(() =>
        {
            Assert.That(membershipCandidate.MaximumEmploymentLevel, Is.EqualTo(membership.MaximumEmploymentLevel));
            Assert.That(membershipCandidate.ElectionTypeId, Is.EqualTo(ElectionType.ReElectionGuid));
            Assert.That(membershipCandidate.FunctionId, Is.EqualTo(membership.FunctionId));
            Assert.That(membershipCandidate.ElectionOfficeId, Is.EqualTo(membership.ElectionOfficeId));
            Assert.That(membershipCandidate.MembershipAdditionId, Is.EqualTo(membership.MembershipAdditionId));
            Assert.That(membershipCandidate.Remarks, Is.EqualTo(membership.Remarks));
            Assert.That(membershipCandidate.RemarksStatus, Is.EqualTo(membership.RemarksStatus));
            Assert.That(membershipCandidate.InCorrelationWithFederalDuty, Is.EqualTo(membership.InCorrelationWithFederalDuty));
            Assert.That(membershipCandidate.Modified, Is.EqualTo(membership.Modified));
            Assert.That(membershipCandidate.ModifiedBy, Is.EqualTo(membership.ModifiedBy));
            Assert.That(membershipCandidate.JustificationLongerDuty, Is.Not.EqualTo(membership.JustificationLongerDuty));
            Assert.That(membershipCandidate.JustificationShorterDuty, Is.Not.EqualTo(membership.JustificationShorterDuty));
            Assert.That(membershipCandidate.JustificationMemberInFederalDuty, Is.Not.EqualTo(membership.JustificationMemberInFederalDuty));
            Assert.That(membershipCandidate.JustificationMemberInFederalAssembly, Is.Not.EqualTo(membership.JustificationMemberInFederalAssembly));
            Assert.That(membershipCandidate.RequirementsProfile, Is.Not.EqualTo(membership.RequirementsProfile));
        });

        await _membershipCandidateRepository.Received(1).CommitChanges();
    }

    [Test]
    public async Task MirrorOrDeleteMembershipForGeneralElection_WithoutDeleteCandidateWithMetadataChanges_ShouldCommitChangesWithJustifications()
    {
        var membershipId = Guid.NewGuid();
        var membership = new MembershipBuilder()
            .WithId(membershipId)
            .WithInCorrelationWithFederalDuty(true)
            .WithJustificationLongerDuty("A")
            .WithJustificationShorterDuty("B")
            .WithJustificationMemberInFederalDuty("C")
            .WithJustificationMemberInFederalAssembly("D")
            .WithRequirementsProfile("E").Build();
        var membershipCandidate = new MembershipCandidateBuilder()
          .WithMembership(membership)
          .WithGeneralElectionCommittee(new GeneralElectionCommitteeBuilder().WithIsValidated(false).WithCandidateListStateId(CandidateListState.Draft)
          .Build()).Build();

        _membershipCandidateRepository.GetByMembershipIdForUpdate(membershipId).Returns(membershipCandidate);

        await _membershipMirrorService.MirrorOrDeleteMembershipForGeneralElection(membership, false, true);

        Assert.Multiple(() =>
        {
            Assert.That(membershipCandidate.MaximumEmploymentLevel, Is.EqualTo(membership.MaximumEmploymentLevel));
            Assert.That(membershipCandidate.ElectionTypeId, Is.EqualTo(ElectionType.ReElectionGuid));
            Assert.That(membershipCandidate.FunctionId, Is.EqualTo(membership.FunctionId));
            Assert.That(membershipCandidate.ElectionOfficeId, Is.EqualTo(membership.ElectionOfficeId));
            Assert.That(membershipCandidate.MembershipAdditionId, Is.EqualTo(membership.MembershipAdditionId));
            Assert.That(membershipCandidate.Remarks, Is.EqualTo(membership.Remarks));
            Assert.That(membershipCandidate.RemarksStatus, Is.EqualTo(membership.RemarksStatus));
            Assert.That(membershipCandidate.InCorrelationWithFederalDuty, Is.EqualTo(membership.InCorrelationWithFederalDuty));
            Assert.That(membershipCandidate.Modified, Is.EqualTo(membership.Modified));
            Assert.That(membershipCandidate.ModifiedBy, Is.EqualTo(membership.ModifiedBy));
            Assert.That(membershipCandidate.JustificationLongerDuty, Is.EqualTo(membership.JustificationLongerDuty));
            Assert.That(membershipCandidate.JustificationShorterDuty, Is.EqualTo(membership.JustificationShorterDuty));
            Assert.That(membershipCandidate.JustificationMemberInFederalDuty, Is.EqualTo(membership.JustificationMemberInFederalDuty));
            Assert.That(membershipCandidate.JustificationMemberInFederalAssembly, Is.EqualTo(membership.JustificationMemberInFederalAssembly));
            Assert.That(membershipCandidate.RequirementsProfile, Is.EqualTo(membership.RequirementsProfile));
        });

        await _membershipCandidateRepository.Received(1).CommitChanges();
    }

    [Test]
    public async Task MirrorOrDeleteMembershipForGeneralElection_WithoutDeleteCandidateAndWithoutMetadataChanges_ShouldSkipChanges()
    {
        var membershipId = Guid.NewGuid();
        var membership = new MembershipBuilder()
            .WithId(membershipId)
            .Build();
        var membershipCandidate = new MembershipCandidateBuilder()
          .WithMembership(membership)
          .WithGeneralElectionCommittee(new GeneralElectionCommitteeBuilder().WithIsValidated(true).WithCandidateListStateId(CandidateListState.ReadyForFederalCouncilProposalForwarded)
          .Build()).Build();

        _membershipCandidateRepository.GetByMembershipIdForUpdate(membershipId).Returns(membershipCandidate);

        await _membershipMirrorService.MirrorOrDeleteMembershipForGeneralElection(membership, false, false);

        await _membershipCandidateRepository.DidNotReceiveWithAnyArgs().CommitChanges();
    }

    [Test]
    public async Task MirrorOrDeleteMembershipForGeneralElection_WithValidatedCandidateList_ShouldSkipChanges()
    {
        var membershipId = Guid.NewGuid();
        var membership = new MembershipBuilder().WithId(membershipId).Build();
        var membershipCandidate = new MembershipCandidateBuilder()
            .WithMembership(membership)
            .WithGeneralElectionCommittee(new GeneralElectionCommitteeBuilder().WithIsValidated(true).WithCandidateListStateId(CandidateListState.Validated)
            .Build()).Build();

        _membershipCandidateRepository.GetByMembershipIdForUpdate(membershipId).Returns(membershipCandidate);

        await _membershipMirrorService.MirrorOrDeleteMembershipForGeneralElection(membership, false, false);

        await _membershipCandidateRepository.DidNotReceiveWithAnyArgs().CommitChanges();
    }

    [Test]
    public async Task MirrorOrDeleteMembershipForGeneralElection_WithDeleteCandidate_ShouldCallDelete()
    {
        var membershipId = Guid.NewGuid();
        var membership = new MembershipBuilder().WithId(membershipId).Build();
        var membershipCandidate = new MembershipCandidateBuilder()
          .WithMembership(membership)
          .WithGeneralElectionCommittee(new GeneralElectionCommitteeBuilder().WithIsValidated(false)
          .Build()).Build();

        _membershipCandidateRepository.GetByMembershipIdForUpdate(membershipId).Returns(membershipCandidate);

        await _membershipMirrorService.MirrorOrDeleteMembershipForGeneralElection(membership, true, false);

        await _membershipCandidateRepository.Received(1).Delete(membershipCandidate);
        await _membershipCandidateRepository.Received(0).CommitChanges();
    }

    [Test]
    public async Task CreateNewMembershipFromCandidate_ShouldCallCreate()
    {
        var membership = new MembershipBuilder().Build();
        _membershipRepository.Create(Arg.Any<Membership>()).Returns(membership);

        var createDto = new MembershipCreateDto
        {
            PersonId = Guid.NewGuid(),
            CommitteeId = Guid.NewGuid(),
            MaximumEmploymentLevel = 80,
            BeginDate = DateOnly.FromDateTime(DateTime.Now),
            EndDate = DateOnly.FromDateTime(DateTime.Now),
            ElectionTypeId = Guid.NewGuid(),
            FunctionId = Guid.NewGuid(),
            ElectionOfficeId = Guid.NewGuid(),
            MembershipAdditionId = Guid.NewGuid(),
            JustificationLongerDuty = "JustificationLongerDuty",
            JustificationShorterDuty = "JustificationShorterDuty",
            JustificationMemberInFederalDuty = "JustificationMemberInFederalDuty",
            JustificationMemberInFederalAssembly = "JustificationMemberInFederalAssembly",
            RequirementsProfile = "RequirementsProfile",
            Remarks = "remarks",
            RemarksStatus = "remarksStatus",
        };

        await _membershipMirrorService.CreateNewMembershipFromCandidate(createDto, "BackgroundService");

        await _membershipRepository.Received(1).Create(Arg.Any<Membership>());
        await _membershipCandidateRepository.Received(0).CommitChanges();
    }

    [Test]
    public async Task UpdateMembershipFromCandidate_ShouldCallUpdate()
    {
        var membershipId = Guid.NewGuid();
        var membership = new MembershipBuilder().WithId(membershipId).Build();

        _membershipRepository.GetByIdForUpdate(Arg.Any<Guid>()).Returns(membership);

        var updateDto = new MembershipUpdateDto
        {
            Id = membershipId,
            PersonId = Guid.NewGuid(),
            CommitteeId = Guid.NewGuid(),
            MaximumEmploymentLevel = 80,
            BeginDate = DateOnly.FromDateTime(DateTime.Now),
            EndDate = DateOnly.FromDateTime(DateTime.Now),
            ElectionTypeId = Guid.NewGuid(),
            FunctionId = Guid.NewGuid(),
            ElectionOfficeId = Guid.NewGuid(),
            MembershipAdditionId = Guid.NewGuid(),
            JustificationLongerDuty = "JustificationLongerDuty",
            JustificationShorterDuty = "JustificationShorterDuty",
            JustificationMemberInFederalDuty = "JustificationMemberInFederalDuty",
            JustificationMemberInFederalAssembly = "JustificationMemberInFederalAssembly",
            RequirementsProfile = "RequirementsProfile",
            Remarks = "remarks",
            RemarksStatus = "remarksStatus",
            RowVersion = 1,
        };

        await _membershipMirrorService.UpdateMembershipFromCandidate(membershipId, updateDto, "BackgroundService");
        await _membershipRepository.Received(1).CommitChanges();
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

        await _membershipMirrorService.InvalidateMembershipCandidateList(_committeeId);

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
}
