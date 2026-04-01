using System.Diagnostics;
using Bk.APG.Business.Services;

namespace Bk.APG.Api.BackgroundServices;

public class OgdExportBackgroundService : BackgroundService
{
    private readonly ILogger<OgdExportBackgroundService> _logger;
    private readonly IServiceProvider _serviceProvider;

    public OgdExportBackgroundService(
        ILogger<OgdExportBackgroundService> logger,
        IServiceProvider serviceProvider)
    {
        _logger = logger;
        _serviceProvider = serviceProvider;
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        return Task.Run(async () =>
        {
            _logger.LogInformation("{BackgroundService} is starting...", nameof(OgdExportBackgroundService));

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    _logger.LogInformation("Exporting search results to Triples Graph");

                    using var scope = _serviceProvider.CreateScope();
                    var settingsService = scope.ServiceProvider.GetRequiredService<IApgGeneralSettingsService>();

                    var ogdExportEnabled = await settingsService.GetCurrentOgdExportSetting();

                    if (ogdExportEnabled)
                    {
                        var timer = Stopwatch.StartNew();
                        var exportService = scope.ServiceProvider.GetRequiredService<IOgdExportService>();
                        await exportService.Export(stoppingToken);
                        timer.Stop();
                        _logger.LogInformation("Export to LINDAS finished. Duration: {Duration:g}", timer.Elapsed);
                    }
                    else
                    {
                        _logger.LogInformation("Export to LINDAS is deactivated!");
                    }
                }
                catch (Exception e)
                {
                    _logger.LogError("OGD Export Failed"); //used for alert by splunk (don't change)
                    _logger.LogError(e, "Error during LINDAS export. Error={Message}", e.Message);
                }
                finally
                {
                    var nextRunTime = DateTime.Today.AddDays(1).AddHours(1); //always at 1 a.m.
                    var delay = nextRunTime - DateTime.Now;
                    await Task.Delay(delay, stoppingToken);
                }
            }
        }, stoppingToken);
    }
}
