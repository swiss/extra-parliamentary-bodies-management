using Bk.APG.Business.Models;
using Bk.APG.CrossCutting.Tests.Builders;

namespace Bk.APG.Business.Tests.Models;

[TestFixture]
internal class MembershipTests
{
    [Test]
    public void IsFuture_WithBeginInFuture_IsSetCorrectly()
    {
        var membership = new MembershipBuilder()
            .WithBeginDate(DateOnly.FromDateTime(DateTime.Now.AddDays(180)))
            .WithEndDate(DateOnly.FromDateTime(DateTime.Now.AddDays(1800)))
            .Build();

        Assert.That(membership.IsFuture, Is.EqualTo(true));
        Assert.That(membership.IsActive, Is.EqualTo(false));
    }

    [Test]
    public void IsFuture_WithBeginInPast_IsSetCorrectly()
    {
        var membership = new MembershipBuilder()
            .WithBeginDate(DateOnly.FromDateTime(DateTime.Now.AddDays(-180)))
            .WithEndDate(DateOnly.FromDateTime(DateTime.Now.AddDays(1800)))
            .Build();

        Assert.That(membership.IsFuture, Is.EqualTo(false));
        Assert.That(membership.IsActive, Is.EqualTo(true));
    }

    [TestCase(-1, 1, true)]
    [TestCase(1, 2, false)]
    [TestCase(-2, -1, false)]
    public void IsActive_ShouldReturnExpected(int dayOffsetStart, int dayOffsetEnd, bool expected)
    {
        var membership = new MembershipBuilder()
            .WithBeginDate(DateOnly.FromDateTime(DateTime.Now.AddDays(dayOffsetStart)))
            .WithEndDate(DateOnly.FromDateTime(DateTime.Now.AddDays(dayOffsetEnd)))
            .Build();

        Assert.That(membership.IsActive, Is.EqualTo(expected));
    }

    [TestCase(-1, 1, null, true)]
    [TestCase(1, 2, null, false)]
    [TestCase(-2, -1, null, false)]
    [TestCase(-2, -1, ElectionType.NewElection, true)]
    [TestCase(-2, -1, ElectionType.ReElection, true)]
    [TestCase(-2, -1, "other", false)]
    public void IsActive_Property_Equals_IsActiveExpression(int dayOffsetStart, int dayOffsetEnd, string? electionTypeUri, bool expected)
    {
        var builder = new MembershipBuilder()
            .WithBeginDate(DateOnly.FromDateTime(DateTime.Now.AddDays(dayOffsetStart)))
            .WithEndDate(DateOnly.FromDateTime(DateTime.Now.AddDays(dayOffsetEnd)));

        if (electionTypeUri != null)
        {
            builder = builder.WithElectionType(new ElectionTypeBuilder().WithUri(electionTypeUri).Build());
        }

        var membership = builder.Build();

        var compiled = Membership.IsActiveExpression.Compile();

        Assert.That(membership.IsActive, Is.EqualTo(compiled(membership)));
        Assert.That(membership.IsActive, Is.EqualTo(expected));
    }

    [TestCase(CommitteeType.AdministrationCommissionGuidAsString, -15, -3, true)]
    [TestCase(CommitteeType.AdministrationCommissionGuidAsString, -15, -4, true)]
    [TestCase(CommitteeType.AdministrationCommissionGuidAsString, 0, 11, true)]
    [TestCase(CommitteeType.AdministrationCommissionGuidAsString, 0, 12, true)]
    [TestCase(CommitteeType.AdministrationCommissionGuidAsString, -2, -1, false)]
    [TestCase(CommitteeType.AuthoritiesCommissionGuidAsString, -15, -3, true)]
    [TestCase(CommitteeType.AuthoritiesCommissionGuidAsString, -15, -4, true)]
    [TestCase(CommitteeType.AuthoritiesCommissionGuidAsString, 0, 11, true)]
    [TestCase(CommitteeType.AuthoritiesCommissionGuidAsString, 0, 12, true)]
    [TestCase(CommitteeType.AuthoritiesCommissionGuidAsString, -2, -1, false)]
    [TestCase("FBEFEF07-CB51-4F6A-9911-FF1AC997554C", -15, -3, false)]
    [TestCase("FBEFEF07-CB51-4F6A-9911-FF1AC997554C", 0, 12, false)]
    public void JustificationLongerDutyNeeded_ShouldReturnExpected(string committeeTypeIdAsString, int yearOffsetStart, int yearOffsetEnd, bool expected)
    {
        var membership = new MembershipBuilder()
            .WithCommittee(
                new CommitteeBuilder()
                    .WithGermanDescription("DE")
                    .WithFrenchDescription("FR")
                    .WithItalianDescription("IT")
                    .WithCommitteeTypeId(new Guid(committeeTypeIdAsString))
                    .Build())
            .WithBeginDate(DateOnly.FromDateTime(DateTime.Today.AddYears(yearOffsetStart)))
            .WithEndDate(DateOnly.FromDateTime(DateTime.Today.AddYears(yearOffsetEnd)))
            .Build();

        Assert.That(membership.JustificationLongerDutyNeeded, Is.EqualTo(expected));
    }

