using System.Net;
using Bk.APG.CrossCutting.Configuration;
using Microsoft.Extensions.Options;
using VDS.RDF.Storage;

namespace Bk.APG.Business.Connections;

public class SparqlClientFactory : ISparqlClientFactory
{
    private readonly SparqlTargetsOptions _targetsOptions;

    public SparqlClientFactory(IOptions<SparqlTargetsOptions> targetsOptions)
    {
        _targetsOptions = targetsOptions.Value;
    }

    public IReadOnlyDictionary<string, IAsyncStorageProvider> GetStorageProviders()
    {
        var providers = new Dictionary<string, IAsyncStorageProvider>();

        foreach (var target in _targetsOptions.Targets.OrderBy(t => t.Key))
        {
            var connector = new SparqlHttpProtocolConnector(target.Value.GraphStoreProtocolEndPoint)
            {
                Timeout = target.Value.RequestTimeoutMs
            };

            if (!string.IsNullOrWhiteSpace(target.Value.Username) && !string.IsNullOrWhiteSpace(target.Value.Password))
            {
                connector.SetCredentials(target.Value.Username, target.Value.Password);
            }

            if (target.Value.Proxy.UseProxy)
            {
                connector.Proxy = new WebProxy(target.Value.Proxy.Address, false);
            }

            providers.Add(target.Key, connector);
        }

        return providers;
    }
}
