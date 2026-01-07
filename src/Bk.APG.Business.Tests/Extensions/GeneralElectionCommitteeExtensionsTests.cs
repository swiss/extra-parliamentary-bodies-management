using Bk.APG.Business.Extensions;
using Bk.APG.Business.Models;
using Bk.APG.CrossCutting.Tests.Builders;

namespace Bk.APG.Business.Tests.Extensions;

[TestFixture]
internal class GeneralElectionCommitteeExtensionsTests
{
    [Test]
    public void GetQuotas_WhenNoCandidates_ReturnsZeroQuotas()
    {
        var committeeType = new CommitteeTypeBuilder()
            .WithFemaleAndMaleThreshold(30, 30)
            .WithLanguagesPercentageThreshold(60, 20, 10, 5)
            .Build();

        var committee = new GeneralElectionCommitteeBuilder()
            .WithCommitteeType(committeeType)
            .Build();

        var result = committee.GetQuotas();

        Assert.Multiple(() =>
        {
            Assert.That(result.MembersCount, Is.EqualTo(0));
            Assert.That(result.IsPercentageBased, Is.True);
            Assert.That(result.FemaleQuota, Is.EqualTo(0));
            Assert.That(result.MaleQuota, Is.EqualTo(0));
            Assert.That(result.GermanQuota, Is.EqualTo(0));
            Assert.That(result.FrenchQuota, Is.EqualTo(0));
            Assert.That(result.ItalianQuota, Is.EqualTo(0));
            Assert.That(result.RomanshQuota, Is.EqualTo(0));
        });
    }

    [Test]
    public void GetQuotas_WithPercentageBasedThresholds_ReturnsPercentageQuotas()
    {
        var committeeType = new CommitteeTypeBuilder()
            .WithFemaleAndMaleThreshold(40, 40)
            .WithLanguagesPercentageThreshold(50, 30, 15, 5)
            .Build();

        var membershipCandidates = new List<MembershipCandidate>
        {
            new MembershipCandidateBuilder()
                .WithIsSelected(true)
                .WithGender(new GenderBuilder().WithUri(Gender.Female).Build())
                .WithLanguage(new LanguageBuilder().WithUri(Language.GermanUri).Build())
                .Build(),
            new MembershipCandidateBuilder()
                .WithIsSelected(true)
                .WithGender(new GenderBuilder().WithUri(Gender.Male).Build())
                .WithLanguage(new LanguageBuilder().WithUri(Language.FrenchUri).Build())
                .Build()
        };

        var committee = new GeneralElectionCommitteeBuilder()
            .WithCommitteeType(committeeType)
            .WithMembershipCandidates(membershipCandidates)
            .Build();

        var result = committee.GetQuotas();

        Assert.Multiple(() =>
        {
            Assert.That(result.MembersCount, Is.EqualTo(2));
            Assert.That(result.IsPercentageBased, Is.True);
            Assert.That(result.PercentageLabel, Is.EqualTo(" % "));
            Assert.That(result.FemaleThreshold, Is.EqualTo(40));
            Assert.That(result.MaleThreshold, Is.EqualTo(40));
            Assert.That(result.GermanThreshold, Is.EqualTo(50));
            Assert.That(result.FrenchThreshold, Is.EqualTo(30));
            Assert.That(result.ItalianThreshold, Is.EqualTo(15));
            Assert.That(result.RomanshThreshold, Is.EqualTo(5));
        });
    }

    [Test]
    public void GetQuotas_WithMinimalThresholds_ReturnsCountBasedQuotas()
    {
        var committeeType = new CommitteeTypeBuilder()
            .WithFemaleAndMaleThreshold(30, 30)
            .WithLanguagesThreshold(5, 3, 2, 1)
            .WithLanguagesPercentageThreshold(null, null, null, null)
            .Build();

        var membershipCandidate = new MembershipCandidateBuilder()
            .WithIsSelected(true)
            .WithGender(new GenderBuilder().WithUri(Gender.Female).Build())
            .WithLanguage(new LanguageBuilder().WithUri(Language.GermanUri).Build())
            .Build();

        var committee = new GeneralElectionCommitteeBuilder()
            .WithCommitteeType(committeeType)
            .WithMembershipCandidate(membershipCandidate)
            .Build();

        var result = committee.GetQuotas();

        Assert.Multiple(() =>
        {
            Assert.That(result.IsPercentageBased, Is.False);
            Assert.That(result.PercentageLabel, Is.EqualTo(" "));
            Assert.That(result.GermanThreshold, Is.EqualTo(5));
            Assert.That(result.FrenchThreshold, Is.EqualTo(3));
            Assert.That(result.ItalianThreshold, Is.EqualTo(2));
            Assert.That(result.RomanshThreshold, Is.EqualTo(1));
        });
    }

