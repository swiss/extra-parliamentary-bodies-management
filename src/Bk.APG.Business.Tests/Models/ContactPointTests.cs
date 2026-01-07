using Bk.APG.Business.Models;
using Bk.APG.CrossCutting.Tests.Builders;

namespace Bk.APG.Business.Tests.Models;

[TestFixture]
internal class ContactPointTests
{
    [Test]
    public void IsFemale_ShouldReturnFalse()
    {
        var contactPoint = new ContactPointBuilder()
            .WithBeginDate(new DateOnly(2000, 1, 1))
            .WithEndDate(new DateOnly(2001, 7, 1))
            .WithGender(new GenderBuilder().WithUri(Gender.Male).Build())
            .Build();

        Assert.That(contactPoint.IsFemale, Is.False);
    }

    [Test]
    public void IsFemale_ShouldReturnTrue()
    {
        var contactPoint = new ContactPointBuilder()
            .WithBeginDate(new DateOnly(2000, 1, 1))
            .WithEndDate(new DateOnly(2001, 7, 1))
            .WithGender(new GenderBuilder().WithUri(Gender.Female).Build())
            .Build();

        Assert.That(contactPoint.IsFemale, Is.True);
    }

    [Test]
    public void IsDataProtectionOfficer_ShouldReturnFalse()
    {
        var contactPoint = new ContactPointBuilder()
            .WithBeginDate(new DateOnly(2000, 1, 1))
            .WithEndDate(new DateOnly(2001, 7, 1))
            .WithContactPointType(new ContactPointTypeBuilder().WithId(ContactPointType.SecretariatGuid).Build())
            .Build();

        Assert.That(contactPoint.IsDataProtectionOfficer, Is.False);
    }

    [Test]
    public void IsDataProtectionOfficer_ShouldReturnTrue()
    {
        var contactPoint = new ContactPointBuilder()
            .WithBeginDate(new DateOnly(2000, 1, 1))
            .WithEndDate(new DateOnly(2001, 7, 1))
            .WithContactPointType(new ContactPointTypeBuilder().WithId(ContactPointType.DataProtectionOfficerGuid).Build())
            .Build();

        Assert.That(contactPoint.IsDataProtectionOfficer, Is.True);
    }
}
