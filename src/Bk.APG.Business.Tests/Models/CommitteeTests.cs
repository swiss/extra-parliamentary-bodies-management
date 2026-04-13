using Bk.APG.Business.Models;
using Bk.APG.CrossCutting.Tests.Builders;

namespace Bk.APG.Business.Tests.Models;

[TestFixture]
internal class CommitteeTests
{
    [TestCase(-1, 1, true)]
    [TestCase(1, 2, false)]
    [TestCase(-2, -1, false)]
    public void IsActive_WithEndDate_ShouldReturnExpected(int dayOffsetStart, int dayOffsetEnd, bool expected)
    {
        var committee = new CommitteeBuilder()
            .WithBeginDate(DateOnly.FromDateTime(DateTime.Today.AddDays(dayOffsetStart)))
            .WithEndDate(DateOnly.FromDateTime(DateTime.Today.AddDays(dayOffsetEnd)))
            .Build();

        Assert.That(committee.IsActive, Is.EqualTo(expected));
    }

    [TestCase(-1, true)]
    [TestCase(1, false)]
    [TestCase(0, true)]
    public void IsActive_WithoutEndDate_ShouldReturnExpected(int dayOffsetStart, bool expected)
    {
        var committee = new CommitteeBuilder()
            .WithBeginDate(DateOnly.FromDateTime(DateTime.Today.AddDays(dayOffsetStart)))
            .WithEndDate(null)
            .Build();

        Assert.That(committee.IsActive, Is.EqualTo(expected));
    }

    [TestCase(-1, 1, true)]
    [TestCase(1, 2, true)]
    [TestCase(-2, -1, false)]
    public void CanCreateMembership_WithEndDate_ShouldReturnExpected(int dayOffsetStart, int dayOffsetEnd, bool expected)
    {
        var committee = new CommitteeBuilder()
            .WithBeginDate(DateOnly.FromDateTime(DateTime.Today.AddDays(dayOffsetStart)))
            .WithEndDate(DateOnly.FromDateTime(DateTime.Today.AddDays(dayOffsetEnd)))
            .Build();

        Assert.That(committee.CanCreateMembership, Is.EqualTo(expected));
    }

    [TestCase(-1, true)]
    [TestCase(1, true)]
    [TestCase(0, true)]
    public void CanCreateMembership_WithoutEndDate_ShouldReturnExpected(int dayOffsetStart, bool expected)
    {
        var committee = new CommitteeBuilder()
            .WithBeginDate(DateOnly.FromDateTime(DateTime.Today.AddDays(dayOffsetStart)))
            .WithEndDate(null)
            .Build();

        Assert.That(committee.CanCreateMembership, Is.EqualTo(expected));
    }

    [TestCase(CommitteeType.AdministrationCommissionGuidAsString, true)]
    [TestCase(CommitteeType.AuthoritiesCommissionGuidAsString, true)]
    [TestCase("FBEFEF07-CB51-4F6A-9911-FF1AC997554C", false)]
    public void ExtraParliamentaryCommission_ShouldReturnCorrectValue(string committeeTypeId, bool expected)
    {
        var committeeDetail = new CommitteeBuilder()
            .WithId(Guid.NewGuid())
            .WithMaximalMember(5)
            .WithCommitteeTypeId(new Guid(committeeTypeId))
            .WithDepartment(new DepartmentBuilder().Build())
            .Build();
        Assert.That(committeeDetail.ExtraParliamentaryCommission, Is.EqualTo(expected));
    }

    [TestCase(CommitteeLevel.FederalCouncilGuidAsString, TermOfOffice.Period4YearsInGeneralElectionGuidAsString, true)]
    [TestCase(CommitteeLevel.FederalCouncilGuidAsString, "FBEFEF07-CB51-4F6A-9911-FF1AC997554C", false)]
    [TestCase("FBEFEF07-CB51-4F6A-9911-FF1AC997554C", TermOfOffice.Period4YearsInGeneralElectionGuidAsString, false)]
    [TestCase("FBEFEF07-CB51-4F6A-9911-FF1AC997554C", "FBEFEF07-CB51-4F6A-9911-FF1AC997554C", false)]
    public void IsInGeneralElection_ShouldReturnCorrectValue(string committeeLevelId, string termOfOfficeId, bool expected)
    {
        var committee = new CommitteeBuilder()
            .WithCommitteeLevelId(new Guid(committeeLevelId))
            .WithTermOfOfficeId(new Guid(termOfOfficeId))
            .Build();

        Assert.That(committee.IsInGeneralElection, Is.EqualTo(expected));
    }

    [Test]
    public void NeedsAttentionBasicData_WithoutRomanshDescription_ShouldReturnTrue()
    {
        var committee = new CommitteeBuilder()
            .WithRomanschDescription(string.Empty)
            .Build();

        Assert.That(committee.NeedsAttentionBasicData, Is.True);
    }

