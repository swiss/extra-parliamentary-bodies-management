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

public class OgdExportService : IOgdExportService
{
    private readonly IReadOnlyDictionary<string, IAsyncStorageProvider> _storageProviders;
    private readonly IDimensionService _dimensionService;
    private readonly ICubeRawDataService _cubeRawDataService;
    private readonly IMembershipService _membershipService;
    private readonly ICommitteeService _committeeService;
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
    private readonly SparqlTargetsOptions _targetsOptions;
    private readonly OgdS3Configuration _ogdS3Configuration;

    public OgdExportService(
        IDimensionService dimensionService,
        ICubeRawDataService cubeRawDataService,
        IPersonRepository personRepository,
        IMembershipService membershipService,
        ICommitteeService committeeService,
        ICommitteeRepository committeeRepository,
        ICommitteeTypeRepository committeeTypeRepository,
        ISparqlClientFactory sparqlClientFactory,
        IMembershipRepository membershipRepository,
        IInterestRepository interestRepository,
        ILogger<OgdExportService> logger,
        IOptions<SparqlOptions> sparqlOptions,
        IOptions<SparqlTargetsOptions> targetsOptions,
        IMasterDataRepository masterDataRepository,
        IContactPointRepository contactPointRepository,
        IDocumentService documentService,
        IOgdDocumentService ogdDocumentService,
        IOptions<OgdS3Configuration> ogdS3Options)
    {
        _storageProviders = sparqlClientFactory.GetStorageProviders();
        _dimensionService = dimensionService;
        _membershipService = membershipService;
        _committeeService = committeeService;
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
        _targetsOptions = targetsOptions.Value;
        _ogdS3Configuration = ogdS3Options.Value;
    }

