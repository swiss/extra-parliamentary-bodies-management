using Bk.APG.Business.Mapper;
using Bk.APG.CrossCutting.Tests.Builders;

namespace Bk.APG.Business.Tests.Mapper;

[TestFixture]
internal class InterestCommitteeMapperTests
{
    [Test]
    public void MapToInterestCommitteeDto_WithModel_ShouldMapToDto()
    {
        var interestCommittee = new InterestCommitteeBuilder().Build();

        var dto = InterestCommitteeMapper.ToInterestCommitteeDto(interestCommittee);

        Assert.That(dto, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(dto.Id, Is.EqualTo(interestCommittee.Id));
            Assert.That(dto.Text, Is.EqualTo(interestCommittee.TextDe));
            Assert.That(dto.Description, Is.EqualTo(interestCommittee.DescriptionDe));
        });
    }
}
