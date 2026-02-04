using VDS.RDF.Storage;

namespace Bk.APG.Business.Connections;

public interface ISparqlClientFactory
{
    IReadOnlyDictionary<string, IAsyncStorageProvider> GetStorageProviders();
}
