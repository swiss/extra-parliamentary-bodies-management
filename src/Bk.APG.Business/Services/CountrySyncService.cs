using Bk.APG.Business.Mapper;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Bk.APG.Business.Services;

public class CountrySyncService : BackgroundService
{
    private readonly ILogger<CountrySyncService> _logger;
    private readonly IServiceProvider _serviceProvider;

    public CountrySyncService(ILogger<CountrySyncService> logger, IServiceProvider serviceProvider)
    {
        _logger = logger;
        _serviceProvider = serviceProvider;
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        return Task.Run(async () =>
        {
            _logger.LogInformation("{BackgroundService} is starting...", nameof(CountrySyncService));

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    using var scope = _serviceProvider.CreateScope();
                    var masterDataCountryService = scope.ServiceProvider.GetRequiredService<MasterData.Services.ICountryService>();

                    var today = DateOnly.FromDateTime(DateTime.Now);

                    _logger.LogDebug("Trying to synchronize countries...");
                    var countries = (await masterDataCountryService.GetCountries(stoppingToken)).ToList();

                    countries = countries.Where(c => c.StartDate < today && (c.EndDate > today || c.EndDate == null)).ToList();

                    if (countries.Count != 0)
                    {
                        var mapped = countries.Select(CountryMapper.ToCountry).ToList();
                        mapped.ForEach(item =>
                        {
                            item.Modified = DateTime.UtcNow;
                            item.ModifiedBy = nameof(CountrySyncService);
                        });

                        var countryService = scope.ServiceProvider.GetRequiredService<ICountryService>();
                        foreach (var item in mapped)
                        {
                            await countryService.CreateOrUpdate(item);
                        }

                        _logger.LogInformation("{Count} countries synchronized", countries.Count);
                    }
                    else
                    {
                        _logger.LogInformation("No countries to synchronize");
                    }
                }
                catch (Exception e)
                {
                    _logger.LogError(e, "Error while trying to synchronize countries. Error: {Message}", e.Message);
                }

                await Task.Delay(TimeSpan.FromDays(1), stoppingToken);
            }
        }, stoppingToken);
    }
}
