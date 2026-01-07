using Bk.APG.Business.Mapper;
using Bk.APG.CrossCutting.Tests.Builders;

namespace Bk.APG.Business.Tests.Mapper;

[TestFixture]
internal class LanguageMapperTests
{
    [Test]
    public void MapToLanguageDto_WithModel_ShouldMapToDto()
    {
        var language = new LanguageBuilder().Build();

        var dto = LanguageMapper.ToLanguageDto(language);

        Assert.That(dto, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(dto.Id, Is.EqualTo(language.Id));
            Assert.That(dto.Text, Is.EqualTo(language.GetText()));
        });
    }
}