    [Test]
    public void GetQuotas_WithMultipleCandidates_CalculatesQuotasCorrectly()
    {
        var committeeType = new CommitteeTypeBuilder()
            .WithFemaleAndMaleThreshold(40, 40)
            .WithLanguagesPercentageThreshold(40, 30, 20, 10)
            .Build();

        var membershipCandidates = new List<MembershipCandidate>
        {
            new MembershipCandidateBuilder()
                .WithIsSelected(true)
                .WithGender(new GenderBuilder().WithUri(Gender.Female).Build())
                .WithLanguage(new LanguageBuilder().WithUri(Language.GermanUri).Build())
                .Build(),
            new MembershipCandidateBuilder()
                .WithIsSelected(true)
                .WithGender(new GenderBuilder().WithUri(Gender.Female).Build())
                .WithLanguage(new LanguageBuilder().WithUri(Language.FrenchUri).Build())
                .Build(),
            new MembershipCandidateBuilder()
                .WithIsSelected(true)
                .WithGender(new GenderBuilder().WithUri(Gender.Male).Build())
                .WithLanguage(new LanguageBuilder().WithUri(Language.ItalianUri).Build())
                .Build(),
            new MembershipCandidateBuilder()
                .WithIsSelected(true)
                .WithGender(new GenderBuilder().WithUri(Gender.Male).Build())
                .WithLanguage(new LanguageBuilder().WithUri(Language.RomanshUri).Build())
                .Build()
        };

        var committee = new GeneralElectionCommitteeBuilder()
            .WithCommitteeType(committeeType)
            .WithMembershipCandidates(membershipCandidates)
            .Build();

        var result = committee.GetQuotas();

        Assert.Multiple(() =>
        {
            Assert.That(result.MembersCount, Is.EqualTo(4));
            Assert.That(result.FemaleQuota, Is.EqualTo(50));
            Assert.That(result.MaleQuota, Is.EqualTo(50));
            Assert.That(result.GermanQuota, Is.EqualTo(25));
            Assert.That(result.FrenchQuota, Is.EqualTo(25));
            Assert.That(result.ItalianQuota, Is.EqualTo(25));
            Assert.That(result.RomanshQuota, Is.EqualTo(25));
        });
    }

    [Test]
    public void GetQuotas_IgnoresDeletedCandidates()
    {
        var committeeType = new CommitteeTypeBuilder()
            .WithFemaleAndMaleThreshold(50, 50)
            .WithLanguagesPercentageThreshold(50, 25, 15, 10)
            .Build();

        var membershipCandidates = new List<MembershipCandidate>
        {
            new MembershipCandidateBuilder()
                .WithIsSelected(true)
                .WithIsDeleted(false)
                .WithGender(new GenderBuilder().WithUri(Gender.Female).Build())
                .WithLanguage(new LanguageBuilder().WithUri(Language.GermanUri).Build())
                .Build(),
            new MembershipCandidateBuilder()
                .WithIsSelected(true)
                .WithIsDeleted(true)
                .WithGender(new GenderBuilder().WithUri(Gender.Male).Build())
                .WithLanguage(new LanguageBuilder().WithUri(Language.FrenchUri).Build())
                .Build()
        };

        var committee = new GeneralElectionCommitteeBuilder()
            .WithCommitteeType(committeeType)
            .WithMembershipCandidates(membershipCandidates)
            .Build();

        var result = committee.GetQuotas();

        Assert.Multiple(() =>
        {
            Assert.That(result.MembersCount, Is.EqualTo(1));
            Assert.That(result.FemaleQuota, Is.EqualTo(100));
            Assert.That(result.MaleQuota, Is.EqualTo(0));
        });
    }
}