    [TestCase(TermOfOffice.Period4YearsInGeneralElectionGuidAsString, -15, -12, true)]
    [TestCase(TermOfOffice.Period4YearsInGeneralElectionGuidAsString, -15, -11, false)]
    [TestCase(TermOfOffice.Period4YearsInGeneralElectionGuidAsString, 0, 3, true)]
    [TestCase(TermOfOffice.Period4YearsInGeneralElectionGuidAsString, 0, 4, false)]
    [TestCase(TermOfOffice.Period4YearsInGeneralElectionGuidAsString, -2, -1, true)]
    [TestCase("FBEFEF07-CB51-4F6A-9911-FF1AC997554C", -2, -1, false)]
    public void JustificationShorterDutyNeeded_ShouldReturnCorrectResult(string termOfOfficeIdAsStringId, int yearOffsetStart, int yearOffsetEnd, bool expected)
    {
        var membership = new MembershipBuilder()
            .WithCommittee(
                new CommitteeBuilder()
                    .WithGermanDescription("DE")
                    .WithFrenchDescription("FR")
                    .WithItalianDescription("IT")
                    .WithTermOfOfficeId(new Guid(termOfOfficeIdAsStringId))
                    .Build())
            .WithBeginDate(DateOnly.FromDateTime(DateTime.UtcNow.AddYears(yearOffsetStart)))
            .WithEndDate(DateOnly.FromDateTime(DateTime.UtcNow.AddYears(yearOffsetEnd).AddDays(-1)))
            .Build();

        Assert.That(membership.JustificationShorterDutyNeeded, Is.EqualTo(expected));
    }

    [TestCase("2024-01-01", "2027-12-31", false)]
    [TestCase("2024-01-01", "2027-12-30", true)]
    [TestCase("2024-12-31", "2027-12-31", true)]
    [TestCase("2024-01-01", "2028-01-01", false)]
    public void JustificationShorterDutyNeeded_ExtendedCases_ShouldReturnCorrectResult(string beginDate, string endDate, bool expected)
    {
        var membership = new MembershipBuilder()
            .WithCommittee(
                new CommitteeBuilder()
                    .WithGermanDescription("DE")
                    .WithFrenchDescription("FR")
                    .WithItalianDescription("IT")
                    .WithTermOfOfficeId(new Guid(TermOfOffice.Period4YearsInGeneralElectionGuidAsString))
                    .Build())
            .WithBeginDate(DateOnly.FromDateTime(DateTime.Parse(beginDate)))
            .WithEndDate(DateOnly.FromDateTime(DateTime.Parse(endDate)))
            .Build();

        Assert.That(membership.JustificationShorterDutyNeeded, Is.EqualTo(expected));
    }

    [TestCase("2024-01-01", "2027-12-31", false)]
    [TestCase("2024-01-01", "2035-12-29", true)]
    [TestCase("2024-01-01", "2035-12-31", true)]
    [TestCase("2024-12-31", "2035-12-31", false)]
    [TestCase("2024-06-01", "2035-06-01", true)]
    [TestCase("2024-06-01", "2035-06-01", true)]
    [TestCase("2024-06-01", "2035-05-31", false)]
    [TestCase("2023-06-01", "2034-05-31", false)]
    public void JustificationLongerDutyNeeded_ExtendedCases_ShouldReturnCorrectResult(string beginDate, string endDate, bool expected)
    {
        var membership = new MembershipBuilder()
            .WithCommittee(
                new CommitteeBuilder()
                    .WithGermanDescription("DE")
                    .WithFrenchDescription("FR")
                    .WithItalianDescription("IT")
                    .WithCommitteeTypeId(CommitteeType.AuthoritiesCommissionGuid)
                    .Build())
            .WithBeginDate(DateOnly.FromDateTime(DateTime.Parse(beginDate)))
            .WithEndDate(DateOnly.FromDateTime(DateTime.Parse(endDate)))
            .Build();

        Assert.That(membership.JustificationLongerDutyNeeded, Is.EqualTo(expected));
    }

