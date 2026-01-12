using Bk.APG.Business.Connections;
using Bk.APG.Business.Mapper;
using Bk.APG.Business.Models;
using Bk.APG.Business.Repositories;
using Bk.APG.CrossCutting;
using Bk.APG.CrossCutting.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Swiss.FCh.Cube.Dimension.Contract;
using Swiss.FCh.Cube.Dimension.Model;
using Swiss.FCh.Cube.RawData.Contract;
using VDS.RDF;
using VDS.RDF.Parsing;
using VDS.RDF.Storage;

namespace Bk.APG.Business.Services;

public class OgdExportService
{
    private readonly IAsyncStorageProvider _storageProvider;
    private readonly IDimensionService _dimensionService;
    private readonly ICubeRawDataService _cubeRawDataService;
    private readonly IMembershipService _membershipService;
    private readonly IPersonRepository _personRepository;
    private readonly ICommitteeRepository _committeeRepository;
    private readonly ICommitteeTypeRepository _committeeTypeRepository;
    private readonly IMembershipRepository _membershipRepository;
    private readonly IMasterDataRepository _masterDataRepository;
    private readonly IContactPointRepository _contactPointRepository;
    private readonly IInterestRepository _interestRepository;
    private readonly IDocumentService _documentService;
    private readonly IOgdDocumentService _ogdDocumentService;
    private readonly ILogger<OgdExportService> _logger;
    private readonly SparqlOptions _sparqlOptions;
    private readonly OgdS3Configuration _ogdS3Configuration;

    public OgdExportService(
        IDimensionService dimensionService,
        ICubeRawDataService cubeRawDataService,
        IPersonRepository personRepository,
        IMembershipService membershipService,
        ICommitteeRepository committeeRepository,
        ICommitteeTypeRepository committeeTypeRepository,
        IConnectionFactory connectionFactory,
        IMembershipRepository membershipRepository,
        IInterestRepository interestRepository,
        ILogger<OgdExportService> logger,
        IOptions<SparqlOptions> sparqlOptions,
        IMasterDataRepository masterDataRepository,
        IContactPointRepository contactPointRepository,
        IDocumentService documentService,
        IOgdDocumentService ogdDocumentService,
        IOptions<OgdS3Configuration> ogdS3Options)
    {
        _storageProvider = connectionFactory.Create();
        _dimensionService = dimensionService;
        _membershipService = membershipService;
        _cubeRawDataService = cubeRawDataService;
        _personRepository = personRepository;
        _committeeRepository = committeeRepository;
        _committeeTypeRepository = committeeTypeRepository;
        _membershipRepository = membershipRepository;
        _interestRepository = interestRepository;
        _logger = logger;
        _masterDataRepository = masterDataRepository;
        _contactPointRepository = contactPointRepository;
        _documentService = documentService;
        _ogdDocumentService = ogdDocumentService;
        _sparqlOptions = sparqlOptions.Value;
        _ogdS3Configuration = ogdS3Options.Value;
    }

