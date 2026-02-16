using Bk.APG.Business.Mapper;
using Bk.APG.Business.Models;
using Bk.APG.CrossCutting.Tests.Builders;

namespace Bk.APG.Business.Tests.Mapper;

[TestFixture]
internal class GeneralElectionMapperTests
{
    [Test]
    public void FromCommitteeToGeneralElectionCommitteeDto_ShouldMapCorrectly()
    {
        var committee = new CommitteeBuilder()
            .WithBeginDate(DateOnly.FromDateTime(DateTime.UtcNow.AddMinutes(-1)))
            .WithEndDate(DateOnly.FromDateTime(DateTime.UtcNow.AddMinutes(1)))
            .WithDepartment(new DepartmentBuilder().Build())
            .WithOffice(new OfficeBuilder().Build())
            .WithTermOfOffice(new TermOfOfficeBuilder().Build())
            .WithCommitteeType(new CommitteeTypeBuilder().Build())
            .WithCommitteeLevel(new CommitteeLevelBuilder().Build())
            .WithMembership(new MembershipBuilder().WithBeginDate(DateOnly.FromDateTime(DateTime.UtcNow.AddDays(-3))).Build())
            .Build();

        var generalElectionCommittee = GeneralElectionMapper.FromCommitteeToGeneralElectionCommittee(committee, "test");

        Assert.That(generalElectionCommittee, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(generalElectionCommittee.CommitteeId, Is.EqualTo(committee.Id));
            Assert.That(generalElectionCommittee.DepartmentId, Is.EqualTo(committee.DepartmentId));
            Assert.That(generalElectionCommittee.OfficeId, Is.EqualTo(committee.OfficeId));
            Assert.That(generalElectionCommittee.CommitteeLevelId, Is.EqualTo(committee.CommitteeLevelId));
            Assert.That(generalElectionCommittee.CommitteeTypeId, Is.EqualTo(committee.CommitteeTypeId));
            Assert.That(generalElectionCommittee.LegalFormId, Is.EqualTo(committee.LegalFormId));
            Assert.That(generalElectionCommittee.LegalBase, Is.EqualTo(committee.LegalBase));
            Assert.That(generalElectionCommittee.ReleaseGeneralElection, Is.EqualTo(committee.ReleaseGeneralElection));
            Assert.That(generalElectionCommittee.FederalLawEstablishment, Is.EqualTo(committee.FederalLawEstablishment));
            Assert.That(generalElectionCommittee.MarketOrientated, Is.EqualTo(committee.MarketOrientated));
            Assert.That(generalElectionCommittee.ExtraParliamentaryCommission, Is.EqualTo(committee.ExtraParliamentaryCommission));
            Assert.That(generalElectionCommittee.SupervisionDuty, Is.EqualTo(committee.SupervisionDuty));
            Assert.That(generalElectionCommittee.BeginDate, Is.EqualTo(committee.BeginDate));
            Assert.That(generalElectionCommittee.EndDate, Is.EqualTo(committee.EndDate));
            Assert.That(generalElectionCommittee.TermOfOfficeId, Is.EqualTo(committee.TermOfOfficeId));
            Assert.That(generalElectionCommittee.MinimalMembers, Is.EqualTo(committee.MinimalMembers));
            Assert.That(generalElectionCommittee.MaximalMembers, Is.EqualTo(committee.MaximalMembers));
            Assert.That(generalElectionCommittee.VacanciesGeneralElection, Is.Null);
            Assert.That(generalElectionCommittee.AdditionalAuthorityMembers, Is.EqualTo(committee.AdditionalAuthorityMembers));
            Assert.That(generalElectionCommittee.LinkAuthorityWebsite, Is.EqualTo(committee.LinkAuthorityWebsite));
            Assert.That(generalElectionCommittee.LinkHomepageGerman, Is.EqualTo(committee.LinkHomepageGerman));
            Assert.That(generalElectionCommittee.LinkHomepageFrench, Is.EqualTo(committee.LinkHomepageFrench));
            Assert.That(generalElectionCommittee.LinkHomepageItalian, Is.EqualTo(committee.LinkHomepageItalian));
            Assert.That(generalElectionCommittee.LinkHomepageRomansh, Is.EqualTo(committee.LinkHomepageRomansh));
            Assert.That(generalElectionCommittee.RemarksBaseData, Is.EqualTo(committee.RemarksBaseData));
            Assert.That(generalElectionCommittee.RemarksBaseDataAdmin, Is.EqualTo(committee.RemarksBaseDataAdmin));
            Assert.That(generalElectionCommittee.IsDeleted, Is.EqualTo(committee.IsDeleted));
            Assert.That(generalElectionCommittee.JustificationMembers, Is.EqualTo(committee.JustificationMembers));
        });
    }

    [Test]
    public void FromMembershipAndPersonToMembershipCandidate_ShouldMapCorrectly()
    {
        var candidateListId = Guid.NewGuid();
        var termOfOfficeBeginDate = new DateOnly(2028, 1, 1);
        var termOfOfficeEndDate = new DateOnly(2031, 12, 31);

        var person = new PersonBuilder().WithLanguage(new LanguageBuilder().Build()).WithGender(new GenderBuilder().Build()).Build();

        var membership = new MembershipBuilder()
            .WithPerson(person)
            .WithBeginDate(DateOnly.FromDateTime(DateTime.Today.AddDays(-3)))
            .WithEndDate(DateOnly.FromDateTime(DateTime.Today.AddDays(2)))
            .WithIsActive(true)
            .Build();

        var membershipCandidate = GeneralElectionMapper.FromMembershipAndPersonToMembershipCandidate(membership, candidateListId, "Fritz Tester", termOfOfficeBeginDate, termOfOfficeEndDate);

        Assert.That(membershipCandidate, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(membershipCandidate.Surname, Is.EqualTo(person.Surname));
            Assert.That(membershipCandidate.GivenName, Is.EqualTo(person.GivenName));
            Assert.That(membershipCandidate.LanguageId, Is.EqualTo(person.LanguageId));
            Assert.That(membershipCandidate.GenderId, Is.EqualTo(person.GenderId));
            Assert.That(membershipCandidate.BirthYear, Is.EqualTo(person.BirthYear));
            Assert.That(membershipCandidate.MaximumEmploymentLevel, Is.EqualTo(membership.MaximumEmploymentLevel));
            Assert.That(membershipCandidate.BeginDate, Is.EqualTo(termOfOfficeBeginDate));
            Assert.That(membershipCandidate.EndDate, Is.EqualTo(termOfOfficeEndDate));
            Assert.That(membershipCandidate.ElectionTypeId, Is.EqualTo(ElectionType.ReElectionGuid));
            Assert.That(membershipCandidate.FunctionId, Is.EqualTo(membership.FunctionId));
            Assert.That(membershipCandidate.ElectionOfficeId, Is.EqualTo(membership.ElectionOfficeId));
            Assert.That(membershipCandidate.MembershipAdditionId, Is.EqualTo(membership.MembershipAdditionId));
            Assert.That(membershipCandidate.JustificationLongerDuty, Is.EqualTo(membership.JustificationLongerDuty));
            Assert.That(membershipCandidate.JustificationShorterDuty, Is.EqualTo(string.Empty));
            Assert.That(membershipCandidate.JustificationMemberInFederalDuty, Is.EqualTo(membership.JustificationMemberInFederalDuty));
            Assert.That(membershipCandidate.JustificationMemberInFederalAssembly, Is.EqualTo(membership.JustificationMemberInFederalAssembly));
            Assert.That(membershipCandidate.RequirementsProfile, Is.EqualTo(string.Empty));
            Assert.That(membershipCandidate.Remarks, Is.EqualTo(membership.Remarks));
            Assert.That(membershipCandidate.RemarksStatus, Is.EqualTo(membership.RemarksStatus));
            Assert.That(membershipCandidate.InCorrelationWithFederalDuty, Is.EqualTo(membership.InCorrelationWithFederalDuty));
            Assert.That(membershipCandidate.IsDeleted, Is.EqualTo(false));
        });
    }

    [Test]
    public void ToCommitteeMemberDto_WhenPersonIsNull_ShouldMapCorrectly()
    {
        var membershipCandidate = new MembershipCandidateBuilder()
            .WithSurname("TestSurname")
            .WithGivenName("TestGivenName")
            .WithGender(new GenderBuilder().WithId(Gender.FemaleGuid).Build())
            .WithLanguage(new LanguageBuilder().Build())
            .Build();

        membershipCandidate.Person = null;

        var result = GeneralElectionMapper.ToCommitteeMemberDto(membershipCandidate);

        Assert.That(result, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(result.Id, Is.EqualTo(membershipCandidate.Id));
            Assert.That(result.PersonId, Is.EqualTo(membershipCandidate.PersonId ?? Guid.Empty));
            Assert.That(result.Surname, Is.EqualTo(membershipCandidate.Surname));
            Assert.That(result.GivenName, Is.EqualTo(membershipCandidate.GivenName));
            Assert.That(result.Gender, Is.EqualTo(membershipCandidate.Gender?.GetText() ?? string.Empty));
            Assert.That(result.Language, Is.EqualTo(membershipCandidate.Language?.GetText() ?? string.Empty));
            Assert.That(result.Function, Is.EqualTo(membershipCandidate.Function?.GetFemaleText() ?? string.Empty));
            Assert.That(result.BeginDate, Is.EqualTo(membershipCandidate.BeginDate));
            Assert.That(result.EndDate, Is.EqualTo(membershipCandidate.EndDate));
            Assert.That(result.ElectionType, Is.EqualTo(membershipCandidate.ElectionType?.GetText() ?? string.Empty));
            Assert.That(result.HasMembershipAddition, Is.EqualTo(membershipCandidate.MembershipAddition is not null));
            Assert.That(result.IsActive, Is.True);
            Assert.That(result.IsFuture, Is.False);
            Assert.That(result.NeedsAttention, Is.EqualTo(membershipCandidate.HasMembershipValidationIssues));
        });
    }
}
