using Bk.APG.Business.Dtos;
using Bk.APG.Business.Mapper;
using Bk.APG.Business.Models;
using Bk.APG.CrossCutting.Tests.Builders;

namespace Bk.APG.Business.Tests.Mapper;

[TestFixture]
internal class ContactPointMapperTests
{
    [Test]
    public void MapToContactPointDetailDto_WithModel_ShouldMapToDto()
    {
        var contactPoint = CreateContactPoint();

        var dto = ContactPointMapper.ToContactPointDetailDto(contactPoint);

        Assert.That(dto, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(dto.Id, Is.EqualTo(contactPoint.Id));
            Assert.That(dto.CompanyName, Is.EqualTo(contactPoint.CompanyName));
            Assert.That(dto.Section, Is.EqualTo(contactPoint.Section));
            Assert.That(dto.Street, Is.EqualTo(contactPoint.Street));
            Assert.That(dto.Zip, Is.EqualTo(contactPoint.Zip));
            Assert.That(dto.City, Is.EqualTo(contactPoint.City));
            Assert.That(dto.Phone, Is.EqualTo(contactPoint.Phone));
            Assert.That(dto.Email, Is.EqualTo(contactPoint.Email));
            Assert.That(dto.Surname, Is.EqualTo(contactPoint.Surname));
            Assert.That(dto.GivenName, Is.EqualTo(contactPoint.GivenName));
            Assert.That(dto.Title, Is.EqualTo(contactPoint.Title));
            Assert.That(dto.PersonalPhone, Is.EqualTo(contactPoint.PersonalPhone));
            Assert.That(dto.PersonalMobile, Is.EqualTo(contactPoint.PersonalMobile));
            Assert.That(dto.PersonalEmail, Is.EqualTo(contactPoint.PersonalEmail));
            Assert.That(dto.ContactPointType, Is.EqualTo(contactPoint.ContactPointType?.TextDe));
            Assert.That(dto.ContactPointTypeId, Is.EqualTo(contactPoint.ContactPointType!.Id));
        });
    }

    [Test]
    public void MapToContactPointListDto_WithPersonalPhoneOnly_ShouldMapToDto()
    {
        var contactPoint = CreateContactPoint();
        contactPoint.Phone = null;
        contactPoint.Email = null;

        var dto = ContactPointMapper.ToContactPointListDto(contactPoint);

        Assert.That(dto, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(dto.Id, Is.EqualTo(contactPoint.Id));
            Assert.That(dto.CompanyName, Is.EqualTo(contactPoint.CompanyName));
            Assert.That(dto.PersonName, Is.EqualTo(contactPoint.Surname + " " + contactPoint.GivenName));
            Assert.That(dto.Section, Is.EqualTo(contactPoint.Section));
            Assert.That(dto.ZipCity, Is.EqualTo(contactPoint.Zip + " " + contactPoint.City));
            Assert.That(dto.Phone, Is.EqualTo(contactPoint.PersonalPhone));
            Assert.That(dto.Mobile, Is.EqualTo(contactPoint.PersonalMobile));
            Assert.That(dto.Email, Is.EqualTo(contactPoint.PersonalEmail));
            Assert.That(dto.ContactPointType, Is.EqualTo(contactPoint.ContactPointType?.TextDe));
        });
    }

    [Test]
    public void MapToContactPointListDto_WithModel_ShouldMapToDto()
    {
        var contactPoint = CreateContactPoint();

        var dto = ContactPointMapper.ToContactPointListDto(contactPoint);

        Assert.That(dto, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(dto.Id, Is.EqualTo(contactPoint.Id));
            Assert.That(dto.CompanyName, Is.EqualTo(contactPoint.CompanyName));
            Assert.That(dto.PersonName, Is.EqualTo(contactPoint.Surname + " " + contactPoint.GivenName));
            Assert.That(dto.Section, Is.EqualTo(contactPoint.Section));
            Assert.That(dto.ZipCity, Is.EqualTo(contactPoint.Zip + " " + contactPoint.City));
            Assert.That(dto.Phone, Is.EqualTo(contactPoint.Phone));
            Assert.That(dto.Mobile, Is.EqualTo(contactPoint.PersonalMobile));
            Assert.That(dto.Email, Is.EqualTo(contactPoint.Email));
            Assert.That(dto.ContactPointType, Is.EqualTo(contactPoint.ContactPointType?.TextDe));
        });
    }

    [Test]
    public void MapFromContactPointUpdateToCreateDto_WithModel_ShouldMapToDto()
    {
        var dto = CreateContactPointUpdateDto();

        var createDto = ContactPointMapper.FromContactPointUpdateToCreateDto(dto);

        Assert.That(dto, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(dto.CompanyName, Is.EqualTo(createDto.CompanyName));
            Assert.That(dto.Section, Is.EqualTo(createDto.Section));
            Assert.That(dto.Phone, Is.EqualTo(createDto.Phone));
            Assert.That(dto.Email, Is.EqualTo(createDto.Email));

            Assert.That(dto.Surname, Is.EqualTo(createDto.Surname));
            Assert.That(dto.GivenName, Is.EqualTo(createDto.GivenName));
            Assert.That(dto.Title, Is.EqualTo(createDto.Title));
            Assert.That(dto.PersonalPhone, Is.EqualTo(createDto.PersonalPhone));
            Assert.That(dto.PersonalMobile, Is.EqualTo(createDto.PersonalMobile));
            Assert.That(dto.PersonalEmail, Is.EqualTo(createDto.PersonalEmail));
            Assert.That(dto.ContactPointTypeId, Is.EqualTo(createDto.ContactPointTypeId));
            Assert.That(dto.ContactPointTypeUri, Is.EqualTo(createDto.ContactPointTypeUri));

            Assert.That(dto.Street, Is.EqualTo(createDto.Street));
            Assert.That(dto.PoBox, Is.EqualTo(createDto.PoBox));
            Assert.That(dto.Zip, Is.EqualTo(createDto.Zip));
            Assert.That(dto.City, Is.EqualTo(createDto.City));
            Assert.That(dto.CommitteeBeginDate, Is.EqualTo(createDto.CommitteeBeginDate));
        });
    }

    [Test]
    public void MapFromContactPointCreateDto_WithModel_ShouldMapToDto()
    {
        var dto = CreateContactPointCreateDto();

        var createDto = ContactPointMapper.FromContactPointCreateDto(dto);

        Assert.That(dto, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(dto.CompanyName, Is.EqualTo(createDto.CompanyName));
            Assert.That(dto.Section, Is.EqualTo(createDto.Section));
            Assert.That(dto.Phone, Is.EqualTo(createDto.Phone));
            Assert.That(dto.Email, Is.EqualTo(createDto.Email));

            Assert.That(dto.Surname, Is.EqualTo(createDto.Surname));
            Assert.That(dto.GivenName, Is.EqualTo(createDto.GivenName));
            Assert.That(dto.Title, Is.EqualTo(createDto.Title));
            Assert.That(dto.PersonalPhone, Is.EqualTo(createDto.PersonalPhone));
            Assert.That(dto.PersonalMobile, Is.EqualTo(createDto.PersonalMobile));
            Assert.That(dto.PersonalEmail, Is.EqualTo(createDto.PersonalEmail));
            Assert.That(dto.ContactPointTypeId, Is.EqualTo(createDto.ContactPointTypeId));

            Assert.That(dto.Street, Is.EqualTo(createDto.Street));
            Assert.That(dto.PoBox, Is.EqualTo(createDto.PoBox));
            Assert.That(dto.Zip, Is.EqualTo(createDto.Zip));
            Assert.That(dto.City, Is.EqualTo(createDto.City));
        });
    }

    [Test]
    public void MapToContactPointUpdateDto_WithModel_ShouldMapToDto()
    {
        var contactPoint = CreateContactPoint();

        var updateDto = ContactPointMapper.ToContactPointUpdateDto(contactPoint);

        Assert.That(contactPoint, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(contactPoint.CompanyName, Is.EqualTo(updateDto.CompanyName));
            Assert.That(contactPoint.Section, Is.EqualTo(updateDto.Section));
            Assert.That(contactPoint.Phone, Is.EqualTo(updateDto.Phone));
            Assert.That(contactPoint.Email, Is.EqualTo(updateDto.Email));

            Assert.That(contactPoint.Surname, Is.EqualTo(updateDto.Surname));
            Assert.That(contactPoint.GivenName, Is.EqualTo(updateDto.GivenName));
            Assert.That(contactPoint.Title, Is.EqualTo(updateDto.Title));
            Assert.That(contactPoint.PersonalPhone, Is.EqualTo(updateDto.PersonalPhone));
            Assert.That(contactPoint.PersonalMobile, Is.EqualTo(updateDto.PersonalMobile));
            Assert.That(contactPoint.PersonalEmail, Is.EqualTo(updateDto.PersonalEmail));
            Assert.That(contactPoint.ContactPointTypeId, Is.EqualTo(updateDto.ContactPointTypeId));

            Assert.That(contactPoint.Street, Is.EqualTo(updateDto.Street));
            Assert.That(contactPoint.PoBox, Is.EqualTo(updateDto.PoBox));
            Assert.That(contactPoint.Zip, Is.EqualTo(updateDto.Zip));
            Assert.That(contactPoint.City, Is.EqualTo(updateDto.City));
            Assert.That(contactPoint.BeginDate, Is.EqualTo(updateDto.BeginDate));
            Assert.That(contactPoint.Committee!.BeginDate, Is.EqualTo(updateDto.CommitteeBeginDate));
            Assert.That(contactPoint.RowVersion, Is.EqualTo(updateDto.RowVersion));
        });
    }

    [Test]
    public void MapFromContactPointUpdateDto_WithModel_ShouldMapToDto()
    {
        var updateDto = CreateContactPointUpdateDto();

        var contactPoint = ContactPointMapper.FromContactPointUpdateDto(updateDto);

        Assert.That(contactPoint, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(contactPoint.CompanyName, Is.EqualTo(updateDto.CompanyName));
            Assert.That(contactPoint.Section, Is.EqualTo(updateDto.Section));
            Assert.That(contactPoint.Phone, Is.EqualTo(updateDto.Phone));
            Assert.That(contactPoint.Email, Is.EqualTo(updateDto.Email));

            Assert.That(contactPoint.Surname, Is.EqualTo(updateDto.Surname));
            Assert.That(contactPoint.GivenName, Is.EqualTo(updateDto.GivenName));
            Assert.That(contactPoint.Title, Is.EqualTo(updateDto.Title));
            Assert.That(contactPoint.PersonalPhone, Is.EqualTo(updateDto.PersonalPhone));
            Assert.That(contactPoint.PersonalMobile, Is.EqualTo(updateDto.PersonalMobile));
            Assert.That(contactPoint.PersonalEmail, Is.EqualTo(updateDto.PersonalEmail));
            Assert.That(contactPoint.ContactPointTypeId, Is.EqualTo(updateDto.ContactPointTypeId));

            Assert.That(contactPoint.Street, Is.EqualTo(updateDto.Street));
            Assert.That(contactPoint.PoBox, Is.EqualTo(updateDto.PoBox));
            Assert.That(contactPoint.Zip, Is.EqualTo(updateDto.Zip));
            Assert.That(contactPoint.City, Is.EqualTo(updateDto.City));
        });

    }

    private static ContactPoint CreateContactPoint()
    {
        var contactPoint = new ContactPointBuilder()
            .WithId(Guid.Parse("8697753c-f57a-4805-b1e9-bb1e546f4101"))
            .WithCommittee(new CommitteeBuilder().WithGermanDescription("Test Gremium").Build())
            .WithContactPointType(new ContactPointTypeBuilder().WithId(Guid.Parse("4CBE9337-95C4-4E44-B0C2-C8E23C0F9A08")).Build())
            .WithGender(new GenderBuilder().Build())
            .WithLanguage(new LanguageBuilder().Build())
            .WithCompanyName("Test Firma")
            .WithSurnameAndGivenName("Berger", "Rüdiger")
            .WithPhone("0234")
            .WithEmail("test@test.ch")
            .WithPersonalPhone("2220234")
            .WithPersonalEmail("testtest@test.ch")
            .WithPersonalMobile("234234234")
            .Build();

        return contactPoint;
    }

    private static ContactPointUpdateDto CreateContactPointUpdateDto()
    {
        var contactPoint = CreateContactPoint();

        return ContactPointMapper.ToContactPointUpdateDto(contactPoint);
    }

    private static ContactPointCreateDto CreateContactPointCreateDto()
    {
        var contactPoint = CreateContactPoint();

        return ContactPointMapper.ToContactPointCreateDto(contactPoint);
    }
}
