using Bk.APG.Business.Connections;
using Bk.APG.Business.Models;
using Bk.APG.Business.Repositories;
using Bk.APG.Business.Services;
using Bk.APG.CrossCutting.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using Swiss.FCh.Cube.Dimension.Contract;
using Swiss.FCh.Cube.Dimension.Model;
using Swiss.FCh.Cube.RawData.Contract;
using Swiss.FCh.Cube.RawData.Model;
using VDS.RDF;
using VDS.RDF.Storage;
using Options = Microsoft.Extensions.Options.Options;

namespace Bk.APG.Business.Tests.Services;

[TestFixture]
internal class OgdExportServiceTests
{
    private OgdExportService _ogdExportService;

    private IAsyncStorageProvider _storageProvider;
    private IDimensionService _dimensionService;
    private ICubeRawDataService _cubeRawDataService;
    private IMembershipService _membershipService;
    private ICommitteeService _committeeService;
    private IPersonRepository _personRepository;
    private ICommitteeRepository _committeeRepository;
    private ICommitteeTypeRepository _committeeTypeRepository;
    private IMembershipRepository _membershipRepository;
    private ILogger<OgdExportService> _logger;
    private IOptions<SparqlOptions> _sparqlOptions;
    private IConnectionFactory _connectionFactory;
    private IMasterDataRepository _masterDataRepository;
    private IContactPointRepository _contactPointRepository;
    private IInterestRepository _interestRepository;

    private readonly Committee _committee = new()
    {
        Id = new Guid("1b062ebf-bad4-4f3b-b87c-c4c7953ea92d"),
        DescriptionGerman = "",
        DescriptionFrench = "",
        DescriptionItalian = "",
        DescriptionRomansh = "",
        TermOfOfficeDateId = Guid.NewGuid(),
        LinkHomepageGerman = "https://www.de.ch",
        LinkHomepageFrench = "https://www.fr.ch",
        LinkHomepageItalian = "https://www.it.ch",
        LinkHomepageRomansh = "https://www.rm.ch",
        Created = default,
        CreatedBy = "",
        Modified = default,
        ModifiedBy = "",
        IsDeleted = false
    };

    private readonly Person _person = new()
    {
        Id = new Guid("3621b683-0643-49d9-8771-b8ba8c787f4d"),
        Surname = "Müller",
        GivenName = "Peter",
        BirthYear = 1980,
        CorrespondenceAddressId = default,
        Created = default,
        CreatedBy = "",
        Modified = default,
        ModifiedBy = ""
    };

    private readonly Membership _membership = new()
    {
        Id = new Guid("4af0febb-91aa-4f01-80f0-d038eafb5efe"),
        InCorrelationWithFederalDuty = false,
        IsDeleted = false,
        Created = default,
        CreatedBy = "",
        Modified = default,
        ModifiedBy = "",
        PersonId = new Guid("3621b683-0643-49d9-8771-b8ba8c787f4d"),
        Committee = new()
        {
            Id = new("1b062ebf-bad4-4f3b-b87c-c4c7953ea92d"),
            DescriptionGerman = "",
            DescriptionFrench = "",
            DescriptionItalian = "",
            DescriptionRomansh = "",
            TermOfOfficeDateId = default,
            IsDeleted = false,
            Created = default,
            CreatedBy = "",
            Modified = default,
            ModifiedBy = "",
        }
    };

    private readonly Interest _interest = new()
    {
        Id = new Guid("9c0b1b43-8c3f-4b8c-b1a0-6cdb82fa0b7f"),
        IsDeleted = false,
        Created = default,
        CreatedBy = "",
        Modified = default,
        ModifiedBy = "",
        PersonId = new Guid("3621b683-0643-49d9-8771-b8ba8c787f4d"),
        InterestFunctionId = new Guid("f3b1bde4-4f59-4c8c-9232-4e5f321aa9b1"),
        InterestCommitteeId = new Guid("1baf47e9-93a2-4e22-b0c7-b73cf5426b3f"),
        LegalFormId = new Guid("da8ef4c4-8a67-4b19-b6df-37cb47891d4f"),
        Text = "Interest",
        InterestText = "InterestText",
        BeginDate = DateOnly.FromDateTime(DateTime.Now.AddDays(-100)),
    };

