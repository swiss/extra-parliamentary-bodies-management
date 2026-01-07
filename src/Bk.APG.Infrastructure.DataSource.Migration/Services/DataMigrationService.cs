using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Bk.APG.Infrastructure.DataSource.Migration.Services;

public class DataMigrationService : BackgroundService
{
    private readonly ILogger<DataMigrationService> _logger;
    private readonly IDatabaseService _databaseService;
    private readonly IAddressService _addressService;
    private readonly IPersonService _personService;
    private readonly ICommitteeService _committeeService;
    private readonly IInterestService _interestService;
    private readonly IAppointmentDecisionService _appointmentDecisionService;
    private readonly DataContext _dataContext;
    private readonly string? _sqlConnectionString;
    private readonly bool _connectionTestOnly;
    private readonly IConfiguration _s3Configuration;

    public DataMigrationService(
    ILogger<DataMigrationService> logger,
    IDatabaseService databaseService,
    IAddressService addressService,
    IPersonService personService,
    ICommitteeService committeeService,
    IInterestService interestService,
    IAppointmentDecisionService appointmentDecisionService,
    DataContext dataContext,
    IConfiguration configuration)
    {
        _logger = logger;
        _databaseService = databaseService;
        _addressService = addressService;
        _personService = personService;
        _committeeService = committeeService;
        _interestService = interestService;
        _appointmentDecisionService = appointmentDecisionService;
        _dataContext = dataContext;

        _logger.LogInformation("Reading SQL-Server db configuration...");

        var sqlserverSection = configuration.GetSection("SqlServerConnectionString");
        _sqlConnectionString = sqlserverSection.GetValue<string>("apg");

        _s3Configuration = configuration.GetSection("S3");

        _connectionTestOnly = configuration.GetValue<bool>("migration:connectionTestOnly");
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await Migrate();
    }

    private async Task Migrate()
    {
        try
        {
            if (!string.IsNullOrWhiteSpace(_sqlConnectionString))
            {
                var connection = await _databaseService.OpenConnection(_sqlConnectionString);

                if (_connectionTestOnly)
                {
                    _logger.LogInformation("Migration mode is set to 'Connection Test Only', skipping actual migration");
                    return;
                }

                // activate, when necessary, if you need to clean a fresh backup from sqlserver!
                _databaseService.PrepareAndFixSourceDatabase(connection);
                await _databaseService.EmptyDatabase();

                _logger.LogInformation("Start data migration.");

                _addressService.MigrateAddresses(connection);
                await _dataContext.SaveChangesAsync();

                await _personService.MigratePersons(connection);
                await _dataContext.SaveChangesAsync();

                _committeeService.MigrateCommittees(connection);

                _interestService.MigrateInterests(connection);
                await _dataContext.SaveChangesAsync();

                await _appointmentDecisionService.MigrateAppointmentDecisions(connection, _s3Configuration);
                await _dataContext.SaveChangesAsync();

                await _databaseService.DataFixesInTargetAfterMigration();

                // takes about 10 Minutes and will be executed by Migration. Once executed, data will be inserted by EF Migration as SQL Statements.
                // _occupationImportService.MigrateOccupationsFromJsonSource();
                // _occupationImportService.MigrateOccupationsFromExcelSource();
                // because of poor data quality we remove at least german duplicates
                // await _occupationImportService.CleanUpGermanDuplicates();

                await _databaseService.CloseConnection(connection);

                _logger.LogInformation("Migration completed successfully, now verifying addresses with PostService.");

                // activate, when necessary
                //await _addressService.VerifyAddresses();
                //await _contactPointService.VerifyContactPointAddresses();

                // There are two ways of verifying the interests!
                // The first one can take up to 3 hours and takes all 12'000 interests. So activate it only on local usage!
                // _interestService.VerifyInterestsWithUid();

                // The second approach takes only the interests of persons, who have at least one active membership (suggestion from Caroline)
                // This takes about one hour.
                // var uniquePersonIds = await _membershipService.GetUniquePersonIdsForAllActiveMemberships();

                // if (uniquePersonIds != null)
                // {
                //     _interestService.VerifyInterestsForActivePersons(uniquePersonIds.ToList());
                // }

                Environment.Exit(0);
            }
            else
            {
                _logger.LogInformation("No connection string to source db found");
            }
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error in Migration");
            throw;
        }
    }
}