    public async Task Export(CancellationToken ct = default)
    {
        if (!_sparqlOptions.ExportEnabled)
        {
            _logger.LogInformation("OGD export is disabled, exiting");
            return;
        }

        _logger.LogInformation("OGD export starting");

        using var graph = new Graph();
        graph.BaseUri = new Uri(_sparqlOptions.ExportGraphBaseUri);

        graph.NamespaceMap.AddNamespace(OgdExportConstants.NamespaceApg, new Uri($"{_sparqlOptions.ExportGraphBaseUri}/"));
        graph.NamespaceMap.AddNamespace(OgdExportConstants.NamespaceContactPointType, new Uri($"{_sparqlOptions.ExportGraphBaseUri}/vocabulary/{OgdExportConstants.NamespaceContactPointType}/"));
        graph.NamespaceMap.AddNamespace(OgdExportConstants.NamespaceCommitteeType, new Uri($"{_sparqlOptions.ExportGraphBaseUri}/vocabulary/{OgdExportConstants.NamespaceCommitteeType}/"));
        graph.NamespaceMap.AddNamespace(OgdExportConstants.NamespaceFunction, new Uri($"{_sparqlOptions.ExportGraphBaseUri}/vocabulary/{OgdExportConstants.NamespaceFunction}/"));
        graph.NamespaceMap.AddNamespace(OgdExportConstants.NamespaceCanton, new Uri($"{_sparqlOptions.ExportGraphBaseUri}/vocabulary/{OgdExportConstants.NamespaceCanton}/"));
        graph.NamespaceMap.AddNamespace(OgdExportConstants.NamespaceInterestFunction, new Uri($"{_sparqlOptions.ExportGraphBaseUri}/vocabulary/{OgdExportConstants.NamespaceInterestFunction}/"));
        graph.NamespaceMap.AddNamespace(OgdExportConstants.NamespaceInterestCommittee, new Uri($"{_sparqlOptions.ExportGraphBaseUri}/vocabulary/{OgdExportConstants.NamespaceInterestCommittee}/"));
        graph.NamespaceMap.AddNamespace(OgdExportConstants.NamespaceOccupation, new Uri($"{_sparqlOptions.ExportGraphBaseUri}/vocabulary/{OgdExportConstants.NamespaceOccupation}/"));
        graph.NamespaceMap.AddNamespace(OgdExportConstants.NamespacePerson, new Uri($"{_sparqlOptions.ExportGraphBaseUri}/{OgdExportConstants.NamespacePerson}/"));
        graph.NamespaceMap.AddNamespace(OgdExportConstants.NamespaceCommittee, new Uri($"{_sparqlOptions.ExportGraphBaseUri}/{OgdExportConstants.NamespaceCommittee}/"));
        graph.NamespaceMap.AddNamespace(OgdExportConstants.NamespaceContactPoint, new Uri($"{_sparqlOptions.ExportGraphBaseUri}/{OgdExportConstants.NamespaceContactPoint}/"));
        graph.NamespaceMap.AddNamespace(OgdExportConstants.NamespaceMembership, new Uri($"{_sparqlOptions.ExportGraphBaseUri}/{OgdExportConstants.NamespaceMembership}/"));
        graph.NamespaceMap.AddNamespace(OgdExportConstants.NamespaceOrganization, new Uri($"{_sparqlOptions.ExportGraphBaseUri}/{OgdExportConstants.NamespaceOrganization}/"));
        graph.NamespaceMap.AddNamespace(OgdExportConstants.NamespaceAppointmentDecision, new Uri($"{_sparqlOptions.ExportGraphBaseUri}/{OgdExportConstants.NamespaceAppointmentDecision}/"));
        graph.NamespaceMap.AddNamespace(OgdExportConstants.NamespaceVestedInterest, new Uri($"{_sparqlOptions.ExportGraphBaseUri}/{OgdExportConstants.NamespaceVestedInterest}/"));
        graph.NamespaceMap.AddNamespace(OgdExportConstants.NamespaceCommitteeFunctionStatistic, new Uri($"{_sparqlOptions.ExportGraphBaseUri}/{OgdExportConstants.NamespaceCommitteeFunctionStatistic}/"));
        graph.NamespaceMap.AddNamespace(OgdExportConstants.NamespaceCommitteeCantonStatistic, new Uri($"{_sparqlOptions.ExportGraphBaseUri}/{OgdExportConstants.NamespaceCommitteeCantonStatistic}/"));
        graph.NamespaceMap.AddNamespace(OgdExportConstants.NamespaceCommitteeCantonDetailStatistic, new Uri($"{_sparqlOptions.ExportGraphBaseUri}/{OgdExportConstants.NamespaceCommitteeCantonDetailStatistic}/"));
        graph.NamespaceMap.AddNamespace(OgdExportConstants.NamespaceCommitteeGenderLanguageStatistic, new Uri($"{_sparqlOptions.ExportGraphBaseUri}/{OgdExportConstants.NamespaceCommitteeGenderLanguageStatistic}/"));
        graph.NamespaceMap.AddNamespace(OgdExportConstants.NamespaceLd, new Uri("https://ld.admin.ch/"));
        graph.NamespaceMap.AddNamespace(OgdExportConstants.NamespaceRld, new Uri("https://register.ld.admin.ch/"));
        graph.NamespaceMap.AddNamespace(OgdExportConstants.NamespaceSchema, new Uri("http://schema.org"));
        graph.NamespaceMap.AddNamespace(OgdExportConstants.NamespaceRdf, new Uri("http://www.w3.org/1999/02/22-rdf-syntax-ns#"));
        graph.NamespaceMap.AddNamespace(OgdExportConstants.NamespaceW3, new Uri("http://www.w3.org/"));
        graph.NamespaceMap.AddNamespace(OgdExportConstants.NamespaceCube, new Uri("https://cube.link/"));

        //TODO extract metadata triples
        List<Triple> membershipMetadataTriples =
        [
            new(
                graph.CreateUriNode(OgdExportConstants.UriMembership),
                graph.CreateUriNode(OgdExportConstants.SchemaPublisher),
                graph.CreateUriNode(OgdExportConstants.LdFCh)),
            new(
                graph.CreateUriNode(OgdExportConstants.UriMembership),
                graph.CreateUriNode(OgdExportConstants.SchemaCreator),
                graph.CreateUriNode(OgdExportConstants.LdFCh)),
            new(
                graph.CreateUriNode(OgdExportConstants.UriMembership),
                graph.CreateUriNode(OgdExportConstants.SchemaDateCreated),
                graph.CreateLiteralNode("2025-05-01", UriFactory.Create(XmlSpecsHelper.XmlSchemaDataTypeDate))),
            new(
                graph.CreateUriNode(OgdExportConstants.UriMembership),
                graph.CreateUriNode(OgdExportConstants.SchemaDatePublished),
                graph.CreateLiteralNode(OgdExportConstants.SchemaPublishedDate, UriFactory.Create(XmlSpecsHelper.XmlSchemaDataTypeDate))),
            new(
                graph.CreateUriNode(OgdExportConstants.UriMembership),
                graph.CreateUriNode(OgdExportConstants.SchemaDateModified),
                graph.CreateLiteralNode(DateTime.Now.ToString("yyyy-MM-dd"), UriFactory.Create(XmlSpecsHelper.XmlSchemaDataTypeDate))),
            new(
                graph.CreateUriNode(OgdExportConstants.UriMembership),
                graph.CreateUriNode(OgdExportConstants.SchemaName),
                graph.CreateLiteralNode("BK-APG Mitgliedschaften (Test)")),
            new(
                graph.CreateUriNode(OgdExportConstants.UriMembership),
                graph.CreateUriNode(OgdExportConstants.SchemaDescription),
                graph.CreateLiteralNode("Export der Mitgliedschaften von APG zu Testzwecken"))
        ];

        List<Triple> interestMetadataTriples =
        [
            new(
                graph.CreateUriNode(OgdExportConstants.UriVestedInterests),
                graph.CreateUriNode(OgdExportConstants.SchemaPublisher),
                graph.CreateUriNode(OgdExportConstants.LdFCh)),
            new(
                graph.CreateUriNode(OgdExportConstants.UriVestedInterests),
                graph.CreateUriNode(OgdExportConstants.SchemaCreator),
                graph.CreateUriNode(OgdExportConstants.LdFCh)),
            new(
                graph.CreateUriNode(OgdExportConstants.UriVestedInterests),
                graph.CreateUriNode(OgdExportConstants.SchemaDateCreated),
                graph.CreateLiteralNode("2025-05-01", UriFactory.Create(XmlSpecsHelper.XmlSchemaDataTypeDate))),
            new(
                graph.CreateUriNode(OgdExportConstants.UriVestedInterests),
                graph.CreateUriNode(OgdExportConstants.SchemaDatePublished),
                graph.CreateLiteralNode(OgdExportConstants.SchemaPublishedDate, UriFactory.Create(XmlSpecsHelper.XmlSchemaDataTypeDate))),
            new(
                graph.CreateUriNode(OgdExportConstants.UriVestedInterests),
                graph.CreateUriNode(OgdExportConstants.SchemaDateModified),
                graph.CreateLiteralNode(DateTime.Now.ToString("yyyy-MM-dd"), UriFactory.Create(XmlSpecsHelper.XmlSchemaDataTypeDate))),
            new(
                graph.CreateUriNode(OgdExportConstants.UriVestedInterests),
                graph.CreateUriNode(OgdExportConstants.SchemaName),
                graph.CreateLiteralNode("BK-APG Interessenbindungen (Test)")),
            new(
                graph.CreateUriNode(OgdExportConstants.UriVestedInterests),
                graph.CreateUriNode(OgdExportConstants.SchemaDescription),
                graph.CreateLiteralNode("Export der Interessenbindungen von APG zu Testzwecken"))
        ];

        List<Triple> committeeFunctionStatisticMetadataTriples =
        [
            new(
                graph.CreateUriNode(OgdExportConstants.UriCommitteeFunctionStatistic),
                graph.CreateUriNode(OgdExportConstants.SchemaPublisher),
                graph.CreateUriNode(OgdExportConstants.LdFCh)),
            new(
                graph.CreateUriNode(OgdExportConstants.UriCommitteeFunctionStatistic),
                graph.CreateUriNode(OgdExportConstants.SchemaCreator),
                graph.CreateUriNode(OgdExportConstants.LdFCh)),
            new(
                graph.CreateUriNode(OgdExportConstants.UriCommitteeFunctionStatistic),
                graph.CreateUriNode(OgdExportConstants.SchemaDateCreated),
                graph.CreateLiteralNode("2025-11-26", UriFactory.Create(XmlSpecsHelper.XmlSchemaDataTypeDate))),
            new(
                graph.CreateUriNode(OgdExportConstants.UriCommitteeFunctionStatistic),
                graph.CreateUriNode(OgdExportConstants.SchemaDatePublished),
                graph.CreateLiteralNode(OgdExportConstants.SchemaPublishedDate, UriFactory.Create(XmlSpecsHelper.XmlSchemaDataTypeDate))),
            new(
                graph.CreateUriNode(OgdExportConstants.UriCommitteeFunctionStatistic),
                graph.CreateUriNode(OgdExportConstants.SchemaDateModified),
                graph.CreateLiteralNode(DateTime.Now.ToString("yyyy-MM-dd"), UriFactory.Create(XmlSpecsHelper.XmlSchemaDataTypeDate))),
            new(
                graph.CreateUriNode(OgdExportConstants.UriCommitteeFunctionStatistic),
                graph.CreateUriNode(OgdExportConstants.SchemaName),
                graph.CreateLiteralNode("BK-APG Gremium Statistiken auf Funktionsebene (Test)")),
            new(
                graph.CreateUriNode(OgdExportConstants.UriCommitteeFunctionStatistic),
                graph.CreateUriNode(OgdExportConstants.SchemaDescription),
                graph.CreateLiteralNode("Export der Gremium Funktions-Statistiken von APG zu Testzwecken"))
        ];

        List<Triple> committeeCantonStatisticMetadataTriples =
        [
            new(
                graph.CreateUriNode(OgdExportConstants.UriCommitteeCantonStatistic),
                graph.CreateUriNode(OgdExportConstants.SchemaPublisher),
                graph.CreateUriNode(OgdExportConstants.LdFCh)),
            new(
                graph.CreateUriNode(OgdExportConstants.UriCommitteeCantonStatistic),
                graph.CreateUriNode(OgdExportConstants.SchemaCreator),
                graph.CreateUriNode(OgdExportConstants.LdFCh)),
            new(
                graph.CreateUriNode(OgdExportConstants.UriCommitteeCantonStatistic),
                graph.CreateUriNode(OgdExportConstants.SchemaDateCreated),
                graph.CreateLiteralNode("2025-11-26", UriFactory.Create(XmlSpecsHelper.XmlSchemaDataTypeDate))),
            new(
                graph.CreateUriNode(OgdExportConstants.UriCommitteeCantonStatistic),
                graph.CreateUriNode(OgdExportConstants.SchemaDatePublished),
                graph.CreateLiteralNode(OgdExportConstants.SchemaPublishedDate, UriFactory.Create(XmlSpecsHelper.XmlSchemaDataTypeDate))),
            new(
                graph.CreateUriNode(OgdExportConstants.UriCommitteeCantonStatistic),
                graph.CreateUriNode(OgdExportConstants.SchemaDateModified),
                graph.CreateLiteralNode(DateTime.Now.ToString("yyyy-MM-dd"), UriFactory.Create(XmlSpecsHelper.XmlSchemaDataTypeDate))),
            new(
                graph.CreateUriNode(OgdExportConstants.UriCommitteeCantonStatistic),
                graph.CreateUriNode(OgdExportConstants.SchemaName),
                graph.CreateLiteralNode("BK-APG Gremium Statistiken Sicht Kantone und Mitglieder (Test)")),
            new(
                graph.CreateUriNode(OgdExportConstants.UriCommitteeCantonStatistic),
                graph.CreateUriNode(OgdExportConstants.SchemaDescription),
                graph.CreateLiteralNode("Export der Gremium Kantons-Statistiken von APG zu Testzwecken"))
        ];

        List<Triple> committeeCantonDetailStatisticMetadataTriples =
        [
            new(
                graph.CreateUriNode(OgdExportConstants.UriCommitteeCantonDetailStatistic),
                graph.CreateUriNode(OgdExportConstants.SchemaPublisher),
                graph.CreateUriNode(OgdExportConstants.LdFCh)),
            new(
                graph.CreateUriNode(OgdExportConstants.UriCommitteeCantonDetailStatistic),
                graph.CreateUriNode(OgdExportConstants.SchemaCreator),
                graph.CreateUriNode(OgdExportConstants.LdFCh)),
            new(
                graph.CreateUriNode(OgdExportConstants.UriCommitteeCantonDetailStatistic),
                graph.CreateUriNode(OgdExportConstants.SchemaDateCreated),
                graph.CreateLiteralNode("2025-11-26", UriFactory.Create(XmlSpecsHelper.XmlSchemaDataTypeDate))),
            new(
                graph.CreateUriNode(OgdExportConstants.UriCommitteeCantonDetailStatistic),
                graph.CreateUriNode(OgdExportConstants.SchemaDatePublished),
                graph.CreateLiteralNode(OgdExportConstants.SchemaPublishedDate, UriFactory.Create(XmlSpecsHelper.XmlSchemaDataTypeDate))),
            new(
                graph.CreateUriNode(OgdExportConstants.UriCommitteeCantonDetailStatistic),
                graph.CreateUriNode(OgdExportConstants.SchemaDateModified),
                graph.CreateLiteralNode(DateTime.Now.ToString("yyyy-MM-dd"), UriFactory.Create(XmlSpecsHelper.XmlSchemaDataTypeDate))),
            new(
                graph.CreateUriNode(OgdExportConstants.UriCommitteeCantonDetailStatistic),
                graph.CreateUriNode(OgdExportConstants.SchemaName),
                graph.CreateLiteralNode("BK-APG Gremium Statistiken detaillierte Sicht Kantone und Mitglieder (Test)")),
            new(
                graph.CreateUriNode(OgdExportConstants.UriCommitteeCantonDetailStatistic),
                graph.CreateUriNode(OgdExportConstants.SchemaDescription),
                graph.CreateLiteralNode("Export der Gremium detaillierten Kantons-Statistiken von APG zu Testzwecken"))
        ];


        List<Triple> committeeGenderLanguageStatisticMetadataTriples =
        [
            new(
                graph.CreateUriNode(OgdExportConstants.UriCommitteeGenderLanguageStatistic),
                graph.CreateUriNode(OgdExportConstants.SchemaPublisher),
                graph.CreateUriNode(OgdExportConstants.LdFCh)),
            new(
                graph.CreateUriNode(OgdExportConstants.UriCommitteeGenderLanguageStatistic),
                graph.CreateUriNode(OgdExportConstants.SchemaCreator),
                graph.CreateUriNode(OgdExportConstants.LdFCh)),
            new(
                graph.CreateUriNode(OgdExportConstants.UriCommitteeGenderLanguageStatistic),
                graph.CreateUriNode(OgdExportConstants.SchemaDateCreated),
                graph.CreateLiteralNode("2025-11-26", UriFactory.Create(XmlSpecsHelper.XmlSchemaDataTypeDate))),
            new(
                graph.CreateUriNode(OgdExportConstants.UriCommitteeGenderLanguageStatistic),
                graph.CreateUriNode(OgdExportConstants.SchemaDatePublished),
                graph.CreateLiteralNode(OgdExportConstants.SchemaPublishedDate, UriFactory.Create(XmlSpecsHelper.XmlSchemaDataTypeDate))),
            new(
                graph.CreateUriNode(OgdExportConstants.UriCommitteeGenderLanguageStatistic),
                graph.CreateUriNode(OgdExportConstants.SchemaDateModified),
                graph.CreateLiteralNode(DateTime.Now.ToString("yyyy-MM-dd"), UriFactory.Create(XmlSpecsHelper.XmlSchemaDataTypeDate))),
            new(
                graph.CreateUriNode(OgdExportConstants.UriCommitteeGenderLanguageStatistic),
                graph.CreateUriNode(OgdExportConstants.SchemaName),
                graph.CreateLiteralNode("BK-APG Gremium Statistiken im Bereich Geschlechter und Sprachen (Test)")),
            new(
                graph.CreateUriNode(OgdExportConstants.UriCommitteeGenderLanguageStatistic),
                graph.CreateUriNode(OgdExportConstants.SchemaDescription),
                graph.CreateLiteralNode("Export der Gremium Kantons-Statistiken im Bereich der Geschlechter & Sprachen von APG zu Testzwecken"))
        ];

        var committeeTypeTriples = CreateCommitteeTypeDimension(graph);
        var functionTriples = await CreateFunctionDimension(graph);
        var occupationTriples = await CreateOccupationDimension(graph);
        var cantonTriples = await CreateCantonDimension(graph);
        var interestFunctionTriples = await CreateInterestFunctionDimension(graph);
        var interestCommitteeTriples = await CreateInterestCommitteeDimension(graph);
        var contactPointTypeTriples = await CreateContactPointTypeDimension(graph);
        var contactPointTriples = CreateContactPointTriples(graph);
        var personTriples = CreatePersonDimension(graph);

        var committees = _committeeRepository.GetAll().ToArray();
        var committeeTriples = CreateCommitteeDimension(graph, committees);

        await _ogdDocumentService.SetupBucket();
        var appointmentDecisionTriples = await CreateAppointmentDecisionDimension(graph, committees);

        var membershipData = _membershipRepository.GetAll().ToArray();

        var membershipRawData =
            _cubeRawDataService.CreateTriples(
                graph,
                OgdExportConstants.UriMembership,
                membershipData.Select(MembershipMapper.ToObservation));

        var interestData = _interestRepository.GetAll();

        var interestRawData =
            _cubeRawDataService.CreateTriples(
                graph,
                OgdExportConstants.UriVestedInterests,
                interestData.Select(InterestMapper.ToObservation));

        var functionStatisticData = _membershipRepository.GetMembershipFunctionsForStatistics();

        var committeeFunctionStatisticRawData =
            _cubeRawDataService.CreateTriples(
                graph,
                OgdExportConstants.UriCommitteeFunctionStatistic,
                functionStatisticData.Select(OgdMapper.ToFunctionStatisticObservation));

        var cantonStatisticData = await _membershipService.GetMembershipsForCantonStatistic(membershipData);

        var committeeCantonStatisticRawData =
            _cubeRawDataService.CreateTriples(
                graph,
                OgdExportConstants.UriCommitteeCantonStatistic,
                cantonStatisticData.Select(OgdMapper.ToCantonStatisticObservation));

        var genderLanguageStatisticData = _membershipService.GetMembershipsForGenderLanguageStatistic(membershipData);

        var committeeGenderLanguageStatisticRawData =
            _cubeRawDataService.CreateTriples(
                graph,
                OgdExportConstants.UriCommitteeGenderLanguageStatistic,
                genderLanguageStatisticData.Select(OgdMapper.ToGenderLanguageStatisticObservation));

        var allTriples = committeeTypeTriples
            .Concat(functionTriples)
            .Concat(interestFunctionTriples)
            .Concat(interestCommitteeTriples)
            .Concat(occupationTriples)
            .Concat(cantonTriples)
            .Concat(contactPointTypeTriples)
            .Concat(contactPointTriples)
            .Concat(committeeTriples)
            .Concat(personTriples)
            .Concat(membershipRawData)
            .Concat(membershipMetadataTriples)
            .Concat(interestMetadataTriples)
            .Concat(committeeFunctionStatisticMetadataTriples)
            .Concat(committeeCantonStatisticMetadataTriples)
            .Concat(committeeGenderLanguageStatisticMetadataTriples)
            .Concat(interestRawData)
            .Concat(committeeFunctionStatisticRawData)
            .Concat(committeeCantonStatisticRawData)
            .Concat(committeeGenderLanguageStatisticRawData)
            .Concat(appointmentDecisionTriples);

        _logger.LogInformation("OGD export: deleting graph");
        await _storageProvider.DeleteGraphAsync(_sparqlOptions.ExportGraphName, ct);

        _logger.LogInformation("OGD export: creating new graph");
        await _storageProvider.UpdateGraphAsync(_sparqlOptions.ExportGraphName, allTriples, [], ct);

        var cantonDetailStatisticData = await _membershipService.GetMembershipsForDetailedCantonStatistic(membershipData);

        var committeeCantonDetailStatisticRawData =
            _cubeRawDataService.CreateTriples(
                graph,
                OgdExportConstants.UriCommitteeCantonDetailStatistic,
                cantonDetailStatisticData.Select(OgdMapper.ToCantonDetailStatisticObservation));

        var additionalTriples = committeeCantonDetailStatisticRawData
            .Concat(committeeCantonDetailStatisticMetadataTriples);

        _logger.LogInformation("OGD export: updating detailed statistic");
        await _storageProvider.UpdateGraphAsync(_sparqlOptions.ExportGraphName, additionalTriples, [], ct);

        _logger.LogInformation("OGD export: completed successfully");
    }

