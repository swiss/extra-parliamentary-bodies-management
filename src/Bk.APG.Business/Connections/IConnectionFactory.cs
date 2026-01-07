using VDS.RDF.Storage;

namespace Bk.APG.Business.Connections;

public interface IConnectionFactory
{
    IAsyncStorageProvider Create();
}
