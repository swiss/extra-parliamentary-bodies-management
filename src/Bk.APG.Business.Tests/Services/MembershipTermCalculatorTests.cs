using Bk.APG.Business.Models;
using Bk.APG.Business.Services;
using Bk.APG.CrossCutting.Tests.Builders;

namespace Bk.APG.Business.Tests.Services;

[TestFixture]
internal class MembershipTermCalculatorTests
{
    [Test]
    public void CalculateCurrentTermInYears_EmptyList_ReturnsZero()
    {
        var result = MembershipTermCalculator.CalculateCurrentTermInYears([]);

        Assert.That(result, Is.EqualTo(0));
    }

    [Test]
    public void CalculateCurrentTermInYears_SingleMembership_ExactYears_ReturnsCorrectYears()
    {
        var memberships = new List<Membership>
        {
            new MembershipBuilder()
                .WithBeginDate(new DateOnly(2020, 1, 1))
                .WithEndDate(new DateOnly(2023, 1, 1))
                .Build()
        };

        var result = MembershipTermCalculator.CalculateCurrentTermInYears(memberships);

        Assert.That(result, Is.EqualTo(3));
    }

    [Test]
    public void CalculateCurrentTermInYears_SingleMembership_PartialYear_ReturnsFlooredYears()
    {
        var memberships = new List<Membership>
        {
            new MembershipBuilder()
                .WithBeginDate(new DateOnly(2020, 1, 1))
                .WithEndDate(new DateOnly(2022, 6, 15))
                .Build()
        };

        var result = MembershipTermCalculator.CalculateCurrentTermInYears(memberships);

        Assert.That(result, Is.EqualTo(2));
    }

    [Test]
    public void CalculateCurrentTermInYears_MultipleMemberships_ReturnsSumOfYears()
    {
        var memberships = new List<Membership>
        {
            new MembershipBuilder()
                .WithBeginDate(new DateOnly(2015, 3, 1))
                .WithEndDate(new DateOnly(2018, 3, 1))
                .Build(),
            new MembershipBuilder()
                .WithBeginDate(new DateOnly(2019, 6, 1))
                .WithEndDate(new DateOnly(2022, 6, 1))
                .Build()
        };

        var result = MembershipTermCalculator.CalculateCurrentTermInYears(memberships);

        Assert.That(result, Is.EqualTo(6)); // 3 years + 3 years
    }

    [Test]
    public void CalculateCurrentTermInYears_MembershipEndingInFuture_UsesTodayAsEndDate()
    {
        var beginDate = DateOnly.FromDateTime(DateTime.Today.AddYears(-2));
        var futureEndDate = DateOnly.FromDateTime(DateTime.Today.AddYears(2));

        var memberships = new List<Membership>
        {
            new MembershipBuilder()
                .WithBeginDate(beginDate)
                .WithEndDate(futureEndDate)
                .Build()
        };

        var result = MembershipTermCalculator.CalculateCurrentTermInYears(memberships);

        Assert.That(result, Is.EqualTo(2));
    }

    [Test]
    public void CalculateCurrentTermInYears_ForGeneralElection_MembershipEndingInFuture_UsesActualEndDate()
    {
        var beginDate = DateOnly.FromDateTime(DateTime.Today.AddYears(-2));
        var futureEndDate = DateOnly.FromDateTime(DateTime.Today.AddYears(2));

        var memberships = new List<Membership>
        {
            new MembershipBuilder()
                .WithBeginDate(beginDate)
                .WithEndDate(futureEndDate)
                .Build()
        };

        var result = MembershipTermCalculator.CalculateCurrentTermInYears(memberships, forGeneralElection: true);

        Assert.That(result, Is.EqualTo(4));
    }

    [Test]
    public void CalculateCurrentTermInYears_EndDateOn31December_AdjustsToNextYear()
    {
        var memberships = new List<Membership>
        {
            new MembershipBuilder()
                .WithBeginDate(new DateOnly(2020, 1, 1))
                .WithEndDate(new DateOnly(2022, 12, 31))
                .Build()
        };

        var result = MembershipTermCalculator.CalculateCurrentTermInYears(memberships);

        // End date 31.12.2022 is adjusted to 01.01.2023, so it's 3 years
        Assert.That(result, Is.EqualTo(3));
    }

