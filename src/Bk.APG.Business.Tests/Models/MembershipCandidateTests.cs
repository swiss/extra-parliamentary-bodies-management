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
            .WithGeneralElectionCommittee(new GeneralElectionCommitteeBuilder()
                .WithCommitteeType(new CommitteeTypeBuilder().WithId(CommitteeType.AuthoritiesCommissionGuid).Build()).Build())
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
            .WithGeneralElectionCommittee(new GeneralElectionCommitteeBuilder()
                .WithCommitteeType(new CommitteeTypeBuilder().WithId(CommitteeType.AuthoritiesCommissionGuid).Build()).Build())
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
            .WithOccupations([new OccupationBuilder().Build()])
            .Build();

        var candidate = new MembershipCandidateBuilder()
            .WithPerson(person)
            .WithElectionTypeId(ElectionType.NewElectionGuid)
            .WithGeneralElectionCommittee(new GeneralElectionCommitteeBuilder()
                .WithCommitteeType(new CommitteeTypeBuilder().WithId(CommitteeType.AuthoritiesCommissionGuid).Build()).Build())
            .Build();

        Assert.That(candidate.NeedsAttentionBasicDataOrOccupation, Is.False);
    }

    [Test]
    public void MaximumEmploymentLevelMissing_WhenMarketOrientatedAndMissing_ShouldReturnTrue()
    {
        var committee = new GeneralElectionCommitteeBuilder()
            .WithMarketOrientated(true)
            .Build();

        var candidate = new MembershipCandidateBuilder()
            .WithGeneralElectionCommittee(committee)
            .Build();
        candidate.MaximumEmploymentLevel = null;

        Assert.That(candidate.MaximumEmploymentLevelMissing, Is.True);
    }

    [Test]
    public void EstimatedTermOfOffice_WhenDatesProvided_ShouldReturnExpectedValue()
    {
        var candidate = new MembershipCandidateBuilder()
            .WithBeginDate(new DateOnly(2010, 1, 1))
            .WithEndDate(new DateOnly(2020, 12, 31))
            .Build();

        Assert.That(candidate.EstimatedTermOfOffice, Is.EqualTo(11));
    }

    [Test]
    public void CurrentTermOfOffice_WhenMembershipsMatchCommittee_ShouldReturnExpectedValue()
    {
        var committeeId = Guid.Parse("f2b4b2f9-c9aa-4b7d-8ff0-3d6a6b6bdb1d");
        var membership = new MembershipBuilder()
            .WithCommitteeId(committeeId)
            .WithBeginDate(new DateOnly(2010, 1, 1))
            .WithEndDate(new DateOnly(2012, 12, 31))
            .Build();

        var person = new PersonBuilder()
            .WithMemberships([membership])
            .Build();

        var committee = new GeneralElectionCommitteeBuilder()
            .WithCommitteeId(committeeId)
            .Build();

        var candidate = new MembershipCandidateBuilder()
            .WithPerson(person)
            .WithGeneralElectionCommittee(committee)
            .Build();

        Assert.That(candidate.CurrentTermOfOffice, Is.EqualTo(3));
    }

    [Test]
    public void NeedsLongerDutyJustification_WhenExtraParliamentaryAndLongTerm_ShouldReturnTrue()
    {
        var committee = new GeneralElectionCommitteeBuilder()
            .WithCommitteeTypeId(CommitteeType.AuthoritiesCommissionGuid)
            .Build();

        var candidate = new MembershipCandidateBuilder()
            .WithGeneralElectionCommittee(committee)
            .WithBeginDate(new DateOnly(2000, 1, 1))
            .WithEndDate(new DateOnly(2013, 12, 31))
            .Build();
        candidate.JustificationLongerDuty = null;

        using (Assert.EnterMultipleScope())
        {
            Assert.That(candidate.NeedsLongerDutyJustification, Is.True);
            Assert.That(candidate.HasMissingLongerDutyJustification, Is.True);
        }
    }

    [Test]
    public void NeedsShorterDutyJustification_WhenTermEndsEarly_ShouldReturnTrue()
    {
        var termOfOfficeDate = new TermOfOfficeDateBuilder()
            .WithEndDate(new DateOnly(2025, 12, 31))
            .Build();

        var committee = new GeneralElectionCommitteeBuilder()
            .WithTermOfOfficeId(TermOfOffice.Period4YearsInGeneralElectionGuid)
            .WithTermOfOfficeDate(termOfOfficeDate)
            .Build();

        var candidate = new MembershipCandidateBuilder()
            .WithGeneralElectionCommittee(committee)
            .WithEndDate(new DateOnly(2024, 12, 31))
            .Build();
        candidate.JustificationShorterDuty = null;

        using (Assert.EnterMultipleScope())
        {
            Assert.That(candidate.NeedsShorterDutyJustification, Is.True);
            Assert.That(candidate.HasMissingShorterDutyJustification, Is.True);
        }
    }

    [Test]
    public void FederalDutyAndAssemblyJustifications_WhenMissing_ShouldReturnTrue()
    {
        var committee = new GeneralElectionCommitteeBuilder()
            .WithCommitteeTypeId(CommitteeType.AdministrationCommissionGuid)
            .Build();

        var person = new PersonBuilder()
            .WithFederalDuty(true)
            .WithFederalAssembly(true)
            .Build();

        var candidate = new MembershipCandidateBuilder()
            .WithGeneralElectionCommittee(committee)
            .WithPerson(person)
            .Build();
        candidate.JustificationMemberInFederalDuty = null;
        candidate.JustificationMemberInFederalAssembly = null;

        using (Assert.EnterMultipleScope())
        {
            Assert.That(candidate.NeedsFederalDutyJustification, Is.True);
            Assert.That(candidate.HasMissingFederalDutyJustification, Is.True);
            Assert.That(candidate.NeedsFederalAssemblyJustification, Is.True);
            Assert.That(candidate.HasMissingFederalAssemblyJustification, Is.True);
        }
    }

    [Test]
    public void NeedsRequirementsProfile_WhenNewElectionAndManagementCommittee_ShouldReturnTrue()
    {
        var committee = new GeneralElectionCommitteeBuilder()
            .WithCommitteeTypeId(CommitteeType.ManagementCommitteeGuid)
            .Build();

        var candidate = new MembershipCandidateBuilder()
            .WithGeneralElectionCommittee(committee)
            .WithElectionTypeId(ElectionType.NewElectionGuid)
            .Build();
        candidate.RequirementsProfile = null;

        using (Assert.EnterMultipleScope())
        {
            Assert.That(candidate.NeedsRequirementsProfile, Is.True);
            Assert.That(candidate.HasMissingRequirementsProfile, Is.True);
        }
    }

    [Test]
    public void MaximumDurationExceeded_WhenExtraParliamentaryAndLongTerm_ShouldReturnTrue()
    {
        var committee = new GeneralElectionCommitteeBuilder()
            .WithCommitteeTypeId(CommitteeType.AuthoritiesCommissionGuid)
            .Build();

        var candidate = new MembershipCandidateBuilder()
            .WithGeneralElectionCommittee(committee)
            .WithBeginDate(new DateOnly(2000, 1, 1))
            .WithEndDate(new DateOnly(2017, 12, 31))
            .Build();

        Assert.That(candidate.MaximumDurationExceeded, Is.True);
    }

    [Test]
    public void HasFederalAssemblyAuthoritiesCommissionConflict_WhenOverlapping_ShouldReturnTrue()
    {
        var committee = new GeneralElectionCommitteeBuilder()
            .WithCommitteeTypeId(CommitteeType.AuthoritiesCommissionGuid)
            .Build();

        var legislaturePeriod = new LegislaturePeriodBuilder()
            .WithStartDate(new DateOnly(2020, 1, 1))
            .WithEndDate(new DateOnly(2022, 12, 31))
            .Build();

        var person = new PersonBuilder()
            .WithFederalAssembly(true)
            .WithLegislaturePeriods([legislaturePeriod])
            .Build();

        var candidate = new MembershipCandidateBuilder()
            .WithGeneralElectionCommittee(committee)
            .WithPerson(person)
            .WithBeginDate(new DateOnly(2021, 1, 1))
            .WithEndDate(new DateOnly(2023, 1, 1))
            .Build();

        Assert.That(candidate.HasFederalAssemblyAuthoritiesCommissionConflict, Is.True);
    }

    [Test]
    public void HasMembershipValidationIssues_WhenCompletedAndIssuesExist_ShouldReturnTrue()
    {
        var committee = new GeneralElectionCommitteeBuilder()
            .WithCandidateListStateId(CandidateListState.Completed)
            .WithMarketOrientated(true)
            .Build();

        var candidate = new MembershipCandidateBuilder()
            .WithGeneralElectionCommittee(committee)
            .Build();
        candidate.MaximumEmploymentLevel = null;

        Assert.That(candidate.HasMembershipValidationIssues, Is.True);
    }
}
