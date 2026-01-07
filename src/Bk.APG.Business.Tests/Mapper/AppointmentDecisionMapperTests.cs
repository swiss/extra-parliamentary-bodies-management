using Bk.APG.Business.Dtos;
using Bk.APG.Business.Mapper;
using Bk.APG.Business.Models;
using Bk.APG.Common.Resources;
using Bk.APG.CrossCutting.Tests.Builders;

namespace Bk.APG.Business.Tests.Mapper;

[TestFixture]
internal class AppointmentDecisionMapperTests
{
    [TestCase("3E0016AE-13DB-4A1F-9E26-ADA79D93834E", "myLink", "myLink")]
    [TestCase("ADD3267C-B676-4EE0-A1C9-A0AC2C703D06", "2024.2032", "https://intranet.resolver-r.bk.admin.ch/exe/2024.2032")]
    [TestCase("E985CC03-51CD-4F8E-9189-50CFF3F2C06E", "myLink", "")]
    [Test]
    public void MapToAppointmentDecisionListDto_WithCorrectData_ShouldMapToDto(Guid appointmentDecisionLinkTypeId, string link, string expectedLink)
    {
        var appointmentDecision = CreateAppointmentDecision();
        appointmentDecision.AppointmentDecisionLinkTypeId = appointmentDecisionLinkTypeId;
        appointmentDecision.Link = link;

        var exeBrcLink = "https://intranet.resolver-r.bk.admin.ch/exe/";

        var dto = AppointmentDecisionMapper.ToAppointmentDecisionListDto(appointmentDecision, exeBrcLink);

        Assert.That(dto, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(dto.Id, Is.EqualTo(appointmentDecision.Id));
            Assert.That(dto.AppointmentDecisionDate, Is.EqualTo(appointmentDecision.AppointmentDecisionDate));
            Assert.That(dto.AppointmentDecisionType, Is.EqualTo(appointmentDecision.AppointmentDecisionType?.GetText()));
            Assert.That(dto.AppointmentDecisionLinkType, Is.EqualTo(appointmentDecision.AppointmentDecisionLinkType?.GetText()));
            Assert.That(dto.Text, Is.EqualTo(appointmentDecision.Text));
            Assert.That(dto.Link, Is.EqualTo(expectedLink));
            Assert.That(dto.LinkText, Is.EqualTo(appointmentDecision.Link));
            Assert.That(dto.FileName, Is.EqualTo(appointmentDecision.OriginalDocument!.DocumentName));
            Assert.That(dto.Modified, Is.EqualTo(appointmentDecision.Modified));
            Assert.That(dto.DocumentStorageId, Is.EqualTo(appointmentDecision.OriginalDocument!.DocumentStorageId));
        });
    }

    [Test]
    public void MapToAppointmentDecisionListDto_WithoutOriginalDocument_ShouldMapReferenceDocumentName()
    {
        var appointmentDecision = new AppointmentDecisionBuilder()
            .WithFileReferenceGerman(new DocumentStorageBuilder().WithDisplayName("Foo Bar").Build())
            .WithFileReferenceFrench(null!)
            .WithFileReferenceItalian(null!)
            .WithFileReferenceRomansh(null!)
            .Build();

        var dto = AppointmentDecisionMapper.ToAppointmentDecisionListDto(appointmentDecision, string.Empty);

        Assert.That(dto.FileName, Is.EqualTo($"Foo Bar"));
    }

    [Test]
    public void MapToAppointmentDecisionListDto_WithoutOriginalDocumentAndWithMultipleFileReferences_ShouldMapReferenceDocumentNameWithPostFix()
    {
        var appointmentDecision = new AppointmentDecisionBuilder()
            .WithFileReferenceGerman(new DocumentStorageBuilder().WithDisplayName("Foo Bar").Build())
            .WithFileReferenceFrench(new DocumentStorageBuilder().Build())
            .Build();

        var dto = AppointmentDecisionMapper.ToAppointmentDecisionListDto(appointmentDecision, string.Empty);

        Assert.That(dto.FileName, Is.EqualTo($"Foo Bar ({BusinessTexts.AppointmentDecision_MultipleFiles})"));
    }

    [Test]
    public void MapFromAppointmentDecisionCreateDto_WithCorrectData_ShouldMapToDto()
    {
        var appointmentDecisionCreateDto = new AppointmentDecisionCreateDto
        {
            AppointmentDecisionDate = DateOnly.FromDateTime(DateTime.UtcNow),
            AppointmentDecisionLinkTypeId = Guid.NewGuid(),
            AppointmentDecisionTypeId = Guid.NewGuid(),
            CommitteeId = Guid.NewGuid(),
            Link = "link",
            Text = "text"
        };

        var appointmentDecision = AppointmentDecisionMapper.FromAppointmentDecisionCreateDto(appointmentDecisionCreateDto, "userName");

        Assert.That(appointmentDecision, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(appointmentDecision.AppointmentDecisionDate, Is.EqualTo(appointmentDecisionCreateDto.AppointmentDecisionDate));
            Assert.That(appointmentDecision.AppointmentDecisionTypeId, Is.EqualTo(appointmentDecisionCreateDto.AppointmentDecisionTypeId));
            Assert.That(appointmentDecision.AppointmentDecisionLinkTypeId, Is.EqualTo(appointmentDecisionCreateDto.AppointmentDecisionLinkTypeId));
            Assert.That(appointmentDecision.Text, Is.EqualTo(appointmentDecisionCreateDto.Text));
            Assert.That(appointmentDecision.Link, Is.EqualTo(appointmentDecisionCreateDto.Link));
            Assert.That(appointmentDecision.Created, Is.GreaterThan(DateTime.UtcNow.AddSeconds(-1)));
            Assert.That(appointmentDecision.CreatedBy, Is.EqualTo("userName"));
            Assert.That(appointmentDecision.Modified, Is.GreaterThan(DateTime.UtcNow.AddSeconds(-1)));
            Assert.That(appointmentDecision.ModifiedBy, Is.EqualTo("userName"));
        });
    }

    [Test]
    public void MapToAppointmentDecisionUpdateDto_WithCorrectData_ShouldMapToDto()
    {
        var appointmentDecision = new AppointmentDecisionBuilder()
            .WithFileReferenceGerman(new DocumentStorageBuilder().WithDisplayName("DE").Build())
            .WithFileReferenceFrench(new DocumentStorageBuilder().WithDisplayName("FR").Build())
            .WithFileReferenceItalian(new DocumentStorageBuilder().WithDisplayName("IT").Build())
            .Build();

        var appointmentDecisionDto = AppointmentDecisionMapper.ToAppointmentDecisionUpdateDto(appointmentDecision, "userName");

        Assert.That(appointmentDecisionDto, Is.Not.Null);
        Assert.That(appointmentDecisionDto.Documents, Is.Not.Null);

        Assert.Multiple(() =>
        {
            Assert.That(appointmentDecisionDto.Id, Is.EqualTo(appointmentDecision.Id));
            Assert.That(appointmentDecisionDto.AppointmentDecisionDate, Is.EqualTo(appointmentDecision.AppointmentDecisionDate));
            Assert.That(appointmentDecisionDto.AppointmentDecisionTypeId, Is.EqualTo(appointmentDecision.AppointmentDecisionTypeId));
            Assert.That(appointmentDecisionDto.AppointmentDecisionLinkTypeId, Is.EqualTo(appointmentDecision.AppointmentDecisionLinkTypeId));
            Assert.That(appointmentDecisionDto.Text, Is.EqualTo(appointmentDecision.Text));
            Assert.That(appointmentDecisionDto.Link, Is.EqualTo(appointmentDecision.Link));
            Assert.That(appointmentDecisionDto.Documents!.ToList(), Has.Count.EqualTo(3));
            Assert.That(appointmentDecisionDto.Documents!.ToList()[0].DisplayName, Is.EqualTo("DE"));
            Assert.That(appointmentDecisionDto.Documents!.ToList()[1].DisplayName, Is.EqualTo("FR"));
            Assert.That(appointmentDecisionDto.Documents!.ToList()[2].DisplayName, Is.EqualTo("IT"));
        });
    }

    private static AppointmentDecision CreateAppointmentDecision()
    {
        var appointmentDecision = new AppointmentDecisionBuilder()
            .WithId(Guid.Parse("1BD310CD-DD70-488A-A0B7-E24464AD0120"))
            .WithCommitteeId(Guid.Parse("9df96395-bd65-4235-a4fa-fb87689df11f"))
            .WithCommittee(new CommitteeBuilder().WithOldId(111000).Build())
            .WithAppointmentDecisionLinkType(new AppointmentDecisionLinkTypeBuilder().WithId(Guid.Parse("F3C34FA4-A2CA-482F-8789-BE8A77C979CF")).Build())
            .WithAppointmentDecisionType(new AppointmentDecisionTypeBuilder().WithId(Guid.Parse("C50E47AD-9582-40FA-BDA3-1989C6B1AE5A")).Build())
            .WithFileReferenceGerman(new DocumentStorageBuilder().Build())
            .WithOriginalDocument(new DocumentStorageBuilder().Build())
            .Build();

        return appointmentDecision;
    }
}