    [TestCase(CommitteeType.AdministrationCommissionGuidAsString, true, true)]
    [TestCase(CommitteeType.AdministrationCommissionGuidAsString, false, false)]
    [TestCase(CommitteeType.AuthoritiesCommissionGuidAsString, true, true)]
    [TestCase(CommitteeType.AuthoritiesCommissionGuidAsString, false, false)]
    [TestCase("FBEFEF07-CB51-4F6A-9911-FF1AC997554C", true, false)]
    public void JustificationMemberInFederalDutyNeeded_ShouldReturnExpected(string committeeTypeIdAsString, bool isFederalDuty, bool expected)
    {
        var membership = new MembershipBuilder()
            .WithCommittee(
                new CommitteeBuilder()
                    .WithGermanDescription("DE")
                    .WithFrenchDescription("FR")
                    .WithItalianDescription("IT")
                    .WithCommitteeTypeId(new Guid(committeeTypeIdAsString))
                    .Build())
            .WithPerson(new PersonBuilder().WithFederalDuty(isFederalDuty).Build())
            .WithBeginDate(DateOnly.FromDateTime(DateTime.Now))
            .WithEndDate(DateOnly.FromDateTime(DateTime.Now.AddDays(1)))
            .Build();

        Assert.That(membership.JustificationMemberInFederalDutyNeeded, Is.EqualTo(expected));
    }

    [TestCase(CommitteeType.AdministrationCommissionGuidAsString, true, true)]
    [TestCase(CommitteeType.AdministrationCommissionGuidAsString, false, false)]
    [TestCase(CommitteeType.AuthoritiesCommissionGuidAsString, true, true)]
    [TestCase(CommitteeType.AuthoritiesCommissionGuidAsString, false, false)]
    [TestCase("FBEFEF07-CB51-4F6A-9911-FF1AC997554C", true, false)]
    public void JustificationMemberInFederalAssemblyNeeded_ShouldReturnExpected(string committeeTypeIdAsString, bool isFederalAssembly, bool expected)
    {
        var membership = new MembershipBuilder()
            .WithCommittee(
                new CommitteeBuilder()
                    .WithGermanDescription("DE")
                    .WithFrenchDescription("FR")
                    .WithItalianDescription("IT")
                    .WithCommitteeTypeId(new Guid(committeeTypeIdAsString))
                    .Build())
            .WithPerson(new PersonBuilder().WithFederalAssembly(isFederalAssembly).Build())
            .WithBeginDate(DateOnly.FromDateTime(DateTime.Now))
            .WithEndDate(DateOnly.FromDateTime(DateTime.Now.AddDays(1)))
            .Build();

        Assert.That(membership.JustificationMemberInFederalAssemblyNeeded, Is.EqualTo(expected));
    }

    [Test]
    public void FunctionName_WithoutPerson_ShouldReturnEmptyString()
    {
        var membership = new MembershipBuilder()
            .WithCommittee(
                new CommitteeBuilder()
                    .WithGermanDescription("DE")
                    .WithFrenchDescription("FR")
                    .WithItalianDescription("IT")
                    .Build())
            .WithBeginDate(DateOnly.FromDateTime(DateTime.Now))
            .WithEndDate(DateOnly.FromDateTime(DateTime.Now.AddDays(1)))
            .Build();

        Assert.That(membership.FunctionName, Is.Empty);
    }

    [Test]
    public void FunctionName_WithoutGender_ShouldReturnEmptyString()
    {
        var membership = new MembershipBuilder()
            .WithCommittee(
                new CommitteeBuilder()
                    .WithGermanDescription("DE")
                    .WithFrenchDescription("FR")
                    .WithItalianDescription("IT")
                    .Build())
            .WithPerson(new PersonBuilder().Build())
            .WithBeginDate(DateOnly.FromDateTime(DateTime.Now))
            .WithEndDate(DateOnly.FromDateTime(DateTime.Now.AddDays(1)))
            .Build();

        Assert.That(membership.FunctionName, Is.Empty);
    }

