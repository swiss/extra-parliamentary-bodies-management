using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Web;
using Bk.APG.Business.Dtos;
using Bk.APG.Business.Repositories;
using Bk.APG.Business.Services;
using Bk.APG.CrossCutting.Configuration;
using Bk.APG.CrossCutting.Exception;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Options;

namespace Bk.APG.Infrastructure.Service.Post;

[System.Diagnostics.CodeAnalysis.SuppressMessage("Reliability", "CA2000:Dispose objects before losing scope",
    Justification = "HttpClient from IHttpClientFactory should not be disposed - factory manages lifetime")]
public class PostService : IPostService, IHealthCheck
{
    private readonly PostConfiguration _postConfiguration;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ICantonRepository _cantonRepository;

    public PostService(IOptions<PostConfiguration> postOptions, IHttpClientFactory httpClientFactory, ICantonRepository cantonRepository)
    {
        ArgumentNullException.ThrowIfNull(postOptions);

        _httpClientFactory = httpClientFactory;
        _cantonRepository = cantonRepository;
        _postConfiguration = postOptions.Value;
    }

    public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = new())
    {
        try
        {
            var client = CreateHttpClient();
            var response = await client.GetAsync($"{_postConfiguration.Uri}/ping", cancellationToken);
            if (response.IsSuccessStatusCode)
            {
                return HealthCheckResult.Healthy("Post API connection is healthy");
            }

            return HealthCheckResult.Unhealthy("Unable to connect to Post API");
        }
        catch (Exception e)
        {
            return HealthCheckResult.Unhealthy("An exception occurred while checking Post API connection", e);
        }
    }

    public async Task<IEnumerable<AddressDto>> Search(AddressSearchDto search)
    {
        ArgumentNullException.ThrowIfNull(search);

        var client = CreateHttpClient();

        var dto = AddressMapper.ToAutoCompleteDto(search);
        var json = JsonSerializer.Serialize(dto);
        using var content = new StringContent(json, Encoding.UTF8, "application/json");

        var response = await client.PostAsync($"{_postConfiguration.Uri}/autocomplete4", content);
        if (!response.IsSuccessStatusCode)
        {
            throw new PostApiException($"Error during communication with Post API. URL={response.RequestMessage?.RequestUri} - StatusCode={response.StatusCode}");
        }

        var result = await ReadResponse<AddressAutocompleteResponseDto>(response);

        var cantons = await _cantonRepository.GetAll();
        return result.QueryResult.Results.Select(item => AddressMapper.ToAddressDto(item, cantons));
    }

    public async Task<Business.Dtos.AddressVerificationResultDto> Verify(AddressSearchDto search)
    {
        ArgumentNullException.ThrowIfNull(search);

        var client = CreateHttpClient();

        var uriBuilder = new UriBuilder($"{_postConfiguration.Uri}/buildingverification4");

        var query = HttpUtility.ParseQueryString(uriBuilder.Query);
        if (!string.IsNullOrWhiteSpace(search.Street))
        {
            var (streetName, houseNumber, houseNumberAddition) = AddressMapper.GetStreet(search.Street);
            query["streetname"] = streetName;
            if (!string.IsNullOrWhiteSpace(houseNumber))
            {
                query["houseNo"] = houseNumber;

                if (!string.IsNullOrWhiteSpace(houseNumberAddition))
                {
                    query["houseNoAddition"] = houseNumberAddition;
                }
            }
        }

        if (!string.IsNullOrWhiteSpace(search.Zip))
        {
            query["zipcode"] = search.Zip;
        }

        if (!string.IsNullOrWhiteSpace(search.City))
        {
            query["townname"] = search.City;
        }

        uriBuilder.Query = query.ToString();

        var response = await client.GetAsync(uriBuilder.Uri);
        if (!response.IsSuccessStatusCode)
        {
            throw new PostApiException($"Error during communication with Post API. URL={response.RequestMessage?.RequestUri} - StatusCode={response.StatusCode}");
        }

        var result = await ReadResponse<AddressVerificationResultDto>(response);

        // TODO PP don't do this every time!
        var cantons = await _cantonRepository.GetAll();

        var status = AddressMapper.ToAddressVerificationStatus(result.QueryResult.Data);
        AddressDto? addressDto = null;
        if (status is AddressVerificationStatus.Ok or AddressVerificationStatus.Corrected)
        {
            addressDto = AddressMapper.ToAddressDto(result.QueryResult.Data, cantons);
        }

        var address = new Business.Dtos.AddressVerificationResultDto
        {
            Address = addressDto,
            Status = status
        };

        return address;
    }

    private HttpClient CreateHttpClient()
    {
        var client = _httpClientFactory.CreateClient();

        var credentials = Convert.ToBase64String(Encoding.ASCII.GetBytes($"{_postConfiguration.Username}:{_postConfiguration.Password}"));
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", credentials);
        client.DefaultRequestHeaders.UserAgent.ParseAdd("APG.Api");
        client.DefaultRequestHeaders.Accept.ParseAdd("application/json");

        return client;
    }

    private static async Task<T> ReadResponse<T>(HttpResponseMessage response) where T : class
    {
        var bytes = await response.Content.ReadAsByteArrayAsync();
        var json = Encoding.UTF8.GetString(bytes);
        var data = JsonSerializer.Deserialize<T>(json);
        return data!;
    }
}