    private IEnumerable<Triple> CreateCommitteeDimension(Graph graph, IEnumerable<Committee> committees)
    {
        var committeeDimensionItems = committees.Select(CommitteeMapper.ToDimensionItem);

        var committeeTriples =
            _dimensionService.CreateTriples(
                committeeDimensionItems,
                graph,
                $"{_sparqlOptions.ExportGraphBaseUri}/{OgdExportConstants.NamespaceCommittee}",
                [new Literal("Behördenkommissionen", "de")]);

        return committeeTriples;
    }

    private async Task<IEnumerable<Triple>> CreateAppointmentDecisionDimension(Graph graph, IEnumerable<Committee> committees)
    {
        var appointmentDecisionItems = new List<DimensionItem>();

        foreach (var committee in committees)
        {
            var appointmentDecision = committee.LatestInstitutionAppointmentDecision;
            if (appointmentDecision is null)
            {
                continue;
            }

            using var documentStream = await _documentService.GetDocument(appointmentDecision.OriginalDocument!.DocumentStorageId);
            if (documentStream == null)
            {
                _logger.LogWarning("Could not retrieve original document for appointment decision {AppointmentDecisionId}", appointmentDecision.Id);
                continue;
            }

            var path = $"committees/{committee.OgdId}/{OgdExportConstants.NamespaceAppointmentDecision}/{appointmentDecision.OgdId}";
            var documentName = appointmentDecision.OriginalDocument!.DocumentName;
            await _ogdDocumentService.UploadDocument(path, documentName, documentStream);

            var encodedDocumentName = Uri.EscapeDataString(documentName);
            var documentUrl = $"{_ogdS3Configuration.BaseUrl}/{_ogdS3Configuration.bucket}/{path}/{encodedDocumentName}";
            appointmentDecisionItems.Add(new DimensionItem(appointmentDecision.OgdId, new Literal(encodedDocumentName),
                [new AdditionalLiteralProperty("schema:url", new Literal(documentUrl, new Uri("http://www.w3.org/2001/XMLSchema#anyURI")))]
            ));
        }

        var appointmentDecisionTriples =
            _dimensionService.CreateTriples(
                appointmentDecisionItems,
                graph,
                $"{_sparqlOptions.ExportGraphBaseUri}/{OgdExportConstants.NamespaceAppointmentDecision}",
                [new Literal("Einsetzungsverfügungen", "de")],
                rdfTypes: ["http://schema.org/DigitalDocument"]);

        return appointmentDecisionTriples;
    }