    [Test]
    public void FunctionName_WithoutFunction_ShouldReturnEmptyString()
    {
        var membership = new MembershipBuilder()
            .WithCommittee(
                new CommitteeBuilder()
                    .WithGermanDescription("DE")
                    .WithFrenchDescription("FR")
                    .WithItalianDescription("IT")
                    .Build())
            .WithPerson(new PersonBuilder().WithGender(new GenderBuilder().Build()).Build())
            .WithBeginDate(DateOnly.FromDateTime(DateTime.Now))
            .WithEndDate(DateOnly.FromDateTime(DateTime.Now.AddDays(1)))
            .Build();

        membership.Function = null;

        Assert.That(membership.FunctionName, Is.Empty);
    }

    [Test]
    public void FunctionName_WithFemaleGender_ShouldReturnFemaleText()
    {
        var membership = new MembershipBuilder()
            .WithCommittee(
                new CommitteeBuilder()
                    .WithGermanDescription("DE")
                    .WithFrenchDescription("FR")
                    .WithItalianDescription("IT")
                    .Build())
            .WithFunction(new FunctionBuilder().WithGermanFemaleText("FEMALE_DE").WithGermanText("DE").Build())
            .WithPerson(new PersonBuilder().WithGender(new GenderBuilder().WithUri(Gender.Female).Build()).Build())
            .WithBeginDate(DateOnly.FromDateTime(DateTime.Now))
            .WithEndDate(DateOnly.FromDateTime(DateTime.Now.AddDays(1)))
            .Build();

        Assert.That(membership.FunctionName, Is.EqualTo("FEMALE_DE"));
    }

    [Test]
    public void FunctionName_MaleGender_ShouldReturnMaleText()
    {
        var membership = new MembershipBuilder()
            .WithCommittee(
                new CommitteeBuilder()
                    .WithGermanDescription("DE")
                    .WithFrenchDescription("FR")
                    .WithItalianDescription("IT")
                    .Build())
            .WithFunction(new FunctionBuilder().WithGermanFemaleText("FEMALE_DE").WithGermanText("DE").Build())
            .WithPerson(new PersonBuilder().WithGender(new GenderBuilder().WithUri(Gender.Male).Build()).Build())
            .WithBeginDate(DateOnly.FromDateTime(DateTime.Now))
            .WithEndDate(DateOnly.FromDateTime(DateTime.Now.AddDays(1)))
            .Build();

        Assert.That(membership.FunctionName, Is.EqualTo("DE"));
    }

    [TestCase(ElectionType.NewElection, true)]
    [TestCase(ElectionType.ReElection, true)]
    [TestCase("other", false)]
    public void NeedsAttentionMembershipExpired_ShouldReturnExpected(string electionTypeUri, bool expected)
    {
        var membership = new MembershipBuilder()
            .WithCommittee(
                new CommitteeBuilder()
                    .WithGermanDescription("DE")
                    .WithFrenchDescription("FR")
                    .WithItalianDescription("IT")
                    .Build())
            .WithPerson(new PersonBuilder().WithGender(new GenderBuilder().WithUri(Gender.Male).Build()).Build())
            .WithBeginDate(DateOnly.FromDateTime(DateTime.Now.AddDays(-2)))
            .WithEndDate(DateOnly.FromDateTime(DateTime.Now.AddDays(-1)))
            .WithElectionType(new ElectionTypeBuilder()
                .WithUri(electionTypeUri).Build())
            .Build();

        Assert.That(membership.NeedsAttentionMembershipExpired, Is.EqualTo(expected));
    }

    [Test]
    public void NeedsAttentionMembershipExpired_WithActiveDateRange_ShouldReturnFalse()
    {
        var membership = new MembershipBuilder()
            .WithCommittee(
                new CommitteeBuilder()
                    .WithGermanDescription("DE")
                    .WithFrenchDescription("FR")
                    .WithItalianDescription("IT")
                    .Build())
            .WithPerson(new PersonBuilder().WithGender(new GenderBuilder().WithUri(Gender.Male).Build()).Build())
            .WithBeginDate(DateOnly.FromDateTime(DateTime.Now.AddDays(-2)))
            .WithEndDate(DateOnly.FromDateTime(DateTime.Now.AddDays(2)))
            .WithElectionType(new ElectionTypeBuilder()
                .WithUri(ElectionType.ReElection).Build())
            .Build();

        Assert.That(membership.NeedsAttentionMembershipExpired, Is.False);
    }

