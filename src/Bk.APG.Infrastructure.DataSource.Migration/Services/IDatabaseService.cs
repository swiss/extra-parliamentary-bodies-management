using Microsoft.Data.SqlClient;

namespace Bk.APG.Infrastructure.DataSource.Migration.Services;

public interface IDatabaseService
{
    Task<SqlConnection> OpenConnection(string connectionString);
    Task CloseConnection(SqlConnection connection);
    Task EmptyDatabase();
    void PrepareAndFixSourceDatabase(SqlConnection connection);
    Task DataFixesInTargetAfterMigration();
}
