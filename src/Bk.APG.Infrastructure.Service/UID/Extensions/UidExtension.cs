using System.ComponentModel.DataAnnotations;
using System.Net;
using System.ServiceModel;
using Bk.APG.Infrastructure.Service.EndpointBehaviors;
using Bk.APG.Infrastructure.Service.UID.Configuration;
using Bk.APG.Infrastructure.Service.UID.PublicServices;
using Bk.APG.Infrastructure.Service.UID.Service;
using Microsoft.Extensions.DependencyInjection;

namespace Bk.APG.Infrastructure.Service.UID.Extensions;

public static class UidExtension
{
    private const string HttpClientName = "Uid";

    public static void AddUidWebService(this IServiceCollection services, UidConfiguration configuration)
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(configuration);

        if (!Uri.TryCreate(configuration.Url, UriKind.Absolute, out var uri))
        {
            throw new ValidationException($"Invalid UID URL: {configuration.Url}");
        }

        services.AddHttpClient(HttpClientName)
            .ConfigurePrimaryHttpMessageHandler(_ => new HttpClientHandler
            {
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(configuration.Username, configuration.Password)
            })
            .ConfigureHttpClient((_, client) =>
                client.BaseAddress = uri);

        services.AddSingleton(provider =>
            new HttpClientFactoryBehavior(
                HttpClientName,
                provider.GetRequiredService<IHttpMessageHandlerFactory>()
            )
        );

        services.AddTransient<IPublicServices, PublicServicesClient>(provider =>
        {
            var client = new PublicServicesClient(
                PublicServicesClient.EndpointConfiguration.BasicHttpBinding_IPublicServices,
                new EndpointAddress(uri));

            client.Endpoint.EndpointBehaviors.Add(provider.GetRequiredService<HttpClientFactoryBehavior>());

            client.ClientCredentials.UserName.UserName = configuration.Username;
            client.ClientCredentials.UserName.Password = configuration.Password;

            ((BasicHttpBinding)client.Endpoint.Binding).Security.Mode = BasicHttpSecurityMode.Transport;
            ((BasicHttpBinding)client.Endpoint.Binding).Security.Transport.ClientCredentialType = HttpClientCredentialType.Basic;

            return client;
        });

        services.AddTransient<IUidService, UidService>();
    }
}