    [TestCase(false, "A", false)]
    [TestCase(false, "", false)]
    [TestCase(true, "", true)]
    [TestCase(true, "A", false)]
    public void NeedsAttentionBasicData_LegalBase_ShouldReturnExpected(bool federalLawEstablishment, string legalBase, bool expected)
    {
        var committee = new CommitteeBuilder()
            .Build();

        committee.FederalLawEstablishment = federalLawEstablishment;
        committee.LegalBase = legalBase;

        Assert.That(committee.NeedsAttentionBasicData, Is.EqualTo(expected));
    }

    [TestCase(CommitteeLevel.FederalCouncilGuidAsString, true)]
    [TestCase("FBEFEF07-CB51-4F6A-9911-FF1AC997554C", false)]
    public void NeedsAttentionBasicData_WithoutMinMembers_ShouldReturnExpected(string committeeLevelId, bool expected)
    {
        var committee = new CommitteeBuilder()
            .WithCommitteeLevelId(new Guid(committeeLevelId))
            .Build();

        committee.MinimalMembers = null;

        Assert.That(committee.NeedsAttentionBasicData, Is.EqualTo(expected));
    }

    [TestCase(CommitteeLevel.FederalCouncilGuidAsString, true)]
    [TestCase("FBEFEF07-CB51-4F6A-9911-FF1AC997554C", false)]
    public void NeedsAttentionBasicData_WithoutMaxMembers_ShouldReturnExpected(string committeeLevelId, bool expected)
    {
        var committee = new CommitteeBuilder()
            .WithCommitteeLevelId(new Guid(committeeLevelId))
            .Build();

        committee.MaximalMembers = null;

        Assert.That(committee.NeedsAttentionBasicData, Is.EqualTo(expected));
    }

    [TestCase(CommitteeType.AdministrationCommissionGuidAsString, true)]
    [TestCase(CommitteeType.AuthoritiesCommissionGuidAsString, true)]
    [TestCase("FBEFEF07-CB51-4F6A-9911-FF1AC997554C", false)]
    public void NeedsAttentionBasicData_WithoutFederalLawEstablishments_ShouldReturnExpected(string committeeTypeId, bool expected)
    {
        var committee = new CommitteeBuilder()
            .WithCommitteeTypeId(new Guid(committeeTypeId))
            .Build();

        committee.FederalLawEstablishment = null;

        Assert.That(committee.NeedsAttentionBasicData, Is.EqualTo(expected));
    }

    [TestCase(CommitteeType.AdministrationCommissionGuidAsString, true)]
    [TestCase(CommitteeType.AuthoritiesCommissionGuidAsString, true)]
    [TestCase("FBEFEF07-CB51-4F6A-9911-FF1AC997554C", false)]
    public void NeedsAttentionBasicData_WithoutSupervisionDuty_ShouldReturnExpected(string committeeTypeId, bool expected)
    {
        var committee = new CommitteeBuilder()
            .WithCommitteeTypeId(new Guid(committeeTypeId))
            .Build();

        committee.SupervisionDuty = null;

        Assert.That(committee.NeedsAttentionBasicData, Is.EqualTo(expected));
    }

    [TestCase(CommitteeType.AdministrationCommissionGuidAsString, true)]
    [TestCase(CommitteeType.AuthoritiesCommissionGuidAsString, true)]
    [TestCase("FBEFEF07-CB51-4F6A-9911-FF1AC997554C", false)]
    public void NeedsAttentionBasicData_WithoutMarketOrientated_ShouldReturnExpected(string committeeTypeId, bool expected)
    {
        var committee = new CommitteeBuilder()
            .WithCommitteeTypeId(new Guid(committeeTypeId))
            .Build();

        committee.MarketOrientated = null;

        Assert.That(committee.NeedsAttentionBasicData, Is.EqualTo(expected));
    }

    [TestCase(CommitteeType.FederalAgenciesCommitteeGuidAsString, true)]
    [TestCase(CommitteeType.ManagementCommitteeGuidAsString, true)]
    [TestCase("FBEFEF07-CB51-4F6A-9911-FF1AC997554C", false)]
    public void NeedsAttentionBasicData_WithoutFederalInstitution_ShouldReturnExpected(string committeeTypeId, bool expected)
    {
        var committee = new CommitteeBuilder()
            .WithCommitteeTypeId(new Guid(committeeTypeId))
            .Build();

        committee.FederalInstitution = null;

        Assert.That(committee.NeedsAttentionBasicData, Is.EqualTo(expected));
    }

    [TestCase(CommitteeType.ManagementCommitteeGuidAsString, true)]
    [TestCase("FBEFEF07-CB51-4F6A-9911-FF1AC997554C", false)]
    public void NeedsAttentionBasicData_WithoutLegalForm_ShouldReturnExpected(string committeeTypeId, bool expected)
    {
        var committee = new CommitteeBuilder()
            .WithCommitteeTypeId(new Guid(committeeTypeId))
            .Build();

        committee.LegalFormId = null;

        Assert.That(committee.NeedsAttentionBasicData, Is.EqualTo(expected));
    }