    public async Task Export(CancellationToken ct = default)
    {
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
        graph.NamespaceMap.AddNamespace(OgdExportConstants.NamespaceCommitteeGenderLanguageStatistic, new Uri($"{_sparqlOptions.ExportGraphBaseUri}/{OgdExportConstants.NamespaceCommitteeGenderLanguageStatistic}/"));
        graph.NamespaceMap.AddNamespace(OgdExportConstants.NamespaceCommitteeTypeStatistic, new Uri($"{_sparqlOptions.ExportGraphBaseUri}/{OgdExportConstants.NamespaceCommitteeTypeStatistic}/"));
        graph.NamespaceMap.AddNamespace(OgdExportConstants.NamespaceCommitteeTypeDepartmentStatistic, new Uri($"{_sparqlOptions.ExportGraphBaseUri}/{OgdExportConstants.NamespaceCommitteeTypeDepartmentStatistic}/"));
        graph.NamespaceMap.AddNamespace(OgdExportConstants.NamespaceLd, new Uri("https://ld.admin.ch/"));
        graph.NamespaceMap.AddNamespace(OgdExportConstants.NamespaceRld, new Uri("https://register.ld.admin.ch/"));
        graph.NamespaceMap.AddNamespace(OgdExportConstants.NamespaceSchema, new Uri("http://schema.org"));
        graph.NamespaceMap.AddNamespace(OgdExportConstants.NamespaceRdf, new Uri("http://www.w3.org/1999/02/22-rdf-syntax-ns#"));
        graph.NamespaceMap.AddNamespace(OgdExportConstants.NamespaceW3, new Uri("http://www.w3.org/"));
        graph.NamespaceMap.AddNamespace(OgdExportConstants.NamespaceCube, new Uri("https://cube.link/"));

        var committeeCubeMetadata = CreateMetaDataTriples(graph, OgdExportConstants.UriCommittee, "2026-01-14", "2026-01-14", "BK-APG Gremien", "Export der Gremien von APG");
        committeeCubeMetadata.AddRange(
            [
                new Triple(
                    graph.CreateUriNode(OgdExportConstants.UriCommittee),
                    graph.CreateUriNode(OgdExportConstants.SchemaCreativeWorkStatus),
                    graph.CreateUriNode(OgdExportConstants.LdCreativeWorkStatusPublished)),
                new Triple(
                    graph.CreateUriNode(OgdExportConstants.UriCommittee),
                    graph.CreateUriNode(OgdExportConstants.SchemaWorkExample),
                    graph.CreateUriNode(OgdExportConstants.LdApplicationVisualize))
            ]
        );

        var personCubeMetadata = CreateMetaDataTriples(graph, OgdExportConstants.UriPerson, "2026-01-14", "2026-01-14", "BK-APG Personen", "Export der Personen von APG");
        personCubeMetadata.AddRange(
            [
                new Triple(
                    graph.CreateUriNode(OgdExportConstants.UriPerson),
                    graph.CreateUriNode(OgdExportConstants.SchemaCreativeWorkStatus),
                    graph.CreateUriNode(OgdExportConstants.LdCreativeWorkStatusPublished)),
                new Triple(
                    graph.CreateUriNode(OgdExportConstants.UriPerson),
                    graph.CreateUriNode(OgdExportConstants.SchemaWorkExample),
                    graph.CreateUriNode(OgdExportConstants.LdApplicationVisualize))
            ]
        );

        var membershipMetadataTriples = CreateMetaDataTriples(graph, OgdExportConstants.UriMembership, "2026-01-14", "2026-01-14", "BK-APG Mitgliedschaften", "Export der Mitgliedschaften von APG");
        var interestMetadataTriples = CreateMetaDataTriples(graph, OgdExportConstants.UriVestedInterests, "2026-01-14", "2026-01-14", "BK-APG Interessenbindungen", "Export der Interessenbindungen von APG");

        var committeeFunctionStatisticCubeMetadata = CreateMetaDataTriples(graph, OgdExportConstants.UriCommitteeFunctionStatistic, "2026-01-14", "2026-01-14", "BK-APG Gremium Statistiken auf Funktionsebene", "Export der Gremium Funktions-Statistiken von APG");
        committeeFunctionStatisticCubeMetadata.AddRange(
            [
                new Triple(
                    graph.CreateUriNode(OgdExportConstants.UriCommitteeFunctionStatistic),
                    graph.CreateUriNode(OgdExportConstants.SchemaCreativeWorkStatus),
                    graph.CreateUriNode(OgdExportConstants.LdCreativeWorkStatusPublished)),
                new Triple(
                    graph.CreateUriNode(OgdExportConstants.UriCommitteeFunctionStatistic),
                    graph.CreateUriNode(OgdExportConstants.SchemaWorkExample),
                    graph.CreateUriNode(OgdExportConstants.LdApplicationVisualize))
            ]
        );

        var committeeCantonStatisticCubeMetadata = CreateMetaDataTriples(graph, OgdExportConstants.UriCommitteeCantonStatistic, "2026-01-14", "2026-01-14", "BK-APG Gremium Statistiken Sicht Kantone und Mitglieder", "Export der Gremium Kantons-Statistiken von APG");
        committeeCantonStatisticCubeMetadata.AddRange(
            [
                new Triple(
                    graph.CreateUriNode(OgdExportConstants.UriCommitteeCantonStatistic),
                    graph.CreateUriNode(OgdExportConstants.SchemaCreativeWorkStatus),
                    graph.CreateUriNode(OgdExportConstants.LdCreativeWorkStatusPublished)),
                new Triple(
                    graph.CreateUriNode(OgdExportConstants.UriCommitteeCantonStatistic),
                    graph.CreateUriNode(OgdExportConstants.SchemaWorkExample),
                    graph.CreateUriNode(OgdExportConstants.LdApplicationVisualize))
            ]
        );

        var committeeGenderLanguageStatisticCubeMetadata = CreateMetaDataTriples(graph, OgdExportConstants.UriCommitteeGenderLanguageStatistic, "2026-01-14", "2026-01-14", "BK-APG Gremium Statistiken im Bereich Geschlechter, Sprachen und Alter", "Export der Gremium Kantons-Statistiken im Bereich der Geschlechter & Sprachen von APG");
        committeeGenderLanguageStatisticCubeMetadata.AddRange(
            [
                new Triple(
                    graph.CreateUriNode(OgdExportConstants.UriCommitteeGenderLanguageStatistic),
                    graph.CreateUriNode(OgdExportConstants.SchemaCreativeWorkStatus),
                    graph.CreateUriNode(OgdExportConstants.LdCreativeWorkStatusPublished)),
                new Triple(
                    graph.CreateUriNode(OgdExportConstants.UriCommitteeGenderLanguageStatistic),
                    graph.CreateUriNode(OgdExportConstants.SchemaWorkExample),
                    graph.CreateUriNode(OgdExportConstants.LdApplicationVisualize))
            ]
        );

        var committeeTypeStatisticCubeMetadata = CreateMetaDataTriples(graph, OgdExportConstants.UriCommitteeTypeStatistic, "2026-01-14", "2026-01-14", "BK-APG Gremiumtypen-Statistik", "Export der Statistiken zu Sprachen, Geschlechtern und Mengen pro Gremiumtyp");
        committeeTypeStatisticCubeMetadata.AddRange(
            [
                new Triple(
                    graph.CreateUriNode(OgdExportConstants.UriCommitteeTypeStatistic),
                    graph.CreateUriNode(OgdExportConstants.SchemaCreativeWorkStatus),
                    graph.CreateUriNode(OgdExportConstants.LdCreativeWorkStatusPublished)),
                new Triple(
                    graph.CreateUriNode(OgdExportConstants.UriCommitteeTypeStatistic),
                    graph.CreateUriNode(OgdExportConstants.SchemaWorkExample),
                    graph.CreateUriNode(OgdExportConstants.LdApplicationVisualize))
            ]
        );

        var committeeTypeDepartmentStatisticCubeMetadata = CreateMetaDataTriples(graph, OgdExportConstants.UriCommitteeTypeDepartmentStatistic, "2026-01-14", "2026-01-14", "BK-APG Gremiumtypen-Statistik", "Export der Gremienstatistik pro Gremiumtyp und Departement");
        committeeTypeDepartmentStatisticCubeMetadata.AddRange(
            [
                new Triple(
                    graph.CreateUriNode(OgdExportConstants.UriCommitteeTypeDepartmentStatistic),
                    graph.CreateUriNode(OgdExportConstants.SchemaCreativeWorkStatus),
                    graph.CreateUriNode(OgdExportConstants.LdCreativeWorkStatusPublished)),
                new Triple(
                    graph.CreateUriNode(OgdExportConstants.UriCommitteeTypeDepartmentStatistic),
                    graph.CreateUriNode(OgdExportConstants.SchemaWorkExample),
                    graph.CreateUriNode(OgdExportConstants.LdApplicationVisualize))
            ]
        );

        var committeeTypeTriples = await CreateCommitteeTypeDimension(graph);
        var functionTriples = await CreateFunctionDimension(graph);
        var occupationTriples = await CreateOccupationDimension(graph);
        var cantonTriples = await CreateCantonDimension(graph);
        var interestFunctionTriples = await CreateInterestFunctionDimension(graph);
        var interestCommitteeTriples = await CreateInterestCommitteeDimension(graph);
        var contactPointTypeTriples = await CreateContactPointTypeDimension(graph);
        var contactPointTriples = CreateContactPointTriples(graph);

        var people = (await _personRepository.GetForOgdExport()).ToArray();
        var personTriples = CreatePersonDimension(graph, people);
        var personCube = _cubeRawDataService.CreateTriples(graph, OgdExportConstants.UriPerson, people.Select(PersonMapper.ToObservation));

        var committees = (await _committeeRepository.GetForOgdExport()).ToArray();
        var committeeTriples = CreateCommitteeDimension(graph, committees);
        var committeeCube = _cubeRawDataService.CreateTriples(graph, OgdExportConstants.UriCommittee, committees.Select(CommitteeMapper.ToObservation));

        await _ogdDocumentService.SetupBucket();
        var appointmentDecisionTriples = await CreateAppointmentDecisionDimension(graph, committees);

        var membershipData = _membershipRepository.GetAllActiveForOgdExport().ToArray();

        var membershipRawData =
            _cubeRawDataService.CreateTriples(
                graph,
                OgdExportConstants.UriMembership,
                membershipData.Select(MembershipMapper.ToObservation));

        var interestData = await _interestRepository.GetAllForOgdExport();

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
                functionStatisticData.Select(OgdFunctionStatisticMapper.ToFunctionStatisticObservation));

