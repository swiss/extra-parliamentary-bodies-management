using System.Text.Json;
using System.Text.Json.Serialization;
using Amazon.S3;
using Audit.Core;
using Audit.EntityFramework;
using Bk.APG.Api.BackgroundServices;
using Bk.APG.Business.Connections;
using Bk.APG.Business.Models;
using Bk.APG.Business.Repositories;
using Bk.APG.Business.Services;
using Bk.APG.CrossCutting.Configuration;
using Bk.APG.Infrastructure.DataSource;
using Bk.APG.Infrastructure.DataSource.Repositories;
using Bk.APG.Infrastructure.Service.Post;
using Swiss.FCh.Utils.Converter;
using Configuration = Audit.Core.Configuration;

namespace Bk.APG.Api.Extensions;

public static class ServiceCollectionExtensions
{
    private static IServiceProvider? _rootProvider;

    internal static void AddApgServices(this IServiceCollection services)
    {
        services.AddScoped<ICantonRepository, CantonRepository>();
        services.AddScoped<IPersonRepository, PersonRepository>();
        services.AddScoped<IAddressRepository, AddressRepository>();
        services.AddScoped<IInterestRepository, InterestRepository>();
        services.AddScoped<IMasterDataRepository, MasterDataRepository>();
        services.AddScoped<ICommitteeRepository, CommitteeRepository>();
        services.AddScoped<IContactPointRepository, ContactPointRepository>();
        services.AddScoped<IMembershipRepository, MembershipRepository>();
        services.AddScoped<ICommitteeTypeRepository, CommitteeTypeRepository>();
        services.AddScoped<IAppointmentDecisionRepository, AppointmentDecisionRepository>();
        services.AddScoped<IDocumentStorageRepository, DocumentStorageRepository>();
        services.AddScoped<IWorklistTaskRepository, WorklistTaskRepository>();
        services.AddScoped<ITermOfOfficeDateRepository, TermOfOfficeDateRepository>();
        services.AddScoped<IGeneralElectionCommitteeRepository, GeneralElectionCommitteeRepository>();
        services.AddScoped<IMembershipCandidateRepository, MembershipCandidateRepository>();
        services.AddScoped<IMembershipCandidateLogMessageRepository, MembershipCandidateLogMessageRepository>();
        services.AddScoped<IOccupationRepository, OccupationRepository>();
        services.AddScoped<IEiamAssignmentRepository, EiamAssignmentRepository>();
        services.AddScoped<IGeneralMeasureRepository, GeneralMeasureRepository>();
        services.AddScoped<IEntityAuditLogRepository, EntityAuditLogRepository>();
        services.AddScoped<IApgGeneralSettingsRepository, ApgGeneralSettingsRepository>();
        services.AddScoped<IFormLetterSenderRepository, FormLetterSenderRepository>();
        services.AddScoped<ICountryRepository, CountryRepository>();

        services.AddScoped<ICantonService, CantonService>();
        services.AddScoped<IPersonService, PersonService>();
        services.AddScoped<IInterestService, InterestService>();
        services.AddScoped<IMasterDataService, MasterDataService>();
        services.AddScoped<IAddressService, AddressService>();
        services.AddScoped<IAuthorizationService, AuthorizationService>();
        services.AddScoped<ICommitteeService, CommitteeService>();
        services.AddScoped<IMembershipService, MembershipService>();
        services.AddScoped<IMembershipMirrorService, MembershipMirrorService>();
        services.AddScoped<IContactPointService, ContactPointService>();
        services.AddScoped<IAppointmentDecisionService, AppointmentDecisionService>();
        services.AddScoped<IDocumentService, DocumentService>();
        services.AddScoped<IDataAnalysisService, DataAnalysisService>();
        services.AddScoped<IElectoralListService, ElectoralListService>();
        services.AddScoped<ICommitteeTypeService, CommitteeTypeService>();
        services.AddScoped<ITermOfOfficeDateService, TermOfOfficeDateService>();
        services.AddScoped<IGeneralElectionService, GeneralElectionService>();
        services.AddScoped<IWorklistTaskService, WorklistTaskService>();
        services.AddScoped<IMembershipCandidateService, MembershipCandidateService>();
        services.AddScoped<IGeneralElectionCommitteeService, GeneralElectionCommitteeService>();
        services.AddScoped<IOccupationService, OccupationService>();
        services.AddScoped<IEiamAssignmentService, EiamAssignmentService>();
        services.AddScoped<IReportService, ReportService>();
        services.AddScoped<IGeneralMeasureService, GeneralMeasureService>();
        services.AddScoped<IEntityAuditLogService, EntityAuditLogService>();
        services.AddScoped<IApgGeneralSettingsService, ApgGeneralSettingsService>();
        services.AddScoped<ISalutationGeneratorService, SalutationGeneratorService>();
        services.AddScoped<IOgdDocumentService, OgdDocumentService>();
        services.AddScoped<IFormLetterSenderService, FormLetterSenderService>();
        services.AddScoped<ICountryService, CountryService>();
        services.AddScoped<IFormLetterService, FormLetterService>();
        services.AddScoped<IOpenDataStackService, OpenDataStackService>();
        services.AddScoped<ICompareListService, CompareListService>();

        services.AddScoped<ISparqlClientFactory, SparqlClientFactory>();
        services.AddScoped<IOgdExportService, OgdExportService>();

        services.AddScoped<IPostService, PostService>();

        services.AddScoped<ICultureService, CultureService>();

        services.AddHostedService<CantonSyncService>();
        services.AddHostedService<CountrySyncService>();
        services.AddHostedService<OgdExportBackgroundService>();
        services.AddHostedService<EiamAssignmentBackgroundService>();
        services.AddHostedService<EntityAuditLogCleanupBackgroundService>();
        services.AddHostedService<EndGeneralElectionBackgroundService>();
    }

