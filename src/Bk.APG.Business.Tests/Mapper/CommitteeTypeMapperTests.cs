using System.Globalization;
using Bk.APG.Business.Mapper;
using Bk.APG.CrossCutting.Tests.Builders;

namespace Bk.APG.Business.Tests.Mapper;

[TestFixture]
internal class CommitteeTypeMapperTests
{
    [Test]
    public void MapToCommitteeTypeListDto_ShouldMapCorrectly()
    {
        var committeeType = new CommitteeTypeBuilder()
            .WithLanguagesPercentageThreshold(12.5, 13.5, 14.5, 15.5)
            .WithFemaleAndMaleThreshold(10, 20)
            .WithGermanDescription("Langer Text DE")
            .WithGermanText("Kurz DE")
            .Build();

        var committeeTypeList = CommitteeTypeMapper.ToCommitteeTypeListDto(committeeType, new CultureInfo("de"));

        Assert.That(committeeTypeList, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(committeeTypeList.Description, Is.EqualTo(committeeType.DescriptionDe));
            Assert.That(committeeTypeList.Text, Is.EqualTo(committeeType.TextDe));
            Assert.That(committeeTypeList.FemaleThreshold, Is.EqualTo(committeeType.FemaleThreshold));
            Assert.That(committeeTypeList.MaleThreshold, Is.EqualTo(committeeType.MaleThreshold));
            Assert.That(committeeTypeList.GermanMinimalThreshold, Is.EqualTo(committeeType.GermanMinimalThreshold));
            Assert.That(committeeTypeList.GermanThresholdPercentage, Is.EqualTo(committeeType.GermanThresholdPercentage));
            Assert.That(committeeTypeList.FrenchMinimalThreshold, Is.EqualTo(committeeType.FrenchMinimalThreshold));
            Assert.That(committeeTypeList.FrenchThresholdPercentage, Is.EqualTo(committeeType.FrenchThresholdPercentage));
            Assert.That(committeeTypeList.ItalianMinimalThreshold, Is.EqualTo(committeeType.ItalianMinimalThreshold));
            Assert.That(committeeTypeList.ItalianThresholdPercentage, Is.EqualTo(committeeType.ItalianThresholdPercentage));
            Assert.That(committeeTypeList.RomanshMinimalThreshold, Is.EqualTo(committeeType.RomanshMinimalThreshold));
            Assert.That(committeeTypeList.RomanshThresholdPercentage, Is.EqualTo(committeeType.RomanshThresholdPercentage));
        });
    }

    [Test]
    public void MapToCommitteeUpdateDto_ShouldMapCorrectly()
    {
        var committeeType = new CommitteeTypeBuilder()
            .WithLanguagesPercentageThreshold(12.5, 13.5, 14.5, 15.5)
            .WithFemaleAndMaleThreshold(10, 20)
            .WithGermanDescription("Langer Text DE")
            .WithGermanText("Kurz DE")
            .Build();

        var committeeTypeList = CommitteeTypeMapper.ToCommitteeTypeUpdateDto(committeeType, new CultureInfo("de"));

        Assert.That(committeeTypeList, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(committeeTypeList.Text, Is.EqualTo(committeeType.TextDe));
            Assert.That(committeeTypeList.FemaleThreshold, Is.EqualTo(committeeType.FemaleThreshold));
            Assert.That(committeeTypeList.MaleThreshold, Is.EqualTo(committeeType.MaleThreshold));
            Assert.That(committeeTypeList.GermanMinimalThreshold, Is.EqualTo(committeeType.GermanMinimalThreshold));
            Assert.That(committeeTypeList.GermanThresholdPercentage, Is.EqualTo(committeeType.GermanThresholdPercentage));
            Assert.That(committeeTypeList.FrenchMinimalThreshold, Is.EqualTo(committeeType.FrenchMinimalThreshold));
            Assert.That(committeeTypeList.FrenchThresholdPercentage, Is.EqualTo(committeeType.FrenchThresholdPercentage));
            Assert.That(committeeTypeList.ItalianMinimalThreshold, Is.EqualTo(committeeType.ItalianMinimalThreshold));
            Assert.That(committeeTypeList.ItalianThresholdPercentage, Is.EqualTo(committeeType.ItalianThresholdPercentage));
            Assert.That(committeeTypeList.RomanshMinimalThreshold, Is.EqualTo(committeeType.RomanshMinimalThreshold));
            Assert.That(committeeTypeList.RomanshThresholdPercentage, Is.EqualTo(committeeType.RomanshThresholdPercentage));
        });
    }
}
