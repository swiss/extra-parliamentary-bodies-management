using Bk.APG.Business.Dtos;
using Bk.APG.Business.Mapper;

namespace Bk.APG.Business.Tests.Mapper;

[TestFixture]
internal class PagingMapperTests
{
    [Test]
    public void MapToPagingParametersDto_WithModel_ShouldMapToDto()
    {
        var paging = new PagingParametersDto
        {
            PageIndex = 42,
            PageSize = 100
        };

        var model = PagingMapper.ToPagingParameters(paging);

        Assert.That(model, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(model.PageIndex, Is.EqualTo(paging.PageIndex));
            Assert.That(model.PageSize, Is.EqualTo(paging.PageSize));
        });
    }
}
