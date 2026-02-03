using Bk.APG.Business.Models;
using Bk.APG.CrossCutting.Tests.Builders;

namespace Bk.APG.Business.Tests.Models;

[TestFixture]
internal class MembershipCandidateTests
{
    [TestCase(CommitteeType.AuthoritiesCommissionGuidAsString)]
    [TestCase(CommitteeType.AdministrationCommissionGuidAsString)]
    [TestCase(CommitteeType.FederalAgenciesCommitteeGuidAsString)]
    [TestCase(CommitteeType.ManagementCommitteeGuidAsString)]
    public void NeedsAttentionInterests_WhenPersonRequiresAttention_ShouldReturnTrue(string committeeTypeId)
    {
        var person = new PersonBuilder()
            .WithNoInterest(false)
            .WithMemberships([
                new MembershipBuilder()
                    .WithBeginDate(DateOnly.FromDateTime(DateTime.Today.AddDays(-1)))
                    .WithCommittee(new CommitteeBuilder()
                        .WithCommitteeTypeId(new Guid(committeeTypeId)).Build()).Build()
            ])
            .Build();

        var candidate = new MembershipCandidateBuilder()
            .WithPerson(person)
            .WithElectionTypeId(ElectionType.ReElectionGuid)
            .Build();

        Assert.That(candidate.NeedsAttentionInterests, Is.True);
    }

    [Test]
    public void NeedsAttentionInterests_NewElection_WithMissingInterest_ShouldReturnTrue()
    {
        var person = new PersonBuilder()
            .WithNoInterest(false)
            .WithInterests([])
            .Build();

        var candidate = new MembershipCandidateBuilder()
            .WithPerson(person)
            .WithElectionTypeId(ElectionType.NewElectionGuid)
            .WithGeneralElectionCommittee(new GeneralElectionCommitteeBuilder()
                .WithCommitteeType(new CommitteeTypeBuilder().WithId(CommitteeType.AuthoritiesCommissionGuid).Build()).Build())
            .Build();

        Assert.That(candidate.NeedsAttentionInterests, Is.True);
    }

    [Test]
    public void NeedsAttentionInterests_NewElection_WithValidInterest_ShouldReturnFalse()
    {
        var person = new PersonBuilder()
            .WithNoInterest(false)
            .WithInterests([
                new InterestBuilder().WithInterestText("interestText")
                    .WithLegalForm(new LegalFormBuilder().Build())
                    .WithInterestFunction(new InterestFunctionBuilder().Build())
                    .WithInterestCommittee(new InterestCommitteeBuilder().Build()).Build()
            ])
            .Build();

        var candidate = new MembershipCandidateBuilder()
            .WithPerson(person)
            .WithElectionTypeId(ElectionType.NewElectionGuid)
            .Build();

        Assert.That(candidate.NeedsAttentionInterests, Is.False);
    }

    [Test]
    public void NeedsAttentionInterests_NewElection_WithNoInterestFlag_ShouldReturnFalse()
    {
        var person = new PersonBuilder()
            .WithNoInterest(true)
            .Build();

        var candidate = new MembershipCandidateBuilder()
            .WithPerson(person)
            .WithElectionTypeId(ElectionType.NewElectionGuid)
            .Build();

        Assert.That(candidate.NeedsAttentionInterests, Is.False);
    }

    [Test]
    public void NeedsAttentionBasicDataOrOccupation_WhenPersonNeedsAttentionBasicData_ShouldReturnTrue()
    {
        var person = new PersonBuilder()
            .WithOfficeAddress(new AddressBuilder()
                .WithMobile(string.Empty)
                .WithEmail(string.Empty)
                .WithPhone("xyz").Build())
            .Build();

        var candidate = new MembershipCandidateBuilder()
            .WithPerson(person)
            .WithElectionTypeId(ElectionType.ReElectionGuid)
            .Build();

        Assert.That(candidate.NeedsAttentionBasicDataOrOccupation, Is.True);
    }

    [TestCase(CommitteeType.AuthoritiesCommissionGuidAsString)]
    [TestCase(CommitteeType.AdministrationCommissionGuidAsString)]
    [TestCase(CommitteeType.FederalAgenciesCommitteeGuidAsString)]
    [TestCase(CommitteeType.ManagementCommitteeGuidAsString)]
    public void NeedsAttentionBasicDataOrOccupation_WhenPersonNeedsAttentionOccupation_ShouldReturnTrue(string committeeTypeId)
    {
        var person = new PersonBuilder()
            .WithFederalDuty(false)
            .WithNoEmployment(false)
            .WithOccupation(string.Empty)
            .WithEmployer(string.Empty)
            .WithMemberships([
                new MembershipBuilder()
                    .WithBeginDate(DateOnly.FromDateTime(DateTime.Now.AddDays(-3)))
                    .WithEndDate(DateOnly.FromDateTime(DateTime.Now.AddDays(3)))
                    .WithCommittee(new CommitteeBuilder()
                        .WithCommitteeTypeId(new Guid(committeeTypeId)).Build()).Build()
            ])
            .Build();

        var candidate = new MembershipCandidateBuilder()
            .WithPerson(person)
            .WithElectionTypeId(ElectionType.ReElectionGuid)
            .Build();

        Assert.That(candidate.NeedsAttentionBasicDataOrOccupation, Is.True);
    }

    [Test]
    public void NeedsAttentionBasicDataOrOccupation_NewElection_WithMissingOccupationOrEmployer_ShouldReturnTrue()
    {
        var person = new PersonBuilder()
            .WithFederalDuty(false)
            .WithNoEmployment(false)
            .WithOccupations([])
            .WithEmployer(string.Empty)
            .Build();

        var candidate = new MembershipCandidateBuilder()
            .WithPerson(person)
            .WithElectionTypeId(ElectionType.NewElectionGuid)
            .WithGeneralElectionCommittee(new GeneralElectionCommitteeBuilder()
                .WithCommitteeType(new CommitteeTypeBuilder().WithId(CommitteeType.AuthoritiesCommissionGuid).Build()).Build())
            .Build();

        Assert.That(candidate.NeedsAttentionBasicDataOrOccupation, Is.True);
    }

    [Test]
    public void NeedsAttentionBasicDataOrOccupation_NewElection_WithOccupationAndEmployer_ShouldReturnFalse()
    {
        var person = new PersonBuilder()
            .WithFederalDuty(false)
            .WithNoEmployment(false)
            .WithOccupation("Engineer")
            .WithEmployer("Acme")
            .Build();

        var candidate = new MembershipCandidateBuilder()
            .WithPerson(person)
            .WithElectionTypeId(ElectionType.NewElectionGuid)
            .Build();

        Assert.That(candidate.NeedsAttentionBasicDataOrOccupation, Is.False);
    }
}
