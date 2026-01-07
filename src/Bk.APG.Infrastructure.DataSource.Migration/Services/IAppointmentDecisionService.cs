using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;

namespace Bk.APG.Infrastructure.DataSource.Migration.Services;

public interface IAppointmentDecisionService
{
    Task MigrateAppointmentDecisions(SqlConnection connection, IConfiguration s3Configuration);
}
