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
    private readonly ILogger<MembershipMirrorService> _logger = NullLogger<MembershipMirrorService>.Instance;

    [SetUp]
    public void SetUp()
    {
        _membershipMirrorService = new MembershipMirrorService(
            _membershipCandidateRepository,
            _membershipRepository,
            _logger);
    }

    [TearDown]
    public void TearDown()
    {
        _membershipCandidateRepository.ClearSubstitute();
    }

    [Test]
    public async Task MirrorOrDeleteMembershipForGeneralElection_WithoutDeleteCandidate_ShouldCommitChanges()
    {
        var membershipId = Guid.NewGuid();
        var membership = new MembershipBuilder().WithId(membershipId).Build();
        var membershipCandidate = new MembershipCandidateBuilder()
          .WithMembership(membership)
          .WithGeneralElectionCommittee(new GeneralElectionCommitteeBuilder().WithIsValidated(false)
          .Build()).Build();

        _membershipCandidateRepository.GetByMembershipIdForUpdate(membershipId).Returns(membershipCandidate);

        await _membershipMirrorService.MirrorOrDeleteMembershipForGeneralElection(membership, false);

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
    public async Task MirrorOrDeleteMembershipForGeneralElection_WithValidatedCandidateList_ShouldSkipChanges()
    {
        var membershipId = Guid.NewGuid();
        var membership = new MembershipBuilder().WithId(membershipId).Build();
        var membershipCandidate = new MembershipCandidateBuilder()
            .WithMembership(membership)
            .WithGeneralElectionCommittee(new GeneralElectionCommitteeBuilder().WithIsValidated(true).WithCandidateListStateId(CandidateListState.Validated)
            .Build()).Build();

        _membershipCandidateRepository.GetByMembershipIdForUpdate(membershipId).Returns(membershipCandidate);

        await _membershipMirrorService.MirrorOrDeleteMembershipForGeneralElection(membership, false);

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

        await _membershipMirrorService.MirrorOrDeleteMembershipForGeneralElection(membership, true);

        await _membershipCandidateRepository.Received(1).Delete(membershipCandidate);
        await _membershipCandidateRepository.Received(0).CommitChanges();
    }
}
