using Amazon.S3;
using Bk.APG.Business.Repositories;
using Bk.APG.Business.Services;
using Bk.APG.CrossCutting.Configuration;
using Bk.APG.Infrastructure.DataSource;
using Bk.APG.Infrastructure.DataSource.Migration.Services;
using Bk.APG.Infrastructure.DataSource.Repositories;
using Bk.APG.Infrastructure.Service.Post;
using Bk.APG.Infrastructure.Service.UID.Configuration;
using Bk.APG.Infrastructure.Service.UID.Extensions;
using Bk.APG.Infrastructure.Service.UID.Service;
using Bk.Utils.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using AddressService = Bk.APG.Infrastructure.DataSource.Migration.Services.AddressService;
using AppointmentDecisionService = Bk.APG.Infrastructure.DataSource.Migration.Services.AppointmentDecisionService;
using CommitteeService = Bk.APG.Infrastructure.DataSource.Migration.Services.CommitteeService;
using ContactPointService = Bk.APG.Infrastructure.DataSource.Migration.Services.ContactPointService;
using IAddressService = Bk.APG.Infrastructure.DataSource.Migration.Services.IAddressService;
using IAppointmentDecisionService = Bk.APG.Infrastructure.DataSource.Migration.Services.IAppointmentDecisionService;
using ICommitteeService = Bk.APG.Infrastructure.DataSource.Migration.Services.ICommitteeService;
using IContactPointService = Bk.APG.Infrastructure.DataSource.Migration.Services.IContactPointService;
using IInterestService = Bk.APG.Infrastructure.DataSource.Migration.Services.IInterestService;
using IMembershipService = Bk.APG.Infrastructure.DataSource.Migration.Services.IMembershipService;
using InterestService = Bk.APG.Infrastructure.DataSource.Migration.Services.InterestService;
using IPersonService = Bk.APG.Infrastructure.DataSource.Migration.Services.IPersonService;
using MembershipService = Bk.APG.Infrastructure.DataSource.Migration.Services.MembershipService;
using PersonService = Bk.APG.Infrastructure.DataSource.Migration.Services.PersonService;

var builder = Host.CreateDefaultBuilder(args);

using var host = builder.ConfigureServices((context, services) =>
    {
        Log.Logger = new LoggerConfiguration()
            .ReadFrom.Configuration(context.Configuration)
            .CreateLogger();

        services.AddDbContext<DataContext>(options =>
        {
            options.ConfigureWarnings(w => w.Throw(RelationalEventId.MultipleCollectionIncludeWarning));
            options.EnableSensitiveDataLogging();

            var host = context.Configuration.GetValue<string>("pgsql:hostname");
            var port = context.Configuration.GetValue<string>("pgsql:port");
            var user = context.Configuration.GetValue<string>("pgsql:username");
            var pass = context.Configuration.GetValue<string>("pgsql:password");
            var database = context.Configuration.GetValue<string>("pgsql:database_name");

            // if you have a special char in the passwort, you might have to escape it...
            // var connectionString = $"Host={host};Port={port};Database={database};Timeout=15;Command Timeout=30;SSL Mode=Prefer;Trust Server Certificate=true;User ID={user};Password={pass}";
            var connectionString = $"Host={host};Port={port};Database={database};Timeout=15;Command Timeout=30;SSL Mode=Prefer;Trust Server Certificate=true;User ID={user};Password=\"{pass}\"";

            options.UseNpgsql(connectionString, o => { o.MigrationsHistoryTable("__EFMigrationsHistory", schema: DataContext.Schema); });
        });

        services.AddHttpClient().AddLogging();
        services.AddHttpContextAccessor();

        services.AddValidatedOptions<PostConfiguration>(context.Configuration, PostConfiguration.SectionKey);
        var s3Options = services.AddValidatedOptions<S3Configuration>(context.Configuration, S3Configuration.SectionKey).Get<S3Configuration>()!;

        var uidOptions = services
            .AddValidatedOptions<UidConfiguration>(context.Configuration, UidConfiguration.SectionKey)
            .Get<UidConfiguration>()!;

        services.AddUidWebService(uidOptions);

        var s3Url = s3Options.s3_endpoint;

        if (!s3Url.StartsWith("http"))
        {
            s3Url = $"https://{s3Url}";
            Log.Information("Using S3 endpoint: {endpoint}", s3Url);
        }

        var config = new AmazonS3Config
        {
            ServiceURL = s3Url,
            ForcePathStyle = true
        };
        services.AddKeyedTransient<IAmazonS3, AmazonS3Client>("apg", (_, __) => new AmazonS3Client(s3Options.access_key, s3Options.secret_access_key, config));

        services.AddScoped<ICultureService, CultureService>();
        services.AddScoped<IDatabaseService, DatabaseService>();
        services.AddScoped<IAddressService, AddressService>();
        services.AddScoped<IAddressService, AddressService>();
        services.AddScoped<IPersonService, PersonService>();
        services.AddScoped<IAuthorizationService, AuthorizationService>();
        services.AddScoped<ICommitteeService, CommitteeService>();
        services.AddScoped<IMembershipService, MembershipService>();
        services.AddScoped<IInterestService, InterestService>();
        services.AddScoped<IContactPointService, ContactPointService>();
        services.AddScoped<IAppointmentDecisionService, AppointmentDecisionService>();
        services.AddScoped<ISalutationGeneratorService, SalutationGeneratorService>();
        services.AddScoped<IAddressRepository, AddressRepository>();
        services.AddScoped<IPersonRepository, PersonRepository>();
        services.AddScoped<ICommitteeRepository, CommitteeRepository>();
        services.AddScoped<IMembershipRepository, MembershipRepository>();
        services.AddScoped<IInterestRepository, InterestRepository>();
        services.AddScoped<IContactPointRepository, ContactPointRepository>();
        services.AddScoped<ICantonRepository, CantonRepository>();
        services.AddScoped<IAppointmentDecisionRepository, AppointmentDecisionRepository>();
        services.AddScoped<IPostService, PostService>();
        services.AddScoped<IUidService, UidService>();
        services.AddScoped<ITermOfOfficeDateRepository, TermOfOfficeDateRepository>();
        services.AddScoped<ITermOfOfficeDateService, TermOfOfficeDateService>();
        services.AddScoped<IMasterDataService, MasterDataService>();
        services.AddScoped<IMasterDataRepository, MasterDataRepository>();
        services.AddScoped<IDocumentService, DocumentService>();
        services.AddScoped<IDocumentStorageRepository, DocumentStorageRepository>();
        services.AddScoped<IOccupationImportService, OccupationImportService>();
        services.AddScoped<IOccupationRepository, OccupationRepository>();
        services.AddScoped<IEiamAssignmentRepository, EiamAssignmentRepository>();

        services.AddHostedService<DataMigrationService>();
    }).UseSerilog()
    .ConfigureAppConfiguration((_, config) =>
    {
        var migConfig = new ConfigurationBuilder()
            .AddJsonFile("appsettings.Migration.json")
            .Build();

        config.AddConfiguration(migConfig);

        if (Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Development")
        {
            config.AddUserSecrets(typeof(Program).Assembly);
        }
    })
    .Build();

try
{
    host.Run();
}
finally
{
    Log.CloseAndFlush();
}
