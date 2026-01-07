using Bk.APG.Business.Mapper;
using Bk.APG.Business.Models;
using Bk.APG.Common.Resources;

namespace Bk.APG.Business.Tests.Mapper;

[TestFixture]
public class EiamAssignmentMapperTests
{
    [Test]
    public void ToDto_ShouldMapPropertiesCorrectly()
    {
        var id = Guid.NewGuid();
        const string externalId = "Admin";
        var eiamAssignment = new EiamAssignment
        {
            Id = id,
            ExternalId = externalId,
            Role = Role.Secretariat
        };

        var result = EiamAssignmentMapper.ToDto(eiamAssignment);

        Assert.Multiple(() =>
        {
            Assert.That(result.Id, Is.EqualTo(id));
            Assert.That(result.Text, Is.EqualTo(BusinessTexts.Worklist_SecretariatRole));
        });
    }

    [Test]
    public void ToDto_ShouldUseDescription_WhenUseDescriptionIsTrue()
    {
        var id = Guid.NewGuid();
        const string externalId = "Admin";
        var eiamAssignment = new EiamAssignment
        {
            Id = id,
            ExternalId = externalId,
            Role = Role.Admin
        };

        var result = EiamAssignmentMapper.ToDto(eiamAssignment, useDescription: true);

        Assert.Multiple(() =>
        {
            Assert.That(result.Id, Is.EqualTo(id));
            Assert.That(result.Text, Is.EqualTo("Admin"));
        });
    }
}
