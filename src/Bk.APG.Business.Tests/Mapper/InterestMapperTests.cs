using Bk.APG.Business.Dtos;
using Bk.APG.Business.Mapper;
using Bk.APG.CrossCutting;
using Bk.APG.CrossCutting.Tests.Builders;

namespace Bk.APG.Business.Tests.Mapper;

[TestFixture]
internal class InterestMapperTests
{
    [Test]
    public void MapToInterestUpdateDto_WithModel_ShouldMapToDto()
    {
        var interest = new InterestBuilder()
            .WithId(Guid.Parse("8697753c-f57a-4805-b1e9-bb1e546f4101"))
            .WithPersonId(Guid.Parse("9df96395-bd65-4235-a4fa-fb87689df11f"))
            .WithInterestCommitteeId(Guid.Parse("D0DB93A9-D8E0-4567-A212-3A36286E6D82"))
            .WithInterestFunctionId(Guid.Parse("7266235D-766D-48EF-9457-C670B7B7580F"))
            .WithInterestLegalFormId(Guid.Parse("C6C9B29B-EFA6-4B8B-81A5-F88134127A2D"))
            .WithLegalFormId(Guid.Parse("f7f1a5f1-671b-4d5f-94cd-f3ff54b20241\n"))
            .WithBeginDate(DateOnly.FromDateTime(DateTime.UtcNow.AddDays(-500)))
            .WithEndDate(DateOnly.FromDateTime(DateTime.UtcNow.AddDays(-5)))
            .Build();

        var dto = InterestMapper.ToInterestUpdateDto(interest);

        Assert.That(dto, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(dto.Id, Is.EqualTo(interest.Id));
            Assert.That(dto.Text, Is.EqualTo(interest.Text));
            Assert.That(dto.InterestCommitteeId, Is.EqualTo(interest.InterestCommitteeId));
            Assert.That(dto.InterestFunctionId, Is.EqualTo(interest.InterestFunctionId));
            Assert.That(dto.InterestLegalFormId, Is.EqualTo(interest.InterestLegalFormId));
            Assert.That(dto.LegalFormId, Is.EqualTo(interest.LegalFormId));
            Assert.That(dto.BeginDate, Is.EqualTo(interest.BeginDate));
            Assert.That(dto.EndDate, Is.EqualTo(interest.EndDate));
            Assert.That(dto.IsInactive, Is.True);
            Assert.That(dto.RowVersion, Is.EqualTo(interest.RowVersion));
        });
    }

    [Test]
    public void MapFromInterestUpdateDto_WithModel_ShouldMapToDto()
    {
        var interest = new InterestUpdateDto
        {
            Id = Guid.Parse("8697753c-f57a-4805-b1e9-bb1e546f4101"),
            PersonId = Guid.Parse("9df96395-bd65-4235-a4fa-fb87689df11f"),
            Text = "my Text",
            InterestText = "my new text",
            InterestCommitteeId = Guid.NewGuid(),
            InterestLegalFormId = Guid.NewGuid(),
            InterestFunctionId = Guid.NewGuid(),
            LegalFormId = Guid.NewGuid(),
            RowVersion = 666
        };

        var dto = InterestMapper.FromInterestUpdateDto(interest);

        Assert.That(dto, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(dto.Id, Is.EqualTo(interest.Id));
            Assert.That(dto.PersonId, Is.EqualTo(interest.PersonId));
            Assert.That(dto.InterestText, Is.EqualTo(interest.InterestText));
            Assert.That(dto.Text, Is.EqualTo(interest.Text));
            Assert.That(dto.InterestCommitteeId, Is.EqualTo(interest.InterestCommitteeId));
            Assert.That(dto.InterestFunctionId, Is.EqualTo(interest.InterestFunctionId));
            Assert.That(dto.InterestLegalFormId, Is.EqualTo(interest.InterestLegalFormId));
            Assert.That(dto.LegalFormId, Is.EqualTo(interest.LegalFormId));
            Assert.That(dto.RowVersion, Is.EqualTo(interest.RowVersion));
        });
    }

    [Test]
    public void ToObservation_WithModel_MapsCorrectly()
    {
        var person = new PersonBuilder()
            .WithOgdId(22)
            .Build();

        var legalForm = new LegalFormBuilder()
            .WithUri("https://legal.form")
            .Build();

        var interestFunction = new InterestFunctionBuilder()
            .WithDescriptionDe("function de")
            .WithDescriptionFr("function fr")
            .WithDescriptionIt("function it")
            .WithDescriptionRm("function rm")
            .WithOgdId(8)
            .Build();

        var interestCommittee = new InterestCommitteeBuilder()
            .WithDescriptionDe("committee de")
            .WithDescriptionFr("committee fr")
            .WithDescriptionIt("committee it")
            .WithDescriptionRm("committee rm")
            .WithOgdId(9)
            .Build();

        var interest = new InterestBuilder()
            .WithOgdId(11)
            .WithBeginDate(new DateOnly(2000, 1, 1))
            .WithEndDate(new DateOnly(2000, 1, 2))
            .WithPerson(person)
            .WithLegalForm(legalForm)
            .WithText("text")
            .WithInterestText(null)
            .WithUidOrganisationId("uid")
            .WithInterestFunction(interestFunction)
            .WithInterestCommittee(interestCommittee)
            .Build();

        var observation = InterestMapper.ToObservation(interest);

        Assert.That(observation, Is.Not.Null);

        Assert.That(observation.KeyUri, Is.EqualTo("vested-interest:11"));
        Assert.That(observation.ValidFrom, Is.EqualTo(new DateTime(2000, 1, 1)));
        Assert.That(observation.ValidTo, Is.EqualTo(new DateTime(2000, 1, 2)));

        Assert.That(observation.KeyDimensionLinks, Has.Count.EqualTo(3));

        Assert.That(observation.KeyDimensionLinks[0].Predicate, Is.EqualTo("vested-interest:hasPerson"));
        Assert.That(observation.KeyDimensionLinks[0].Uri, Is.EqualTo("person:22"));

        Assert.That(observation.KeyDimensionLinks[1].Predicate, Is.EqualTo("vested-interest:hasFunction"));
        Assert.That(observation.KeyDimensionLinks[1].Uri, Is.EqualTo("interest-function:8"));

        Assert.That(observation.KeyDimensionLinks[2].Predicate, Is.EqualTo("vested-interest:hasCommittee"));
        Assert.That(observation.KeyDimensionLinks[2].Uri, Is.EqualTo("interest-committee:9"));

        Assert.That(observation.Values, Has.Count.EqualTo(3));

        Assert.That(observation.Values[0].Predicate, Is.EqualTo(OgdExportConstants.SchemaLegalName));
        Assert.That(observation.Values[0].Object, Is.EqualTo("https://legal.form"));

        Assert.That(observation.Values[1].Predicate, Is.EqualTo(OgdExportConstants.SchemaName));
        Assert.That(observation.Values[1].Object, Is.EqualTo("text"));

        Assert.That(observation.Values[2].Predicate, Is.EqualTo(OgdExportConstants.SchemaIdentifier));
        Assert.That(observation.Values[2].Object, Is.EqualTo("uid"));
    }

    [Test]
    public void ToObservation_WithLdAdminLegalFormUri_MapsToLdNamespace()
    {
        var person = new PersonBuilder()
            .WithOgdId(22)
            .Build();

        var legalForm = new LegalFormBuilder()
            .WithUri("https://ld.admin.ch/ech/97/legalforms/0109")
            .Build();

        var interestFunction = new InterestFunctionBuilder()
            .WithDescriptionDe("function de")
            .WithDescriptionFr("function fr")
            .WithDescriptionIt("function it")
            .WithDescriptionRm("function rm")
            .WithOgdId(8)
            .Build();

        var interestCommittee = new InterestCommitteeBuilder()
            .WithDescriptionDe("committee de")
            .WithDescriptionFr("committee fr")
            .WithDescriptionIt("committee it")
            .WithDescriptionRm("committee rm")
            .WithOgdId(9)
            .Build();

        var interest = new InterestBuilder()
            .WithOgdId(11)
            .WithBeginDate(new DateOnly(2000, 1, 1))
            .WithEndDate(new DateOnly(2000, 1, 2))
            .WithPerson(person)
            .WithLegalForm(legalForm)
            .WithText("text")
            .WithInterestText("interestText")
            .WithUidOrganisationId("uid")
            .WithInterestFunction(interestFunction)
            .WithInterestCommittee(interestCommittee)
            .Build();

        var observation = InterestMapper.ToObservation(interest);

        Assert.That(observation, Is.Not.Null);

        Assert.That(observation.KeyUri, Is.EqualTo("vested-interest:11"));
        Assert.That(observation.ValidFrom, Is.EqualTo(new DateTime(2000, 1, 1)));
        Assert.That(observation.ValidTo, Is.EqualTo(new DateTime(2000, 1, 2)));

        Assert.That(observation.KeyDimensionLinks, Has.Count.EqualTo(4));

        Assert.That(observation.KeyDimensionLinks[0].Predicate, Is.EqualTo("vested-interest:hasPerson"));
        Assert.That(observation.KeyDimensionLinks[0].Uri, Is.EqualTo("person:22"));

        Assert.That(observation.KeyDimensionLinks[1].Predicate, Is.EqualTo("vested-interest:hasFunction"));
        Assert.That(observation.KeyDimensionLinks[1].Uri, Is.EqualTo("interest-function:8"));

        Assert.That(observation.KeyDimensionLinks[2].Predicate, Is.EqualTo("vested-interest:hasCommittee"));
        Assert.That(observation.KeyDimensionLinks[2].Uri, Is.EqualTo("interest-committee:9"));

        Assert.That(observation.KeyDimensionLinks[3].Predicate, Is.EqualTo(OgdExportConstants.SchemaLegalName));
        Assert.That(observation.KeyDimensionLinks[3].Uri, Is.EqualTo("ld:ech/97/legalforms/0109"));

        Assert.That(observation.Values, Has.Count.EqualTo(2));

        Assert.That(observation.Values[0].Predicate, Is.EqualTo(OgdExportConstants.SchemaName));
        Assert.That(observation.Values[0].Object, Is.EqualTo("interestText"));

        Assert.That(observation.Values[1].Predicate, Is.EqualTo(OgdExportConstants.SchemaIdentifier));
        Assert.That(observation.Values[1].Object, Is.EqualTo("uid"));
    }
}