    [Test]
    public void NeedsAttentionNoMembers_WithoutMembers_ShouldReturnTrue()
    {
        var committee = new CommitteeBuilder()
            .Build();

        Assert.That(committee.NeedsAttentionNoMembers, Is.True);
    }

    [Test]
    public void NeedsAttentionNoMembers_WithoutActiveMembers_ShouldReturnTrue()
    {
        var committee = new CommitteeBuilder()
            .WithMembership(new MembershipBuilder()
                .WithBeginDate(DateOnly.FromDateTime(DateTime.Today.AddDays(11))).Build())
            .Build();

        Assert.That(committee.NeedsAttentionNoMembers, Is.True);
    }

    [Test]
    public void NeedsAttentionNoMembers_WithActiveMembers_ShouldReturnFalse()
    {
        var committee = new CommitteeBuilder()
            .WithMembership(new MembershipBuilder()
                .WithBeginDate(DateOnly.FromDateTime(DateTime.Today.AddDays(-3)))
                .WithEndDate(DateOnly.FromDateTime(DateTime.Today.AddDays(1)))
                .Build())
            .Build();

        Assert.That(committee.NeedsAttentionNoMembers, Is.False);
    }

    [TestCase(CommitteeType.AuthoritiesCommissionGuidAsString)]
    [TestCase(CommitteeType.AdministrationCommissionGuidAsString)]
    public void NeedsAttentionAboveMaxMembers_WithLessMembers_ShouldReturnFalse(string committeeTypeId)
    {
        var committee = new CommitteeBuilder()
            .WithCommitteeTypeId(new Guid(committeeTypeId))
            .WithMembership(new MembershipBuilder()
                .WithBeginDate(DateOnly.FromDateTime(DateTime.Today.AddDays(-1))).Build())
            .Build();
        committee.MaximalMembers = 2;

        Assert.That(committee.NeedsAttentionAboveMaxMembers, Is.False);
    }

    [TestCase(CommitteeType.AuthoritiesCommissionGuidAsString)]
    [TestCase(CommitteeType.AdministrationCommissionGuidAsString)]
    public void NeedsAttentionAboveMaxMembers_WithMoreMembers_ShouldReturnTrue(string committeeTypeId)
    {
        var committee = new CommitteeBuilder()
            .WithCommitteeTypeId(new Guid(committeeTypeId))
            .WithMembership(new MembershipBuilder()
                .WithBeginDate(DateOnly.FromDateTime(DateTime.Today.AddDays(-3)))
                .WithEndDate(DateOnly.FromDateTime(DateTime.Today.AddDays(3))).Build())
            .WithMembership(new MembershipBuilder()
                .WithBeginDate(DateOnly.FromDateTime(DateTime.Today.AddDays(-3)))
                .WithEndDate(DateOnly.FromDateTime(DateTime.Today.AddDays(3))).Build())
            .WithMembership(new MembershipBuilder()
                .WithBeginDate(DateOnly.FromDateTime(DateTime.Today.AddDays(-3)))
                .WithEndDate(DateOnly.FromDateTime(DateTime.Today.AddDays(3))).Build())
            .Build();
        committee.MaximalMembers = 2;

        Assert.That(committee.NeedsAttentionAboveMaxMembers, Is.True);
    }

    [TestCase(CommitteeType.AuthoritiesCommissionGuidAsString)]
    [TestCase(CommitteeType.AdministrationCommissionGuidAsString)]
    public void NeedsAttentionAboveMaxMembers_WithInactiveMembers_ShouldReturnFalse(string committeeTypeId)
    {
        var committee = new CommitteeBuilder()
            .WithCommitteeTypeId(new Guid(committeeTypeId))
            .WithMembership(new MembershipBuilder()
                .WithBeginDate(DateOnly.FromDateTime(DateTime.Today.AddDays(1))).Build())
            .WithMembership(new MembershipBuilder()
                .WithBeginDate(DateOnly.FromDateTime(DateTime.Today.AddDays(1))).Build())
            .WithMembership(new MembershipBuilder()
                .WithBeginDate(DateOnly.FromDateTime(DateTime.Today.AddDays(-1))).Build())
            .Build();
        committee.MaximalMembers = 2;

        Assert.That(committee.NeedsAttentionAboveMaxMembers, Is.False);
    }

    [Test]
    public void NeedsAttentionAboveMaxMembers_WithNonExtraParliamentaryCommission_ShouldReturnFalse()
    {
        var committee = new CommitteeBuilder()
            .WithCommitteeTypeId(Guid.NewGuid())
            .WithMembership(new MembershipBuilder()
                .WithBeginDate(DateOnly.FromDateTime(DateTime.Today).AddDays(-1)).Build())
            .WithMembership(new MembershipBuilder()
                .WithBeginDate(DateOnly.FromDateTime(DateTime.Today).AddDays(-1)).Build())
            .WithMembership(new MembershipBuilder()
                .WithBeginDate(DateOnly.FromDateTime(DateTime.Today).AddDays(-1)).Build())
            .Build();
        committee.MaximalMembers = 2;

        Assert.That(committee.NeedsAttentionAboveMaxMembers, Is.False);
    }