    [Test]
    public void CalculateCurrentTermInYears_SameDayOneYearLater_ReturnsOneYear()
    {
        var memberships = new List<Membership>
        {
            new MembershipBuilder()
                .WithBeginDate(new DateOnly(2021, 5, 15))
                .WithEndDate(new DateOnly(2022, 5, 15))
                .Build()
        };

        var result = MembershipTermCalculator.CalculateCurrentTermInYears(memberships);

        Assert.That(result, Is.EqualTo(1));
    }

    [Test]
    public void CalculateCurrentTermInYears_EndDayBeforeBeginDay_ReturnsFlooredYears()
    {
        var memberships = new List<Membership>
        {
            new MembershipBuilder()
                .WithBeginDate(new DateOnly(2020, 5, 15))
                .WithEndDate(new DateOnly(2023, 5, 10)) // 5 days before same month
                .Build()
        };

        var result = MembershipTermCalculator.CalculateCurrentTermInYears(memberships);

        Assert.That(result, Is.EqualTo(2)); // Not quite 3 full years
    }

    [Test]
    public void CalculateCurrentTermInYears_LessThanOneYear_ReturnsZero()
    {
        var memberships = new List<Membership>
        {
            new MembershipBuilder()
                .WithBeginDate(new DateOnly(2022, 6, 1))
                .WithEndDate(new DateOnly(2022, 11, 1))
                .Build()
        };

        var result = MembershipTermCalculator.CalculateCurrentTermInYears(memberships);

        Assert.That(result, Is.EqualTo(0));
    }

    [Test]
    public void CalculateCurrentTermInYears_BeginDateEqualsEndDate_ReturnsZero()
    {
        var date = new DateOnly(2023, 6, 15);
        var memberships = new List<Membership>
        {
            new MembershipBuilder()
                .WithBeginDate(date)
                .WithEndDate(date)
                .Build()
        };

        var result = MembershipTermCalculator.CalculateCurrentTermInYears(memberships);

        Assert.That(result, Is.EqualTo(0));
    }

    [Test]
    public void CalculateCurrentTermInYears_MembershipEndingYesterday_UsesActualEndDate()
    {
        var yesterday = DateOnly.FromDateTime(DateTime.Today.AddDays(-1));
        var memberships = new List<Membership>
        {
            new MembershipBuilder()
                .WithBeginDate(yesterday.AddYears(-2))
                .WithEndDate(yesterday)
                .Build()
        };

        var result = MembershipTermCalculator.CalculateCurrentTermInYears(memberships);

        Assert.That(result, Is.EqualTo(2));
    }

    [TestCase("01.01.2007", "31.12.2022")]
    [TestCase("02.01.2007", "31.12.2023")]
    [TestCase("01.01.2008", "31.12.2023")]
    [TestCase("02.01.2008", "31.12.2024")]
    [TestCase("01.01.2009", "31.12.2024")]
    [TestCase("02.01.2021", "31.12.2037")]
    [TestCase("01.01.2022", "31.12.2037")]
    public void CalculateEstimatedTermInYears_ByGivenExamples_ShouldCalculate16Years(string beginDateString, string endDateString)
    {
        var beginDate = DateOnly.ParseExact(beginDateString, "dd.MM.yyyy");
        var endDate = DateOnly.ParseExact(endDateString, "dd.MM.yyyy");

        var result = MembershipTermCalculator.CalculateEstimatedTermInYears(beginDate, endDate);

        Assert.That(result, Is.EqualTo(16));
    }

    [TestCase("01.01.2007", "01.01.2023")]
    [TestCase("01.01.2007", "31.12.2023")]
    [TestCase("01.01.2008", "01.01.2024")]
    [TestCase("01.01.2008", "31.12.2024")]
    [TestCase("01.01.2009", "01.12.2025")]
    [TestCase("01.01.2021", "31.12.2037")]
    [TestCase("01.01.2022", "01.01.2038")]
    public void CalculateEstimatedTermInYears_ByGivenExamples_ShouldCalculate17Years(string beginDateString, string endDateString)
    {
        var beginDate = DateOnly.ParseExact(beginDateString, "dd.MM.yyyy");
        var endDate = DateOnly.ParseExact(endDateString, "dd.MM.yyyy");

        var result = MembershipTermCalculator.CalculateEstimatedTermInYears(beginDate, endDate);

        Assert.That(result, Is.EqualTo(17));
    }
}
