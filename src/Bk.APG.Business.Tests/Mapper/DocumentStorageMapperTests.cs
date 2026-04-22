using Bk.APG.Business.Dtos;
using Bk.APG.Business.Mapper;
using Bk.APG.CrossCutting.Tests.Builders;
using Bogus;

namespace Bk.APG.Business.Tests.Mapper;

[TestFixture]
internal class DocumentStorageMapperTests
{
    [Test]
    public void MapToUpdateDto_ShouldMapToDto()
    {
        var documentStorage = new DocumentStorageBuilder().Build();
        var languageId = Guid.NewGuid();

        var dto = DocumentStorageMapper.ToUpdateDto(documentStorage, languageId, true);

        Assert.That(dto, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(dto.Id, Is.EqualTo(documentStorage.Id));
            Assert.That(dto.DisplayName, Is.EqualTo(documentStorage.DocumentName));
            Assert.That(dto.DocumentStorageId, Is.EqualTo(documentStorage.DocumentStorageId));
            Assert.That(dto.LanguageId, Is.EqualTo(languageId));
            Assert.That(dto.IsOriginal, Is.True);
        });
    }

    [Test]
    public void MapFromModificationDto_WithoutIncomingId_ShouldMapToDtoWithNewId()
    {
        var updateDto = new Faker<DocumentStorageModificationDto>().Generate();
        updateDto.Id = null;
        updateDto.DocumentStorageId = "storageId";

        var dto = DocumentStorageMapper.FromModificationDto(updateDto, "userName");

        Assert.That(dto, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(dto.Id, Is.Not.Empty);
            Assert.That(dto.DocumentName, Is.EqualTo(updateDto.DisplayName));
            Assert.That(dto.CreatedBy, Is.EqualTo("userName"));
            Assert.That(dto.Created, Is.GreaterThan(DateTime.UtcNow.AddSeconds(-1)));
            Assert.That(dto.ModifiedBy, Is.EqualTo("userName"));
            Assert.That(dto.Modified, Is.GreaterThan(DateTime.UtcNow.AddSeconds(-1)));
            Assert.That(dto.DocumentStorageId, Is.EqualTo("storageId"));
        });
    }

    [Test]
    public void MapFromModificationDto_WithIncomingId_ShouldMapToDto()
    {
        var updateDto = new Faker<DocumentStorageModificationDto>().Generate();
        updateDto.Id = new Guid("3E0016AE-13DB-4A1F-9E26-ADA79D93834E");
        updateDto.DocumentStorageId = "storageId";

        var dto = DocumentStorageMapper.FromModificationDto(updateDto, "userName");

        Assert.That(dto, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(dto.Id, Is.EqualTo(updateDto.Id));
            Assert.That(dto.DocumentName, Is.EqualTo(updateDto.DisplayName));
            Assert.That(dto.CreatedBy, Is.EqualTo("userName"));
            Assert.That(dto.Created, Is.GreaterThan(DateTime.UtcNow.AddSeconds(-1)));
            Assert.That(dto.ModifiedBy, Is.EqualTo("userName"));
            Assert.That(dto.Modified, Is.GreaterThan(DateTime.UtcNow.AddSeconds(-1)));
            Assert.That(dto.DocumentStorageId, Is.EqualTo("storageId"));
        });
    }
}
