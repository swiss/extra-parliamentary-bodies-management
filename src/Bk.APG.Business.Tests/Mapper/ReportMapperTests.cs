using Bk.APG.Business.Mapper;
using Bk.APG.Business.Models;
using Bk.APG.CrossCutting.Tests.Builders;

namespace Bk.APG.Business.Tests.Mapper;

[TestFixture]
internal class ReportMapperTests
{
    [Test]
    public void FromGeneralElectionCommitteeToReportGeneralElectionCommitteeDto_ShouldMapCorrectly()
    {
        var generalElectionCommittee = GenerateTestGeneralElectionCommittee();

        var reportDto = ReportMapper.FromGeneralElectionCommitteeToReportGeneralElectionCommitteeDto(generalElectionCommittee);

        Assert.That(reportDto, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(reportDto.Id, Is.EqualTo(generalElectionCommittee.CommitteeId));
            Assert.That(reportDto.DescriptionGerman, Is.EqualTo(generalElectionCommittee.DescriptionGerman));
            Assert.That(reportDto.DescriptionFrench, Is.EqualTo(generalElectionCommittee.DescriptionFrench));
            Assert.That(reportDto.DescriptionItalian, Is.EqualTo(generalElectionCommittee.DescriptionItalian));
            Assert.That(reportDto.DescriptionRomansh, Is.EqualTo(generalElectionCommittee.DescriptionRomansh));
            Assert.That(reportDto.DepartmentId, Is.EqualTo(generalElectionCommittee.DepartmentId));
            Assert.That(reportDto.OfficeId, Is.EqualTo(generalElectionCommittee.OfficeId));
            Assert.That(reportDto.CommitteeLevelId, Is.EqualTo(generalElectionCommittee.CommitteeLevelId));
            Assert.That(reportDto.CommitteeTypeId, Is.EqualTo(generalElectionCommittee.CommitteeTypeId));
            Assert.That(reportDto.LegalFormId, Is.EqualTo(generalElectionCommittee.LegalFormId));
            Assert.That(reportDto.LegalBase, Is.EqualTo(generalElectionCommittee.LegalBase));
            Assert.That(reportDto.ReleaseGeneralElection, Is.EqualTo(generalElectionCommittee.ReleaseGeneralElection));
            Assert.That(reportDto.FederalLawEstablishment, Is.EqualTo(generalElectionCommittee.FederalLawEstablishment));
            Assert.That(reportDto.MarketOrientated, Is.EqualTo(generalElectionCommittee.MarketOrientated));
            Assert.That(reportDto.ExtraParliamentaryCommission, Is.EqualTo(generalElectionCommittee.ExtraParliamentaryCommission));
            Assert.That(reportDto.SupervisionDuty, Is.EqualTo(generalElectionCommittee.SupervisionDuty));
            Assert.That(reportDto.BeginDate, Is.EqualTo(generalElectionCommittee.BeginDate));
            Assert.That(reportDto.EndDate, Is.EqualTo(generalElectionCommittee.EndDate));
            Assert.That(reportDto.TermOfOfficeId, Is.EqualTo(generalElectionCommittee.TermOfOfficeId));
            Assert.That(reportDto.MinimalMembers, Is.EqualTo(generalElectionCommittee.MinimalMembers));
            Assert.That(reportDto.MaximalMembers, Is.EqualTo(generalElectionCommittee.MaximalMembers));
            Assert.That(reportDto.VacanciesGeneralElection, Is.EqualTo(generalElectionCommittee.VacanciesGeneralElection));
            Assert.That(reportDto.AdditionalAuthorityMembers, Is.EqualTo(generalElectionCommittee.AdditionalAuthorityMembers));
            Assert.That(reportDto.LinkAuthorityWebsite, Is.EqualTo(generalElectionCommittee.LinkAuthorityWebsite));
            Assert.That(reportDto.RemarksBaseData, Is.EqualTo(generalElectionCommittee.RemarksBaseData));
            Assert.That(reportDto.RemarksBaseDataAdmin, Is.EqualTo(generalElectionCommittee.RemarksBaseDataAdmin));
            Assert.That(reportDto.IsDeleted, Is.EqualTo(generalElectionCommittee.IsDeleted));
            Assert.That(reportDto.JustificationMembers, Is.EqualTo(generalElectionCommittee.JustificationMembers));
            Assert.That(reportDto.SelectionProcedure, Is.EqualTo(generalElectionCommittee.SelectionProcedure));
            Assert.That(reportDto.IsValidated, Is.EqualTo(generalElectionCommittee.IsValidated));
            Assert.That(reportDto.Memberships.Count, Is.EqualTo(2));
        });
    }

    [Test]
    public void MapFromMembershipCandidateToReportMembershipDto_ShouldMapToDto()
    {
        var candidate = GenerateTestCandidate();

        var dto = ReportMapper.FromMembershipCandidateToReportMembershipDto(candidate);

        Assert.That(dto, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(dto.Person, Is.EqualTo(candidate.Person));
            Assert.That(dto.FunctionId, Is.EqualTo(candidate.FunctionId));
            Assert.That(dto.BeginDate, Is.EqualTo(candidate.BeginDate));
            Assert.That(dto.EndDate, Is.EqualTo(candidate.EndDate));
            Assert.That(dto.ElectionTypeId, Is.EqualTo(candidate.ElectionTypeId));
            Assert.That(dto.Surname, Is.EqualTo(candidate.Surname));
            Assert.That(dto.GivenName, Is.EqualTo(candidate.GivenName));
            Assert.That(dto.LanguageId, Is.EqualTo(candidate.LanguageId));
            Assert.That(dto.GenderId, Is.EqualTo(candidate.GenderId));
            Assert.That(dto.Remarks, Is.EqualTo(candidate.Remarks));
            Assert.That(dto.RemarksStatus, Is.EqualTo(candidate.RemarksStatus));
            Assert.That(dto.IsSelected, Is.EqualTo(candidate.IsSelected));
        });
    }

    [Test]
    public void MapFromMembershipToReportMembershipDto_ShouldMapToDto()
    {
        var membership = GenerateTestMembership();

        var dto = ReportMapper.FromMembershipToReportMembershipDto(membership);

        Assert.That(dto, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(dto.Person, Is.EqualTo(membership.Person));
            Assert.That(dto.FunctionId, Is.EqualTo(membership.FunctionId));
            Assert.That(dto.BeginDate, Is.EqualTo(membership.BeginDate));
            Assert.That(dto.EndDate, Is.EqualTo(membership.EndDate));
            Assert.That(dto.Surname, Is.EqualTo(membership.Person!.Surname));
            Assert.That(dto.GivenName, Is.EqualTo(membership.Person!.GivenName));
            Assert.That(dto.LanguageId, Is.EqualTo(membership.Person!.LanguageId));
            Assert.That(dto.GenderId, Is.EqualTo(membership.Person!.GenderId));
            Assert.That(dto.ElectionTypeId, Is.EqualTo(membership.ElectionTypeId));
            Assert.That(dto.Remarks, Is.EqualTo(membership.Remarks));
            Assert.That(dto.RemarksStatus, Is.EqualTo(membership.RemarksStatus));
            Assert.That(dto.IsSelected, Is.EqualTo(true));
        });
    }

    private static GeneralElectionCommittee GenerateTestGeneralElectionCommittee()
    {
        var generalElectionCommittee = new GeneralElectionCommitteeBuilder()
            .WithBeginDate(DateOnly.FromDateTime(DateTime.UtcNow.AddMinutes(-1)))
            .WithEndDate(DateOnly.FromDateTime(DateTime.UtcNow.AddMinutes(1)))
            .WithDepartment(new DepartmentBuilder().Build())
            .WithOffice(new OfficeBuilder().Build())
            .WithTermOfOffice(new TermOfOfficeBuilder().WithId(TermOfOffice.Period4YearsInGeneralElectionGuid).Build())
            .WithCommitteeType(new CommitteeTypeBuilder().Build())
            .WithCommitteeLevel(new CommitteeLevelBuilder().Build())
            .WithMinimalMember(11)
            .Build();

        var membershipCandidate1 = new MembershipCandidateBuilder()
            .WithBeginDate(DateOnly.FromDateTime(DateTime.Today.AddDays(-3)))
            .WithEndDate(DateOnly.FromDateTime(DateTime.Today.AddDays(2)))
            .WithIsActive(true)
            .WithIsSelected(true)
            .Build();

        var membershipCandidate2 = new MembershipCandidateBuilder()
            .WithBeginDate(DateOnly.FromDateTime(DateTime.Today.AddDays(-3)))
            .WithEndDate(DateOnly.FromDateTime(DateTime.Today.AddDays(2)))
            .WithIsActive(true)
            .WithIsSelected(true)
            .Build();

        generalElectionCommittee.MembershipCandidates.Add(membershipCandidate1);
        generalElectionCommittee.MembershipCandidates.Add(membershipCandidate2);

        return generalElectionCommittee;
    }

    private static MembershipCandidate GenerateTestCandidate()
    {
        var membershipCandidate = new MembershipCandidateBuilder()
            .WithSurname("Niven")
            .WithGivenName("David")
            .WithGenderId(Guid.NewGuid())
            .WithLanguageId(Guid.NewGuid())
            .WithBeginDate(DateOnly.FromDateTime(DateTime.Today.AddDays(-1)))
            .WithEndDate(DateOnly.FromDateTime(DateTime.Today.AddDays(+1)))
            .WithIsActive(true)
            .WithIsSelected(true)
            .Build();

        return membershipCandidate;
    }

    private static Membership GenerateTestMembership()
    {
        var person = GenerateTestPerson();

        var membership = new MembershipBuilder()
            .WithPerson(person)
            .WithBeginDate(DateOnly.FromDateTime(DateTime.Today.AddDays(-1)))
            .WithEndDate(DateOnly.FromDateTime(DateTime.Today.AddDays(+1)))
            .WithIsActive(true)
            .Build();

        return membership;
    }

    private static Person GenerateTestPerson()
    {
        var person = new PersonBuilder()
            .WithCorrespondenceAddress(new AddressBuilder().Build())
            .WithId(Guid.NewGuid())
            .WithSurname("Sellers")
            .WithGivenName("Peter")
            .WithGenderId(Guid.NewGuid())
            .WithLanguageId(Guid.NewGuid())
            .Build();

        return person;
    }
}
