using Bk.APG.Business.Mapper;
using Bk.APG.CrossCutting.Tests.Builders;

namespace Bk.APG.Business.Tests.Mapper;

[TestFixture]
internal class InterestLegalFormMapperTests
{
    [Test]
    public void MapToInterestLegalFormDto_WithModel_ShouldMapToDto()
    {
        var interestLegalForm = new InterestLegalFormBuilder().Build();

        var dto = InterestLegalFormMapper.ToInterestLegalFormDto(interestLegalForm);

        Assert.That(dto, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(dto.Id, Is.EqualTo(interestLegalForm.Id));
            Assert.That(dto.Text, Is.EqualTo(interestLegalForm.TextDe));
            Assert.That(dto.Description, Is.EqualTo(interestLegalForm.DescriptionDe));
        });
    }
}
