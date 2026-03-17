using System.Diagnostics;
using Bk.APG.Business.Services;

namespace Bk.APG.Api.BackgroundServices;

public class EndGeneralElectionBackgroundService : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<EndGeneralElectionBackgroundService> _logger;

    public EndGeneralElectionBackgroundService(IServiceProvider serviceProvider, ILogger<EndGeneralElectionBackgroundService> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    protected override Task ExecuteAsync(CancellationToken ct)
    {
        return Task.Run(async () =>
        {
            _logger.LogInformation("{BackgroundService} is starting...", nameof(EndGeneralElectionBackgroundService));

            while (!ct.IsCancellationRequested)
            {
                try
                {
                    _logger.LogInformation("Checking for running GeneralElection to end");

                    using var scope = _serviceProvider.CreateScope();
                    var termOfOfficeDateService = scope.ServiceProvider.GetRequiredService<ITermOfOfficeDateService>();

                    var termOfOfficeDate = await termOfOfficeDateService.GetNextTermOfOfficeDate();

                    // Make sure, this can only happen once! Publication must be planned, due, and not yet published and GE has to be running!
                    if (termOfOfficeDate != null && termOfOfficeDate.PlannedPublicationDate <= DateOnly.FromDateTime(DateTime.Today) && termOfOfficeDate.PublicationDate is null &&
                        termOfOfficeDate.IsGeneralElection == true)
                    {
                        var timer = Stopwatch.StartNew();
                        var generalElectionService = scope.ServiceProvider.GetRequiredService<IGeneralElectionService>();
                        await generalElectionService.EndGeneralElection();
                        timer.Stop();
                        _logger.LogInformation("General election was terminated! Duration: {Duration:g}", timer.Elapsed);
                    }
                    else
                    {
                        _logger.LogInformation("No active general election with matching end date found!");
                    }
                }
                catch (Exception e)
                {
                    _logger.LogError("End GeneralElection failed"); //used for alert by splunk (don't change)
                    _logger.LogError(e, "Error during ending GeneralElection. Error={Message}", e.Message);
                }
                finally
                {
                    var nextRunTime = DateTime.Today.AddDays(1).AddHours(1); //always at 1 a.m.
                    var delay = nextRunTime - DateTime.Now;
                    await Task.Delay(delay, ct);
                }
            }
        }, ct);
    }
}
