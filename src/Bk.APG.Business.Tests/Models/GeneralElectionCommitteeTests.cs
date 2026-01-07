using Bk.APG.Business.Models;
using Bk.APG.CrossCutting.Tests.Builders;

namespace Bk.APG.Business.Tests.Models;

[TestFixture]
internal class GeneralElectionCommitteeTests
{
    [Test]
    public void ActiveMemberCount_WithData_ShouldReturnNumber()
    {
        var committee = new GeneralElectionCommitteeBuilder()
            .WithCommitteeType(new CommitteeTypeBuilder().WithFemaleAndMaleThreshold(40, 40).Build())
            .Build();

        Assert.Multiple(() =>
        {
            Assert.That(committee.FemaleUnderStaffed, Is.False);
            Assert.That(committee.MaleUnderStaffed, Is.False);
        });

        committee.MembershipCandidates.Add(
            new MembershipCandidateBuilder()
                .WithGender(new GenderBuilder().WithUri(Gender.Male).Build())
                .WithIsSelected(true)
                .Build()
        );
        committee.MembershipCandidates.Add(
            new MembershipCandidateBuilder()
                .WithGender(new GenderBuilder().WithUri(Gender.Female).Build())
                .WithIsSelected(true)
                .Build()
        );

        Assert.Multiple(() =>
        {
            Assert.That(committee.ActiveMemberCount, Is.EqualTo(2));
            Assert.That(committee.FemaleCount, Is.EqualTo(1));
            Assert.That(committee.MaleCount, Is.EqualTo(1));
            Assert.That(committee.FemaleQuota, Is.EqualTo(50));
            Assert.That(committee.MaleQuota, Is.EqualTo(50));
            Assert.That(committee.FemaleUnderStaffed, Is.False);
            Assert.That(committee.MaleUnderStaffed, Is.False);
        });
    }

    [Test]
    public void IsJustificationGendersRequired_WhenNotValidated_ShouldReturnFalse()
    {
        var committee = new GeneralElectionCommitteeBuilder()
            .WithIsValidated(false)
            .WithCommitteeType(new CommitteeTypeBuilder().WithFemaleAndMaleThreshold(40, 40).Build())
            .Build();

        Assert.That(committee.IsJustificationGendersRequired, Is.False);
    }

    [Test]
    public void IsJustificationGendersRequired_WhenValidatedAndMaleUnderStaffed_ShouldReturnTrue()
    {
        var committee = new GeneralElectionCommitteeBuilder()
            .WithIsValidated(true)
            .WithCommitteeType(new CommitteeTypeBuilder()
                .WithFemaleAndMaleThreshold(40, 40).Build())
            .Build();

        committee.MembershipCandidates.Add(
            new MembershipCandidateBuilder()
                .WithPerson(new PersonBuilder().WithGender(new GenderBuilder().WithUri(Gender.Female).Build()).Build())
                .WithIsSelected(true)
                .Build());

        Assert.That(committee.IsJustificationGendersRequired, Is.True);
    }

    [Test]
    public void IsJustificationGendersRequired_WhenValidatedAndFemaleUnderStaffed_ShouldReturnTrue()
    {
        var committee = new GeneralElectionCommitteeBuilder()
            .WithIsValidated(true)
            .WithCommitteeType(new CommitteeTypeBuilder()
                .WithFemaleAndMaleThreshold(40, 40).Build())
            .Build();

        committee.MembershipCandidates.Add(
            new MembershipCandidateBuilder()
                .WithPerson(new PersonBuilder().WithGender(new GenderBuilder().WithUri(Gender.Male).Build()).Build())
                .WithIsSelected(true)
                .Build());

        Assert.That(committee.IsJustificationGendersRequired, Is.True);
    }

    [Test]
    public void IsJustificationGendersRequired_WhenValidatedAndGendersBalanced_ShouldReturnFalse()
    {
        var committee = new GeneralElectionCommitteeBuilder()
            .WithIsValidated(true)
            .WithCommitteeType(new CommitteeTypeBuilder()
                .WithFemaleAndMaleThreshold(40, 40).Build())
            .WithMembershipCandidates([
                new MembershipCandidateBuilder()
                    .WithGender(new GenderBuilder().WithUri(Gender.Male).Build())
                    .WithIsSelected(true)
                    .Build(),
                new MembershipCandidateBuilder()
                    .WithGender(new GenderBuilder().WithUri(Gender.Female).Build())
                    .WithIsSelected(true)
                    .Build()
            ])
            .Build();

        Assert.That(committee.IsJustificationGendersRequired, Is.False);
    }

    [Test]
    public void IsJustificationLanguagesRequired_WhenNotValidated_ShouldReturnFalse()
    {
        var committee = new GeneralElectionCommitteeBuilder()
            .WithIsValidated(false)
            .WithCommitteeType(new CommitteeTypeBuilder()
                .WithLanguagesThreshold(1, 1, 1, null).Build())
            .Build();
        Assert.That(committee.IsJustificationLanguagesRequired, Is.False);
    }

    [Test]
    public void IsJustificationLanguagesRequired_WhenValidatedAndGermanUnderStaffed_ShouldReturnTrue()
    {
        var committee = new GeneralElectionCommitteeBuilder()
            .WithIsValidated(true)
            .WithCommitteeType(new CommitteeTypeBuilder()
                .WithLanguagesThreshold(1, 0, 0, null).Build())
            .Build();

        Assert.That(committee.IsJustificationLanguagesRequired, Is.True);
    }

