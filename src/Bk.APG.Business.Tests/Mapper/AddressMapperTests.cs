using Bk.APG.Business.Dtos;
using Bk.APG.Business.Mapper;
using Bk.APG.CrossCutting.Tests.Builders;

namespace Bk.APG.Business.Tests.Mapper;

[TestFixture]
internal class AddressMapperTests
{
    [Test]
    public void ToAddressDto_WhenCalled_ShouldMapToDto()
    {
        var address = new AddressBuilder().Build();

        var addressDto = AddressMapper.ToAddressDetailDto(address, address.Id, false);

        Assert.That(addressDto, Is.Not.Null);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(addressDto.Id, Is.EqualTo(address.Id));
            Assert.That(addressDto.CompanyName, Is.EqualTo(address.CompanyName));
            Assert.That(addressDto.Street, Is.EqualTo(address.Street));
            Assert.That(addressDto.PoBox, Is.EqualTo(address.PoBox));
            Assert.That(addressDto.CountryCode, Is.EqualTo(address.CountryCode));
            Assert.That(addressDto.Zip, Is.EqualTo(address.Zip));
            Assert.That(addressDto.City, Is.EqualTo(address.City));
            Assert.That(addressDto.Phone, Is.EqualTo(address.Phone));
            Assert.That(addressDto.Mobile, Is.EqualTo(address.Mobile));
            Assert.That(addressDto.Email, Is.EqualTo(address.Email));
            Assert.That(addressDto.ActiveAddress, Is.True);
        }
    }

    [Test]
    public void ToAddressDto_WithAddressToMask_ShouldMaskAddress()
    {
        var address = new AddressBuilder().Build();

        var addressDto = AddressMapper.ToAddressDetailDto(address, address.Id, true);

        Assert.That(addressDto, Is.Not.Null);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(addressDto.Id, Is.EqualTo(address.Id));
            Assert.That(addressDto.CompanyName, Is.Empty);
            Assert.That(addressDto.Street, Is.Empty);
            Assert.That(addressDto.PoBox, Is.Empty);
            Assert.That(addressDto.CountryCode, Is.Empty);
            Assert.That(addressDto.Zip, Is.Empty);
            Assert.That(addressDto.City, Is.Empty);
            Assert.That(addressDto.Phone, Is.Empty);
            Assert.That(addressDto.Mobile, Is.Empty);
            Assert.That(addressDto.Email, Is.Empty);
            Assert.That(addressDto.ActiveAddress, Is.True);
        }
    }

    [Test]
    public void FromAddressUpdateDto_WhenCalled_ShouldMapMapFromDto()
    {
        var addressDto = new AddressUpdateDto
        {
            Id = Guid.NewGuid(),
            CantonId = Guid.NewGuid(),
            City = "city_private",
            Street = "street_private",
            CompanyName = "companyname_private",
            CountryCode = "countrycode_private",
            Email = "email_private",
            Mobile = "mobile_private",
            Phone = "phone_private",
            PoBox = "pobox_private",
            Zip = "zip_private",
        };

        var address = AddressMapper.FromAddressUpdateDto(addressDto);

        Assert.That(address, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(address.Id, Is.EqualTo(addressDto.Id));
            Assert.That(address.CompanyName, Is.EqualTo(addressDto.CompanyName));
            Assert.That(address.Street, Is.EqualTo(addressDto.Street));
            Assert.That(address.PoBox, Is.EqualTo(addressDto.PoBox));
            Assert.That(address.CountryCode, Is.EqualTo(addressDto.CountryCode));
            Assert.That(address.Zip, Is.EqualTo(addressDto.Zip));
            Assert.That(address.City, Is.EqualTo(addressDto.City));
            Assert.That(address.CantonId, Is.EqualTo(addressDto.CantonId));
            Assert.That(address.Phone, Is.EqualTo(addressDto.Phone));
            Assert.That(address.Mobile, Is.EqualTo(addressDto.Mobile));
            Assert.That(address.Email, Is.EqualTo(addressDto.Email));
        });
    }
}
