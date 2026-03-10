using Bk.APG.Business.Mapper;
using Bk.APG.CrossCutting.Tests.Builders;

namespace Bk.APG.Business.Tests.Mapper;

[TestFixture]
internal class CountryMapperTests
{
    [Test]
    public void MapToCountryDto_WithModel_ShouldMapToDto()
    {
        var country = new CountryBuilder().Build();

        var dto = CountryMapper.ToCountryDto(country);

        Assert.That(dto, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(dto.Id, Is.EqualTo(country.Id));
            Assert.That(dto.Text, Is.EqualTo(country.GetText()));
        });
    }

    [Test]
    public void ToCountry_WithMasterDataModel_ShouldMapToModel()
    {
        var masterDataModel = new Swiss.FCh.MasterData.Models.Country
        {
            Uri = "test_uri",
            NameDe = "test_name_de",
            NameFr = "test_name_fr",
            NameIt = "testname_it",
            ShortNameDe = "test_short_name_de",
            ShortNameFr = "test_short_name_fr",
            ShortNameIt = "test_short_name_it",
            StartDate = DateOnly.FromDateTime(DateTime.Now),
        };

        var result = CountryMapper.ToCountry(masterDataModel);

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
        });
    }
}