    private IEnumerable<Triple> CreatePersonDimension(Graph graph)
    {
        var personDimensionItems =
            _personRepository
                .GetAll()
                .Select(PersonMapper.ToDimensionItem);

        var personTriples =
            _dimensionService.CreateTriples(
                personDimensionItems,
                graph,
                $"{_sparqlOptions.ExportGraphBaseUri}/{OgdExportConstants.NamespacePerson}",
                [new Literal("Personen in einer Ausserparlamentarischen Kommission", "de")],
                rdfTypes: ["http://schema.org/Person"]);

        return personTriples;
    }

    private async Task<IEnumerable<Triple>> CreateInterestFunctionDimension(Graph graph)
    {
        var interestFunctions = await _masterDataRepository.GetInterestFunctions();
        var interestFunctionItems = interestFunctions.Select(MasterDataMapper.ToDimensionItem);

        var interestFunctionTriples =
            _dimensionService.CreateTriples(
                interestFunctionItems,
                graph,
                $"{_sparqlOptions.ExportGraphBaseUri}/vocabulary/{OgdExportConstants.NamespaceInterestFunction}",
                [new Literal("Funktion bei Interessenbindungen", "de")]);

        return interestFunctionTriples;
    }

    private async Task<IEnumerable<Triple>> CreateInterestCommitteeDimension(Graph graph)
    {
        var interestCommittees = await _masterDataRepository.GetInterestCommittees();
        var interestCommitteeItems = interestCommittees.Select(MasterDataMapper.ToDimensionItem);

        var interestCommitteTriples =
            _dimensionService.CreateTriples(
                interestCommitteeItems,
                graph,
                $"{_sparqlOptions.ExportGraphBaseUri}/vocabulary/{OgdExportConstants.NamespaceInterestCommittee}",
                [new Literal("Gremium bei Interessenbindungen", "de")]);

        return interestCommitteTriples;
    }

