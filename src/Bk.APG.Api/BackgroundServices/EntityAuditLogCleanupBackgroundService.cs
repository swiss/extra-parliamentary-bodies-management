using Bk.APG.Infrastructure.DataSource;
using Microsoft.EntityFrameworkCore;

namespace Bk.APG.Api.BackgroundServices;

public class EntityAuditLogCleanupBackgroundService : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<EntityAuditLogCleanupBackgroundService> _logger;

    public EntityAuditLogCleanupBackgroundService(IServiceProvider serviceProvider, ILogger<EntityAuditLogCleanupBackgroundService> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("{BackgroundService} is starting...", nameof(EntityAuditLogCleanupBackgroundService));

        while (!cancellationToken.IsCancellationRequested)
        {
            try
            {
                await CleanupEntityAuditLog(cancellationToken);
            }
            catch (OperationCanceledException)
            {
                break;
            }
            finally
            {
                var nextRunTime = DateTime.Today.AddDays(1).AddHours(3);
                var delay = nextRunTime - DateTime.Now;
                await Task.Delay(delay, cancellationToken);
            }
        }
    }

    private async Task CleanupEntityAuditLog(CancellationToken cancellationToken)
    {
        try
        {
            using var scope = _serviceProvider.CreateScope();
            var dataContext = scope.ServiceProvider.GetRequiredService<DataContext>();

            var entityAuditLogsToDelete = await dataContext.EntityAuditLog
                .Where(x => x.AuditDate < DateTime.UtcNow.AddYears(-4))
                .ToListAsync(cancellationToken);

            _logger.LogInformation("Found {Count} entity audit logs to delete", entityAuditLogsToDelete.Count);

            dataContext.EntityAuditLog.RemoveRange(entityAuditLogsToDelete);
            await dataContext.SaveChangesAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred during {BackgroundService}", nameof(EntityAuditLogCleanupBackgroundService));
        }
    }
}
