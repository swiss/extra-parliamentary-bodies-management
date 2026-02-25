using Bk.APG.Business.Dtos;
using Bk.APG.Business.Models;

namespace Bk.APG.Business.Mapper;

public static class AddressMapper
{
    public static AddressDetailsDto ToAddressDetailDto(Address address, Guid activeAddressId, bool maskAddress)
    {
        return new AddressDetailsDto
        {
            Id = address.Id,
            CompanyName = maskAddress ? string.Empty : address.CompanyName,
            Street = maskAddress ? string.Empty : address.Street,
            PoBox = maskAddress ? string.Empty : address.PoBox,
            Country = maskAddress ? string.Empty : address.Country?.GetText(),
            Zip = maskAddress ? string.Empty : address.Zip,
            City = maskAddress ? string.Empty : address.City,
            Canton = maskAddress ? string.Empty : address.Canton?.GetText(),
            Phone = maskAddress ? string.Empty : address.Phone,
            Mobile = maskAddress ? string.Empty : address.Mobile,
            Email = maskAddress ? string.Empty : address.Email,
            ActiveAddress = address.Id.Equals(activeAddressId)
        };
    }

    public static AddressUpdateDto ToAddressUpdateDto(Address address, Guid activeAddressId)
    {
        return new AddressUpdateDto
        {
            Id = address.Id,
            CompanyName = address.CompanyName,
            Street = address.Street,
            PoBox = address.PoBox,
            CountryId = address.CountryId,
            Zip = address.Zip,
            City = address.City,
            CantonId = address.Canton?.Id,
            Phone = address.Phone,
            Mobile = address.Mobile,
            Email = address.Email,
            ActiveAddress = address.Id.Equals(activeAddressId)
        };
    }

    public static Address FromAddressUpdateDto(AddressUpdateDto addressUpdateDto)
    {
        var address = new Address
        {
            Id = addressUpdateDto.Id ?? Guid.NewGuid(),
            CompanyName = addressUpdateDto.CompanyName,
            Street = addressUpdateDto.Street,
            PoBox = addressUpdateDto.PoBox,
            CountryId = addressUpdateDto.CountryId,
            Zip = addressUpdateDto.Zip,
            City = addressUpdateDto.City,
            CantonId = addressUpdateDto.CantonId,
            Phone = addressUpdateDto.Phone,
            Mobile = addressUpdateDto.Mobile,
            Email = addressUpdateDto.Email,
            Created = DateTime.UtcNow,
            Modified = DateTime.UtcNow,
            CreatedBy = "SYSTEM",
            ModifiedBy = "SYSTEM"
        };

        return address;
    }
}