    [Test]
    public void NeedsAttentionAboveMaxMembers_ShouldIgnoreMembershipsWithOtherElectionOffice()
    {
        var committee = new CommitteeBuilder()
            .WithCommitteeTypeId(CommitteeType.AdministrationCommissionGuid)
            .WithMembership(new MembershipBuilder()
                .WithElectionOfficeId(ElectionOffice.OtherGuid)
                .WithIsActive(true)
                .Build())
            .WithMembership(new MembershipBuilder()
                .WithIsActive(true)
                .Build())
            .Build();
        committee.MaximalMembers = 1;

        Assert.That(committee.NeedsAttentionAboveMaxMembers, Is.False);
    }

    [Test]
    public void NeedsAttentionDataProtectionOfficer_WithDpo_ShouldReturnFalse()
    {
        var committee = new CommitteeBuilder()
            .WithCommitteeTypeId(new Guid(CommitteeType.AuthoritiesCommissionGuidAsString))
            .WithContactPoint(new ContactPointBuilder()
                .WithContactPointType(new ContactPointTypeBuilder()
                    .WithId(ContactPointType.DataProtectionOfficerGuid).Build())
                .WithEndDate(DateOnly.FromDateTime(DateTime.Now).AddDays(1)).Build())
            .Build();

        Assert.That(committee.NeedsAttentionDataProtectionOfficer, Is.False);
    }

    [TestCase(CommitteeType.AuthoritiesCommissionGuidAsString, true)]
    [TestCase(CommitteeType.AdministrationCommissionGuidAsString, true)]
    [TestCase("FBEFEF07-CB51-4F6A-9911-FF1AC997554C", false)]
    public void NeedsAttentionDataProtectionOfficer_WithoutDpo_ShouldReturnExpected(string committeeTypeId, bool expected)
    {
        var committee = new CommitteeBuilder()
            .WithCommitteeTypeId(new Guid(committeeTypeId))
            .Build();

        Assert.That(committee.NeedsAttentionDataProtectionOfficer, Is.EqualTo(expected));
    }

    [Test]
    public void NeedsAttentionSecretariat_WithSecretariat_ShouldReturnFalse()
    {
        var committee = new CommitteeBuilder()
            .WithContactPoint(new ContactPointBuilder()
                .WithContactPointType(new ContactPointTypeBuilder().WithId(ContactPointType.SecretariatGuid).Build())
                .WithEndDate(DateOnly.FromDateTime(DateTime.UtcNow.AddDays(1)))
                .Build())
            .Build();

        Assert.That(committee.NeedsAttentionSecretariat, Is.False);
    }

    [Test]
    public void NeedsAttentionSecretariat_WithoutSecretariat_ShouldReturnTrue()
    {
        var committee = new CommitteeBuilder().Build();

        Assert.That(committee.NeedsAttentionSecretariat, Is.True);
    }

    [Test]
    public void NeedsAttention_WithActiveCommittee_ShouldReturnTrue()
    {
        var membership = new MembershipBuilder()
            .WithCommittee(
                new CommitteeBuilder()
                    .WithGermanDescription("DE")
                    .WithFrenchDescription("FR")
                    .WithItalianDescription("IT")
                    .Build())
            .WithPerson(new PersonBuilder().WithGender(new GenderBuilder().WithUri(Gender.Male).Build()).Build())
            .WithBeginDate(DateOnly.FromDateTime(DateTime.Today.AddDays(-2)))
            .WithEndDate(DateOnly.FromDateTime(DateTime.Today.AddDays(-1)))
            .WithElectionType(new ElectionTypeBuilder()
                .WithUri(ElectionType.NewElection).Build())
            .Build();

        var committee = new CommitteeBuilder()
            .WithBeginDate(DateOnly.FromDateTime(DateTime.Today.AddDays(-2)))
            .WithEndDate(DateOnly.FromDateTime(DateTime.Today.AddDays(2)))
            .WithMembership(membership).Build();

        Assert.That(committee.NeedsAttention, Is.True);
    }

    [Test]
    public void NeedsAttention_WithInActiveCommittee_ShouldReturnFalse()
    {
        var membership = new MembershipBuilder()
            .WithCommittee(
                new CommitteeBuilder()
                    .WithGermanDescription("DE")
                    .WithFrenchDescription("FR")
                    .WithItalianDescription("IT")
                    .Build())
            .WithPerson(new PersonBuilder().WithGender(new GenderBuilder().WithUri(Gender.Male).Build()).Build())
            .WithBeginDate(DateOnly.FromDateTime(DateTime.Today.AddDays(-2)))
            .WithEndDate(DateOnly.FromDateTime(DateTime.Today.AddDays(-1)))
            .WithElectionType(new ElectionTypeBuilder()
                .WithUri(ElectionType.NewElection).Build())
            .Build();

        var committee = new CommitteeBuilder()
            .WithBeginDate(DateOnly.FromDateTime(DateTime.Today.AddDays(-2)))
            .WithEndDate(DateOnly.FromDateTime(DateTime.Today.AddDays(-2)))
            .WithMembership(membership).Build();

        Assert.That(committee.NeedsAttention, Is.False);
    }

