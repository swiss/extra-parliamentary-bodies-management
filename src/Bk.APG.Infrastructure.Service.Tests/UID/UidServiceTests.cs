using Bk.APG.Business.Services;
using Bk.APG.Infrastructure.Service.UID.Configuration;
using Bk.APG.Infrastructure.Service.UID.PublicServices;
using Bk.APG.Infrastructure.Service.UID.Service;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Options;

namespace Bk.APG.Infrastructure.Service.Tests.UID;

[TestFixture]
internal class UidServiceTests
{
    private readonly IPublicServices _publicServices = Substitute.For<IPublicServices>();
    private readonly IMasterDataService _masterDataService = Substitute.For<IMasterDataService>();
    private UidService _uidService = null!;
    private readonly IOptions<UidConfiguration> _uidConfiguration = Substitute.For<IOptions<UidConfiguration>>();
    private uidEntityPublicSearchRequest _request = null!;
    private searchConfiguration _config = null!;
    private uidEntitySearchResponse _response = null!;
    private uidEntitySearchResultItem _resultItem = null!;
    private List<uidEntitySearchResultItem> _resultItems = null!;
    private readonly Guid _legalFormId = Guid.NewGuid();
    private readonly UidConfiguration _uidConfigurationValues = new()
    {
        Url = "https://www.uid-wse-a.admin.ch/V5.0/PublicServices.svc",
        MinimalMatchQuality = 95,
        Username = "username",
        Password = "password"
    };

    [SetUp]
    public void SetUp()
    {
        _uidConfiguration.Value.Returns(_uidConfigurationValues);

        _uidService = new UidService(_publicServices, _masterDataService, _uidConfiguration);

        _request = new uidEntityPublicSearchRequest
        {
            Item = new uidEntityPublicSearchParameters
            {
                organisationName = "searchString"
            }
        };

        _config = new searchConfiguration
        {
            searchMode = searchMode.Auto,
            maxNumberOfRecords = 100,
            searchNameAndAddressHistory = false
        };

        _masterDataService.GetLegalFormGuidByLegalFormId(Arg.Any<string>()).Returns(_legalFormId);
        _masterDataService.GetLegalFormTextByLegalFormId(Arg.Any<string>()).Returns("LegalFormText");
    }

    [TearDown]
    public void TearDown()
    {
        _publicServices.ClearSubstitute();
        _masterDataService.ClearSubstitute();
    }

    [Test]
    public async Task CheckHealth_WithNotSuccessStatusCode_ShouldReturnUnhealthy()
    {
        _response = new uidEntitySearchResponse();

        _publicServices.SearchAsync(_request, _config).Returns(_response);

        var status = await _uidService.CheckHealthAsync(null!);

        Assert.That(status.Status, Is.EqualTo(HealthStatus.Unhealthy));
    }

    [Test]
    public async Task CheckHealth_WithSuccessStatusCode_ShouldReturnHealthy()
    {
        _response = new uidEntitySearchResponse();
        _resultItem = new uidEntitySearchResultItem
        {
            rating = 99
        };
        _resultItems = new List<uidEntitySearchResultItem>();
        _resultItems.Add(_resultItem);
        _response.uidEntitySearchResultItem = _resultItems.ToArray();

        _publicServices.SearchAsync(Arg.Any<uidEntityPublicSearchRequest>(), Arg.Any<searchConfiguration>()).Returns(_response);

        var status = await _uidService.CheckHealthAsync(null!);

        Assert.That(status.Status, Is.EqualTo(HealthStatus.Healthy));
    }