        var cantonStatisticData = await _membershipService.GetMembershipsForCantonStatistic(membershipData);

        var committeeCantonStatisticRawData =
            _cubeRawDataService.CreateTriples(
                graph,
                OgdExportConstants.UriCommitteeCantonStatistic,
                cantonStatisticData.Select(OgdCantonStatisticMapper.ToCantonStatisticObservation));

        var committeeGenderLanguageStatisticData = _membershipService.GetMembershipsForGenderLanguageStatistic(membershipData);

        var departmentGenderLanguageStatisticData = _membershipService.GetMembershipsForCommitteeTypeAndDepartmentGenderLanguageStatistic(membershipData);

        var extraAndNonExtraParliamentaryCommissions = _membershipService.GetExtraAndNonExtraParliamentaryCommitteesStatistic(membershipData);

        var genderLanguageStatisticData = committeeGenderLanguageStatisticData.Concat(departmentGenderLanguageStatisticData).Concat(extraAndNonExtraParliamentaryCommissions);

        var committeeGenderLanguageStatisticRawData =
            _cubeRawDataService.CreateTriples(
                graph,
                OgdExportConstants.UriCommitteeGenderLanguageStatistic,
                genderLanguageStatisticData.Select(OgdGenderLanguageStatisticMapper.ToGenderLanguageStatisticObservation));

