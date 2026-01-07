using System.Text;
using System.Text.RegularExpressions;
using Bk.APG.Business.Dtos;
using Bk.APG.Business.Mapper;
using Bk.APG.Business.Models;

namespace Bk.APG.Infrastructure.Service.Post;

internal static partial class AddressMapper
{
    internal static (string? streetName, string? houseNumber, string? houseNumberAddition) GetStreet(string? street)
    {
        string? streetName = null;
        string? houseNumber = null;
        string? houseNumberAddition = null;

        if (!string.IsNullOrWhiteSpace(street))
        {
            var match = StreetRegex().Match(street);
            var tempStreetName = match.Groups["StreetName"].Value.Trim();
            var tempHouseNumber = match.Groups["HouseNumber"].Value;
            var tempHouseNumberAddition = match.Groups["HouseNumberAddition"].Value;

            if (!string.IsNullOrWhiteSpace(tempStreetName))
            {
                streetName = tempStreetName;
            }

            if (!string.IsNullOrWhiteSpace(tempHouseNumber))
            {
                houseNumber = tempHouseNumber;
            }

            if (!string.IsNullOrWhiteSpace(tempHouseNumberAddition))
            {
                houseNumberAddition = tempHouseNumberAddition;
            }
        }

        return (streetName, houseNumber, houseNumberAddition);
    }

    internal static AddressAutocompleteRequestDto ToAutoCompleteDto(AddressSearchDto search)
    {
        var requestDto = new AddressAutocompleteRequestDto { Request = new AddressRequestDto() };

        var (streetName, houseNumber, houseNumberAddition) = GetStreet(search.Street);
        requestDto.Request.StreetName = streetName ?? string.Empty;
        requestDto.Request.HouseNo = houseNumber ?? string.Empty;
        requestDto.Request.HouseNoAddition = houseNumberAddition ?? string.Empty;

        if (!string.IsNullOrWhiteSpace(search.Zip))
        {
            requestDto.Request.ZipCode = search.Zip;
        }

        if (!string.IsNullOrWhiteSpace(search.City))
        {
            requestDto.Request.TownName = search.City;
        }

        return requestDto;
    }

    internal static AddressDto ToAddressDto(AddressAutocompleteResultDto dto, IEnumerable<Canton>? cantons = null)
    {
        string? street = null;
        if (!string.IsNullOrWhiteSpace(dto.StreetName))
        {
            var streetString = new StringBuilder();

            streetString.Append(dto.StreetName);

            if (!string.IsNullOrWhiteSpace(dto.HouseNo))
            {
                streetString.Append($" {dto.HouseNo}");

                if (!string.IsNullOrWhiteSpace(dto.HouseNoAddition))
                {
                    streetString.Append(dto.HouseNoAddition);
                }
            }

            street = streetString.ToString();
        }

        string? zipCode = null;
        if (!string.IsNullOrWhiteSpace(dto.ZipCode))
        {
            zipCode = dto.ZipCode;
        }

        string? city = null;
        if (!string.IsNullOrWhiteSpace(dto.TownName))
        {
            city = dto.TownName;
        }

        cantons = (cantons ?? []).ToList();
        CantonDto? canton = null;
        if (!string.IsNullOrWhiteSpace(dto.Canton) && cantons.Any())
        {
            var found = cantons.FirstOrDefault(item => item.TextDe.Equals(dto.Canton, StringComparison.InvariantCultureIgnoreCase));
            if (found is not null)
            {
                canton = CantonMapper.ToCantonDto(found);
            }
        }

        return new AddressDto
        {
            Id = Guid.Empty,
            Street = street,
            Zip = zipCode,
            City = city,
            Canton = canton
        };
    }

    internal static AddressDto ToAddressDto(BuildingVerificationDataDto dto, IEnumerable<Canton>? cantons = null)
    {
        string? street = null;
        if (!string.IsNullOrWhiteSpace(dto.StreetName))
        {
            var streetString = new StringBuilder();

            streetString.Append(dto.StreetName);

            if (!string.IsNullOrWhiteSpace(dto.HouseNo))
            {
                streetString.Append($" {dto.HouseNo}");

                if (!string.IsNullOrWhiteSpace(dto.HouseNoAddition))
                {
                    streetString.Append(dto.HouseNoAddition);
                }
            }

            street = streetString.ToString();
        }

        string? zipCode = null;
        if (!string.IsNullOrWhiteSpace(dto.ZipCode))
        {
            zipCode = dto.ZipCode;
        }

        string? city = null;
        if (!string.IsNullOrWhiteSpace(dto.TownName))
        {
            city = dto.TownName;
        }

        cantons = (cantons ?? []).ToList();
        CantonDto? canton = null;
        if (!string.IsNullOrWhiteSpace(dto.Canton) && cantons.Any())
        {
            var found = cantons.FirstOrDefault(item => item.TextDe.Equals(dto.Canton, StringComparison.InvariantCultureIgnoreCase));
            if (found is not null)
            {
                canton = CantonMapper.ToCantonDto(found);
            }
        }

        return new AddressDto
        {
            Id = Guid.Empty,
            Street = street,
            Zip = zipCode,
            City = city,
            Canton = canton
        };
    }

    internal static AddressVerificationStatus ToAddressVerificationStatus(BuildingVerificationDataDto dto)
    {
        return dto.Status switch
        {
            "1" => AddressVerificationStatus.Ok,
            "2" or "3" or "4" or "5" => AddressVerificationStatus.Corrected,
            "8" => AddressVerificationStatus.Ambiguous,
            _ => AddressVerificationStatus.Invalid
        };
    }

    [GeneratedRegex(@"^(?<StreetName>[^\d]+?)(?:\s+(?<HouseNumber>\d+(?:\/\d+)?)(?<HouseNumberAddition>[A-Za-z])?)?\s*$")]
    private static partial Regex StreetRegex();
}
