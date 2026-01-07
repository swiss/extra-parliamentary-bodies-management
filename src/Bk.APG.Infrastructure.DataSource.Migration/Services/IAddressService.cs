using Microsoft.Data.SqlClient;

namespace Bk.APG.Infrastructure.DataSource.Migration.Services;

public interface IAddressService
{
    void MigrateAddresses(SqlConnection connection);
    Task VerifyAddresses();
}