        var committeeTypeStatisticData = await _committeeService.GetCommitteeTypeStatistic();

        var committeeTypeStatisticDataRawData =
            _cubeRawDataService.CreateTriples(
                graph,
                OgdExportConstants.UriCommitteeTypeStatistic,
                committeeTypeStatisticData.Select(OgdCommitteeTypeStatisticMapper.ToCommitteeTypeStatisticObservation));

        var committeeTypeDepartmentStatisticData = await _committeeService.GetCommitteeTypeDepartmentStatistic();

        var committeeTypeDepartmentStatisticRawData =
            _cubeRawDataService.CreateTriples(
                graph,
                OgdExportConstants.UriCommitteeTypeDepartmentStatistic,
                committeeTypeDepartmentStatisticData.Select(OgdCommitteeTypeDepartmentStatisticMapper.ToCommitteeTypeDepartmentStatisticObservation));

        var chunks = committeeTypeTriples
            .Concat(functionTriples)
            .Concat(interestFunctionTriples)
            .Concat(interestCommitteeTriples)
            .Concat(occupationTriples)
            .Concat(cantonTriples)
            .Concat(contactPointTypeTriples)
            .Concat(contactPointTriples)
            .Concat(committeeTriples)
            .Concat(committeeCube)
            .Concat(committeeCubeMetadata)
            .Concat(personTriples)
            .Concat(personCube)
            .Concat(personCubeMetadata)
            .Concat(membershipRawData)
            .Concat(membershipMetadataTriples)
            .Concat(interestMetadataTriples)
            .Concat(committeeFunctionStatisticCubeMetadata)
            .Concat(committeeCantonStatisticCubeMetadata)
            .Concat(committeeGenderLanguageStatisticCubeMetadata)
            .Concat(committeeTypeStatisticCubeMetadata)
            .Concat(committeeTypeDepartmentStatisticCubeMetadata)
            .Concat(interestRawData)
            .Concat(committeeFunctionStatisticRawData)
            .Concat(committeeCantonStatisticRawData)
            .Concat(committeeGenderLanguageStatisticRawData)
            .Concat(committeeTypeStatisticDataRawData)
            .Concat(committeeTypeDepartmentStatisticRawData)
            .Concat(appointmentDecisionTriples)
            .Chunk(10000);

