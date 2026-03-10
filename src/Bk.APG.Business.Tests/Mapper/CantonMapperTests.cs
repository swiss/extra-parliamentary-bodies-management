using Bk.APG.Business.Mapper;
using Bk.APG.CrossCutting.Tests.Builders;

namespace Bk.APG.Business.Tests.Mapper;

[TestFixture]
internal class CantonMapperTests
{
    [Test]
    public void MapToCantonDto_WithModel_ShouldMapToDto()
    {
        var canton = new CantonBuilder().Build();

        var dto = CantonMapper.ToCantonDto(canton);

        Assert.That(dto, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(dto.Id, Is.EqualTo(canton.Id));
            Assert.That(dto.Text, Is.EqualTo(canton.GetText()));
        });
    }

    [Test]
    public void ToCanton_WithMasterDataModel_ShouldMapToModel()
    {
        var masterDataModel = new MasterData.Models.Canton
        {
            Uri = "test_uri",
            NameDe = "test_name_de",
            NameFr = "test_name_fr",
            NameIt = "testname_it",
            ShortNameDe = "test_short_name_de",
            ShortNameFr = "test_short_name_fr",
            ShortNameIt = "test_short_name_it"
        };

        var result = CantonMapper.ToCanton(masterDataModel);

        Assert.That(result, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(result.Id, Is.Default);
            Assert.That(result.Created, Is.Default);
            Assert.That(result.CreatedBy, Is.Default);
            Assert.That(result.Modified, Is.Default);
            Assert.That(result.ModifiedBy, Is.Default);
            Assert.That(result.IsDeleted, Is.False);
            Assert.That(result.Uri, Is.EqualTo(masterDataModel.Uri));
            Assert.That(result.TextDe, Is.EqualTo(masterDataModel.ShortNameDe));
            Assert.That(result.TextFr, Is.EqualTo(masterDataModel.ShortNameFr));
            Assert.That(result.TextIt, Is.EqualTo(masterDataModel.ShortNameIt));
            Assert.That(result.TextRm, Is.Empty);
            Assert.That(result.DescriptionDe, Is.EqualTo(masterDataModel.NameDe));
            Assert.That(result.DescriptionFr, Is.EqualTo(masterDataModel.NameFr));
            Assert.That(result.DescriptionIt, Is.EqualTo(masterDataModel.NameIt));
            Assert.That(result.Sort, Is.Default);
            Assert.That(result.Region, Is.Default);
        });
    }
}
