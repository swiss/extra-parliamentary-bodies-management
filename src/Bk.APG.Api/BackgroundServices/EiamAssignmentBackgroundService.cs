using Bk.APG.Business.Models;
using Bk.APG.Infrastructure.DataSource;
using Microsoft.EntityFrameworkCore;

namespace Bk.APG.Api.BackgroundServices;

public class EiamAssignmentBackgroundService : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<EiamAssignmentBackgroundService> _logger;

    public EiamAssignmentBackgroundService(IServiceProvider serviceProvider, ILogger<EiamAssignmentBackgroundService> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken ct)
    {
        _logger.LogInformation("{BackgroundService} is starting...", nameof(EiamAssignmentBackgroundService));

        while (!ct.IsCancellationRequested)
        {
            try
            {
                await ProcessCommitteesAsync(ct);
            }
            catch (OperationCanceledException)
            {
                break;
            }
            finally
            {
                var nextRunTime = DateTime.Today.AddDays(1).AddHours(2);
                var delay = nextRunTime - DateTime.Now;
                await Task.Delay(delay, ct);
            }
        }
    }

    private async Task ProcessCommitteesAsync(CancellationToken ct)
    {
        try
        {
            using var scope = _serviceProvider.CreateScope();
            var dataContext = scope.ServiceProvider.GetRequiredService<DataContext>();

            var committeesWithoutEiamAssignment = await dataContext.Committees
                .Where(c => c.EiamAssignmentId == null)
                .Where(Committee.IsActiveExpression)
                .Include(c => c.Office)
                .ThenInclude(o => o!.EiamAssignment)
                .ToListAsync(ct);

            _logger.LogInformation("Found {Count} committees without EIAM assignment", committeesWithoutEiamAssignment.Count);

            foreach (var committee in committeesWithoutEiamAssignment)
            {
                if (ct.IsCancellationRequested)
                {
                    break;
                }

                var eiamAssignment = new EiamAssignment
                {
                    Id = Guid.NewGuid(),
                    ExternalId = committee.CommitteeNumber.ToString(),
                    Role = Role.Secretariat,
                    CommitteeId = committee.Id,
                    ParentId = committee.Office!.EiamAssignmentId
                };

                dataContext.EiamAssignments.Add(eiamAssignment);
                committee.EiamAssignmentId = eiamAssignment.Id;
            }

            if (committeesWithoutEiamAssignment.Count > 0)
            {
                await dataContext.SaveChangesAsync(ct);
                _logger.LogInformation("Successfully processed {Count} committees and created EIAM assignments", committeesWithoutEiamAssignment.Count);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred during {BackgroundService} EIAM assignment processing", nameof(EiamAssignmentBackgroundService));
        }
    }
}