        // Delete graph on all targets before updating with new data
        foreach (var target in _storageProviders.Keys.OrderBy(k => k))
        {
            try
            {
                var storageProvider = _storageProviders[target];
                var graphUri = _targetsOptions.Targets[target].GraphName;

                _logger.LogInformation("OGD export: Deleting graph on target {TargetName}", target);
                await storageProvider.DeleteGraphAsync(graphUri, ct);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "OGD export: Error while deleting graph on target {TargetName}", target);
            }
        }

        // Update graph on all targets in chunks
        var current = 0;
        foreach (var chunk in chunks)
        {
            current++;
            foreach (var target in _storageProviders.Keys.OrderBy(k => k))
            {
                try
                {
                    var storageProvider = _storageProviders[target];
                    var graphUri = _targetsOptions.Targets[target].GraphName;

                    _logger.LogInformation("OGD export: Updating data on target {TargetName}. Chunk: {Current}", target, current);
                    await storageProvider.UpdateGraphAsync(graphUri, chunk, [], ct);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "OGD export: Error while updating data on target {TargetName}. Chunk: {Current}", target, current);
                }
            }
        }

        // Note:
        // According to MiLu (meeting from 03.02.2026 with Pascal) the detailed canton statistic is not needed in the current version of the OGD export.
        // Code stays here commented out for the time being, in case it is needed again in the future.

        //  var cantonDetailStatisticData = await _membershipService.GetMembershipsForDetailedCantonStatistic(membershipData);
        //
        //  var committeeCantonDetailStatisticRawData =
        //      _cubeRawDataService.CreateTriples(
        //          graph,
        //          OgdExportConstants.UriCommitteeCantonDetailStatistic,
        //          cantonDetailStatisticData.Select(OgdMapper.ToCantonDetailStatisticObservation));
        //
        //  var additionalTriples = committeeCantonDetailStatisticRawData
        //      .Concat(committeeCantonDetailStatisticMetadataTriples);
        //
        // _logger.LogInformation("OGD export: updating detailed statistic");
        // await _storageProvider.UpdateGraphAsync(_sparqlOptions.ExportGraphName, additionalTriples, [], ct);

        _logger.LogInformation("OGD export: completed successfully");
    }

    private List<Triple> CreateCommitteeDimension(Graph graph, Committee[] committees)
    {
        var committeeTriples = new List<Triple>();

        committeeTriples.AddRange(
            _dimensionService.CreateTriples(
                committees.Select(CommitteeMapper.ToDimensionItem),
                graph,
                $"{_sparqlOptions.ExportGraphBaseUri}/{OgdExportConstants.NamespaceCommittee}",
                [new Literal("Behördenkommissionen", "de")]));

        foreach (var committee in committees)
        {
            var committeeUri = $"{OgdExportConstants.NamespaceCommittee}:{committee.OgdId}";
            var homepageUri = $"{committeeUri}/homepage";

            if (!string.IsNullOrWhiteSpace(committee.LinkHomepageGerman))
            {
                committeeTriples.Add(new Triple(graph.CreateUriNode(committeeUri), graph.CreateUriNode(OgdExportConstants.SchemaWebpage), graph.CreateUriNode(homepageUri)));
                committeeTriples.Add(new Triple(graph.CreateUriNode(homepageUri), graph.CreateUriNode(OgdExportConstants.RdfType), graph.CreateUriNode(OgdExportConstants.SchemaWebpage)));
                committeeTriples.Add(new Triple(graph.CreateUriNode(homepageUri), graph.CreateUriNode(OgdExportConstants.SchemaUrl), graph.CreateLiteralNode(committee.LinkHomepageGerman, "de")));
                committeeTriples.Add(new Triple(graph.CreateUriNode(homepageUri), graph.CreateUriNode(OgdExportConstants.SchemaInLanguage), graph.CreateLiteralNode("de")));
                committeeTriples.Add(new Triple(graph.CreateUriNode(homepageUri), graph.CreateUriNode(OgdExportConstants.SchemaAdditionalType), graph.CreateUriNode($"{OgdExportConstants.LdWebsiteType}/Homepage")));
            }

            if (!string.IsNullOrWhiteSpace(committee.LinkHomepageFrench))
            {
                committeeTriples.Add(new Triple(graph.CreateUriNode(committeeUri), graph.CreateUriNode(OgdExportConstants.SchemaWebpage), graph.CreateUriNode(homepageUri)));
                committeeTriples.Add(new Triple(graph.CreateUriNode(homepageUri), graph.CreateUriNode(OgdExportConstants.RdfType), graph.CreateUriNode(OgdExportConstants.SchemaWebpage)));
                committeeTriples.Add(new Triple(graph.CreateUriNode(homepageUri), graph.CreateUriNode(OgdExportConstants.SchemaUrl), graph.CreateLiteralNode(committee.LinkHomepageFrench, "fr")));
                committeeTriples.Add(new Triple(graph.CreateUriNode(homepageUri), graph.CreateUriNode(OgdExportConstants.SchemaInLanguage), graph.CreateLiteralNode("fr")));
                committeeTriples.Add(new Triple(graph.CreateUriNode(homepageUri), graph.CreateUriNode(OgdExportConstants.SchemaAdditionalType), graph.CreateUriNode($"{OgdExportConstants.LdWebsiteType}/Homepage")));
            }

            if (!string.IsNullOrWhiteSpace(committee.LinkHomepageItalian))
            {
                committeeTriples.Add(new Triple(graph.CreateUriNode(committeeUri), graph.CreateUriNode(OgdExportConstants.SchemaWebpage), graph.CreateUriNode(homepageUri)));
                committeeTriples.Add(new Triple(graph.CreateUriNode(homepageUri), graph.CreateUriNode(OgdExportConstants.RdfType), graph.CreateUriNode(OgdExportConstants.SchemaWebpage)));
                committeeTriples.Add(new Triple(graph.CreateUriNode(homepageUri), graph.CreateUriNode(OgdExportConstants.SchemaUrl), graph.CreateLiteralNode(committee.LinkHomepageItalian, "it")));
                committeeTriples.Add(new Triple(graph.CreateUriNode(homepageUri), graph.CreateUriNode(OgdExportConstants.SchemaInLanguage), graph.CreateLiteralNode("it")));
                committeeTriples.Add(new Triple(graph.CreateUriNode(homepageUri), graph.CreateUriNode(OgdExportConstants.SchemaAdditionalType), graph.CreateUriNode($"{OgdExportConstants.LdWebsiteType}/Homepage")));
            }

            if (!string.IsNullOrWhiteSpace(committee.LinkHomepageRomansh))
            {
                committeeTriples.Add(new Triple(graph.CreateUriNode(committeeUri), graph.CreateUriNode(OgdExportConstants.SchemaWebpage), graph.CreateUriNode(homepageUri)));
                committeeTriples.Add(new Triple(graph.CreateUriNode(homepageUri), graph.CreateUriNode(OgdExportConstants.RdfType), graph.CreateUriNode(OgdExportConstants.SchemaWebpage)));
                committeeTriples.Add(new Triple(graph.CreateUriNode(homepageUri), graph.CreateUriNode(OgdExportConstants.SchemaUrl), graph.CreateLiteralNode(committee.LinkHomepageRomansh, "rm")));
                committeeTriples.Add(new Triple(graph.CreateUriNode(homepageUri), graph.CreateUriNode(OgdExportConstants.SchemaInLanguage), graph.CreateLiteralNode("rm")));
                committeeTriples.Add(new Triple(graph.CreateUriNode(homepageUri), graph.CreateUriNode(OgdExportConstants.SchemaAdditionalType), graph.CreateUriNode($"{OgdExportConstants.LdWebsiteType}/Homepage")));
            }

            if (!string.IsNullOrWhiteSpace(committee.LinkAuthorityWebsite))
            {
                var linkUri = $"{OgdExportConstants.NamespaceCommittee}:{committee.OgdId}/authorityWebsite";
                committeeTriples.Add(new Triple(graph.CreateUriNode(committeeUri), graph.CreateUriNode(OgdExportConstants.SchemaWebpage), graph.CreateUriNode(linkUri)));
                committeeTriples.Add(new Triple(graph.CreateUriNode(linkUri), graph.CreateUriNode(OgdExportConstants.RdfType), graph.CreateUriNode(OgdExportConstants.SchemaWebpage)));
                committeeTriples.Add(new Triple(graph.CreateUriNode(linkUri), graph.CreateUriNode(OgdExportConstants.SchemaUrl), graph.CreateLiteralNode(committee.LinkAuthorityWebsite, new Uri(OgdExportConstants.SchemaAnyUri))));
                committeeTriples.Add(new Triple(graph.CreateUriNode(linkUri), graph.CreateUriNode(OgdExportConstants.SchemaAdditionalType), graph.CreateUriNode($"{OgdExportConstants.LdWebsiteType}/AuthorityWebsite")));
            }
        }

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
                [new AdditionalLiteralProperty(OgdExportConstants.SchemaUrl, new Literal(documentUrl, new Uri(OgdExportConstants.SchemaAnyUri)))]
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

    private IEnumerable<Triple> CreatePersonDimension(Graph graph, IEnumerable<Person> people)
    {
        var personTriples =
            _dimensionService.CreateTriples(
                people.Select(PersonMapper.ToDimensionItem),
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

    private async Task<IEnumerable<Triple>> CreateCommitteeTypeDimension(Graph graph)
    {
        var committeeTypes = (await _committeeTypeRepository.GetList()).Where(x => x.Id != CommitteeType.CrossBorderFederalAgenciesCommitteeGuid);

        var committeeTypeTriples =
            _dimensionService.CreateTriples(
                committeeTypes.Select(MasterDataMapper.ToDimensionItem),
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

            var triples = new List<Triple>
            {
                new(graph.CreateUriNode(contactPointUri), graph.CreateUriNode(OgdExportConstants.RdfType), graph.CreateUriNode(OgdExportConstants.SchemaGovernmentOrganization)),
                new(graph.CreateUriNode(contactPointUri), graph.CreateUriNode(OgdExportConstants.SchemaAddress), graph.CreateUriNode(addressUri)),
                new(graph.CreateUriNode(addressUri), graph.CreateUriNode(OgdExportConstants.RdfType), graph.CreateUriNode(OgdExportConstants.SchemaPostalAddress)),
            };

            if (!string.IsNullOrWhiteSpace(cp.CompanyName))
            {
                triples.Add(new Triple(graph.CreateUriNode(contactPointUri), graph.CreateUriNode(OgdExportConstants.SchemaName), graph.CreateLiteralNode(cp.CompanyName)));
            }

            if (!string.IsNullOrWhiteSpace(cp.Street))
            {
                triples.Add(new Triple(graph.CreateUriNode(addressUri), graph.CreateUriNode(OgdExportConstants.SchemaStreetAddress), graph.CreateLiteralNode(cp.Street)));
            }

            if (!string.IsNullOrWhiteSpace(cp.PoBox))
            {
                triples.Add(new Triple(graph.CreateUriNode(addressUri), graph.CreateUriNode(OgdExportConstants.SchemaPostOfficeBoxNumber), graph.CreateLiteralNode(cp.PoBox)));
            }

            if (!string.IsNullOrWhiteSpace(cp.Zip))
            {
                triples.Add(new Triple(graph.CreateUriNode(addressUri), graph.CreateUriNode(OgdExportConstants.SchemaPostalCode), graph.CreateLiteralNode(cp.Zip)));
            }

            if (!string.IsNullOrWhiteSpace(cp.City))
            {
                triples.Add(new(graph.CreateUriNode(addressUri), graph.CreateUriNode(OgdExportConstants.SchemaAddressLocality), graph.CreateLiteralNode(cp.City)));
            }

            if (cp.ReleasePersonData)
            {
                triples.Add(new Triple(graph.CreateUriNode(contactPointUri), graph.CreateUriNode(OgdExportConstants.SchemaMember), graph.CreateUriNode(memberUri)));

                if (!string.IsNullOrWhiteSpace(cp.Surname))
                {
                    triples.Add(new Triple(graph.CreateUriNode(memberUri), graph.CreateUriNode(OgdExportConstants.SchemaFamilyName), graph.CreateLiteralNode(cp.Surname)));
                }

                if (!string.IsNullOrWhiteSpace(cp.GivenName))
                {
                    triples.Add(new Triple(graph.CreateUriNode(memberUri), graph.CreateUriNode(OgdExportConstants.SchemaGivenName), graph.CreateLiteralNode(cp.GivenName)));
                }

                if (!string.IsNullOrWhiteSpace(cp.Title))
                {
                    triples.Add(new Triple(graph.CreateUriNode(memberUri), graph.CreateUriNode(OgdExportConstants.SchemaTitle), graph.CreateLiteralNode(cp.Title)));
                }

                if (!string.IsNullOrWhiteSpace(cp.PersonalEmail))
                {
                    triples.Add(new Triple(
                        graph.CreateUriNode(memberUri),
                        graph.CreateUriNode(OgdExportConstants.SchemaEmail),
                        graph.CreateLiteralNode(cp.PersonalEmail)
                    ));
                }

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

            if (!string.IsNullOrWhiteSpace(cp.Email))
            {
                triples.Add(new Triple(
                    graph.CreateUriNode(contactPointUri),
                    graph.CreateUriNode(OgdExportConstants.SchemaEmail),
                    graph.CreateLiteralNode(cp.Email)
                ));
            }

            if (!string.IsNullOrWhiteSpace(cp.Phone))
            {
                triples.Add(new Triple(
                    graph.CreateUriNode(contactPointUri),
                    graph.CreateUriNode(OgdExportConstants.SchemaTelephone),
                    graph.CreateLiteralNode(cp.Phone)
                ));
            }

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

    private static List<Triple> CreateMetaDataTriples(Graph graph, string uri, string createDate, string publishDate, string schemaName, string schemaDescription)
    {
        var list = new List<Triple>
        {
            new(
                graph.CreateUriNode(uri),
                graph.CreateUriNode(OgdExportConstants.SchemaPublisher),
                graph.CreateUriNode(OgdExportConstants.LdFCh)),
            new(
                graph.CreateUriNode(uri),
                graph.CreateUriNode(OgdExportConstants.SchemaPublisher),
                graph.CreateUriNode(OgdExportConstants.LdFCh)),
            new(
                graph.CreateUriNode(uri),
                graph.CreateUriNode(OgdExportConstants.SchemaCreator),
                graph.CreateUriNode(OgdExportConstants.LdFCh)),
            new(
                graph.CreateUriNode(uri),
                graph.CreateUriNode(OgdExportConstants.SchemaContactPoint),
                graph.CreateUriNode(OgdExportConstants.LdFCh)),
            new(
                graph.CreateUriNode(uri),
                graph.CreateUriNode(OgdExportConstants.SchemaContributor),
                graph.CreateUriNode(OgdExportConstants.LdFCh)),
            new(
                graph.CreateUriNode(uri),
                graph.CreateUriNode(OgdExportConstants.SchemaDateCreated),
                graph.CreateLiteralNode(createDate, UriFactory.Create(XmlSpecsHelper.XmlSchemaDataTypeDate))),
            new(
                graph.CreateUriNode(uri),
                graph.CreateUriNode(OgdExportConstants.SchemaDatePublished),
                graph.CreateLiteralNode(publishDate, UriFactory.Create(XmlSpecsHelper.XmlSchemaDataTypeDate))),
            new(
                graph.CreateUriNode(uri),
                graph.CreateUriNode(OgdExportConstants.SchemaDateModified),
                graph.CreateLiteralNode(DateTime.Now.ToString("yyyy-MM-dd"), UriFactory.Create(XmlSpecsHelper.XmlSchemaDataTypeDate))),
            new(
                graph.CreateUriNode(uri),
                graph.CreateUriNode(OgdExportConstants.SchemaName),
                graph.CreateLiteralNode(schemaName)),
            new(
                graph.CreateUriNode(uri),
                graph.CreateUriNode(OgdExportConstants.SchemaDescription),
                graph.CreateLiteralNode(schemaDescription))
        };

        return list;
    }
}
