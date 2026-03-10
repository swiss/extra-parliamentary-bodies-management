using Bk.APG.Business.Mapper;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Bk.APG.Business.Services;

public class CantonSyncService : BackgroundService
{
    private readonly ILogger<CantonSyncService> _logger;
    private readonly IServiceProvider _serviceProvider;

    public CantonSyncService(ILogger<CantonSyncService> logger, IServiceProvider serviceProvider)
    {
        _logger = logger;
        _serviceProvider = serviceProvider;
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        return Task.Run(async () =>
        {
            _logger.LogInformation("{BackgroundService} is starting...", nameof(CantonSyncService));

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    using var scope = _serviceProvider.CreateScope();
                    var masterDataCantonService = scope.ServiceProvider.GetRequiredService<Swiss.FCh.MasterData.Services.ICantonService>();

                    _logger.LogDebug("Trying to synchronize cantons...");
                    var cantons = (await masterDataCantonService.GetCantons(stoppingToken)).ToList();

                    if (cantons.Count != 0)
                    {
                        var mapped = cantons.Select(CantonMapper.ToCanton).ToList();
                        mapped.ForEach(item =>
                        {
                            item.Modified = DateTime.UtcNow;
                            item.ModifiedBy = nameof(CantonSyncService);
                        });

                        var cantonService = scope.ServiceProvider.GetRequiredService<ICantonService>();
                        foreach (var item in mapped)
                        {
                            await cantonService.CreateOrUpdate(item);
                        }

                        _logger.LogInformation("{Count} cantons synchronized", cantons.Count);
                    }
                    else
                    {
                        _logger.LogInformation("No cantons to synchronize");
                    }
                }
                catch (Exception e)
                {
                    _logger.LogError(e, "Error while trying to synchronize cantons.");
                }

                await Task.Delay(TimeSpan.FromDays(1), stoppingToken);
            }
        }, stoppingToken);
    }
}
