using Microsoft.Data.SqlClient;

namespace Bk.APG.Infrastructure.DataSource.Migration.Services;

public interface IMembershipService
{
    void MigrateMembershipsForCommittee(SqlConnection connection, Guid committeeId);
    Task<IEnumerable<Guid>> GetUniquePersonIdsForAllActiveMemberships();
}
