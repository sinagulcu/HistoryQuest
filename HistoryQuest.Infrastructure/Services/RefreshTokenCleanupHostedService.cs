using HistoryQuest.Application.Auth.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace HistoryQuest.Infrastructure.Services;

public class RefreshTokenCleanupHostedService : BackgroundService
{
    private readonly IServiceScopeFactory _serviceScopeFactory;
    private readonly IRefreshTokenCleanupService _cleanupService;
    private readonly ILogger<RefreshTokenCleanupHostedService> _logger;
    private readonly TimeSpan _interval = TimeSpan.FromHours(1);

    public RefreshTokenCleanupHostedService(
        IRefreshTokenCleanupService cleanupService,
        ILogger<RefreshTokenCleanupHostedService> logger,
        IServiceScopeFactory serviceScopeFactory)
    {
        _cleanupService = cleanupService;
        _logger = logger;
        _serviceScopeFactory = serviceScopeFactory;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                using var scope = _serviceScopeFactory.CreateScope();
                var cleanupService = scope.ServiceProvider.GetRequiredService<IRefreshTokenCleanupService>();
                await cleanupService.CleaupExpiredTokensAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while cleaning up expired refresh tokens.");
            }
            await Task.Delay(_interval, stoppingToken);
        }
    }
}
