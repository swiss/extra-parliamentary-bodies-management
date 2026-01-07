using Bk.APG.Business.Dtos;
using Bk.APG.Business.Mapper;
using Bk.APG.CrossCutting.Tests.Builders;
using Bogus;

namespace Bk.APG.Business.Tests.Mapper;

[TestFixture]
internal class CommitteeDetailMapperTests
{
    [Test]
    public void FromCommitteeCreateDto_ShouldMapCorrectly()
    {
        var createDto = new Faker<CommitteeCreateDto>().Generate();
        var committeeId = Guid.NewGuid();

        var committeeDetail = CommitteeMapper.FromCommitteeCreateDto(createDto, "foo bar");

        Assert.That(committeeDetail, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(committeeDetail.DescriptionGerman, Is.EqualTo(createDto.DescriptionGerman));
            Assert.That(committeeDetail.DescriptionFrench, Is.EqualTo(createDto.DescriptionFrench));
            Assert.That(committeeDetail.DescriptionItalian, Is.EqualTo(createDto.DescriptionItalian));
            Assert.That(committeeDetail.DescriptionRomansh, Is.EqualTo(createDto.DescriptionRomansh));
            Assert.That(committeeDetail.DepartmentId, Is.EqualTo(createDto.DepartmentId));
            Assert.That(committeeDetail.OfficeId, Is.EqualTo(createDto.OfficeId));
            Assert.That(committeeDetail.CommitteeLevelId, Is.EqualTo(createDto.LevelId));
            Assert.That(committeeDetail.CommitteeTypeId, Is.EqualTo(createDto.CommitteeTypeId));
            Assert.That(committeeDetail.TermOfOfficeId, Is.EqualTo(createDto.TermOfOfficeId));
            Assert.That(committeeDetail.LegalFormId, Is.EqualTo(createDto.LegalFormId));
            Assert.That(committeeDetail.LegalBase, Is.EqualTo(createDto.LegalBase));
            Assert.That(committeeDetail.FederalLawEstablishment, Is.EqualTo(createDto.FederalLawEstablishment));
            Assert.That(committeeDetail.MarketOrientated, Is.EqualTo(createDto.MarketOrientated));
            Assert.That(committeeDetail.SupervisionDuty, Is.EqualTo(createDto.SupervisionDuty));
            Assert.That(committeeDetail.MinimalMembers, Is.EqualTo(createDto.MinimalMembers));
            Assert.That(committeeDetail.MaximalMembers, Is.EqualTo(createDto.MaximalMembers));
            Assert.That(committeeDetail.AdditionalAuthorityMembers, Is.EqualTo(createDto.AdditionalAuthorityMembers));
            Assert.That(committeeDetail.LinkAuthorityWebsite, Is.EqualTo(createDto.LinkAuthorityWebsite));
        });
    }

    [Test]
    public void ToCommitteeJustificationUpdateDto_ShouldMapCorrectly()
    {
        var committee = new CommitteeBuilder().Build();

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
        });
    }
}
