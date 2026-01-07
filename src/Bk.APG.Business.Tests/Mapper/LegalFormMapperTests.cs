using Bk.APG.Business.Mapper;
using Bk.APG.CrossCutting.Tests.Builders;

namespace Bk.APG.Business.Tests.Mapper;

[TestFixture]
internal class LegalFormMapperTests
{
    [Test]
    public void MapToLegalFormDto_WithModel_ShouldMapToDto()
    {
        var legalForm = new LegalFormBuilder().Build();

        var dto = LegalFormMapper.ToLegalFormDto(legalForm);

        Assert.That(dto, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(dto.Id, Is.EqualTo(legalForm.Id));
            Assert.That(dto.Text, Is.EqualTo(legalForm.TextDe));
            Assert.That(dto.Description, Is.EqualTo(legalForm.DescriptionDe));
            Assert.That(dto.LegalFormId, Is.EqualTo(legalForm.LegalFormId));
        });
    }
}
