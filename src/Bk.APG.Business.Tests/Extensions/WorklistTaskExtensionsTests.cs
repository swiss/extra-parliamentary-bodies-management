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
    internal class GetSection : WorklistTaskExtensionsTests
    {
        [Test]
        public void ShouldReturnPersonNameAndCommitteeDescription_WhenTaskTypeIsPersonRelated()
        {
            var person = new PersonBuilder()
                .WithGivenName("Jane")
                .WithSurname("Doe")
                .Build();
            var committee = new CommitteeBuilder()
                .WithGermanDescription("Committee A")
                .WithFrenchDescription("Committee A")
                .WithItalianDescription("Committee A")
                .WithRomanschDescription("Committee A")
                .Build();
            var personRelatedTaskTypes = new[]
            {
                WorklistTaskType.GeneralElectionPersonBaseData,
                WorklistTaskType.GeneralElectionPersonInterests,
                WorklistTaskType.GeneralElectionMembershipValidation,
            };

            foreach (var worklistTaskTypeId in personRelatedTaskTypes)
            {
                _worklistTask = new WorklistTaskBuilder()
                    .WithWorklistTaskTypeId(worklistTaskTypeId)
                    .Build();
                _worklistTask.Person = person;
                _worklistTask.PersonId = person.Id;
                _worklistTask.Committee = committee;
                _worklistTask.CommitteeId = committee.Id;

                var result = _worklistTask.GetSection();

                Assert.That(result, Is.EqualTo("Jane Doe; Committee A"), $"Failed for task type: {worklistTaskTypeId}");
            }
        }

        [Test]
        public void ShouldReturnCommitteeDescription_WhenTaskTypeIsNotPersonRelated()
        {
            var committee = new CommitteeBuilder()
                .WithGermanDescription("Committee B")
                .WithFrenchDescription("Committee B")
                .WithItalianDescription("Committee B")
                .WithRomanschDescription("Committee B")
                .Build();
            _worklistTask = new WorklistTaskBuilder()
                .WithWorklistTaskTypeId(WorklistTaskType.GeneralElectionDispatch)
                .Build();
            _worklistTask.Committee = committee;
            _worklistTask.CommitteeId = committee.Id;

            var result = _worklistTask.GetSection();

            Assert.That(result, Is.EqualTo("Committee B"));
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

            Assert.That(result, Is.EqualTo($"/general-election/committees/{committeeId}?tab=data"));
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

        [Test]
        public void ShouldReturnUrl_WhenTaskTypeIsGeneralMeasureCheck()
        {
            _worklistTask = new WorklistTaskBuilder()
                .WithWorklistTaskTypeId(WorklistTaskType.GeneralMeasureCheck)
                .Build();

            var result = _worklistTask.GetNavigationUrl();

            Assert.That(result, Is.EqualTo("/administration/generalMeasures"));
        }

        [Test]
        public void ShouldReturnUrl_WhenTaskTypeIsGeneralMeasureValidate()
        {
            _worklistTask = new WorklistTaskBuilder()
                .WithWorklistTaskTypeId(WorklistTaskType.GeneralMeasureValidate)
                .Build();

            var result = _worklistTask.GetNavigationUrl();

            Assert.That(result, Is.EqualTo("/administration/generalMeasures"));
        }
    }
}
