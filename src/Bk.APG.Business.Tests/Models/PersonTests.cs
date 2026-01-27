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
                    .WithBeginDate(DateOnly.FromDateTime(DateTime.Now.AddDays(-1)))
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
                    .WithBeginDate(DateOnly.FromDateTime(DateTime.Now.AddDays(-1)))
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
        var person = new PersonBuilder()
            .WithNoInterest(false)
            .WithMemberships([
                new MembershipBuilder()
                    .WithBeginDate(DateOnly.FromDateTime(DateTime.Now.AddDays(-5)))
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
        var person = new PersonBuilder()
            .WithMemberships([
                new MembershipBuilder()
                    .WithBeginDate(DateOnly.FromDateTime(DateTime.Now.AddDays(-2)))
                    .WithEndDate(DateOnly.FromDateTime(DateTime.Now.AddDays(-1)))
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
        var person = new PersonBuilder()
            .WithMemberships([
                new MembershipBuilder()
                    .WithBeginDate(DateOnly.FromDateTime(DateTime.Now.AddDays(-1)))
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

    [TestCase(CommitteeType.AuthoritiesCommissionGuidAsString, true, false, true)]
    [TestCase(CommitteeType.AdministrationCommissionGuidAsString, true, false, true)]
    [TestCase(CommitteeType.FederalAgenciesCommitteeGuidAsString, false, false, true)]
    [TestCase(CommitteeType.ManagementCommitteeGuidAsString, false, false, true)]
    [TestCase(CommitteeType.ManagementCommitteeGuidAsString, true, true, false)]
    [TestCase("FBEFEF07-CB51-4F6A-9911-FF1AC997554C", true, false, false)]
    public void NeedsAttentionOccupation_WithOrWithoutOccupation_ShouldReturnExpected(string committeeTypeId, bool hasOccupation, bool hasEmployer, bool expected)
    {
        var person = new PersonBuilder()
            .WithFederalDuty(false)
            .WithNoEmployment(false)
            .WithMemberships([
                new MembershipBuilder()
                    .WithBeginDate(DateOnly.FromDateTime(DateTime.Now.AddDays(-3)))
                    .WithEndDate(DateOnly.FromDateTime(DateTime.Now.AddDays(3)))
                    .WithCommittee(new CommitteeBuilder()
                        .WithCommitteeTypeId(new Guid(committeeTypeId)).Build()).Build()
            ])
            .WithOccupations(hasOccupation ? [new OccupationBuilder().Build()] : [])
            .WithEmployer(hasEmployer ? "EmployerName" : string.Empty)
            .Build();

        Assert.That(person.NeedsAttentionOccupation, Is.EqualTo(expected));
    }
}
