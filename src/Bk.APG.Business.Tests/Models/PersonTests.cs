using Bk.APG.Business.Models;
using Bk.APG.CrossCutting.Tests.Builders;

namespace Bk.APG.Business.Tests.Models;

[TestFixture]
internal class PersonTests
{
    [Test]
    public void Age_CalculatesByBirthYear()
    {
        var person = new PersonBuilder().WithBirthYear(2000).Build();

        Assert.That(person.Age, Is.EqualTo(DateTime.UtcNow.Year - 2000));
    }

    [Test]
    public void ActiveMembershipCount_CalculatesActiveMemberships()
    {
        var person = new PersonBuilder()
            .WithMemberships(
                [
                    new MembershipBuilder().WithIsActive(true).Build(),
                    new MembershipBuilder().WithIsActive(false).Build(),
                ]
            )
            .Build();

        Assert.That(person.ActiveMembershipCount, Is.EqualTo(1));
    }

    [Test]
    public void TotalEmploymentLevel_CalculatesByActiveMemberships()
    {
        var person = new PersonBuilder()
            .WithMemberships(
                [
                    new MembershipBuilder().WithIsActive(true).WithEmploymentLevel(10).Build(),
                    new MembershipBuilder().WithIsActive(true).WithEmploymentLevel(20).Build(),
                    new MembershipBuilder().WithIsActive(false).WithEmploymentLevel(10).Build(),
                ]
            )
            .Build();

        Assert.That(person.TotalEmploymentLevel, Is.EqualTo(30));
    }

    [Test]
    public void ActiveCommittees_ReturnsCommitteesOfActiveMemberships()
    {
        var person = new PersonBuilder()
            .WithMemberships(
                [
                    new MembershipBuilder().WithIsActive(true).Build(),
                    new MembershipBuilder().WithIsActive(false).Build()
                ]
            )
            .Build();

        Assert.That(person.ActiveCommittees.Count(), Is.EqualTo(1));
    }

    [TestCase(CommitteeType.AuthoritiesCommissionGuidAsString)]
    [TestCase(CommitteeType.AdministrationCommissionGuidAsString)]
    [TestCase(CommitteeType.FederalAgenciesCommitteeGuidAsString)]
    [TestCase(CommitteeType.ManagementCommitteeGuidAsString)]
    public void NeedsAttentionInterests_WithInterest_ShouldReturnFalse(string committeeTypeId)
    {
        var today = DateOnly.FromDateTime(DateTime.Now);
        var person = new PersonBuilder()
            .WithNoInterest(false)
            .WithInterests([
                new InterestBuilder().WithInterestText("interestText")
                    .WithLegalForm(new LegalFormBuilder().Build())
                    .WithInterestFunction(new InterestFunctionBuilder().Build())
                    .WithInterestCommittee(new InterestCommitteeBuilder().Build()).Build()
            ])
            .WithMemberships([
                new MembershipBuilder()
                    .WithBeginDate(today.AddDays(-1))
                    .WithEndDate(today.AddDays(1))
                    .WithCommittee(new CommitteeBuilder()
                        .WithCommitteeTypeId(new Guid(committeeTypeId)).Build()).Build()
            ])
            .Build();

        Assert.That(person.NeedsAttentionInterests, Is.False);
    }

    [TestCase(CommitteeType.AuthoritiesCommissionGuidAsString)]
    [TestCase(CommitteeType.AdministrationCommissionGuidAsString)]
    [TestCase(CommitteeType.FederalAgenciesCommitteeGuidAsString)]
    [TestCase(CommitteeType.ManagementCommitteeGuidAsString)]
    public void NeedsAttentionInterests_WithEmptyInterest_ShouldReturnTrue(string committeeTypeId)
    {
        var today = DateOnly.FromDateTime(DateTime.Now);
        var person = new PersonBuilder()
            .WithNoInterest(false)
            .WithInterests([
                new InterestBuilder().WithInterestText("")
                    .WithLegalForm(new LegalFormBuilder().Build())
                    .WithInterestFunction(new InterestFunctionBuilder().Build())
                    .WithInterestCommittee(new InterestCommitteeBuilder().Build()).Build()
            ])
            .WithMemberships([
                new MembershipBuilder()
                    .WithBeginDate(today.AddDays(-1))
                    .WithEndDate(today.AddDays(1))
                    .WithCommittee(new CommitteeBuilder()
                        .WithCommitteeTypeId(new Guid(committeeTypeId)).Build()).Build()
            ])
            .Build();

        Assert.That(person.NeedsAttentionInterests, Is.True);
    }

