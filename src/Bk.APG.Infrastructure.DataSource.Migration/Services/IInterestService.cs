using Microsoft.Data.SqlClient;

namespace Bk.APG.Infrastructure.DataSource.Migration.Services;

public interface IInterestService
{
    void MigrateInterests(SqlConnection connection);
    void VerifyInterestsWithUid();
    void VerifyInterestsForActivePersons(List<Guid> activePersons);
}
