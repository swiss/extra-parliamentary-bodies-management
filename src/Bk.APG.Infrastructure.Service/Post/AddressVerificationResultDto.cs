using System.Text.Json.Serialization;

namespace Bk.APG.Infrastructure.Service.Post;

// https://developer.post.ch/en/address-web-services-rest#
internal class AddressVerificationResultDto
{
    [JsonPropertyName("QueryBuildingVerification4Result")]
    public required AddressVerificationQueryResultDto QueryResult { get; init; }

}

internal class AddressVerificationQueryResultDto
{
    [JsonPropertyName("BuildingVerificationData")]
    public required BuildingVerificationDataDto Data { get; init; }
}

internal class BuildingVerificationDataDto
{
    [JsonPropertyName("Canton")]
    public string Canton { get; init; } = null!;
    [JsonPropertyName("CountryCode")]
    public string CountryCode { get; init; } = null!;
    [JsonPropertyName("HouseKey")]
    public string HouseKey { get; init; } = null!;
    [JsonPropertyName("HouseNo")]
    public string HouseNo { get; init; } = null!;
    [JsonPropertyName("HouseNoAddition")]
    public string HouseNoAddition { get; init; } = null!;
    [JsonPropertyName("ONRP")]
    public string ONrP { get; init; } = null!;
    [JsonPropertyName("PSTAT")]
    public string Status { get; init; } = null!;
    [JsonPropertyName("STRID")]
    public string StreetId { get; init; } = null!;
    [JsonPropertyName("StreetName")]
    public string StreetName { get; init; } = null!;
    [JsonPropertyName("TownName")]
    public string TownName { get; init; } = null!;
    [JsonPropertyName("ZipAddition")]
    public string ZipAddition { get; init; } = null!;
    [JsonPropertyName("ZipCode")]
    public string ZipCode { get; init; } = null!;
}
