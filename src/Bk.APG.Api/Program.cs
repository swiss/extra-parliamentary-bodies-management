using System.Text.Json;
using System.Text.Json.Serialization;
using Bk.APG.Api.Extensions;
using Bk.APG.Api.Filters;
using Bk.APG.Business.Policies;
using Bk.APG.Business.Validators;
using Bk.APG.CrossCutting.Configuration;
using Bk.APG.Infrastructure.DataSource;
using Bk.APG.Infrastructure.Service.Post;
using Bk.APG.Infrastructure.Service.UID.Configuration;
using Bk.APG.Infrastructure.Service.UID.Extensions;
using Bk.DocumentService.Client.Extensions;
using Bk.MasterData.Configuration;
using Bk.MasterData.Extensions;
using FluentValidation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Localization;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.OpenApi.Models;
using Serilog;
using SharpGrip.FluentValidation.AutoValidation.Mvc.Extensions;
using Swiss.FCh.Cube.Dimension.Extensions;
using Swiss.FCh.Cube.RawData.Extensions;
using Swiss.FCh.Monitoring.Extensions;
using Swiss.FCh.Utils.Converter;
using Swiss.FCh.Utils.Extensions;
using Swiss.FCh.Utils.Rhos.Extensions;

try
{
    var builder = WebApplication.CreateBuilder(args);

    builder
        .AddSerilog("Bk.APG")
        .AddRhosConfigurations()
        .AddRhosPostgresConfiguration(vaultPath: "/app/vault/pg-database-credentials.json")
        .AddRhosS3Configuration(vaultPath: "/app/vault/s3-credentials.json")
        .AddRhosS3Configuration(vaultPath: "/app/vault/ogd-s3-credentials.json");

    var authenticationOptions = builder.Services.AddValidatedOptions<AuthenticationOptions>(builder.Configuration, AuthenticationOptions.SectionKey)
        .Get<AuthenticationOptions>()!;
    var authorizationOptions = builder.Services.AddValidatedOptions<Bk.APG.CrossCutting.Configuration.AuthorizationOptions>(builder.Configuration, Bk.APG.CrossCutting.Configuration.AuthorizationOptions.SectionKey)
        .Get<Bk.APG.CrossCutting.Configuration.AuthorizationOptions>()!;
    var swaggerOptions = builder.Services
        .AddValidatedOptions<SwaggerOptions>(builder.Configuration, SwaggerOptions.SectionKey)
        .Get<SwaggerOptions>()!;
    var uidOptions = builder.Services
        .AddValidatedOptions<UidConfiguration>(builder.Configuration, UidConfiguration.SectionKey)
        .Get<UidConfiguration>()!;
    builder.Services.AddValidatedOptions<PostConfiguration>(builder.Configuration, PostConfiguration.SectionKey);
    var s3Options = builder.Services
        .AddValidatedOptions<S3Configuration>(builder.Configuration, S3Configuration.SectionKey)
        .Get<S3Configuration>()!;
    var ogdS3Options = builder.Services
        .AddValidatedOptions<OgdS3Configuration>(builder.Configuration, OgdS3Configuration.SectionKey)
        .Get<OgdS3Configuration>()!;
    builder.Services.AddValidatedOptions<FrontendOptions>(builder.Configuration, FrontendOptions.SectionKey);
    var entityAuditOptions = builder.Services
        .AddValidatedOptions<EntityAuditOptions>(builder.Configuration, EntityAuditOptions.SectionKey)
        .Get<EntityAuditOptions>();
    var sparqlOptions = builder.Services
        .AddValidatedOptions<SparqlOptions>(builder.Configuration, SparqlOptions.SectionKey)
        .Get<SparqlOptions>()!;
    builder.Services.AddValidatedOptions<SparqlTargetsOptions>(builder.Configuration, SparqlTargetsOptions.SectionKey);
    builder.Services.AddValidatedOptions<AppointmentDecisionOptions>(builder.Configuration, AppointmentDecisionOptions.SectionKey);

    builder.Services.AddUidWebService(uidOptions);

    builder.Services.AddHttpClient();
    builder.Services.AddHttpContextAccessor();

    builder.Services.AddValidatorsFromAssemblyContaining<CommitteeUpdateValidator>();
    builder.Services.AddFluentValidationAutoValidation();

    builder.Services
        .AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options =>
        {
            options.Authority = authenticationOptions.Url;
            options.Audience = authenticationOptions.ClientId;
            options.TokenValidationParameters.ValidIssuer = authenticationOptions.Url;
            options.TokenValidationParameters.ValidAudiences = [authenticationOptions.ClientId, "account"];
            options.RequireHttpsMetadata = authenticationOptions.IsHttps;
            options.SaveToken = true;

            if (builder.Environment.IsDevelopment())
            {
                options.IncludeErrorDetails = true;
            }
        });
    builder.Services.AddAuthorization(options =>
    {
        var eiamPolicyBuilder = new AuthorizationPolicyBuilder(JwtBearerDefaults.AuthenticationScheme);
        options.AddPolicy(APGPolicies.RequireAllowRole, p => p.RequireRole(authorizationOptions.Allow));
        options.AddPolicy(APGPolicies.RequireObserverRole, p => p.RequireRole(authorizationOptions.Observer));
        options.AddPolicy(APGPolicies.RequireDepartmentRole, p => p.RequireRole(authorizationOptions.Department));
        options.AddPolicy(APGPolicies.RequireOfficeRole, p => p.RequireRole(authorizationOptions.Office));
        options.AddPolicy(APGPolicies.RequireSecretariatRole, p => p.RequireRole(authorizationOptions.Secretariat));
        options.AddPolicy(APGPolicies.RequireAdminRole, p => p.RequireRole(authorizationOptions.Admin));
        options.AddPolicy(APGPolicies.RequireAdminDepartmentOfficeOrSecretariatRole, p => p.RequireRole(authorizationOptions.Admin, authorizationOptions.Secretariat, authorizationOptions.Department, authorizationOptions.Office));
        options.AddPolicy(APGPolicies.RequireAdminDepartmentRole, p => p.RequireRole(authorizationOptions.Admin, authorizationOptions.Department));
    });

    builder.Services
        .AddControllers(options =>
        {
            options.Filters.Add<DbUpdateConcurrencyExceptionFilter>();
            options.Filters.Add<EntityNotFoundExceptionFilter>();
        })
        .AddJsonOptions(options =>
        {
            options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
            options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
            options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
            options.JsonSerializerOptions.Converters.Add(new DateTimeJsonConverter());
            options.JsonSerializerOptions.Converters.Add(new SanitizeStringJsonConverter());
        });

    if (swaggerOptions.Enabled)
    {
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen(config =>
        {
            const string securityScheme = "oauth2";
            config.CustomOperationIds(e => $"{e.ActionDescriptor.RouteValues["controller"]}_{e.RelativePath}_{e.HttpMethod}");
            config.AddSecurityDefinition(securityScheme,
                new OpenApiSecurityScheme
                {
                    Type = SecuritySchemeType.OAuth2,
                    Flows = new OpenApiOAuthFlows
                    {
                        AuthorizationCode = new OpenApiOAuthFlow
                        {
                            AuthorizationUrl = new Uri(authenticationOptions.Url + "/protocol/openid-connect/auth"),
                            TokenUrl = new Uri(authenticationOptions.Url + "/protocol/openid-connect/token"),
                            Scopes = new Dictionary<string, string> { { "openid", "OpenId Scope" } }
                        }
                    }
                });
            config.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                { new OpenApiSecurityScheme { Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = securityScheme } }, ["openid"] }
            });
        });
    }

    builder.Services.AddDbContext<DataContext>(options =>
    {
        if (builder.Environment.IsDevelopment())
        {
            options.ConfigureWarnings(w => w.Throw(RelationalEventId.MultipleCollectionIncludeWarning));
            options.EnableSensitiveDataLogging();
        }

        var connectionString = builder.Configuration.GetPostgresConnectionString();
        options.UseNpgsql(connectionString, o => { o.MigrationsHistoryTable("__EFMigrationsHistory", schema: DataContext.Schema); });
    });

    builder.Services.AddApgS3Storage(s3Options);
    builder.Services.AddOgdS3Storage(ogdS3Options);

    builder.Services.AddApgServices();
    builder.Services.AddMasterDataService(new BkMasterDataSettings { ProxyAddress = sparqlOptions.MasterDataProxy.UseProxy ? sparqlOptions.MasterDataProxy.Address : null });
    builder.Services.AddDocumentService(builder.Configuration);

    builder.Services.AddEntityAuditLog(entityAuditOptions!);

    builder.Services
        .AddHealthChecks()
        .AddDatabase<DataContext>()
        .AddCheck<PostService>("post")
        .AddDocumentService();

    builder.Services.AddDimensionService();
    builder.Services.AddRawDataService();

    var app = builder.Build();

    if (swaggerOptions.Enabled)
    {
        app.UseSwagger(config =>
        {
            config.RouteTemplate = "api/swagger/{documentName}/swagger.{json|yaml}";
        });
        app.UseSwaggerUI(config =>
        {
            config.SwaggerEndpoint("/api/swagger/v1/swagger.json", "BK APG");
            config.RoutePrefix = "api/swagger";

            config.OAuthClientId(authenticationOptions.ClientId);
            config.OAuthScopes("openid");
            config.OAuthUsePkce();
        });
    }

    using var scope = app.Services.CreateScope();
    var db = scope.ServiceProvider.GetRequiredService<DataContext>();
    db.Database.Migrate();

    if (!builder.Environment.IsDevelopment())
    {
        app.UseHttpsRedirection();
    }

    var supportedCultures = new[] { "de", "fr", "it" };
    var localizationOptions = new RequestLocalizationOptions()
        .SetDefaultCulture(supportedCultures[0])
        .AddSupportedCultures(supportedCultures)
        .AddSupportedUICultures(supportedCultures)
        .AddInitialRequestCultureProvider(new AcceptLanguageHeaderRequestCultureProvider());

    app.UseRequestLocalization(localizationOptions);

    app.UseSerilogRequestLogging();

    app.UseRouting();
    app.UseAuthentication();
    app.UseAuthorization();

    app.MapControllers();
    app.MapFChHealthChecks();

    Log.Information("Starting web host: Bk.APG.Api");
    app.Run();

    return 0;
}
catch (Exception ex) when (ex is not HostAbortedException)
{
    Log.Fatal(ex, "Host terminated unexpectedly");
    return 1;
}
finally
{
    Log.CloseAndFlush();
}