    internal static void AddEntityAuditLog(this IServiceCollection services, EntityAuditOptions options)
    {
        _rootProvider ??= services.BuildServiceProvider();

        Configuration
            .Setup()
            .AuditDisabled(!options.Enabled)
            .UseEntityFramework(ef => ef
                .AuditTypeMapper(_ => typeof(EntityAuditLog))
                .AuditEntityAction<EntityAuditLog>(MapToEntityAuditLog)
                .IgnoreMatchedProperties());

        Audit.EntityFramework.Configuration
            .Setup()
            .ForContext<DataContext>()
            .UseOptIn()
            .IncludeAny(type => options.Entities.Contains(type.Name));
    }

    private static void MapToEntityAuditLog(AuditEvent _, EventEntry entry, EntityAuditLog entity)
    {
        var serializerOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
        };
        serializerOptions.Converters.Add(new JsonStringEnumConverter());
        serializerOptions.Converters.Add(new DateTimeJsonConverter());

        entity.EntityType = entry.Name;
        entity.EntitySnapshot = JsonSerializer.Serialize(entry.ColumnValues, serializerOptions);
        entity.EntityPrimaryKey = entry.PrimaryKey is { Count: > 0 }
            ? string.Join(";", entry.PrimaryKey.OrderBy(kv => kv.Key).Select(kv => kv.Value?.ToString() ?? "null"))
            : "unknown";
        entity.AuditDate = DateTime.UtcNow;

        using var scope = _rootProvider!.CreateScope();

        entity.AuditUser = (
            entry.ColumnValues.TryGetValue("modified_by", out var value) && value is not null
                ? value.ToString()
                : "system"
        ) ?? "system";

        entity.AuditAction = entry.Action;
        entity.AuditData = entry.Changes is not null
            ? JsonSerializer.Serialize(entry.Changes.Where(x => x.ColumnName is not "modified" and not "modified_by"), serializerOptions)
            : null;
    }

    internal static void AddApgS3Storage(this IServiceCollection services, S3Configuration configuration)
    {
        services.AddS3Storage("apg", configuration.s3_endpoint, configuration.access_key, configuration.secret_access_key);
    }

    internal static void AddOgdS3Storage(this IServiceCollection services, OgdS3Configuration configuration)
    {
        services.AddS3Storage("ogd", configuration.s3_endpoint, configuration.access_key, configuration.secret_access_key);
    }

    private static void AddS3Storage(this IServiceCollection services, string key, string endpoint, string accessKey, string secretAccessKey)
    {
        ArgumentNullException.ThrowIfNull(endpoint);

        var s3Url = !endpoint.StartsWith("http", StringComparison.InvariantCultureIgnoreCase)
            ? $"https://{endpoint}"
            : endpoint;

        services.AddKeyedTransient<IAmazonS3, AmazonS3Client>(key, (_, _) => new AmazonS3Client(accessKey, secretAccessKey, new AmazonS3Config
        {
            ServiceURL = s3Url,
            ForcePathStyle = true
        }));
    }
}
