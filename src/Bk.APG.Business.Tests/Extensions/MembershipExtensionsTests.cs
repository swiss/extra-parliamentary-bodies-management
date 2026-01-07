using Bk.APG.Business.Extensions;
using Bk.APG.Business.Models;
using Bk.APG.CrossCutting.Tests.Builders;

namespace Bk.APG.Business.Tests.Extensions;

[TestFixture]
internal class MembershipExtensionsTests
{
    private Membership _membership = null!;

    [SetUp]
    public void Setup()
    {
        _membership = new MembershipBuilder().Build();
    }

    [Test]
    public void GetEmploymentLevel_PartTime_WithMaxEmploymentLevel_ReturnsRange()
    {
        _membership.MaximumEmploymentLevel = 80;
        Assert.That(_membership.GetEmploymentLevel(), Is.EqualTo("80%"));
    }

    [Test]
    public void GetEmploymentLevel_PartTime_WithoutMaximum_ReturnsZero()
    {
        _membership.MaximumEmploymentLevel = 0;
        Assert.That(_membership.GetEmploymentLevel(), Is.EqualTo("0%"));
    }

    [Test]
    public void GetEmploymentLevel_WhenEmpty_ReturnsZero()
    {
        _membership.MaximumEmploymentLevel = null;
        Assert.That(_membership.GetEmploymentLevel(), Is.EqualTo("0%"));
    }

    [Test]
    public void GetQuotas_WhenEmpty_ReturnsZeroQuotas()
    {
        var committee = new CommitteeBuilder()
            .WithMembership(new MembershipBuilder().WithBeginDate(DateOnly.FromDateTime(DateTime.Now.AddDays(-3))).WithEndDate(DateOnly.FromDateTime(DateTime.Now.AddDays(-2)))
            .WithPerson(new PersonBuilder().WithLanguage(new LanguageBuilder().Build()).WithGender(new GenderBuilder().Build()).Build())
            .WithCommittee(new CommitteeBuilder().WithCommitteeType(new CommitteeTypeBuilder().WithFemaleAndMaleThreshold(10, 10).WithLanguagesPercentageThreshold(20, 20, 20, 5).Build()).Build()).Build())
            .Build();

        var result = committee.GetQuotas();

        Assert.Multiple(() =>
        {
            Assert.That(result.MembersCount, Is.EqualTo(0));
            Assert.That(result.FemaleQuota, Is.EqualTo(0));
            Assert.That(result.MaleQuota, Is.EqualTo(0));
            Assert.That(result.GermanQuota, Is.EqualTo(0));
            Assert.That(result.FrenchQuota, Is.EqualTo(0));
            Assert.That(result.ItalianQuota, Is.EqualTo(0));
            Assert.That(result.RomanshQuota, Is.EqualTo(0));
        });
    }

    [Test]
    public void GetQuotas_WithMembers_CalculatesQuotasCorrectly()
    {
        var committee = new CommitteeBuilder()
                .WithMembership(new MembershipBuilder()
                    .WithBeginDate(DateOnly.FromDateTime(DateTime.Now.AddDays(-3)))
                    .WithEndDate(DateOnly.FromDateTime(DateTime.Now.AddDays(30)))
                    .WithPerson(new PersonBuilder()
                        .WithGender(new GenderBuilder().WithUri(Gender.Female).Build())
                        .WithLanguage(new LanguageBuilder().WithUri(Language.GermanUri).Build()).Build()).Build())
                .WithMembership(new MembershipBuilder()
                    .WithBeginDate(DateOnly.FromDateTime(DateTime.Now.AddDays(-3)))
                    .WithEndDate(DateOnly.FromDateTime(DateTime.Now.AddDays(30)))
                    .WithPerson(new PersonBuilder()
                        .WithGender(new GenderBuilder().WithUri(Gender.Female).Build())
                        .WithLanguage(new LanguageBuilder().WithUri(Language.FrenchUri).Build()).Build()).Build())
                .WithMembership(new MembershipBuilder()
                    .WithBeginDate(DateOnly.FromDateTime(DateTime.Now.AddDays(-3)))
                    .WithEndDate(DateOnly.FromDateTime(DateTime.Now.AddDays(30)))
                    .WithPerson(new PersonBuilder()
                        .WithGender(new GenderBuilder().WithUri(Gender.Male).Build())
                        .WithLanguage(new LanguageBuilder().WithUri(Language.ItalianUri).Build()).Build()).Build())
                .WithMembership(new MembershipBuilder()
                    .WithBeginDate(DateOnly.FromDateTime(DateTime.Now.AddDays(-3)))
                    .WithEndDate(DateOnly.FromDateTime(DateTime.Now.AddDays(30)))
                    .WithPerson(new PersonBuilder()
                        .WithGender(new GenderBuilder().WithUri(Gender.Male).Build())
                        .WithLanguage(new LanguageBuilder().WithUri(Language.RomanshUri).Build()).Build()).Build())
                .WithCommitteeType(new CommitteeTypeBuilder().WithFemaleAndMaleThreshold(10, 10).WithLanguagesPercentageThreshold(20, 20, 20, 5).Build())
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

}