    [TestCase(CommitteeType.AuthoritiesCommissionGuidAsString)]
    [TestCase(CommitteeType.AdministrationCommissionGuidAsString)]
    [TestCase(CommitteeType.FederalAgenciesCommitteeGuidAsString)]
    [TestCase(CommitteeType.ManagementCommitteeGuidAsString)]
    public void NeedsAttentionInterests_WithoutInterestAndActiveMembership_ShouldReturnTrue(string committeeTypeId)
    {
        var today = DateOnly.FromDateTime(DateTime.Now);
        var person = new PersonBuilder()
            .WithNoInterest(false)
            .WithMemberships([
                new MembershipBuilder()
                    .WithBeginDate(today.AddDays(-5))
                    .WithEndDate(today.AddDays(5))
                    .WithCommittee(new CommitteeBuilder()
                        .WithCommitteeTypeId(new Guid(committeeTypeId)).Build()).Build()
            ])
            .Build();

        Assert.That(person.NeedsAttentionInterests, Is.True);
    }

    [TestCase(CommitteeType.AuthoritiesCommissionGuidAsString)]
    [TestCase(CommitteeType.AdministrationCommissionGuidAsString)]
    [TestCase(CommitteeType.FederalAgenciesCommitteeGuidAsString)]
    [TestCase(CommitteeType.ManagementCommitteeGuidAsString)]
    public void NeedsAttentionInterests_WithInterestAndInactiveMembership_ShouldReturnTrue(string committeeTypeId)
    {
        var today = DateOnly.FromDateTime(DateTime.Now);
        var person = new PersonBuilder()
            .WithMemberships([
                new MembershipBuilder()
                    .WithBeginDate(today.AddDays(-2))
                    .WithEndDate(today.AddDays(-1))
                    .WithCommittee(new CommitteeBuilder()
                        .WithCommitteeTypeId(new Guid(committeeTypeId)).Build()).Build()
            ])
            .Build();
        person.NoInterest = false;

        Assert.That(person.NeedsAttentionInterests, Is.False);
    }

    [Test]
    public void NeedsAttentionInterests_WithOtherCommitteeType_ShouldReturnFalse()
    {
        var today = DateOnly.FromDateTime(DateTime.Now);
        var person = new PersonBuilder()
            .WithMemberships([
                new MembershipBuilder()
                    .WithBeginDate(today.AddDays(-1))
                    .WithEndDate(today.AddDays(1))
                    .WithCommittee(new CommitteeBuilder()
                        .WithCommitteeTypeId(Guid.NewGuid()).Build()).Build()
            ])
            .Build();
        person.NoInterest = false;

        Assert.That(person.NeedsAttentionInterests, Is.False);
    }

    [Test]
    public void NeedsAttentionBasicData_WithInvalidPhoneNumber_ShouldReturnTrue()
    {
        var person = new PersonBuilder()
            .WithOfficeAddress(new AddressBuilder()
                .WithMobile(string.Empty)
                .WithPhone(string.Empty)
                .WithPhone("xyz").Build())
            .Build();

        Assert.That(person.NeedsAttentionBasicData, Is.True);
    }

    [Test]
    public void NeedsAttentionBasicData_WithValidPhoneNumber_ShouldReturnFalse()
    {
        var person = new PersonBuilder()
            .WithOfficeAddress(new AddressBuilder()
                .WithMobile(string.Empty)
                .WithEmail(string.Empty)
                .WithPhone("+41 445 444 555").Build())
            .Build();

        Assert.That(person.NeedsAttentionBasicData, Is.False);
    }

    [Test]
    public void NeedsAttentionBasicData_WithInvalidMobileNumber_ShouldReturnTrue()
    {
        var person = new PersonBuilder()
            .WithOfficeAddress(new AddressBuilder()
                .WithPhone(string.Empty)
                .WithEmail(string.Empty)
                .WithMobile("xyz").Build())
            .Build();

        Assert.That(person.NeedsAttentionBasicData, Is.True);
    }

    [Test]
    public void NeedsAttentionBasicData_WithValidMobileNumber_ShouldReturnFalse()
    {
        var person = new PersonBuilder()
            .WithOfficeAddress(new AddressBuilder()
                .WithEmail(string.Empty)
                .WithPhone(string.Empty)
                .WithMobile("+417820012345").Build())
            .Build();

        Assert.That(person.NeedsAttentionBasicData, Is.False);
    }

    [Test]
    public void NeedsAttentionBasicData_WithInvalidEmail_ShouldReturnTrue()
    {
        var person = new PersonBuilder()
            .WithOfficeAddress(new AddressBuilder()
                .WithMobile(string.Empty)
                .WithPhone(string.Empty)
                .WithEmail("xyz").Build())
            .Build();

        Assert.That(person.NeedsAttentionBasicData, Is.True);
    }