    [SetUp]
    public void SetUp()
    {
        _storageProvider = Substitute.For<IAsyncStorageProvider>();
        _dimensionService = Substitute.For<IDimensionService>();
        _cubeRawDataService = Substitute.For<ICubeRawDataService>();
        _committeeService = Substitute.For<ICommitteeService>();
        _membershipService = Substitute.For<IMembershipService>();
        _personRepository = Substitute.For<IPersonRepository>();
        _committeeRepository = Substitute.For<ICommitteeRepository>();
        _committeeTypeRepository = Substitute.For<ICommitteeTypeRepository>();
        _membershipRepository = Substitute.For<IMembershipRepository>();
        _connectionFactory = Substitute.For<IConnectionFactory>();
        _masterDataRepository = Substitute.For<IMasterDataRepository>();
        _contactPointRepository = Substitute.For<IContactPointRepository>();
        _interestRepository = Substitute.For<IInterestRepository>();
        _logger = new NullLogger<OgdExportService>();

        _connectionFactory.Create().Returns(_storageProvider);

        var sparqlOptions = new SparqlOptions
        {
            RequestTimeoutMs = 0,
            Endpoint = "",
            MasterDataProxy = new ProxyOptions { UseProxy = false },
            ExportProxy = new ProxyOptions { UseProxy = false },
            ExportGraphName = "ExportGraphName",
            ExportGraphBaseUri = "http://example.base.uri.org",
            ExportGraphVersion = "1",
            ExportEnabled = true
        };

        _sparqlOptions = Substitute.For<IOptions<SparqlOptions>>();
        _sparqlOptions.Value.Returns(sparqlOptions);

        var ogdS3Configuration = new OgdS3Configuration
        {
            BaseUrl = "Foo",
            bucket = "Foo",
            access_key = "Foo",
            s3_endpoint = "Foo",
            secret_access_key = "Foo"
        };

        _ogdExportService =
            new(
                _dimensionService,
                _cubeRawDataService,
                _personRepository,
                _membershipService,
                _committeeService,
                _committeeRepository,
                _committeeTypeRepository,
                _connectionFactory,
                _membershipRepository,
                _interestRepository,
                _logger,
                _sparqlOptions,
                _masterDataRepository,
                _contactPointRepository,
                Substitute.For<IDocumentService>(),
                Substitute.For<IOgdDocumentService>(),
                Options.Create(ogdS3Configuration));
    }

    [TearDown]
    public void TearDown()
    {
        _storageProvider.Dispose();
    }

    [Test]
    public async Task Export_WhenCalled_WiresEverythingTogetherCorrectly()
    {
        _committeeRepository.GetAll().Returns([_committee]);
        _personRepository.GetAll().Returns([_person]);
        _membershipRepository.GetAllActiveForOgdExport().Returns([_membership]);
        _interestRepository.GetAll().Returns([_interest]);

        await _ogdExportService.Export(CancellationToken.None);

        _dimensionService.Received().CreateTriples(Arg.Any<IEnumerable<DimensionItem>>(), Arg.Any<Graph>(), "http://example.base.uri.org/person", Arg.Any<IList<Literal>>(), null, Arg.Any<IList<string>>());
        _dimensionService.Received().CreateTriples(Arg.Any<IEnumerable<DimensionItem>>(), Arg.Any<Graph>(), "http://example.base.uri.org/committee", Arg.Any<IList<Literal>>());
        _dimensionService.Received().CreateTriples(Arg.Any<IEnumerable<DimensionItem>>(), Arg.Any<Graph>(), "http://example.base.uri.org/appointment-decision", Arg.Any<IList<Literal>>(), rdfTypes: Arg.Is<IList<string>>(x => x.Single() == "http://schema.org/DigitalDocument"));

        _cubeRawDataService.Received(1).CreateTriples(Arg.Any<Graph>(), "membership:1", Arg.Any<IEnumerable<ObservationDataRow>>());
        _cubeRawDataService.Received(1).CreateTriples(Arg.Any<Graph>(), "vested-interest:1", Arg.Any<IEnumerable<ObservationDataRow>>());

        await _storageProvider.Received().DeleteGraphAsync("ExportGraphName", CancellationToken.None);
        await _storageProvider.Received().UpdateGraphAsync("ExportGraphName", Arg.Any<IEnumerable<Triple>>(), [], CancellationToken.None);
    }
}