    private IEnumerable<Triple> CreateCommitteeTypeDimension(Graph graph)
    {
        var committeeTypeDimensionItems =
            _committeeTypeRepository
                .GetAll()
                .Select(MasterDataMapper.ToDimensionItem);

        var committeeTypeTriples =
            _dimensionService.CreateTriples(
                committeeTypeDimensionItems,
                graph,
                $"{_sparqlOptions.ExportGraphBaseUri}/vocabulary/{OgdExportConstants.NamespaceCommitteeType}",
                [new Literal("Behördenkommissionen", "de")]);

        return committeeTypeTriples;
    }

    private async Task<IEnumerable<Triple>> CreateFunctionDimension(Graph graph)
    {
        var functions = await _masterDataRepository.GetFunctions();
        var functionDimensionItems = functions.Select(MasterDataMapper.ToDimensionItem);

        var functionTriples =
            _dimensionService.CreateTriples(
                functionDimensionItems,
                graph,
                $"{_sparqlOptions.ExportGraphBaseUri}/vocabulary/{OgdExportConstants.NamespaceFunction}",
                [new Literal("Funktionen", "de")]);

        return functionTriples;
    }

    private async Task<IEnumerable<Triple>> CreateOccupationDimension(Graph graph)
    {
        var occupations = await _masterDataRepository.GetOccupations();
        var occupationDimensionItems = occupations.Select(MasterDataMapper.ToDimensionItem);

        var occupationTriples =
            _dimensionService.CreateTriples(
                occupationDimensionItems,
                graph,
                $"{_sparqlOptions.ExportGraphBaseUri}/vocabulary/{OgdExportConstants.NamespaceOccupation}",
                [new Literal("Berufe", "de")]);

        return occupationTriples;
    }

