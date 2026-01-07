using System.Globalization;
using Bk.APG.Business.Dtos;
using Bk.APG.Business.Mapper;
using Bk.APG.Business.Models;
using Bk.APG.CrossCutting.Tests.Builders;
using Bogus;

namespace Bk.APG.Business.Tests.Mapper;

[TestFixture]
internal class GeneralElectionCommitteeMapperTests
{
    [Test]
    public void ToGeneralElectionCommitteeDetailDto_ShouldMapCorrectly()
    {
        var generalElectionCommittee = GenerateTestData();

        var committeeDetailDto = GeneralElectionCommitteeMapper.ToGeneralElectionCommitteeDetailDto(generalElectionCommittee);

        Assert.That(committeeDetailDto, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(committeeDetailDto.Id, Is.EqualTo(generalElectionCommittee.Id));
            Assert.That(committeeDetailDto.Description, Is.EqualTo(generalElectionCommittee.GetDescription()));
            Assert.That(committeeDetailDto.Department, Is.EqualTo(generalElectionCommittee.Department!.GetText()));
            Assert.That(committeeDetailDto.Office, Is.EqualTo(generalElectionCommittee.Office!.GetText()));
            Assert.That(committeeDetailDto.CommitteeLevel, Is.EqualTo(generalElectionCommittee.CommitteeLevel!.GetText()));
            Assert.That(committeeDetailDto.CommitteeType, Is.EqualTo(generalElectionCommittee.CommitteeType!.GetText()));
            Assert.That(committeeDetailDto.CommitteeTypeId, Is.EqualTo(generalElectionCommittee.CommitteeTypeId));
            Assert.That(committeeDetailDto.LegalForm, Is.EqualTo(generalElectionCommittee.LegalForm!.GetText()));
            Assert.That(committeeDetailDto.LegalBase, Is.EqualTo(generalElectionCommittee.LegalBase));
            Assert.That(committeeDetailDto.ReleaseGeneralElection, Is.EqualTo(generalElectionCommittee.ReleaseGeneralElection));
            Assert.That(committeeDetailDto.FederalLawEstablishment, Is.EqualTo(generalElectionCommittee.FederalLawEstablishment));
            Assert.That(committeeDetailDto.MarketOrientated, Is.EqualTo(generalElectionCommittee.MarketOrientated));
            Assert.That(committeeDetailDto.ExtraParliamentaryCommission, Is.EqualTo(generalElectionCommittee.ExtraParliamentaryCommission));
            Assert.That(committeeDetailDto.SupervisionDuty, Is.EqualTo(generalElectionCommittee.SupervisionDuty));
            Assert.That(committeeDetailDto.BeginDate, Is.EqualTo(generalElectionCommittee.BeginDate));
            Assert.That(committeeDetailDto.EndDate, Is.EqualTo(generalElectionCommittee.EndDate));
            Assert.That(committeeDetailDto.TermOfOffice, Is.EqualTo(generalElectionCommittee.TermOfOffice!.GetText()));
            Assert.That(committeeDetailDto.Period4YearsInGeneralElection, Is.True);
            Assert.That(committeeDetailDto.MinimalMembers, Is.EqualTo(generalElectionCommittee.MinimalMembers));
            Assert.That(committeeDetailDto.MaximalMembers, Is.EqualTo(generalElectionCommittee.MaximalMembers));
            Assert.That(committeeDetailDto.VacanciesGeneralElection, Is.EqualTo(generalElectionCommittee.VacanciesGeneralElection));
            Assert.That(committeeDetailDto.AdditionalAuthorityMembers, Is.EqualTo(generalElectionCommittee.AdditionalAuthorityMembers));
            Assert.That(committeeDetailDto.LinkAuthorityWebsite, Is.EqualTo(generalElectionCommittee.LinkAuthorityWebsite));
            Assert.That(committeeDetailDto.RemarksBaseData, Is.EqualTo(generalElectionCommittee.RemarksBaseData));
            Assert.That(committeeDetailDto.RemarksBaseDataAdmin, Is.EqualTo(generalElectionCommittee.RemarksBaseDataAdmin));
            Assert.That(committeeDetailDto.IsDeleted, Is.EqualTo(generalElectionCommittee.IsDeleted));
            Assert.That(committeeDetailDto.JustificationMembers, Is.EqualTo(generalElectionCommittee.JustificationMembers));
            Assert.That(committeeDetailDto.CalculatedVacancies, Is.EqualTo(9));
            Assert.That(committeeDetailDto.SelectionProcedure, Is.EqualTo(generalElectionCommittee.SelectionProcedure));
            Assert.That(committeeDetailDto.IsValidated, Is.EqualTo(generalElectionCommittee.IsValidated));
            Assert.That(committeeDetailDto.JustificationsNeedAttention, Is.EqualTo(generalElectionCommittee.JustificationsNeedAttention));
            Assert.That(committeeDetailDto.CandidateListState, Is.Not.Empty);
            Assert.That(committeeDetailDto.Candidates.Count(), Is.EqualTo(2));
        });
    }

    [Test]
    public void FromGeneralElectionCommitteeCreateDto_ShouldMapCorrectly()
    {
        var createDto = new Faker<GeneralElectionCommitteeCreateDto>().Generate();

        var generalElectionCommittee = GeneralElectionCommitteeMapper.FromGeneralElectionCommitteeCreateDto(createDto, "foo bar");

        Assert.That(generalElectionCommittee, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(generalElectionCommittee.BeginDate, Is.EqualTo(createDto.BeginDate));
            Assert.That(generalElectionCommittee.EndDate, Is.EqualTo(createDto.EndDate));
            Assert.That(generalElectionCommittee.DescriptionGerman, Is.EqualTo(createDto.DescriptionGerman));
            Assert.That(generalElectionCommittee.DescriptionFrench, Is.EqualTo(createDto.DescriptionFrench));
            Assert.That(generalElectionCommittee.DescriptionItalian, Is.EqualTo(createDto.DescriptionItalian));
            Assert.That(generalElectionCommittee.DescriptionRomansh, Is.EqualTo(createDto.DescriptionRomansh));
            Assert.That(generalElectionCommittee.DepartmentId, Is.EqualTo(createDto.DepartmentId));
            Assert.That(generalElectionCommittee.OfficeId, Is.EqualTo(createDto.OfficeId));
            Assert.That(generalElectionCommittee.CommitteeLevelId, Is.EqualTo(createDto.LevelId));
            Assert.That(generalElectionCommittee.CommitteeTypeId, Is.EqualTo(createDto.CommitteeTypeId));
            Assert.That(generalElectionCommittee.TermOfOfficeId, Is.EqualTo(createDto.TermOfOfficeId));
            Assert.That(generalElectionCommittee.LegalFormId, Is.EqualTo(createDto.LegalFormId));
            Assert.That(generalElectionCommittee.LegalBase, Is.EqualTo(createDto.LegalBase));
            Assert.That(generalElectionCommittee.FederalLawEstablishment, Is.EqualTo(createDto.FederalLawEstablishment));
            Assert.That(generalElectionCommittee.MarketOrientated, Is.EqualTo(createDto.MarketOrientated));
            Assert.That(generalElectionCommittee.SupervisionDuty, Is.EqualTo(createDto.SupervisionDuty));
            Assert.That(generalElectionCommittee.MinimalMembers, Is.EqualTo(createDto.MinimalMembers));
            Assert.That(generalElectionCommittee.MaximalMembers, Is.EqualTo(createDto.MaximalMembers));
            Assert.That(generalElectionCommittee.AdditionalAuthorityMembers, Is.EqualTo(createDto.AdditionalAuthorityMembers));
            Assert.That(generalElectionCommittee.LinkAuthorityWebsite, Is.EqualTo(createDto.LinkAuthorityWebsite));
        });
    }

    [Test]
    public void MapToGeneralElectionCommitteeList_ShouldMapToCommitteeListDto()
    {
        var committee = new GeneralElectionCommitteeBuilder()
            .WithBeginDate(DateOnly.FromDateTime(DateTime.UtcNow.AddMinutes(-1)))
            .WithEndDate(DateOnly.FromDateTime(DateTime.UtcNow.AddMinutes(1)))
            .WithDepartment(new DepartmentBuilder().Build())
            .WithOffice(new OfficeBuilder().Build())
            .WithCommitteeType(new CommitteeTypeBuilder().Build())
            .WithCommitteeLevel(new CommitteeLevelBuilder().Build())
            .WithSupervisionDuty(true)
            .WithMarketOrientated(true)
            .WithVacanciesGeneralElection(2)
            .Build();

        var committeeList = GeneralElectionCommitteeMapper.ToGeneralElectionCommitteeListDto(committee, new CultureInfo("fr"));

        Assert.That(committeeList, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(committeeList.Id, Is.EqualTo(committee.Id));
            Assert.That(committeeList.CommitteeId, Is.EqualTo(committee.CommitteeId));
            Assert.That(committeeList.Description, Is.EqualTo(committee.DescriptionFrench));
            Assert.That(committeeList.Department, Is.EqualTo(committee.Department!.TextFr));
            Assert.That(committeeList.Office, Is.EqualTo(committee.Office!.TextFr));
            Assert.That(committeeList.CommitteeType, Is.EqualTo(committee.CommitteeType!.TextFr));
            Assert.That(committeeList.Status, Is.Empty);
            Assert.That(committeeList.VacanciesGeneralElection, Is.EqualTo(committee.VacanciesGeneralElection));
            Assert.That(committeeList.StatusProposal, Is.Empty);
            Assert.That(committeeList.IsMarketOrientated, Is.EqualTo(committee.MarketOrientated));
            Assert.That(committeeList.HasSupervisionDuty, Is.EqualTo(committee.SupervisionDuty));
            Assert.That(committeeList.Modified, Is.EqualTo(committee.Modified));
            Assert.That(committeeList.ModifiedBy, Is.EqualTo(committee.ModifiedBy));
        });
    }

    private static GeneralElectionCommittee GenerateTestData()
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
            .WithCandidateListState(new CandidateListStateBuilder().Build())
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

    [Test]
    public void ToGeneralElectionCommitteeJustificationUpdateDto_ShouldMapCorrectly()
    {
        var generalElectionCommittee = new GeneralElectionCommitteeBuilder()
            .WithCommitteeType(new CommitteeTypeBuilder().Build())
            .Build();

        var justificationUpdateDto = GeneralElectionCommitteeMapper.ToGeneralElectionCommitteeJustificationUpdateDto(generalElectionCommittee);

        Assert.That(justificationUpdateDto, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(justificationUpdateDto.Id, Is.EqualTo(generalElectionCommittee.Id));
            Assert.That(justificationUpdateDto.JustificationMembers, Is.EqualTo(generalElectionCommittee.JustificationMembers));
            Assert.That(justificationUpdateDto.JustificationGenders, Is.EqualTo(generalElectionCommittee.JustificationGenders));
            Assert.That(justificationUpdateDto.MeasuresGenders, Is.EqualTo(generalElectionCommittee.MeasuresGenders));
            Assert.That(justificationUpdateDto.JustificationLanguages, Is.EqualTo(generalElectionCommittee.JustificationLanguages));
            Assert.That(justificationUpdateDto.MeasuresLanguages, Is.EqualTo(generalElectionCommittee.MeasuresLanguages));
            Assert.That(justificationUpdateDto.SelectionProcedure, Is.EqualTo(generalElectionCommittee.SelectionProcedure));
            Assert.That(justificationUpdateDto.CurrentMemberCount, Is.EqualTo(generalElectionCommittee.ActiveMemberCount));
            Assert.That(justificationUpdateDto.CurrentGenderQuota, Is.Not.Empty);
            Assert.That(justificationUpdateDto.CurrentLanguageQuota, Is.Not.Empty);
            Assert.That(justificationUpdateDto.RowVersion, Is.EqualTo(generalElectionCommittee.RowVersion));
        });
    }
}