    [TestCase(CommitteeType.AuthoritiesCommissionGuidAsString, "", 13, true)]
    [TestCase(CommitteeType.AuthoritiesCommissionGuidAsString, "justification", 13, false)]
    [TestCase(CommitteeType.AdministrationCommissionGuidAsString, "", 13, true)]
    [TestCase(CommitteeType.AdministrationCommissionGuidAsString, "justification", 13, false)]
    [TestCase(CommitteeType.AuthoritiesCommissionGuidAsString, "", 1, false)]
    [TestCase(CommitteeType.AuthoritiesCommissionGuidAsString, "justification", 1, false)]
    [TestCase(CommitteeType.AdministrationCommissionGuidAsString, "", 1, false)]
    [TestCase(CommitteeType.AdministrationCommissionGuidAsString, "justification", 1, false)]
    public void NeedsAttentionLongerDuty_ShouldReturnExpected(string committeeTypeId, string justificationLongerDuty, int endYearsOffset, bool expected)
    {
        var membership = new MembershipBuilder()
            .WithCommittee(
                new CommitteeBuilder()
                    .WithCommitteeType(new CommitteeTypeBuilder().WithId(new Guid(committeeTypeId)).Build())
                    .WithGermanDescription("DE")
                    .WithFrenchDescription("FR")
                    .WithItalianDescription("IT")
                    .Build())
            .WithPerson(new PersonBuilder().WithGender(new GenderBuilder().WithUri(Gender.Male).Build()).Build())
            .WithBeginDate(DateOnly.FromDateTime(DateTime.Now.AddDays(-2)))
            .WithEndDate(DateOnly.FromDateTime(DateTime.Now.AddYears(endYearsOffset)))
            .WithJustificationLongerDuty(justificationLongerDuty)
            .Build();

        Assert.That(membership.NeedsAttentionLongerDuty, Is.EqualTo(expected));
    }

    [TestCase(TermOfOffice.Period4YearsInGeneralElectionGuidAsString, "", 1, true)]
    [TestCase("FBEFEF07-CB51-4F6A-9911-FF1AC997554C", "", 1, false)]
    [TestCase(TermOfOffice.Period4YearsInGeneralElectionGuidAsString, "", 10, false)]
    [TestCase("FBEFEF07-CB51-4F6A-9911-FF1AC997554C", "", 10, false)]
    public void NeedsAttentionShorterDuty_ShouldReturnExpected(string termOfOfficeId, string justificationShorterDuty, int endYearsOffset, bool expected)
    {
        var membership = new MembershipBuilder()
            .WithCommittee(
                new CommitteeBuilder()
                    .WithGermanDescription("DE")
                    .WithFrenchDescription("FR")
                    .WithItalianDescription("IT")
                    .WithTermOfOffice(new TermOfOfficeBuilder().WithId(new Guid(termOfOfficeId)).Build())
                    .Build())
            .WithPerson(new PersonBuilder().WithGender(new GenderBuilder().WithUri(Gender.Male).Build()).Build())
            .WithBeginDate(DateOnly.FromDateTime(DateTime.Now.AddDays(-2)))
            .WithEndDate(DateOnly.FromDateTime(DateTime.UtcNow.AddYears(endYearsOffset)))
            .WithJustificationShorterDuty(justificationShorterDuty)
            .Build();

        Assert.That(membership.NeedsAttentionShorterDuty, Is.EqualTo(expected));
    }

    [TestCase(CommitteeType.AuthoritiesCommissionGuidAsString, "", true, true)]
    [TestCase(CommitteeType.AuthoritiesCommissionGuidAsString, "justification", true, false)]
    [TestCase(CommitteeType.AdministrationCommissionGuidAsString, "", true, true)]
    [TestCase(CommitteeType.AdministrationCommissionGuidAsString, "justification", true, false)]
    public void NeedsAttentionFederalDuty_ShouldReturnExpected(string committeeTypeId, string justificationFederalDuty, bool federalDuty, bool expected)
    {
        var membership = new MembershipBuilder()
            .WithCommittee(
                new CommitteeBuilder()
                    .WithCommitteeType(new CommitteeTypeBuilder().WithId(new Guid(committeeTypeId)).Build())
                    .WithGermanDescription("DE")
                    .WithFrenchDescription("FR")
                    .WithItalianDescription("IT")
                    .Build())
            .WithPerson(new PersonBuilder().WithGender(new GenderBuilder().WithUri(Gender.Male).Build()).WithFederalDuty(federalDuty).Build())
            .WithBeginDate(DateOnly.FromDateTime(DateTime.Now.AddDays(-2)))
            .WithEndDate(DateOnly.FromDateTime(DateTime.UtcNow.AddYears(2)))
            .WithJustificationMemberInFederalDuty(justificationFederalDuty)
            .Build();

        Assert.That(membership.NeedsAttentionFederalDuty, Is.EqualTo(expected));
    }

