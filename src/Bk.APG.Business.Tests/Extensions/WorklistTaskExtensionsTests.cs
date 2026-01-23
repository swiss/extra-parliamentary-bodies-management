using Bk.APG.Business.Extensions;
using Bk.APG.Business.Models;
using Bk.APG.CrossCutting.Tests.Builders;

namespace Bk.APG.Business.Tests.Extensions;

[TestFixture]
internal class WorklistTaskExtensionsTests
{
    private WorklistTask _worklistTask = null!;

    [SetUp]
    public void Setup()
    {
        _worklistTask = new WorklistTaskBuilder().Build();
    }

    [TestFixture]
    internal class GetCanBeForwarded : WorklistTaskExtensionsTests
    {
        [Test]
        public void ShouldReturnTrue_WhenAllConditionsAreMet()
        {
            const string currentExternalId = "user123";
            _worklistTask = new WorklistTaskBuilder()
                .WithAssignedTo(new EiamAssignmentBuilder().WithExternalId(currentExternalId).Build())
                .WithWorklistTaskStateId(WorklistTaskState.Active)
                .WithWorklistTaskTypeId(WorklistTaskType.GeneralElectionDispatch)
                .Build();

            var result = _worklistTask.GetCanBeForwarded(currentExternalId);

            Assert.That(result, Is.True);
        }

        [Test]
        public void ShouldReturnFalse_WhenAssignedToExternalIdDoesNotMatch()
        {
            const string currentExternalId = "user123";
            _worklistTask = new WorklistTaskBuilder()
                .WithAssignedTo(new EiamAssignmentBuilder().WithExternalId("differentUser").Build())
                .WithWorklistTaskStateId(WorklistTaskState.Active)
                .WithWorklistTaskTypeId(WorklistTaskType.GeneralElectionDispatch)
                .Build();

            var result = _worklistTask.GetCanBeForwarded(currentExternalId);

            Assert.That(result, Is.False);
        }

        [Test]
        public void ShouldReturnFalse_WhenWorklistTaskStateIsNotActive()
        {
            var currentExternalId = "user123";
            _worklistTask = new WorklistTaskBuilder()
                .WithAssignedTo(new EiamAssignmentBuilder().WithExternalId(currentExternalId).Build())
                .WithWorklistTaskStateId(WorklistTaskState.Completed)
                .WithWorklistTaskTypeId(WorklistTaskType.GeneralElectionDispatch)
                .Build();

            var result = _worklistTask.GetCanBeForwarded(currentExternalId);

            Assert.That(result, Is.False);
        }

        [Test]
        public void ShouldReturnFalse_WhenWorklistTaskTypeIsNotGeneralElectionDispatch()
        {
            var currentExternalId = "user123";
            _worklistTask = new WorklistTaskBuilder()
                .WithAssignedTo(new EiamAssignmentBuilder().WithExternalId(currentExternalId).Build())
                .WithWorklistTaskStateId(WorklistTaskState.Active)
                .WithWorklistTaskTypeId(WorklistTaskType.GeneralElectionStart)
                .Build();

            var result = _worklistTask.GetCanBeForwarded(currentExternalId);

            Assert.That(result, Is.False);
        }
    }

    [TestFixture]
    internal class GetNavigationUrl : WorklistTaskExtensionsTests
    {
        [Test]
        public void ShouldReturnUrl_WhenTaskTypeIsCandidateListCreate()
        {
            var committeeId = Guid.NewGuid();
            _worklistTask = new WorklistTaskBuilder()
                .WithWorklistTaskTypeId(WorklistTaskType.CandidateListCreate)
                .WithCommitteeId(committeeId)
                .Build();

            var result = _worklistTask.GetNavigationUrl();

            Assert.That(result, Is.EqualTo($"/general-election/committees/{committeeId}?tab=candidateList"));
        }

        [Test]
        public void ShouldReturnNull_WhenTaskTypeIsNotCandidateList()
        {
            var committeeId = Guid.NewGuid();
            _worklistTask = new WorklistTaskBuilder()
                .WithWorklistTaskTypeId(WorklistTaskType.GeneralElectionDispatch)
                .WithCommitteeId(committeeId)
                .Build();

            var result = _worklistTask.GetNavigationUrl();

            Assert.That(result, Is.Null);
        }

        [Test]
        public void ShouldReturnUrl_WhenTaskTypeIsCandidateListForward()
        {
            var committeeId = Guid.NewGuid();
            _worklistTask = new WorklistTaskBuilder()
                .WithWorklistTaskTypeId(WorklistTaskType.CandidateListForward)
                .WithCommitteeId(committeeId)
                .Build();

            var result = _worklistTask.GetNavigationUrl();

            Assert.That(result, Is.EqualTo($"/general-election/committees/{committeeId}?tab=candidateList"));
        }