    [Test]
    public void NeedsAttention_WithNeedsAttentionSecretariat_ShouldReturnTrue()
    {
        var membership = new MembershipBuilder()
            .WithPerson(new PersonBuilder().WithGender(new GenderBuilder().WithUri(Gender.Male).Build()).Build())
            .WithBeginDate(DateOnly.FromDateTime(DateTime.Today.AddDays(-2)))
            .WithEndDate(DateOnly.FromDateTime(DateTime.Today.AddDays(2)))
            .WithElectionType(new ElectionTypeBuilder()
                .WithUri(ElectionType.NewElection).Build())
            .Build();

        var committee = new CommitteeBuilder()
            .WithBeginDate(DateOnly.FromDateTime(DateTime.Today.AddDays(-2)))
            .WithEndDate(DateOnly.FromDateTime(DateTime.Today.AddDays(2)))
            .WithMembership(membership)
            .Build();

        using (Assert.EnterMultipleScope())
        {
            Assert.That(committee.NeedsAttentionSecretariat, Is.True);
            Assert.That(committee.NeedsAttention, Is.True);
        }
    }

    [Test]
    public void NeedsAttention_WithSecretariatAndNoOtherIssues_ShouldReturnFalse()
    {
        var membership = new MembershipBuilder()
            .WithPerson(new PersonBuilder().WithGender(new GenderBuilder().WithUri(Gender.Male).Build()).Build())
            .WithBeginDate(DateOnly.FromDateTime(DateTime.Today.AddDays(-2)))
            .WithEndDate(DateOnly.FromDateTime(DateTime.Today.AddDays(2)))
            .WithElectionType(new ElectionTypeBuilder()
                .WithUri(ElectionType.NewElection).Build())
            .Build();

        var committee = new CommitteeBuilder()
            .WithBeginDate(DateOnly.FromDateTime(DateTime.Today.AddDays(-2)))
            .WithEndDate(DateOnly.FromDateTime(DateTime.Today.AddDays(2)))
            .WithMembership(membership)
            .WithContactPoint(new ContactPointBuilder()
                .WithContactPointType(new ContactPointTypeBuilder().WithId(ContactPointType.SecretariatGuid).Build())
                .WithEndDate(DateOnly.FromDateTime(DateTime.UtcNow.AddDays(10)))
                .Build())
            .Build();

        Assert.That(committee.NeedsAttentionSecretariat, Is.False);
    }

    [Test]
    public void ActiveMemberCountFuture_WithData_ShouldReturnNumber()
    {
        // future
        var membershipFutureMale = new MembershipBuilder()
            .WithPerson(new PersonBuilder().WithGender(new GenderBuilder().WithUri(Gender.Male).Build()).Build())
            .WithBeginDate(DateOnly.FromDateTime(DateTime.Today.AddDays(200)))
            .WithEndDate(DateOnly.FromDateTime(DateTime.Today.AddDays(400)))
            .WithElectionType(new ElectionTypeBuilder()
                .WithUri(ElectionType.NewElection).Build())
            .Build();

        var membershipFutureFemale = new MembershipBuilder()
            .WithPerson(new PersonBuilder().WithGender(new GenderBuilder().WithUri(Gender.Female).Build()).Build())
            .WithBeginDate(DateOnly.FromDateTime(DateTime.Today.AddDays(200)))
            .WithEndDate(DateOnly.FromDateTime(DateTime.Today.AddDays(400)))
            .WithElectionType(new ElectionTypeBuilder()
                .WithUri(ElectionType.NewElection).Build())
            .Build();

        // current
        var membershipPresentMale = new MembershipBuilder()
            .WithPerson(new PersonBuilder().WithGender(new GenderBuilder().WithUri(Gender.Male).Build()).Build())
            .WithBeginDate(DateOnly.FromDateTime(DateTime.Today.AddDays(-200)))
            .WithEndDate(DateOnly.FromDateTime(DateTime.Today.AddDays(+400)))
            .WithElectionType(new ElectionTypeBuilder()
                .WithUri(ElectionType.NewElection).Build())
            .Build();

        var membershipPresentFemale = new MembershipBuilder()
            .WithPerson(new PersonBuilder().WithGender(new GenderBuilder().WithUri(Gender.Female).Build()).Build())
            .WithBeginDate(DateOnly.FromDateTime(DateTime.Today.AddDays(-200)))
            .WithEndDate(DateOnly.FromDateTime(DateTime.Today.AddDays(+400)))
            .WithElectionType(new ElectionTypeBuilder()
                .WithUri(ElectionType.NewElection).Build())
            .Build();

        // old
        var membership = new MembershipBuilder()
            .WithPerson(new PersonBuilder().WithGender(new GenderBuilder().WithUri(Gender.Male).Build()).Build())
            .WithBeginDate(DateOnly.FromDateTime(DateTime.Today.AddDays(-200)))
            .WithEndDate(DateOnly.FromDateTime(DateTime.Today.AddDays(-10)))
            .WithElectionType(new ElectionTypeBuilder()
                .WithId(ElectionType.RetirementGuid).Build())
            .Build();

        var committee = new CommitteeBuilder()
            .WithBeginDate(DateOnly.FromDateTime(DateTime.Today.AddDays(-2)))
            .WithEndDate(DateOnly.FromDateTime(DateTime.Today.AddDays(-2)))
            .WithCommitteeType(new CommitteeTypeBuilder()
                .WithFemaleAndMaleThreshold(40, 40).Build())
            .WithMembership(membership).Build();

        committee.Memberships.Add(membershipFutureFemale);
        committee.Memberships.Add(membershipPresentFemale);

        Assert.Multiple(() =>
        {
            Assert.That(committee.FemaleUnderStuffed, Is.False);
            Assert.That(committee.MaleUnderStuffed, Is.True);
            Assert.That(committee.FemaleUnderStuffedFuture, Is.False);
            Assert.That(committee.MaleUnderStuffedFuture, Is.True);
        });

        committee.Memberships.Add(membershipFutureMale);
        committee.Memberships.Add(membershipPresentMale);

        Assert.Multiple(() =>
        {
            Assert.That(committee.ActiveMemberCountFuture, Is.EqualTo(2));
            Assert.That(committee.ActiveMemberCount, Is.EqualTo(2));
            Assert.That(committee.FemaleCountFuture, Is.EqualTo(1));
            Assert.That(committee.MaleCountFuture, Is.EqualTo(1));
            Assert.That(committee.FemaleCount, Is.EqualTo(1));
            Assert.That(committee.MaleCount, Is.EqualTo(1));
            Assert.That(committee.MaleUnderStuffed, Is.False);
            Assert.That(committee.MaleUnderStuffedFuture, Is.False);
        });
    }

