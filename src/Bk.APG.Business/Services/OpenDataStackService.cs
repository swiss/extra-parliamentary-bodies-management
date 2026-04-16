using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json.Serialization;
using Bk.APG.CrossCutting.Configuration;
using Microsoft.Extensions.Options;

namespace Bk.APG.Business.Services;

public class OpenDataStackService : IOpenDataStackService
{
#pragma warning disable CA1812 // Instantiated by System.Text.Json during deserialization
    private sealed record ExchangeTokenResponse([property: JsonPropertyName("code")] string Code);
#pragma warning restore CA1812

    private readonly IHttpClientFactory _httpClientFactory;
    private readonly OpenDataStackOptions _openDataStackOptions;

    public OpenDataStackService(IHttpClientFactory httpClientFactory, IOptions<OpenDataStackOptions> openDataStackOptions)
    {
        ArgumentNullException.ThrowIfNull(httpClientFactory);
        ArgumentNullException.ThrowIfNull(openDataStackOptions);

        _httpClientFactory = httpClientFactory;
        _openDataStackOptions = openDataStackOptions.Value;
    }

    public async Task<string> ExchangeToken(string accessToken)
    {
        using var client = _httpClientFactory.CreateClient();

        client.BaseAddress = new Uri(_openDataStackOptions.BaseUrl);
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

        var response = await client.PostAsync(_openDataStackOptions.TokenEndpoint, null);

        response.EnsureSuccessStatusCode();

        var result = await response.Content.ReadFromJsonAsync<ExchangeTokenResponse>();

        return string.IsNullOrWhiteSpace(result?.Code)
            ? throw new InvalidOperationException("Failed to exchange token: code is null or empty.")
            : result.Code;
    }
}
