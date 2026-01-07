using System.Net;
using Bk.APG.CrossCutting.Configuration;
using Microsoft.Extensions.Options;
using VDS.RDF.Storage;

namespace Bk.APG.Business.Connections;

public class ConnectionFactory : IConnectionFactory
{
    private readonly SparqlOptions _sparqlOptions;

    public ConnectionFactory(IOptions<SparqlOptions> sparqlOptions)
    {
        _sparqlOptions = sparqlOptions.Value;
    }

    public IAsyncStorageProvider Create()
    {
        var connector = new SparqlHttpProtocolConnector(_sparqlOptions.Endpoint)
        {
            Timeout = _sparqlOptions.RequestTimeoutMs
        };

        var username = _sparqlOptions.Username;
        var password = _sparqlOptions.Password;

        if (!string.IsNullOrWhiteSpace(username) && !string.IsNullOrWhiteSpace(password))
        {
            connector.SetCredentials(username, password);
        }

        if (_sparqlOptions.ExportProxy.UseProxy)
        {
            connector.Proxy = new WebProxy(_sparqlOptions.ExportProxy.Address, false);
        }

        return connector;
    }
}