    [Test]
    public async Task Search_WithMatches_ShouldReturnList()
    {
        var searchString = "searchString";
        _response = new uidEntitySearchResponse();

        _resultItems = new List<uidEntitySearchResultItem>();

        _resultItem = new uidEntitySearchResultItem
        {
            rating = 100
        };

        var organisation = new organisationType();
        var organisation1 = new organisationType1();
        var uidRegInformation = new uidregInformationType();
        var identificationType = new organisationIdentificationType();
        var uid = new uidStructureType();
        var addresses = new List<organisationAddressType>();
        var address = new organisationAddressType
        {
            Items = new[] { "5012" },
            town = "Town"
        };
        addresses.Add(address);
        uid.uidOrganisationId = "MyId";
        uidRegInformation.uidregStatusEnterpriseDetail = uidregStatusEnterpriseDetailType.Item3;
        identificationType.organisationName = "Test AG";
        identificationType.uid = uid;
        identificationType.legalForm = "0103";
        organisation1.organisationIdentification = identificationType;
        organisation1.address = addresses.ToArray();
        organisation.organisation = organisation1;
        organisation.uidregInformation = uidRegInformation;

        _resultItem.organisation = organisation;
        _resultItems.Add(_resultItem);

        _resultItem = new uidEntitySearchResultItem
        {
            rating = 99,
            organisation = organisation
        };
        _resultItems.Add(_resultItem);

        _resultItem = new uidEntitySearchResultItem
        {
            rating = 92
        };
        _resultItems.Add(_resultItem);

        _resultItem = new uidEntitySearchResultItem
        {
            rating = 21
        };
        _resultItems.Add(_resultItem);

        _response.uidEntitySearchResultItem = _resultItems.ToArray();

        _publicServices.SearchAsync(Arg.Any<uidEntityPublicSearchRequest>(), Arg.Any<searchConfiguration>()).Returns(_response);

        var uidResults = await _uidService.Search(searchString);

        Assert.That(uidResults.Count(), Is.EqualTo(2));

        Assert.That(uidResults.First().UidOrganisationId, Is.EqualTo("MyId"));
        Assert.That(uidResults.First().OrganizationName, Is.EqualTo("Test AG"));
        Assert.That(uidResults.First().MatchQuality, Is.EqualTo(100));
        Assert.That(uidResults.First().Zip, Is.EqualTo("5012"));
        Assert.That(uidResults.First().City, Is.EqualTo("Town"));
        Assert.That(uidResults.First().LegalFormId, Is.EqualTo(_legalFormId));
        Assert.That(uidResults.First().LegalFormText, Is.EqualTo("LegalFormText"));

        Assert.That(uidResults.Last().MatchQuality, Is.EqualTo(99));
    }

    [Test]
    public async Task Search_WithNonMatchingLegalForm_ShouldReturnNoHits()
    {
        var searchString = "searchString";
        _response = new uidEntitySearchResponse();

        _resultItems = new List<uidEntitySearchResultItem>();

        _resultItem = new uidEntitySearchResultItem
        {
            rating = 100
        };

        var organisation = new organisationType();
        var organisation1 = new organisationType1();
        var identificationType = new organisationIdentificationType();
        var uid = new uidStructureType();
        var addresses = new List<organisationAddressType>();
        var address = new organisationAddressType
        {
            Items = new[] { "5012" },
            town = "Town"
        };
        addresses.Add(address);
        uid.uidOrganisationId = "MyId";
        identificationType.organisationName = "Test AG";
        identificationType.uid = uid;
        identificationType.legalForm = "0101";
        organisation1.organisationIdentification = identificationType;
        organisation1.address = addresses.ToArray();
        organisation.organisation = organisation1;

        _resultItem.organisation = organisation;
        _resultItems.Add(_resultItem);

        _resultItem = new uidEntitySearchResultItem
        {
            rating = 99,
            organisation = organisation
        };
        _resultItems.Add(_resultItem);

        _response.uidEntitySearchResultItem = _resultItems.ToArray();

        _publicServices.SearchAsync(Arg.Any<uidEntityPublicSearchRequest>(), Arg.Any<searchConfiguration>()).Returns(_response);

        var uidResults = await _uidService.Search(searchString);

        Assert.That(uidResults.Count(), Is.EqualTo(0));
    }

    [Test]
    public async Task Search_WithNonMatchingState_ShouldReturnNoHits()
    {
        var searchString = "searchString";
        _response = new uidEntitySearchResponse();

        _resultItems = new List<uidEntitySearchResultItem>();

        _resultItem = new uidEntitySearchResultItem
        {
            rating = 100
        };

        var organisation = new organisationType();
        var organisation1 = new organisationType1();
        var uidRegInformation = new uidregInformationType();
        var identificationType = new organisationIdentificationType();
        var uid = new uidStructureType();
        var addresses = new List<organisationAddressType>();
        var address = new organisationAddressType
        {
            Items = new[] { "5012" },
            town = "Town"
        };
        addresses.Add(address);
        uid.uidOrganisationId = "MyId";
        uidRegInformation.uidregStatusEnterpriseDetail = uidregStatusEnterpriseDetailType.Item1;
        identificationType.organisationName = "Test AG";
        identificationType.uid = uid;
        identificationType.legalForm = "0103";
        organisation1.organisationIdentification = identificationType;
        organisation1.address = addresses.ToArray();
        organisation.organisation = organisation1;
        organisation.uidregInformation = uidRegInformation;

        _resultItem.organisation = organisation;
        _resultItems.Add(_resultItem);

        _resultItem = new uidEntitySearchResultItem
        {
            rating = 99,
            organisation = organisation
        };
        _resultItems.Add(_resultItem);

        _response.uidEntitySearchResultItem = _resultItems.ToArray();

        _publicServices.SearchAsync(Arg.Any<uidEntityPublicSearchRequest>(), Arg.Any<searchConfiguration>()).Returns(_response);

        var uidResults = await _uidService.Search(searchString);

        Assert.That(uidResults.Count(), Is.EqualTo(0));
    }
}