        [Test]
        public void ShouldReturnUrl_WhenTaskTypeIsCandidateListApprove()
        {
            var committeeId = Guid.NewGuid();
            _worklistTask = new WorklistTaskBuilder()
                .WithWorklistTaskTypeId(WorklistTaskType.CandidateListApprove)
                .WithCommitteeId(committeeId)
                .Build();

            var result = _worklistTask.GetNavigationUrl();

            Assert.That(result, Is.EqualTo($"/general-election/committees/{committeeId}?tab=candidateList"));
        }

        [Test]
        public void ShouldReturnUrl_WhenTaskTypeIsReadyForFederalCouncilProposal()
        {
            var committeeId = Guid.NewGuid();
            _worklistTask = new WorklistTaskBuilder()
                .WithWorklistTaskTypeId(WorklistTaskType.ReadyForFederalCouncilProposal)
                .WithCommitteeId(committeeId)
                .Build();

            var result = _worklistTask.GetNavigationUrl();

            Assert.That(result, Is.EqualTo($"/general-election/committees/{committeeId}"));
        }

        [Test]
        public void ShouldReturnUrl_WhenTaskTypeIsGeneralElectionMissingJustifications()
        {
            var committeeId = Guid.NewGuid();
            _worklistTask = new WorklistTaskBuilder()
                .WithWorklistTaskTypeId(WorklistTaskType.GeneralElectionMissingJustifications)
                .WithCommitteeId(committeeId)
                .Build();

            var result = _worklistTask.GetNavigationUrl();

            Assert.That(result, Is.EqualTo($"/general-election/committees/{committeeId}?tab=justifications"));
        }

        [Test]
        public void ShouldReturnUrl_WhenTaskTypeIsGeneralElectionPersonInterests()
        {
            var personId = Guid.NewGuid();
            _worklistTask = new WorklistTaskBuilder()
                .WithWorklistTaskTypeId(WorklistTaskType.GeneralElectionPersonInterests)
                .WithPersonId(personId)
                .Build();

            var result = _worklistTask.GetNavigationUrl();

            Assert.That(result, Is.EqualTo($"/persons/{personId}?tab=interests"));
        }

        [Test]
        public void ShouldReturnUrl_WhenTaskTypeIsGeneralElectionPersonBaseData()
        {
            var personId = Guid.NewGuid();
            _worklistTask = new WorklistTaskBuilder()
                .WithWorklistTaskTypeId(WorklistTaskType.GeneralElectionPersonBaseData)
                .WithPersonId(personId)
                .Build();

            var result = _worklistTask.GetNavigationUrl();

            Assert.That(result, Is.EqualTo($"/persons/{personId}?tab=data"));
        }

        [Test]
        public void ShouldReturnUrl_WhenTaskTypeIsGeneralElectionMembershipValidation()
        {
            var committeeId = Guid.NewGuid();
            var membershipCandidateId = Guid.NewGuid();
            _worklistTask = new WorklistTaskBuilder()
                .WithWorklistTaskTypeId(WorklistTaskType.GeneralElectionMembershipValidation)
                .WithCommitteeId(committeeId)
                .WithMembershipCandidateId(membershipCandidateId)
                .Build();

            var result = _worklistTask.GetNavigationUrl();

            Assert.That(result, Is.EqualTo($"/general-election/committees/{committeeId}/membership-candidate/{membershipCandidateId}"));
        }

        [Test]
        public void ShouldReturnUrl_WhenTaskTypeIsGeneralElectionMissingSecretariat()
        {
            var committeeId = Guid.NewGuid();
            _worklistTask = new WorklistTaskBuilder()
                .WithWorklistTaskTypeId(WorklistTaskType.GeneralElectionMissingSecretariat)
                .WithCommitteeId(committeeId)
                .Build();

            var result = _worklistTask.GetNavigationUrl();

            Assert.That(result, Is.EqualTo($"/committees/{committeeId}?tab=contacts"));
        }

        [Test]
        public void ShouldReturnUrl_WhenTaskTypeIsGeneralElectionMissingDataProtectionOfficer()
        {
            var committeeId = Guid.NewGuid();
            _worklistTask = new WorklistTaskBuilder()
                .WithWorklistTaskTypeId(WorklistTaskType.GeneralElectionMissingDataProtectionOfficer)
                .WithCommitteeId(committeeId)
                .Build();

            var result = _worklistTask.GetNavigationUrl();

            Assert.That(result, Is.EqualTo($"/committees/{committeeId}?tab=contacts"));
        }
    }
}
