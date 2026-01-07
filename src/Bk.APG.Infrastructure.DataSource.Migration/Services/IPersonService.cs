using Microsoft.Data.SqlClient;

namespace Bk.APG.Infrastructure.DataSource.Migration.Services;

public interface IPersonService
{
    Task MigratePersons(SqlConnection connection);
}