    [Test]
    public void FemaleQuota_WithActiveMembers_ShouldReturnCorrectPercentage()
    {
        var committee = new CommitteeBuilder()
            .WithMembership(new MembershipBuilder()
                .WithIsActive(true)
                .WithPerson(new PersonBuilder().WithGender(new GenderBuilder().WithUri(Gender.Female).Build()).Build())
                .Build())
            .WithMembership(new MembershipBuilder()
                .WithIsActive(true)
                .WithPerson(new PersonBuilder().WithGender(new GenderBuilder().WithUri(Gender.Male).Build()).Build())
                .Build())
            .WithMembership(new MembershipBuilder()
                .WithIsActive(true)
                .WithPerson(new PersonBuilder().WithGender(new GenderBuilder().WithUri(Gender.Male).Build()).Build())
                .Build())
            .WithMembership(new MembershipBuilder()
                .WithIsActive(true)
                .WithElectionOfficeId(ElectionOffice.OtherGuid)
                .WithPerson(new PersonBuilder().WithGender(new GenderBuilder().WithUri(Gender.Male).Build()).Build())
                .Build())
            .Build();

        Assert.That(committee.FemaleQuota, Is.EqualTo(33.33));
    }

    [Test]
    public void FemaleQuota_WithoutActiveMembers_ShouldReturnZero()
    {
        var committee = new CommitteeBuilder().Build();

        Assert.That(committee.FemaleQuota, Is.Zero);
    }

    [Test]
    public void MaleQuota_WithActiveMembers_ShouldReturnCorrectPercentage()
    {
        var committee = new CommitteeBuilder()
            .WithMembership(new MembershipBuilder()
                .WithIsActive(true)
                .WithPerson(new PersonBuilder().WithGender(new GenderBuilder().WithUri(Gender.Male).Build()).Build())
                .Build())
            .WithMembership(new MembershipBuilder()
                .WithIsActive(true)
                .WithPerson(new PersonBuilder().WithGender(new GenderBuilder().WithUri(Gender.Female).Build()).Build())
                .Build())
            .WithMembership(new MembershipBuilder()
                .WithIsActive(true)
                .WithPerson(new PersonBuilder().WithGender(new GenderBuilder().WithUri(Gender.Female).Build()).Build())
                .Build())
            .WithMembership(new MembershipBuilder()
                .WithIsActive(true)
                .WithElectionOfficeId(ElectionOffice.OtherGuid)
                .WithPerson(new PersonBuilder().WithGender(new GenderBuilder().WithUri(Gender.Female).Build()).Build())
                .Build())
            .Build();

        Assert.That(committee.MaleQuota, Is.EqualTo(33.33));
    }

    [Test]
    public void MaleQuota_WithoutActiveMembers_ShouldReturnZero()
    {
        var committee = new CommitteeBuilder().Build();

        Assert.That(committee.MaleQuota, Is.Zero);
    }