    private async Task<IEnumerable<Triple>> CreateCantonDimension(Graph graph)
    {
        var cantons = await _masterDataRepository.GetCantons();
        var cantonDimensionItems = cantons.Select(MasterDataMapper.ToDimensionItem);

        var occupationTriples =
            _dimensionService.CreateTriples(
                cantonDimensionItems,
                graph,
                $"{_sparqlOptions.ExportGraphBaseUri}/vocabulary/{OgdExportConstants.NamespaceCanton}",
                [new Literal("Kantone", "de")]);

        return occupationTriples;
    }

    private async Task<IEnumerable<Triple>> CreateContactPointTypeDimension(Graph graph)
    {
        var contactPointTypes = await _masterDataRepository.GetContactPointTypes();

        var contactPointDimensionItems = contactPointTypes.Select(MasterDataMapper.ToDimensionItem);

        var contactPointTypeTriples =
            _dimensionService.CreateTriples(
                contactPointDimensionItems,
                graph,
                $"{_sparqlOptions.ExportGraphBaseUri}/vocabulary/{OgdExportConstants.NamespaceContactPointType}",
                [new Literal("Kontaktstelle Typ", "de")]);

        return contactPointTypeTriples;
    }

    private IEnumerable<Triple> CreateContactPointTriples(Graph graph)
    {
        //exports all the contact points as triples that can be referenced by the committee.
        //background: this should actually be exported by AdminDir (but that functionality is not available there yet).
        //            since the contact points are not a 'dimension', these triples are created manually and not via FCh.Dimension.

        foreach (var cp in _contactPointRepository.GetAllActiveContactPoints())
        {
            //this has to match the uri pointing from the committee to the contact point (see CommitteeMapper.ToDimensionItem)
            var contactPointUri = $"{OgdExportConstants.NamespaceOrganization}:{cp.OgdId}";
            var addressUri = $"{contactPointUri}/address";
            var memberUri = $"{contactPointUri}/member/1";

            // mandatory triples
            var triples = new List<Triple>
            {
                new(graph.CreateUriNode(contactPointUri), graph.CreateUriNode(OgdExportConstants.RdfType), graph.CreateUriNode(OgdExportConstants.SchemaGovernmentOrganization)),
                new(graph.CreateUriNode(contactPointUri), graph.CreateUriNode(OgdExportConstants.SchemaName), graph.CreateLiteralNode(cp.CompanyName)),
                new(graph.CreateUriNode(contactPointUri), graph.CreateUriNode(OgdExportConstants.SchemaAddress), graph.CreateUriNode(addressUri)),

                new(graph.CreateUriNode(addressUri), graph.CreateUriNode(OgdExportConstants.RdfType), graph.CreateUriNode(OgdExportConstants.SchemaPostalAddress)),
                new(graph.CreateUriNode(addressUri), graph.CreateUriNode(OgdExportConstants.SchemaStreetAddress), graph.CreateLiteralNode(cp.Street)),
                new(graph.CreateUriNode(addressUri), graph.CreateUriNode(OgdExportConstants.SchemaPostOfficeBoxNumber), graph.CreateLiteralNode(cp.PoBox)),
                new(graph.CreateUriNode(addressUri), graph.CreateUriNode(OgdExportConstants.SchemaPostalCode), graph.CreateLiteralNode(cp.Zip)),
                new(graph.CreateUriNode(addressUri), graph.CreateUriNode(OgdExportConstants.SchemaAddressLocality), graph.CreateLiteralNode(cp.City)),
            };

            if (cp.ReleasePersonData)
            {
                triples.Add(new(graph.CreateUriNode(contactPointUri), graph.CreateUriNode(OgdExportConstants.SchemaMember), graph.CreateUriNode(memberUri)));
                triples.Add(new(graph.CreateUriNode(memberUri), graph.CreateUriNode(OgdExportConstants.SchemaFamilyName), graph.CreateLiteralNode(cp.Surname)));
                triples.Add(new(graph.CreateUriNode(memberUri), graph.CreateUriNode(OgdExportConstants.SchemaGivenName), graph.CreateLiteralNode(cp.GivenName)));
                triples.Add(new(graph.CreateUriNode(memberUri), graph.CreateUriNode(OgdExportConstants.SchemaTitle), graph.CreateLiteralNode(cp.Title)));

                // optional: personal email
                if (!string.IsNullOrWhiteSpace(cp.PersonalEmail))
                {
                    triples.Add(new Triple(
                        graph.CreateUriNode(memberUri),
                        graph.CreateUriNode(OgdExportConstants.SchemaEmail),
                        graph.CreateLiteralNode(cp.PersonalEmail)
                    ));
                }

                // optional: personal phone or mobile
                if (!string.IsNullOrWhiteSpace(cp.PersonalPhone))
                {
                    triples.Add(new Triple(
                        graph.CreateUriNode(memberUri),
                        graph.CreateUriNode(OgdExportConstants.SchemaTelephone),
                        graph.CreateLiteralNode(cp.PersonalPhone)
                    ));
                }

                if (!string.IsNullOrWhiteSpace(cp.PersonalMobile))
                {
                    triples.Add(new Triple(
                        graph.CreateUriNode(memberUri),
                        graph.CreateUriNode(OgdExportConstants.SchemaTelephone),
                        graph.CreateLiteralNode(cp.PersonalMobile)
                    ));
                }
            }

            if (cp.ContactPointType is not null)
            {
                triples.Add(new Triple(
                    graph.CreateUriNode(contactPointUri),
                    graph.CreateUriNode(OgdExportConstants.SchemaAdditionalType),
                    graph.CreateUriNode($"{OgdExportConstants.NamespaceContactPointType}:{cp.ContactPointType!.OgdId}")
                ));
            }

            // optional: email
            if (!string.IsNullOrWhiteSpace(cp.Email))
            {
                triples.Add(new Triple(
                    graph.CreateUriNode(contactPointUri),
                    graph.CreateUriNode(OgdExportConstants.SchemaEmail),
                    graph.CreateLiteralNode(cp.Email)
                ));
            }

            // optional: phone
            if (!string.IsNullOrWhiteSpace(cp.Phone))
            {
                triples.Add(new Triple(
                    graph.CreateUriNode(contactPointUri),
                    graph.CreateUriNode(OgdExportConstants.SchemaTelephone),
                    graph.CreateLiteralNode(cp.Phone)
                ));
            }

            // optional: section
            if (!string.IsNullOrWhiteSpace(cp.Section))
            {
                triples.Add(new Triple(
                    graph.CreateUriNode(contactPointUri),
                    graph.CreateUriNode(OgdExportConstants.SchemaSection),
                    graph.CreateLiteralNode(cp.Section)
                ));
            }

            foreach (var t in triples)
            {
                yield return t;
            }
        }
    }
}
