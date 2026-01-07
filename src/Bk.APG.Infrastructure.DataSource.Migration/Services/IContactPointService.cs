using Microsoft.Data.SqlClient;

namespace Bk.APG.Infrastructure.DataSource.Migration.Services;

public interface IContactPointService
{
    void MigrateContactPointsForCommittee(SqlConnection connection, Guid committeeId);
    Task VerifyContactPointAddresses();
}
