using System.Text.Json.Serialization;

namespace Bk.APG.Infrastructure.Service.Post;

// https://developer.post.ch/en/address-web-services-rest#
internal class AddressAutocompleteRequestDto
{
    [JsonPropertyName("request")]
    public required AddressRequestDto Request { get; init; }
    [JsonPropertyName("zipOrderMode")]
    public int ZipOrderMode { get; init; } = 0;
    [JsonPropertyName("zipFilterMode")]
    public int ZipFilterMode { get; init; } = 0;
}

internal class AddressRequestDto
{
    [JsonPropertyName("ONRP")]
    public int ONrP { get; set; } = 0;
    [JsonPropertyName("ZipCode")]
    public string ZipCode { get; set; } = string.Empty;
    [JsonPropertyName("ZipAddition")]
    public string ZipAddition { get; set; } = string.Empty;
    [JsonPropertyName("TownName")]
    public string TownName { get; set; } = string.Empty;
    [JsonPropertyName("STRID")]
    public int StreetId { get; set; } = 0;
    [JsonPropertyName("StreetName")]
    public string StreetName { get; set; } = string.Empty;
    [JsonPropertyName("HouseKey")]
    public int HouseKey { get; set; } = 0;
    [JsonPropertyName("HouseNo")]
    public string HouseNo { get; set; } = string.Empty;
    [JsonPropertyName("HouseNoAddition")]
    public string HouseNoAddition { get; set; } = string.Empty;
}