    [Test]
    public void GermanQuota_WithActiveMembers_ShouldReturnCorrectPercentage()
    {
        var committee = new CommitteeBuilder()
            .WithMembership(new MembershipBuilder()
                .WithIsActive(true)
                .WithPerson(new PersonBuilder().WithLanguage(new LanguageBuilder().WithUri(Language.GermanUri).Build()).Build())
                .Build())
            .WithMembership(new MembershipBuilder()
                .WithIsActive(true)
                .WithPerson(new PersonBuilder().WithLanguage(new LanguageBuilder().WithUri(Language.FrenchUri).Build()).Build())
                .Build())
            .WithMembership(new MembershipBuilder()
                .WithIsActive(true)
                .WithElectionOfficeId(ElectionOffice.OtherGuid)
                .WithPerson(new PersonBuilder().WithLanguage(new LanguageBuilder().WithUri(Language.FrenchUri).Build()).Build())
                .Build())
            .Build();

        Assert.That(committee.GermanQuota, Is.EqualTo(50));
    }

    [Test]
    public void GermanQuota_WithoutActiveMembers_ShouldReturnZero()
    {
        var committee = new CommitteeBuilder().Build();

        Assert.That(committee.GermanQuota, Is.Zero);
    }

    [Test]
    public void FrenchQuota_WithActiveMembers_ShouldReturnCorrectPercentage()
    {
        var committee = new CommitteeBuilder()
            .WithMembership(new MembershipBuilder()
                .WithIsActive(true)
                .WithPerson(new PersonBuilder().WithLanguage(new LanguageBuilder().WithUri(Language.FrenchUri).Build()).Build())
                .Build())
            .WithMembership(new MembershipBuilder()
                .WithIsActive(true)
                .WithPerson(new PersonBuilder().WithLanguage(new LanguageBuilder().WithUri(Language.GermanUri).Build()).Build())
                .Build())
            .WithMembership(new MembershipBuilder()
                .WithIsActive(true)
                .WithPerson(new PersonBuilder().WithLanguage(new LanguageBuilder().WithUri(Language.GermanUri).Build()).Build())
                .Build())
            .WithMembership(new MembershipBuilder()
                .WithIsActive(true)
                .WithElectionOfficeId(ElectionOffice.OtherGuid)
                .WithPerson(new PersonBuilder().WithLanguage(new LanguageBuilder().WithUri(Language.GermanUri).Build()).Build())
                .Build())
            .Build();

        Assert.That(committee.FrenchQuota, Is.EqualTo(33.33));
    }

    [Test]
    public void FrenchQuota_WithoutActiveMembers_ShouldReturnZero()
    {
        var committee = new CommitteeBuilder().Build();

        Assert.That(committee.FrenchQuota, Is.Zero);
    }

    [Test]
    public void ItalianQuota_WithActiveMembers_ShouldReturnCorrectPercentage()
    {
        var committee = new CommitteeBuilder()
            .WithMembership(new MembershipBuilder()
                .WithIsActive(true)
                .WithPerson(new PersonBuilder().WithLanguage(new LanguageBuilder().WithUri(Language.ItalianUri).Build()).Build())
                .Build())
            .WithMembership(new MembershipBuilder()
                .WithIsActive(true)
                .WithPerson(new PersonBuilder().WithLanguage(new LanguageBuilder().WithUri(Language.GermanUri).Build()).Build())
                .Build())
            .WithMembership(new MembershipBuilder()
                .WithIsActive(true)
                .WithElectionOfficeId(ElectionOffice.OtherGuid)
                .WithPerson(new PersonBuilder().WithLanguage(new LanguageBuilder().WithUri(Language.GermanUri).Build()).Build())
                .Build())
            .Build();

        Assert.That(committee.ItalianQuota, Is.EqualTo(50));
    }

    [Test]
    public void ItalianQuota_WithoutActiveMembers_ShouldReturnZero()
    {
        var committee = new CommitteeBuilder().Build();

        Assert.That(committee.ItalianQuota, Is.Zero);
    }

    [Test]
    public void RomanshQuota_WithActiveMembers_ShouldReturnCorrectPercentage()
    {
        var committee = new CommitteeBuilder()
            .WithMembership(new MembershipBuilder()
                .WithIsActive(true)
                .WithPerson(new PersonBuilder().WithLanguage(new LanguageBuilder().WithUri(Language.RomanshUri).Build()).Build())
                .Build())
            .WithMembership(new MembershipBuilder()
                .WithIsActive(true)
                .WithPerson(new PersonBuilder().WithLanguage(new LanguageBuilder().WithUri(Language.GermanUri).Build()).Build())
                .Build())
            .WithMembership(new MembershipBuilder()
                .WithIsActive(true)
                .WithPerson(new PersonBuilder().WithLanguage(new LanguageBuilder().WithUri(Language.GermanUri).Build()).Build())
                .Build())
            .WithMembership(new MembershipBuilder()
                .WithIsActive(true)
                .WithPerson(new PersonBuilder().WithLanguage(new LanguageBuilder().WithUri(Language.GermanUri).Build()).Build())
                .Build())
            .WithMembership(new MembershipBuilder()
                .WithIsActive(true)
                .WithElectionOfficeId(ElectionOffice.OtherGuid)
                .WithPerson(new PersonBuilder().WithLanguage(new LanguageBuilder().WithUri(Language.GermanUri).Build()).Build())
                .Build())
            .Build();

        Assert.That(committee.RomanshQuota, Is.EqualTo(25));
    }

