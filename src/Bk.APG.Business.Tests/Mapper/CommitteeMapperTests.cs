using System.Globalization;
using Bk.APG.Business.Dtos;
using Bk.APG.Business.Mapper;
using Bk.APG.Business.Models;
using Bk.APG.CrossCutting;
using Bk.APG.CrossCutting.Tests.Builders;
using Bogus;

namespace Bk.APG.Business.Tests.Mapper;

[TestFixture]
internal class CommitteeMapperTests
{
    [Test]
    public void MapToCommitteeList_ShouldMapToCommitteeListDto()
    {
        var committee = new CommitteeBuilder()
            .WithBeginDate(DateOnly.FromDateTime(DateTime.Now.AddDays(-3)))
            .WithEndDate(DateOnly.FromDateTime(DateTime.Now.AddDays(2)))
            .WithDepartment(new DepartmentBuilder().Build())
            .WithOffice(new OfficeBuilder().Build())
            .WithTermOfOffice(new TermOfOfficeBuilder().Build())
            .WithCommitteeType(new CommitteeTypeBuilder().Build())
            .WithCommitteeLevel(new CommitteeLevelBuilder().Build())
            .WithMembership(new MembershipBuilder().WithBeginDate(DateOnly.FromDateTime(DateTime.Now.AddDays(-3))).Build())
            .Build();

        var committeeList = CommitteeMapper.ToCommitteeListDto(committee, new CultureInfo("fr"));

        Assert.That(committeeList, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(committeeList.CommitteeId, Is.EqualTo(committee.CommitteeNumber));
            Assert.That(committeeList.Description, Is.EqualTo(committee.DescriptionFrench));
            Assert.That(committeeList.Department, Is.EqualTo(committee.Department!.TextFr));
            Assert.That(committeeList.Office, Is.EqualTo(committee.Office!.TextFr));
            Assert.That(committeeList.Term, Is.EqualTo(committee.TermOfOffice!.TextFr));
            Assert.That(committeeList.CommitteeType, Is.EqualTo(committee.CommitteeType!.TextFr));
            Assert.That(committeeList.Level, Is.EqualTo(committee.CommitteeLevel!.TextFr));
            Assert.That(committeeList.IsActive, Is.EqualTo(committee.IsActive));
            Assert.That(committeeList.NeedsAttention, Is.EqualTo(committee.NeedsAttention));
        });
    }

    [Test]
    public void ToCommitteeDetailDto_ShouldMapCorrectly()
    {
        var committee = GenerateTestData();

        var committeeDetailDto = CommitteeMapper.ToCommitteeDetailDto(committee);

        var activeMembers = committee.Memberships.Where(x => x.IsActive || x.NeedsAttention).ToList();
        var activeMembersCount = activeMembers.Count;

        Assert.That(committeeDetailDto, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(committeeDetailDto.Id, Is.EqualTo(committee.Id));
            Assert.That(committeeDetailDto.CommitteeNumber, Is.EqualTo(committee.CommitteeNumber));
            Assert.That(committeeDetailDto.Description, Is.EqualTo(committee.GetDescription()));
            Assert.That(committeeDetailDto.Department, Is.EqualTo(committee.Department!.GetText()));
            Assert.That(committeeDetailDto.Office, Is.EqualTo(committee.Office!.GetText()));
            Assert.That(committeeDetailDto.CommitteeLevel, Is.EqualTo(committee.CommitteeLevel!.GetText()));
            Assert.That(committeeDetailDto.CommitteeType, Is.EqualTo(committee.CommitteeType!.GetText()));
            Assert.That(committeeDetailDto.CommitteeTypeId, Is.EqualTo(committee.CommitteeTypeId));
            Assert.That(committeeDetailDto.LegalForm, Is.EqualTo(committee.LegalForm!.GetText()));
            Assert.That(committeeDetailDto.OldLegalForm, Is.EqualTo(committee.OldLegalForm));
            Assert.That(committeeDetailDto.LegalBase, Is.EqualTo(committee.LegalBase));
            Assert.That(committeeDetailDto.FederalLawEstablishment, Is.EqualTo(committee.FederalLawEstablishment));
            Assert.That(committeeDetailDto.MarketOrientated, Is.EqualTo(committee.MarketOrientated));
            Assert.That(committeeDetailDto.ExtraParliamentaryCommission, Is.EqualTo(committee.ExtraParliamentaryCommission));
            Assert.That(committeeDetailDto.SupervisionDuty, Is.EqualTo(committee.SupervisionDuty));
            Assert.That(committeeDetailDto.BeginDate, Is.EqualTo(committee.BeginDate));
            Assert.That(committeeDetailDto.EndDate, Is.EqualTo(committee.EndDate));
            Assert.That(committeeDetailDto.TermOfOffice, Is.EqualTo(committee.TermOfOffice!.GetText()));
            Assert.That(committeeDetailDto.Period4YearsInGeneralElection, Is.True);
            Assert.That(committeeDetailDto.MembersCount, Is.EqualTo(activeMembersCount));
            Assert.That(committeeDetailDto.MinimalMembers, Is.EqualTo(committee.MinimalMembers));
            Assert.That(committeeDetailDto.MaximalMembers, Is.EqualTo(committee.MaximalMembers));
            Assert.That(committeeDetailDto.VacanciesGeneralElection, Is.EqualTo(committee.VacanciesGeneralElection));
            Assert.That(committeeDetailDto.AdditionalAuthorityMembers, Is.EqualTo(committee.AdditionalAuthorityMembers));
            Assert.That(committeeDetailDto.LinkAuthorityWebsite, Is.EqualTo(committee.LinkAuthorityWebsite));
            Assert.That(committeeDetailDto.RemarksBaseData, Is.EqualTo(committee.RemarksBaseData));
            Assert.That(committeeDetailDto.RemarksBaseDataAdmin, Is.EqualTo(committee.RemarksBaseDataAdmin));
            Assert.That(committeeDetailDto.IsDeleted, Is.EqualTo(committee.IsDeleted));
            Assert.That(committeeDetailDto.IsActive, Is.EqualTo(committee.IsActive));
            Assert.That(committeeDetailDto.CanCreateMembership, Is.EqualTo(committee.CanCreateMembership));
            Assert.That(committeeDetailDto.VacanciesInCurrentTermOfOffice, Is.EqualTo(activeMembersCount - committee.MinimalMembers >= 0 ? 0 : (activeMembersCount - committee.MinimalMembers) * -1));

            Assert.That(committeeDetailDto.JustificationMembers, Is.EqualTo(committee.JustificationMembers));

            Assert.That(committeeDetailDto.FemaleThreshold, Is.EqualTo(committee.CommitteeType!.FemaleThreshold));
            Assert.That(committeeDetailDto.FemaleQuota, Is.EqualTo(activeMembersCount > 0 ? (double)activeMembers.Count(x => x.Person!.Gender!.Uri == Gender.Female) / activeMembersCount * 100 : 0));
            Assert.That(committeeDetailDto.MaleThreshold, Is.EqualTo(committee.CommitteeType!.MaleThreshold));
            Assert.That(committeeDetailDto.MaleQuota, Is.EqualTo(activeMembersCount > 0 ? (double)activeMembers.Count(x => x.Person!.Gender!.Uri == Gender.Male) / activeMembersCount * 100 : 0));
            Assert.That(committeeDetailDto.JustificationGenders, Is.EqualTo(committee.JustificationGenders));
            Assert.That(committeeDetailDto.MeasuresGenders, Is.EqualTo(committee.MeasuresGenders));

            var committeeType = committee.CommitteeType!;
            var isPercentageBased = committeeType.GermanThresholdPercentage is not null;
            Assert.That(committeeDetailDto.IsPercentageBased, Is.EqualTo(isPercentageBased));
            Assert.That(committeeDetailDto.GermanThreshold, Is.EqualTo((isPercentageBased ? committeeType.GermanThresholdPercentage : committeeType.GermanMinimalThreshold) ?? 0));
            Assert.That(committeeDetailDto.GermanQuota, Is.EqualTo(isPercentageBased ? committee.GermanQuota : committee.GermanCount));
            Assert.That(committeeDetailDto.FrenchThreshold, Is.EqualTo((isPercentageBased ? committeeType.FrenchThresholdPercentage : committeeType.FrenchMinimalThreshold) ?? 0));
            Assert.That(committeeDetailDto.FrenchQuota, Is.EqualTo(isPercentageBased ? committee.FrenchQuota : committee.FrenchCount));
            Assert.That(committeeDetailDto.ItalianThreshold, Is.EqualTo((isPercentageBased ? committeeType.ItalianThresholdPercentage : committeeType.ItalianMinimalThreshold) ?? 0));
            Assert.That(committeeDetailDto.ItalianQuota, Is.EqualTo(isPercentageBased ? committee.ItalianQuota : committee.ItalianCount));
            Assert.That(committeeDetailDto.RomanshThreshold, Is.EqualTo((isPercentageBased ? committeeType.RomanshThresholdPercentage : committeeType.RomanshMinimalThreshold) ?? 0));
            Assert.That(committeeDetailDto.RomanshQuota, Is.EqualTo(isPercentageBased ? committee.RomanshQuota : committee.RomanshCount));
            Assert.That(committeeDetailDto.JustificationLanguages, Is.EqualTo(committee.JustificationLanguages));
            Assert.That(committeeDetailDto.MeasuresLanguages, Is.EqualTo(committee.MeasuresLanguages));

            Assert.That(committeeDetailDto.NeedsAttentionShorterDuty, Is.EqualTo(committee.NeedsAttentionShorterDuty));
            Assert.That(committeeDetailDto.NeedsAttentionLongerDuty, Is.EqualTo(committee.NeedsAttentionLongerDuty));
            Assert.That(committeeDetailDto.NeedsAttentionFederalDuty, Is.EqualTo(committee.NeedsAttentionFederalDuty));
            Assert.That(committeeDetailDto.NeedsAttentionFederalAssembly, Is.EqualTo(committee.NeedsAttentionFederalAssembly));
            Assert.That(committeeDetailDto.NeedsAttentionNoMembers, Is.EqualTo(committee.NeedsAttentionNoMembers));
            Assert.That(committeeDetailDto.NeedsAttentionAboveMaxMembers, Is.EqualTo(committee.NeedsAttentionAboveMaxMembers));
            Assert.That(committeeDetailDto.NeedsAttentionDataProtectionOfficer, Is.EqualTo(committee.NeedsAttentionDataProtectionOfficer));
            Assert.That(committeeDetailDto.NeedsAttentionSecretariat, Is.EqualTo(committee.NeedsAttentionSecretariat));
            Assert.That(committeeDetailDto.NeedsAttentionBasicData, Is.EqualTo(committee.NeedsAttentionBasicData));
            Assert.That(committeeDetailDto.NeedsAttentionMembershipExpired, Is.EqualTo(committee.NeedsAttentionMembershipExpired));
        });
    }

    [Test]
    public void FromCommitteeCreateDto_ShouldMapCorrectly()
    {
        var createDto = new Faker<CommitteeCreateDto>().Generate();

        var committee = CommitteeMapper.FromCommitteeCreateDto(createDto, "foo bar");

        Assert.That(committee, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(committee.BeginDate, Is.EqualTo(createDto.BeginDate));
            Assert.That(committee.EndDate, Is.EqualTo(createDto.EndDate));
            Assert.That(committee.DescriptionGerman, Is.EqualTo(createDto.DescriptionGerman));
            Assert.That(committee.DescriptionFrench, Is.EqualTo(createDto.DescriptionFrench));
            Assert.That(committee.DescriptionItalian, Is.EqualTo(createDto.DescriptionItalian));
            Assert.That(committee.DescriptionRomansh, Is.EqualTo(createDto.DescriptionRomansh));
            Assert.That(committee.DepartmentId, Is.EqualTo(createDto.DepartmentId));
            Assert.That(committee.OfficeId, Is.EqualTo(createDto.OfficeId));
            Assert.That(committee.CommitteeLevelId, Is.EqualTo(createDto.LevelId));
            Assert.That(committee.CommitteeTypeId, Is.EqualTo(createDto.CommitteeTypeId));
            Assert.That(committee.TermOfOfficeId, Is.EqualTo(createDto.TermOfOfficeId));
            Assert.That(committee.LegalFormId, Is.EqualTo(createDto.LegalFormId));
            Assert.That(committee.LegalBase, Is.EqualTo(createDto.LegalBase));
            Assert.That(committee.FederalLawEstablishment, Is.EqualTo(createDto.FederalLawEstablishment));
            Assert.That(committee.MarketOrientated, Is.EqualTo(createDto.MarketOrientated));
            Assert.That(committee.SupervisionDuty, Is.EqualTo(createDto.SupervisionDuty));
            Assert.That(committee.MinimalMembers, Is.EqualTo(createDto.MinimalMembers));
            Assert.That(committee.MaximalMembers, Is.EqualTo(createDto.MaximalMembers));
            Assert.That(committee.AdditionalAuthorityMembers, Is.EqualTo(createDto.AdditionalAuthorityMembers));
            Assert.That(committee.LinkAuthorityWebsite, Is.EqualTo(createDto.LinkAuthorityWebsite));
            Assert.That(committee.VacanciesGeneralElection, Is.EqualTo(createDto.VacanciesInGeneralElection));
        });
    }

    [Test]
    public void ToCommitteeJustificationUpdateDto_ShouldMapCorrectly()
    {
        var committee = GenerateTestData();

        var committeeJustificationUpdateDto = CommitteeMapper.ToCommitteeJustificationUpdateDto(committee);

        Assert.That(committee, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(committeeJustificationUpdateDto.Id, Is.EqualTo(committee.Id));
            Assert.That(committeeJustificationUpdateDto.JustificationMembers, Is.EqualTo(committee.JustificationMembers));
            Assert.That(committeeJustificationUpdateDto.JustificationGenders, Is.EqualTo(committee.JustificationGenders));
            Assert.That(committeeJustificationUpdateDto.MeasuresGenders, Is.EqualTo(committee.MeasuresGenders));
            Assert.That(committeeJustificationUpdateDto.JustificationLanguages, Is.EqualTo(committee.JustificationLanguages));
            Assert.That(committeeJustificationUpdateDto.MeasuresLanguages, Is.EqualTo(committee.MeasuresLanguages));
            Assert.That(committeeJustificationUpdateDto.CurrentMemberCount, Is.EqualTo(committee.Memberships.Count(m => m.IsActive)));
            Assert.That(committeeJustificationUpdateDto.CurrentGenderQuota, Is.Not.Empty);
            Assert.That(committeeJustificationUpdateDto.CurrentLanguageQuota, Is.Not.Empty);
            Assert.That(committeeJustificationUpdateDto.RowVersion, Is.EqualTo(committee.RowVersion));
        });
    }

    [Test]
    public void ToDimensionItem_WithValidCommittee_ShouldMapCorrectly()
    {
        var committeeId = Guid.NewGuid();
        const int committeeOgdId = 1;

        var committeeTypeId = Guid.NewGuid();

        const int secretariatOgdId = 3;

        var contactPointTypeSecretariat =
            new ContactPointTypeBuilder()
                .WithId(ContactPointType.SecretariatGuid)
                .Build();

        var secretariat =
            new ContactPointBuilder()
                .WithId(Guid.NewGuid())
                .WithOgdId(secretariatOgdId)
                .WithContactPointType(contactPointTypeSecretariat)
                .Build();

        var committee =
            new CommitteeBuilder()
                .WithId(committeeId)
                .WithOgdId(committeeOgdId)
                .WithGermanDescription("de")
                .WithFrenchDescription("fr")
                .WithItalianDescription("it")
                .WithRomanschDescription("rm")
                .WithCommitteeTypeId(committeeTypeId)
                .WithContactPoint(secretariat)
                .Build();

        var result = CommitteeMapper.ToDimensionItem(committee);

        Assert.That(result, Is.Not.Null);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(result.Key, Is.EqualTo(committeeOgdId));
            Assert.That(result.Name.Text, Is.EqualTo("de"));
            Assert.That(result.AdditionalUriProperties, Has.Count.EqualTo(2));
        }

        using (Assert.EnterMultipleScope())
        {
            Assert.That(result.AdditionalUriProperties[0].Predicate, Is.EqualTo(OgdExportConstants.CommitteeHasSecretariat));
            Assert.That(result.AdditionalUriProperties[0].Object, Is.EqualTo($"{OgdExportConstants.NamespaceOrganization}:{secretariat.OgdId}"));
            Assert.That(result.AdditionalUriProperties[1].Predicate, Is.EqualTo(OgdExportConstants.CommitteeHasLegalForm));
            Assert.That(result.AdditionalUriProperties[1].Object, Is.EqualTo(OgdExportConstants.CreateUriLinkForLdAdminCh(committee.LegalForm!.Uri)));
        }

        using (Assert.EnterMultipleScope())
        {
            Assert.That(result.AdditionalLiteralProperties[4].Predicate, Is.EqualTo(OgdExportConstants.CommitteeAdditionalAuthorityMembers));
#pragma warning disable CA1308
            Assert.That(result.AdditionalLiteralProperties[4].Object.Text, Is.EqualTo(committee.AdditionalAuthorityMembers.ToString(CultureInfo.InvariantCulture).ToLowerInvariant()));
#pragma warning restore CA1308
            Assert.That(result.AdditionalLiteralProperties[4].Object.DataType, Is.EqualTo(new Uri(OgdExportConstants.DataTypeBoolean)));
        }
    }

    [Test]
    public void ToCommitteeUpdateDto_ShouldMapCorrectly()
    {
        var committee = GenerateTestData();

        var committeeUpdateDto = CommitteeMapper.ToCommitteeUpdateDto(committee);

        Assert.That(committeeUpdateDto, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(committeeUpdateDto.Id, Is.EqualTo(committee.Id));
            Assert.That(committeeUpdateDto.CommitteeNumber, Is.EqualTo(committee.CommitteeNumber));
            Assert.That(committeeUpdateDto.BeginDate, Is.EqualTo(committee.BeginDate));
            Assert.That(committeeUpdateDto.EndDate, Is.EqualTo(committee.EndDate));
            Assert.That(committeeUpdateDto.IsActive, Is.EqualTo(committee.IsActive));
            Assert.That(committeeUpdateDto.DescriptionGerman, Is.EqualTo(committee.DescriptionGerman));
            Assert.That(committeeUpdateDto.DescriptionFrench, Is.EqualTo(committee.DescriptionFrench));
            Assert.That(committeeUpdateDto.DescriptionItalian, Is.EqualTo(committee.DescriptionItalian));
            Assert.That(committeeUpdateDto.DescriptionRomansh, Is.EqualTo(committee.DescriptionRomansh));
            Assert.That(committeeUpdateDto.LevelId, Is.EqualTo(committee.CommitteeLevelId));
            Assert.That(committeeUpdateDto.OfficeId, Is.EqualTo(committee.OfficeId));
            Assert.That(committeeUpdateDto.DepartmentId, Is.EqualTo(committee.DepartmentId));
            Assert.That(committeeUpdateDto.CommitteeTypeId, Is.EqualTo(committee.CommitteeTypeId));
            Assert.That(committeeUpdateDto.FederalLawEstablishment, Is.EqualTo(committee.FederalLawEstablishment));
            Assert.That(committeeUpdateDto.SupervisionDuty, Is.EqualTo(committee.SupervisionDuty));
            Assert.That(committeeUpdateDto.MarketOrientated, Is.EqualTo(committee.MarketOrientated));
            Assert.That(committeeUpdateDto.LegalFormId, Is.EqualTo(committee.LegalFormId));
            Assert.That(committeeUpdateDto.OldLegalForm, Is.EqualTo(committee.OldLegalForm));
            Assert.That(committeeUpdateDto.LegalBase, Is.EqualTo(committee.LegalBase));
            Assert.That(committeeUpdateDto.TermOfOfficeId, Is.EqualTo(committee.TermOfOfficeId));
            Assert.That(committeeUpdateDto.MinimalMembers, Is.EqualTo(committee.MinimalMembers));
            Assert.That(committeeUpdateDto.MaximalMembers, Is.EqualTo(committee.MaximalMembers));
            Assert.That(committeeUpdateDto.AdditionalAuthorityMembers, Is.EqualTo(committee.AdditionalAuthorityMembers));
            Assert.That(committeeUpdateDto.LinkAuthorityWebsite, Is.EqualTo(committee.LinkAuthorityWebsite));
            Assert.That(committeeUpdateDto.LinkHomepageGerman, Is.EqualTo(committee.LinkHomepageGerman));
            Assert.That(committeeUpdateDto.LinkHomepageFrench, Is.EqualTo(committee.LinkHomepageFrench));
            Assert.That(committeeUpdateDto.LinkHomepageItalian, Is.EqualTo(committee.LinkHomepageItalian));
            Assert.That(committeeUpdateDto.LinkHomepageRomansh, Is.EqualTo(committee.LinkHomepageRomansh));
            Assert.That(committeeUpdateDto.FederalInstitution, Is.EqualTo(committee.FederalInstitution));
            Assert.That(committeeUpdateDto.SelfOrganized, Is.EqualTo(committee.SelfOrganized));
            Assert.That(committeeUpdateDto.MembersCount, Is.EqualTo(committee.ActiveMemberCount));
            Assert.That(committeeUpdateDto.VacanciesInGeneralElection, Is.EqualTo(committee.VacanciesGeneralElection));
            Assert.That(committeeUpdateDto.MembershipAdditionsInGeneralElection, Is.EqualTo(committee.MembershipAdditionsInGeneralElection.Select(x => x.Id).ToArray()));
            Assert.That(committeeUpdateDto.RowVersion, Is.EqualTo(committee.RowVersion));
        });
    }

    private static Committee GenerateTestData()
    {
        var person1 = new PersonBuilder().WithLanguage(new LanguageBuilder().Build()).WithGender(new GenderBuilder().Build()).Build();
        var person2 = new PersonBuilder().WithLanguage(new LanguageBuilder().Build()).WithGender(new GenderBuilder().Build()).Build();

        var committee = new CommitteeBuilder()
            .WithBeginDate(DateOnly.FromDateTime(DateTime.Now.AddDays(-3)))
            .WithEndDate(DateOnly.FromDateTime(DateTime.Now.AddDays(2)))
            .WithDepartment(new DepartmentBuilder().Build())
            .WithOffice(new OfficeBuilder().Build())
            .WithTermOfOffice(new TermOfOfficeBuilder().WithId(TermOfOffice.Period4YearsInGeneralElectionGuid).Build())
            .WithCommitteeType(new CommitteeTypeBuilder().Build())
            .WithCommitteeLevel(new CommitteeLevelBuilder().WithId(CommitteeLevel.FederalCouncilGuid).Build())
            .Build();

        var membership1 = new MembershipBuilder()
            .WithPerson(person1)
            .WithBeginDate(DateOnly.FromDateTime(DateTime.Now.AddDays(-3)))
            .WithEndDate(DateOnly.FromDateTime(DateTime.Now.AddDays(2)))
            .WithIsActive(true)
            .Build();

        var membership2 = new MembershipBuilder()
            .WithPerson(person2)
            .WithBeginDate(DateOnly.FromDateTime(DateTime.Now.AddDays(-3)))
            .WithEndDate(DateOnly.FromDateTime(DateTime.Now.AddDays(2)))
            .WithIsActive(true)
            .Build();

        committee.Memberships.Add(membership1);
        committee.Memberships.Add(membership2);

        return committee;
    }
}