    [TestCase(CommitteeType.AuthoritiesCommissionGuidAsString, "", true, true, false)]
    [TestCase(CommitteeType.AuthoritiesCommissionGuidAsString, "justification", true, false, false)]
    [TestCase(CommitteeType.AdministrationCommissionGuidAsString, "", true, false, true)]
    [TestCase(CommitteeType.AdministrationCommissionGuidAsString, "justification", true, false, false)]
    public void NeedsAttentionFederalAssembly_ShouldReturnExpected(string committeeTypeId, string justificationFederalAssembly, bool federalAssembly, bool expectedAuthoritiesCommission, bool expectedAdministrationCommission)
    {
        var membership = new MembershipBuilder()
            .WithCommittee(
                new CommitteeBuilder()
                    .WithCommitteeType(new CommitteeTypeBuilder().WithId(new Guid(committeeTypeId)).Build())
                    .WithGermanDescription("DE")
                    .WithFrenchDescription("FR")
                    .WithItalianDescription("IT")
                    .Build())
            .WithPerson(new PersonBuilder().WithGender(new GenderBuilder().WithUri(Gender.Male).Build()).WithFederalAssembly(federalAssembly).Build())
            .WithBeginDate(DateOnly.FromDateTime(DateTime.Now.AddDays(-2)))
            .WithEndDate(DateOnly.FromDateTime(DateTime.Now.AddDays(2)))
            .WithJustificationMemberInFederalAssembly(justificationFederalAssembly)
            .Build();

        Assert.That(membership.NeedsAttentionFederalAssemblyAdministrationCommission, Is.EqualTo(expectedAdministrationCommission));
        Assert.That(membership.NeedsAttentionFederalAssemblyAuthoritiesCommission, Is.EqualTo(expectedAuthoritiesCommission));
    }

    [Test]
    public void NeedsAttention_WithInactive_ShouldReturnFalse()
    {
        var membership = new MembershipBuilder()
            .WithCommittee(
                new CommitteeBuilder()
                    .WithCommitteeType(new CommitteeTypeBuilder().WithId(new Guid(CommitteeType.AuthoritiesCommissionGuidAsString)).Build())
                    .WithGermanDescription("DE")
                    .WithFrenchDescription("FR")
                    .WithItalianDescription("IT")
                    .Build())
            .WithPerson(new PersonBuilder().WithGender(new GenderBuilder().WithUri(Gender.Male).Build()).WithFederalAssembly(true).Build())
            .WithBeginDate(DateOnly.FromDateTime(DateTime.Now.AddDays(-2)))
            .WithEndDate(DateOnly.FromDateTime(DateTime.Now.AddYears(-1)))
            .WithJustificationMemberInFederalAssembly(string.Empty)
            .Build();

        Assert.That(membership.NeedsAttention, Is.False);
    }

    [Test]
    public void NeedsAttention_WithActive_ShouldReturnTrue()
    {
        var membership = new MembershipBuilder()
            .WithCommittee(
                new CommitteeBuilder()
                    .WithCommitteeType(new CommitteeTypeBuilder().WithId(new Guid(CommitteeType.AuthoritiesCommissionGuidAsString)).Build())
                    .WithGermanDescription("DE")
                    .WithFrenchDescription("FR")
                    .WithItalianDescription("IT")
                    .Build())
            .WithPerson(new PersonBuilder().WithGender(new GenderBuilder().WithUri(Gender.Male).Build()).WithFederalAssembly(true).Build())
            .WithBeginDate(DateOnly.FromDateTime(DateTime.Now.AddDays(-2)))
            .WithEndDate(DateOnly.FromDateTime(DateTime.UtcNow.AddYears(1)))
            .WithJustificationMemberInFederalAssembly(string.Empty)
            .Build();

        Assert.That(membership.NeedsAttention, Is.True);
    }
}
