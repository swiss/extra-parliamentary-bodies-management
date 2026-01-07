using Bk.APG.Business.Mapper;
using Bk.APG.CrossCutting.Tests.Builders;

namespace Bk.APG.Business.Tests.Mapper;

[TestFixture]
internal class InterestFunctionMapperTests
{
    [Test]
    public void MapToInterestFunctionDto_WithModel_ShouldMapToDto()
    {
        var interestFunction = new InterestFunctionBuilder().Build();

        var dto = InterestFunctionMapper.ToInterestFunctionDto(interestFunction);

        Assert.That(dto, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(dto.Id, Is.EqualTo(interestFunction.Id));
            Assert.That(dto.Text, Is.EqualTo(interestFunction.TextDe));
            Assert.That(dto.Description, Is.EqualTo(interestFunction.DescriptionDe));
        });
    }
}
