using Bk.APG.Business.Dtos;
using Bk.APG.CrossCutting.Tests.Builders;
using Bk.APG.Infrastructure.Service.Post;

namespace Bk.APG.Infrastructure.Service.Tests.Post;

[TestFixture]
internal class AddressMapperTests
{
    [TestCase(null, null, null, null)]
    [TestCase("", null, null, null)]
    [TestCase("Pflaumenweg", "Pflaumenweg", null, null)]
    [TestCase("Teststrasse", "Teststrasse", null, null)]
    [TestCase("Teststrasse 14", "Teststrasse", "14", null)]
    [TestCase("Pflaumenweg       42", "Pflaumenweg", "42", null)]
    [TestCase("Pflaumenweg       17      ", "Pflaumenweg", "17", null)]
    [TestCase("Alter Weg       18", "Alter Weg", "18", null)]
    [TestCase("Pflaumenweg 5/8", "Pflaumenweg", "5/8", null)]
    [TestCase("Seeweg 42C", "Seeweg", "42", "C")]
    [TestCase("Alte Straße 7A", "Alte Straße", "7", "A")]
    [TestCase("Holzweg 7/8D", "Holzweg", "7/8", "D")]
    public void ToAutoCompleteDto_WithStreet_ShouldMapToStreetName(string? street, string? expectedStreetName, string? expectedHouseNumber, string? expectedHouseNumberAddition)
    {
        var (streetName, houseNumber, houseNumberAddition) = AddressMapper.GetStreet(street);

        Assert.Multiple(() =>
        {
            Assert.That(streetName, Is.EqualTo(expectedStreetName));
            Assert.That(houseNumber, Is.EqualTo(expectedHouseNumber));
            Assert.That(houseNumberAddition, Is.EqualTo(expectedHouseNumberAddition));
        });
    }

    [Test]
    public void ToAutoCompleteDto_WithValues_ShouldMapValues()
    {
        var search = new AddressSearchDto
        {
            Street = "Musterweg 14B",
            City = "Musterstadt",
            Zip = "7352"
        };

        var result = AddressMapper.ToAutoCompleteDto(search);

        Assert.Multiple(() =>
        {
            Assert.That(result.Request.StreetName, Is.EqualTo("Musterweg"));
            Assert.That(result.Request.HouseNo, Is.EqualTo("14"));
            Assert.That(result.Request.HouseNoAddition, Is.EqualTo("B"));
            Assert.That(result.Request.TownName, Is.EqualTo(search.City));
            Assert.That(result.Request.ZipCode, Is.EqualTo(search.Zip));
        });
    }

    [Test]
    public void ToAddressDto_WithEmptyResult_ShouldMapToEmptyAddressDto()
    {
        var result = AddressMapper.ToAddressDto(new AddressAutocompleteResultDto());

        Assert.Multiple(() =>
        {
            Assert.That(result.Id, Is.EqualTo(Guid.Empty));
            Assert.That(result.Street, Is.Null);
            Assert.That(result.Zip, Is.Null);
            Assert.That(result.City, Is.Null);
            Assert.That(result.Canton, Is.Null);
        });
    }

    [TestCase("", "", "", null)]
    [TestCase("", "", "B", null)]
    [TestCase("", "14", "A", null)]
    [TestCase("Musterstraße", "", "", "Musterstraße")]
    [TestCase("Musterstraße", "", "C", "Musterstraße")]
    [TestCase("Musterstraße", "14", "D", "Musterstraße 14D")]
    [TestCase("Alte Straße", "3/6", "", "Alte Straße 3/6")]
    [TestCase("Alte Straße", "3/6", "A", "Alte Straße 3/6A")]
    public void ToAddressDto_WithStreet_ShouldMapToStreet(string streetName, string houseNumber, string houseNumberAddition, string? expectedStreetName)
    {
        var dto = new AddressAutocompleteResultDto
        {
            StreetName = streetName,
            HouseNo = houseNumber,
            HouseNoAddition = houseNumberAddition
        };

        var result = AddressMapper.ToAddressDto(dto);

        Assert.That(result.Street, Is.EqualTo(expectedStreetName));
    }

    [Test]
    public void ToAddressDto_WithValues_ShouldMapToAddress()
    {
        var dto = new AddressAutocompleteResultDto
        {
            StreetName = "Musterstraße",
            HouseNo = "14",
            ZipCode = "7654",
            TownName = "Musterstadt"
        };

        var result = AddressMapper.ToAddressDto(dto);

        Assert.Multiple(() =>
        {
            Assert.That(result.Street, Is.EqualTo("Musterstraße 14"));
            Assert.That(result.Zip, Is.EqualTo(dto.ZipCode));
            Assert.That(result.City, Is.EqualTo(dto.TownName));
        });
    }

    [Test]
    public void ToAddressDto_WithoutCanton_ShouldMapCantonToNull()
    {
        var dto = new AddressAutocompleteResultDto { Canton = "test_canton" };

        var result = AddressMapper.ToAddressDto(dto);

        Assert.That(result.Canton, Is.Null);
    }

    [Test]
    public void ToAddressDto_WithCanton_ShouldMapCanton()
    {
        var canton = new CantonBuilder().WithTextDe("test_canton").Build();
        var dto = new AddressAutocompleteResultDto { Canton = "test_canton" };

        var result = AddressMapper.ToAddressDto(dto, [canton]);

        Assert.That(result.Canton, Is.Not.Null);
        Assert.That(result.Canton.Id, Is.EqualTo(canton.Id));
    }

    [Test]
    public void ToAddressDto_ForVerificationResultWithValues_ShouldMapToAddress()
    {
        var dto = new BuildingVerificationDataDto
        {
            StreetName = "Musterstraße",
            HouseNo = "14",
            ZipCode = "7654",
            TownName = "Musterstadt"
        };

        var result = AddressMapper.ToAddressDto(dto);

        Assert.Multiple(() =>
        {
            Assert.That(result.Street, Is.EqualTo("Musterstraße 14"));
            Assert.That(result.Zip, Is.EqualTo(dto.ZipCode));
            Assert.That(result.City, Is.EqualTo(dto.TownName));
        });
    }

    [TestCase("1", AddressVerificationStatus.Ok)]
    [TestCase("2", AddressVerificationStatus.Corrected)]
    [TestCase("3", AddressVerificationStatus.Corrected)]
    [TestCase("4", AddressVerificationStatus.Corrected)]
    [TestCase("5", AddressVerificationStatus.Corrected)]
    [TestCase("6", AddressVerificationStatus.Invalid)]
    [TestCase("7", AddressVerificationStatus.Invalid)]
    [TestCase("8", AddressVerificationStatus.Ambiguous)]
    [TestCase("9", AddressVerificationStatus.Invalid)]
    [TestCase("10", AddressVerificationStatus.Invalid)]
    [TestCase("40_000", AddressVerificationStatus.Invalid)]
    [TestCase("test_text", AddressVerificationStatus.Invalid)]
    public void ToAddressVerificationStatus_WithStatus_ShouldMapToStatus(string value, AddressVerificationStatus expectedStatus)
    {
        var dto = new BuildingVerificationDataDto { Status = value };

        var result = AddressMapper.ToAddressVerificationStatus(dto);

        Assert.That(result, Is.EqualTo(expectedStatus));
    }
}
