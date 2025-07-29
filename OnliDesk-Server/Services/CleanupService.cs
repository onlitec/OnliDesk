using Microsoft.EntityFrameworkCore;
using OnliDesk.Server.Data;

namespace OnliDesk.Server.Services;

public class CleanupService : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<CleanupService> _logger;
    private readonly TimeSpan _interval = TimeSpan.FromMinutes(5);

    public CleanupService(IServiceProvider serviceProvider, ILogger<CleanupService> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await CleanupInactiveClientsAsync();
                await Task.Delay(_interval, stoppingToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during cleanup service execution");
                await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
            }
        }
    }

    private async Task CleanupInactiveClientsAsync()
    {
        using var scope = _serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<OnliDeskDbContext>();

        var cutoffTime = DateTime.UtcNow.AddMinutes(-30); // 30 minutes timeout

        var inactiveClients = await context.Clients
            .Where(c => c.IsOnline && c.LastSeen < cutoffTime)
            .ToListAsync();

        foreach (var client in inactiveClients)
        {
            client.IsOnline = false;
            _logger.LogInformation("Marked client {ClientId} as offline due to inactivity", client.ClientId);
        }

        if (inactiveClients.Any())
        {
            await context.SaveChangesAsync();
        }

        // Clean old connection logs (keep only last 30 days)
        var oldLogsCutoff = DateTime.UtcNow.AddDays(-30);
        var oldLogs = await context.ConnectionLogs
            .Where(l => l.Timestamp < oldLogsCutoff)
            .ToListAsync();

        if (oldLogs.Any())
        {
            context.ConnectionLogs.RemoveRange(oldLogs);
            await context.SaveChangesAsync();
            _logger.LogInformation("Cleaned up {Count} old connection logs", oldLogs.Count);
        }
    }
}
