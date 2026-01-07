using Microsoft.Data.SqlClient;

namespace Bk.APG.Infrastructure.DataSource.Migration.Services;

public interface ICommitteeService
{
    void MigrateCommittees(SqlConnection connection);
}