    [Test]
    public void IsJustificationLanguagesRequired_WhenValidatedAndFrenchUnderStaffed_ShouldReturnTrue()
    {
        var committee = new GeneralElectionCommitteeBuilder()
            .WithIsValidated(true)
            .WithCommitteeType(new CommitteeTypeBuilder()
                .WithLanguagesThreshold(0, 1, 0, null).Build())
            .Build();

        Assert.That(committee.IsJustificationLanguagesRequired, Is.True);
    }

    [Test]
    public void IsJustificationLanguagesRequired_WhenValidatedAndItalianUnderStaffed_ShouldReturnTrue()
    {
        var committee = new GeneralElectionCommitteeBuilder()
            .WithIsValidated(true)
            .WithCommitteeType(new CommitteeTypeBuilder()
                .WithLanguagesThreshold(0, 0, 1, null).Build())
            .Build();

        Assert.That(committee.IsJustificationLanguagesRequired, Is.True);
    }

    [Test]
    public void IsJustificationLanguagesRequired_WhenValidatedAndLanguagesBalanced_ShouldReturnFalse()
    {
        var committee = new GeneralElectionCommitteeBuilder()
            .WithIsValidated(true)
            .WithCommitteeType(new CommitteeTypeBuilder()
                .WithLanguagesThreshold(1, 1, 1, null).Build())
            .Build();

        committee.MembershipCandidates.Add(
            new MembershipCandidateBuilder()
                .WithLanguage(new LanguageBuilder().WithUri(Language.GermanUri).Build())
                .WithIsSelected(true)
                .Build());
        committee.MembershipCandidates.Add(
            new MembershipCandidateBuilder()
                .WithLanguage(new LanguageBuilder().WithUri(Language.FrenchUri).Build())
                .WithIsSelected(true)
                .Build());
        committee.MembershipCandidates.Add(
            new MembershipCandidateBuilder()
                .WithLanguage(new LanguageBuilder().WithUri(Language.ItalianUri).Build())
                .WithIsSelected(true)
                .Build());

        Assert.That(committee.IsJustificationLanguagesRequired, Is.False);
    }

    [Test]
    public void JustificationsNeedAttention_WhenGenderJustificationRequiredButMissing_ShouldReturnTrue()
    {
        var committee = new GeneralElectionCommitteeBuilder()
            .WithIsValidated(true)
            .WithCommitteeType(new CommitteeTypeBuilder()
                .WithFemaleAndMaleThreshold(40, 40).Build())
            .Build();
        committee.MembershipCandidates.Add(
            new MembershipCandidateBuilder()
                .WithGender(new GenderBuilder().WithUri(Gender.Male).Build())
                .WithIsSelected(true)
                .Build());

        Assert.That(committee.JustificationsNeedAttention, Is.True);
    }

    [Test]
    public void JustificationsNeedAttention_WhenGenderJustificationProvidedButMeasuresMissing_ShouldReturnTrue()
    {
        var committee = new GeneralElectionCommitteeBuilder()
            .WithIsValidated(true)
            .WithCommitteeType(new CommitteeTypeBuilder()
                .WithFemaleAndMaleThreshold(40, 40).Build())
            .WithJustificationGenders("Some justification")
            .Build();

        committee.MembershipCandidates.Add(
            new MembershipCandidateBuilder()
                .WithGender(new GenderBuilder().WithUri(Gender.Male).Build())
                .WithIsSelected(true)
                .Build());

        Assert.That(committee.JustificationsNeedAttention, Is.True);
    }

    [Test]
    public void JustificationsNeedAttention_WhenLanguageJustificationRequiredButMissing_ShouldReturnTrue()
    {
        var committee = new GeneralElectionCommitteeBuilder()
            .WithIsValidated(true)
            .WithCommitteeType(new CommitteeTypeBuilder()
                .WithLanguagesThreshold(1, 0, 0, null).Build())
            .Build();

        Assert.That(committee.JustificationsNeedAttention, Is.True);
    }

    [Test]
    public void JustificationsNeedAttention_WhenAllJustificationsProvided_ShouldReturnFalse()
    {
        var committee = new GeneralElectionCommitteeBuilder()
            .WithIsValidated(true)
            .WithCommitteeType(new CommitteeTypeBuilder()
                .WithFemaleAndMaleThreshold(40, 40)
                .WithLanguagesThreshold(1, 0, 0, 0)
                .Build())
            .WithJustificationGenders("Justification")
            .WithMeasuresGenders("Measures")
            .WithJustificationLanguages("Justification")
            .WithMeasuresLanguages("Measures")
            .Build();

        committee.MembershipCandidates.Add(
            new MembershipCandidateBuilder()
                .WithPerson(new PersonBuilder().WithGender(new GenderBuilder().WithUri(Gender.Male).Build()).Build())
                .WithIsSelected(true)
                .Build());

        Assert.That(committee.JustificationsNeedAttention, Is.False);
    }

    [Test]
    public void JustificationsNeedAttention_WhenNoJustificationRequired_ShouldReturnFalse()
    {
        var committee = new GeneralElectionCommitteeBuilder()
            .WithIsValidated(true)
            .WithCommitteeType(new CommitteeTypeBuilder()
                .WithFemaleAndMaleThreshold(0, 0)
                .WithLanguagesThreshold(0, 0, 0, 0)
                .Build())
            .WithMembershipCandidates([
                new MembershipCandidateBuilder()
                    .WithPerson(new PersonBuilder().Build())
                    .WithIsSelected(true)
                    .WithGender(new GenderBuilder().WithUri(Gender.Male).Build())
                    .Build(),
                new MembershipCandidateBuilder()
                    .WithPerson(new PersonBuilder().WithGender(new GenderBuilder().WithUri(Gender.Female).Build()).Build())
                    .WithIsSelected(true)
                    .Build()
            ]).Build();

        Assert.That(committee.JustificationsNeedAttention, Is.False);
    }
}