    [Test]
    public void NeedsAttentionBasicData_WithValidEmail_ShouldReturnFalse()
    {
        var person = new PersonBuilder()
            .WithOfficeAddress(new AddressBuilder()
                .WithMobile(string.Empty)
                .WithPhone(string.Empty)
                .WithEmail("test@test.ch").Build())
            .Build();

        Assert.That(person.NeedsAttentionBasicData, Is.False);
    }

    [Test]
    public void NeedsAttentionBasicData_WithFederalDutyAndWithoutOffice_ShouldReturnTrue()
    {
        var person = new PersonBuilder()
            .WithOfficeAddress(new AddressBuilder().WithEmail("test@test.ch").Build())
            .WithFederalDuty(true)
            .Build();

        Assert.That(person.NeedsAttentionBasicData, Is.True);
    }

    [Test]
    public void NeedsAttentionBasicData_WithFederalAssemblyAndWithoutCouncil_ShouldReturnTrue()
    {
        var person = new PersonBuilder()
            .WithOfficeAddress(new AddressBuilder().WithEmail("test@test.ch").Build())
            .WithFederalAssembly(true)
            .Build();

        Assert.That(person.NeedsAttentionBasicData, Is.True);
    }

    [TestCase(CommitteeType.AuthoritiesCommissionGuidAsString, true, false, true)]
    [TestCase(CommitteeType.AdministrationCommissionGuidAsString, true, false, true)]
    [TestCase(CommitteeType.FederalAgenciesCommitteeGuidAsString, false, false, true)]
    [TestCase(CommitteeType.ManagementCommitteeGuidAsString, false, false, true)]
    [TestCase(CommitteeType.ManagementCommitteeGuidAsString, true, true, false)]
    [TestCase("FBEFEF07-CB51-4F6A-9911-FF1AC997554C", true, false, false)]
    public void NeedsAttentionOccupation_WithOrWithoutOccupation_ShouldReturnExpected(string committeeTypeId, bool hasOccupation, bool hasEmployer, bool expected)
    {
        var today = DateOnly.FromDateTime(DateTime.Now);
        var person = new PersonBuilder()
            .WithFederalDuty(false)
            .WithNoEmployment(false)
            .WithMemberships([
                new MembershipBuilder()
                    .WithBeginDate(today.AddDays(-3))
                    .WithEndDate(today.AddDays(3))
                    .WithCommittee(new CommitteeBuilder()
                        .WithCommitteeTypeId(new Guid(committeeTypeId)).Build()).Build()
            ])
            .WithOccupations(hasOccupation ? [new OccupationBuilder().Build()] : [])
            .WithEmployer(hasEmployer ? "EmployerName" : string.Empty)
            .Build();

        Assert.That(person.NeedsAttentionOccupation, Is.EqualTo(expected));
    }

    [Test]
    public void NeedsAttention_WhenNoAttentionFlagsAreSet_ShouldReturnFalse()
    {
        var person = new PersonBuilder().Build();

        Assert.That(person.NeedsAttention, Is.False);
    }

    [TestCaseSource(nameof(NeedsAttentionCases))]
    public void NeedsAttention_WhenAnyAttentionFlagIsSet_ShouldReturnTrue(Person person)
    {
        Assert.That(person.NeedsAttention, Is.True);
    }

    private static IEnumerable<TestCaseData> NeedsAttentionCases()
    {
        yield return new TestCaseData(BuildPersonWithExpiredMembershipAttention())
            .SetName("NeedsAttention_WhenMembershipExpired_ShouldReturnTrue");
        yield return new TestCaseData(BuildPersonWithLongerDutyAttention())
            .SetName("NeedsAttention_WhenLongerDutyNeedsJustification_ShouldReturnTrue");
        yield return new TestCaseData(BuildPersonWithShorterDutyAttention())
            .SetName("NeedsAttention_WhenShorterDutyNeedsJustification_ShouldReturnTrue");
        yield return new TestCaseData(BuildPersonWithFederalDutyAttention())
            .SetName("NeedsAttention_WhenFederalDutyNeedsJustification_ShouldReturnTrue");
        yield return new TestCaseData(BuildPersonWithFederalAssemblyAuthoritiesAttention())
            .SetName("NeedsAttention_WhenFederalAssemblyAuthoritiesNeedsJustification_ShouldReturnTrue");
        yield return new TestCaseData(BuildPersonWithFederalAssemblyAdministrationAttention())
            .SetName("NeedsAttention_WhenFederalAssemblyAdministrationNeedsJustification_ShouldReturnTrue");
        yield return new TestCaseData(BuildPersonWithInterestsAttention())
            .SetName("NeedsAttention_WhenInterestsAreMissing_ShouldReturnTrue");
        yield return new TestCaseData(BuildPersonWithOccupationAttention())
            .SetName("NeedsAttention_WhenOccupationDataIsMissing_ShouldReturnTrue");
    }

