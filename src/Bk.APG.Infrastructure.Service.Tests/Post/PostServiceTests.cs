using System.Net;
using Bk.APG.Business.Dtos;
using Bk.APG.Business.Repositories;
using Bk.APG.CrossCutting.Configuration;
using Bk.APG.CrossCutting.Exception;
using Bk.APG.Infrastructure.Service.Post;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Options;
using RichardSzalay.MockHttp;

namespace Bk.APG.Infrastructure.Service.Tests.Post;

[TestFixture]
internal class PostServiceTests
{
    private readonly IOptions<PostConfiguration> _postConfigurationOptions = Substitute.For<IOptions<PostConfiguration>>();
    private readonly PostConfiguration _postConfiguration = new() { Uri = "https://post.ch/api", Username = "test_username", Password = "test_password" };
    private readonly IHttpClientFactory _httpClientFactory = Substitute.For<IHttpClientFactory>();
    private readonly ICantonRepository _cantonRepository = Substitute.For<ICantonRepository>();
    private MockHttpMessageHandler _mockHttpMessageHandler = null!;

    private PostService _postService = null!;

    [SetUp]
    public void SetUp()
    {
        _postConfigurationOptions.Value.Returns(_postConfiguration);

        _mockHttpMessageHandler = new MockHttpMessageHandler();
        var client = _mockHttpMessageHandler.ToHttpClient();
        using var clientFactory = _httpClientFactory.CreateClient();
        clientFactory.Returns(client);

        _postService = new PostService(_postConfigurationOptions, _httpClientFactory, _cantonRepository);
    }

    [TearDown]
    public void TearDown()
    {
        _cantonRepository.ClearSubstitute();
        _httpClientFactory.ClearSubstitute();
        _postConfigurationOptions.ClearSubstitute();
        _mockHttpMessageHandler.Dispose();
    }

    [Test]
    public async Task CheckHealth_WithRequestThrowing_ShouldReturnUnhealthy()
    {
        _mockHttpMessageHandler.When("https://post.ch/api/ping")
            .Throw(new HttpRequestException());

        var status = await _postService.CheckHealthAsync(null!);

        Assert.That(status.Status, Is.EqualTo(HealthStatus.Unhealthy));
    }

    [Test]
    public async Task CheckHealth_WithNotSuccessStatusCode_ShouldReturnUnhealthy()
    {
        _mockHttpMessageHandler.When("https://post.ch/api/ping")
            .Respond(HttpStatusCode.InternalServerError);

        var status = await _postService.CheckHealthAsync(null!);

        Assert.That(status.Status, Is.EqualTo(HealthStatus.Unhealthy));
    }

    [Test]
    public async Task CheckHealth_WithSuccessStatusCode_ShouldReturnHealthy()
    {
        _mockHttpMessageHandler.When("https://post.ch/api/ping")
            .Respond(HttpStatusCode.OK);

        var status = await _postService.CheckHealthAsync(null!);

        Assert.That(status.Status, Is.EqualTo(HealthStatus.Healthy));
    }

    [Test]
    public void Search_WithNotSuccessStatusCode_ShouldThrowPostApiException()
    {
        _mockHttpMessageHandler.When("https://post.ch/api/autocomplete4")
            .Respond(HttpStatusCode.InternalServerError);

        Assert.ThrowsAsync<PostApiException>(async () => await _postService.Search(new AddressSearchDto()));
    }

    [Test]
    public async Task Search_WithSuccessStatusCode_ShouldReturnMappedResult()
    {
        const string jsonResponse = "{\n  \"QueryAutoComplete4Result\": {\n    \"AutoCompleteResult\": [\n      {\n        \"Canton\": \"ZH\",\n        \"CountryCode\": \"CH\",\n        \"HouseKey\": \"58126513\",\n        \"HouseNo\": \"2\",\n        \"HouseNoAddition\": \"C\",\n        \"ONRP\": \"4665\",\n        \"STRID\": \"67264\",\n        \"StreetName\": \"Sonnenhofstrasse\",\n        \"TownName\": \"Hinwil\",\n        \"ZipAddition\": \"00\",\n        \"ZipCode\": \"8340\"\n      },\n      {\n        \"Canton\": \"AG\",\n        \"CountryCode\": \"CH\",\n        \"HouseKey\": \"76201152\",\n        \"HouseNo\": \"2\",\n        \"HouseNoAddition\": \"C\",\n        \"ONRP\": \"3119\",\n        \"STRID\": \"76107230\",\n        \"StreetName\": \"Sonnenhofstrasse\",\n        \"TownName\": \"Zufikon\",\n        \"ZipAddition\": \"00\",\n        \"ZipCode\": \"5621\"\n      }\n    ],\n    \"Status\": 0\n  }\n}";
        using var stringContent = new StringContent(jsonResponse);
        _mockHttpMessageHandler.When("https://post.ch/api/autocomplete4")
            .Respond(HttpStatusCode.OK, stringContent);
        _cantonRepository.GetAll().Returns([]);

        var result = await _postService.Search(new AddressSearchDto());

        Assert.That(result, Is.Not.Null);
        Assert.That(result.Count(), Is.EqualTo(2));

        await _cantonRepository.Received(1).GetAll();
    }

    [Test]
    public async Task Verify_WithValidQueryParameters_ShouldReturnMappedResult()
    {
        const string jsonResponse = "{\n  \"QueryBuildingVerification4Result\": {\n    \"BuildingVerificationData\": {\n      \"Canton\": \"AG\",\n      \"CountryCode\": \"CH\",\n      \"HouseKey\": \"76201152\",\n      \"HouseNo\": \"123\",\n      \"HouseNoAddition\": \"B\",\n      \"ONRP\": \"3119\",\n      \"PSTAT\": \"1\",\n      \"STRID\": \"76107230\",\n      \"StreetName\": \"Test Street\",\n      \"TownName\": \"Test City\",\n      \"ZipAddition\": \"00\",\n      \"ZipCode\": \"12345\"\n    },\n    \"Status\": 0\n  }\n}";
        var search = new AddressSearchDto
        {
            Street = "Test Street 123B",
            Zip = "12345",
            City = "Test City"
        };
        using var stringContent = new StringContent(jsonResponse);
        _mockHttpMessageHandler.When($"{_postConfiguration.Uri}/buildingverification4*")
            .WithQueryString("streetname", "Test Street")
            .WithQueryString("houseNo", "123")
            .WithQueryString("houseNoAddition", "B")
            .WithQueryString("zipcode", "12345")
            .WithQueryString("townname", "Test City")
            .Respond(HttpStatusCode.OK, stringContent);
        _cantonRepository.GetAll().Returns([]);

        var result = await _postService.Verify(search);

        Assert.That(result, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(result.Status, Is.EqualTo(AddressVerificationStatus.Ok));
            Assert.That(result.Address, Is.Not.Null);
            Assert.That(result.Address!.Street, Is.EqualTo("Test Street 123B"));
            Assert.That(result.Address.City, Is.EqualTo("Test City"));
            Assert.That(result.Address.Zip, Is.EqualTo("12345"));
        });

        await _cantonRepository.Received(1).GetAll();
    }

    [Test]
    public void Verify_WithNotSuccessStatusCode_ShouldThrowPostApiException()
    {
        _mockHttpMessageHandler.When("https://post.ch/api/buildingverification4*")
            .Respond(HttpStatusCode.InternalServerError);

        Assert.ThrowsAsync<PostApiException>(async () => await _postService.Verify(new AddressSearchDto()));
    }
}