    [Test]
    public void RomanshQuota_WithoutActiveMembers_ShouldReturnZero()
    {
        var committee = new CommitteeBuilder().Build();

        Assert.That(committee.RomanshQuota, Is.Zero);
    }

    [Test]
    public void LanguageUnderStaffed_AboveThreshold_ShouldReturnFalse()
    {
        var committee = new CommitteeBuilder()
            .WithCommitteeType(new CommitteeTypeBuilder().WithLanguagesThreshold(0, 0, 0, 0).WithLanguagesPercentageThreshold(null, null, null, null).Build())
            .WithMembership(new MembershipBuilder()
                .WithPerson(new PersonBuilder().WithLanguage(new LanguageBuilder().WithUri(Language.GermanUri).Build()).Build())
                .WithBeginDate(DateOnly.FromDateTime(DateTime.Today.AddDays(-1)))
                .Build())
            .WithMembership(new MembershipBuilder()
                .WithPerson(new PersonBuilder().WithLanguage(new LanguageBuilder().WithUri(Language.FrenchUri).Build()).Build())
                .WithBeginDate(DateOnly.FromDateTime(DateTime.Today.AddDays(-1)))
                .Build())
            .WithMembership(new MembershipBuilder()
                .WithPerson(new PersonBuilder().WithLanguage(new LanguageBuilder().WithUri(Language.ItalianUri).Build()).Build())
                .WithBeginDate(DateOnly.FromDateTime(DateTime.Today.AddDays(-1)))
                .Build())
            .Build();

        Assert.Multiple(() =>
        {
            Assert.That(committee.GermanUnderStaffed, Is.False);
            Assert.That(committee.FrenchUnderStaffed, Is.False);
            Assert.That(committee.ItalianUnderStaffed, Is.False);
        });
    }

    [Test]
    public void LanguageUnderStaffed_BelowThreshold_ShouldReturnTrue()
    {
        var committee = new CommitteeBuilder()
            .WithCommitteeType(new CommitteeTypeBuilder().WithLanguagesThreshold(1, 1, 1, 1).WithLanguagesPercentageThreshold(null, null, null, null).Build())
            .Build();

        Assert.Multiple(() =>
        {
            Assert.That(committee.GermanUnderStaffed, Is.True);
            Assert.That(committee.FrenchUnderStaffed, Is.True);
            Assert.That(committee.ItalianUnderStaffed, Is.True);
        });
    }

    [Test]
    public void LanguageUnderStaffed_AbovePercentage_ShouldReturnFalse()
    {
        var committee = new CommitteeBuilder()
            .WithCommitteeType(new CommitteeTypeBuilder().WithLanguagesPercentageThreshold(0, 0, 0, 0).WithLanguagesThreshold(null, null, null, null).Build())
            .WithMembership(new MembershipBuilder().WithPerson(new PersonBuilder().WithLanguage(new LanguageBuilder().WithUri(Language.GermanUri).Build()).Build()).Build())
            .WithMembership(new MembershipBuilder().WithPerson(new PersonBuilder().WithLanguage(new LanguageBuilder().WithUri(Language.FrenchUri).Build()).Build()).Build())
            .WithMembership(new MembershipBuilder().WithPerson(new PersonBuilder().WithLanguage(new LanguageBuilder().WithUri(Language.ItalianUri).Build()).Build()).Build())
            .Build();

        Assert.Multiple(() =>
        {
            Assert.That(committee.GermanUnderStaffed, Is.False);
            Assert.That(committee.FrenchUnderStaffed, Is.False);
            Assert.That(committee.ItalianUnderStaffed, Is.False);
        });
    }

    [Test]
    public void LanguageUnderStaffed_BelowPercentage_ShouldReturnTrue()
    {
        var committee = new CommitteeBuilder()
            .WithCommitteeType(new CommitteeTypeBuilder()
                .WithLanguagesPercentageThreshold(10, 10, 10, 10)
                .WithLanguagesThreshold(null, null, null, null).Build())
            .Build();

        Assert.Multiple(() =>
        {
            Assert.That(committee.GermanUnderStaffed, Is.True);
            Assert.That(committee.FrenchUnderStaffed, Is.True);
            Assert.That(committee.ItalianUnderStaffed, Is.True);
        });
    }

    [Test]
    public void ActiveMemberCount_ShouldIgnoreMembershipsWithOtherElectionOffice()
    {
        var committee = new CommitteeBuilder()
            .WithMembership(new MembershipBuilder().WithIsActive(true).WithElectionOfficeId(ElectionOffice.OtherGuid).Build())
            .WithMembership(new MembershipBuilder().WithIsActive(true).Build())
            .Build();

        Assert.That(committee.ActiveMemberCount, Is.EqualTo(1));
    }
}