    private static Person BuildPersonWithExpiredMembershipAttention()
    {
        var today = DateOnly.FromDateTime(DateTime.Now);
        var membership = new MembershipBuilder()
            .WithBeginDate(today.AddYears(-1))
            .WithEndDate(today.AddDays(-1))
            .WithElectionType(new ElectionTypeBuilder().WithUri(ElectionType.NewElection).Build())
            .Build();

        return new PersonBuilder().WithMemberships([membership]).Build();
    }

    private static Person BuildPersonWithLongerDutyAttention()
    {
        var today = DateOnly.FromDateTime(DateTime.Now);
        var membership = new MembershipBuilder()
            .WithBeginDate(today.AddYears(-13))
            .WithEndDate(today.AddDays(1))
            .WithCommittee(new CommitteeBuilder().WithCommitteeTypeId(CommitteeType.AuthoritiesCommissionGuid).Build())
            .WithJustificationLongerDuty(string.Empty)
            .Build();

        return new PersonBuilder().WithMemberships([membership]).Build();
    }

    private static Person BuildPersonWithShorterDutyAttention()
    {
        var today = DateOnly.FromDateTime(DateTime.Now);
        var committee = new CommitteeBuilder()
            .WithCommitteeTypeId(CommitteeType.AuthoritiesCommissionGuid)
            .WithTermOfOfficeId(TermOfOffice.Period4YearsInGeneralElectionGuid)
            .WithTermOfOfficeDate(new TermOfOfficeDateBuilder().WithEndDate(today.AddDays(2)).Build())
            .Build();

        var membership = new MembershipBuilder()
            .WithBeginDate(today.AddDays(-10))
            .WithEndDate(today.AddDays(1))
            .WithCommittee(committee)
            .WithJustificationShorterDuty(string.Empty)
            .Build();

        return new PersonBuilder().WithMemberships([membership]).Build();
    }

    private static Person BuildPersonWithFederalDutyAttention()
    {
        var membership = ActiveMembershipBuilder(CommitteeType.AuthoritiesCommissionGuid)
            .WithPerson(new PersonBuilder().WithFederalDuty(true).Build())
            .WithJustificationMemberInFederalDuty(string.Empty)
            .Build();

        return new PersonBuilder().WithMemberships([membership]).Build();
    }

    private static Person BuildPersonWithFederalAssemblyAuthoritiesAttention()
    {
        var membership = ActiveMembershipBuilder(CommitteeType.AuthoritiesCommissionGuid)
            .WithPerson(new PersonBuilder().WithFederalAssembly(true).Build())
            .WithJustificationMemberInFederalAssembly(string.Empty)
            .Build();

        return new PersonBuilder().WithMemberships([membership]).Build();
    }

    private static Person BuildPersonWithFederalAssemblyAdministrationAttention()
    {
        var membership = ActiveMembershipBuilder(CommitteeType.AdministrationCommissionGuid)
            .WithPerson(new PersonBuilder().WithFederalAssembly(true).Build())
            .WithJustificationMemberInFederalAssembly(string.Empty)
            .Build();

        return new PersonBuilder().WithMemberships([membership]).Build();
    }

    private static Person BuildPersonWithInterestsAttention()
    {
        var person = new PersonBuilder()
            .WithNoInterest(false)
            .WithMemberships([ActiveMembershipBuilder(CommitteeType.AuthoritiesCommissionGuid).Build()])
            .Build();

        return person;
    }

    private static Person BuildPersonWithOccupationAttention()
    {
        var person = new PersonBuilder()
            .WithFederalDuty(false)
            .WithNoEmployment(false)
            .WithEmployer(string.Empty)
            .WithMemberships([ActiveMembershipBuilder(CommitteeType.ManagementCommitteeGuid).Build()])
            .Build();

        return person;
    }

    private static MembershipBuilder ActiveMembershipBuilder(Guid committeeTypeId)
    {
        return new MembershipBuilder()
            .WithIsActive(true)
            .WithCommittee(new CommitteeBuilder().WithCommitteeTypeId(committeeTypeId).Build());
    }
}
