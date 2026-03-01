using HistoryQuest.Application.Auth.Interfaces;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace HistoryQuest.Infrastructure.Services;

public class RefreshTokenCleanupHostedService : BackgroundService
{
    private readonly IRefreshTokenCleanupService _cleanupService;
    private readonly ILogger<RefreshTokenCleanupHostedService> _logger;
    private readonly TimeSpan _interval = TimeSpan.FromHours(1);

    public RefreshTokenCleanupHostedService(
        IRefreshTokenCleanupService cleanupService,
        ILogger<RefreshTokenCleanupHostedService> logger)
    {
        _cleanupService = cleanupService;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await _cleanupService.CleaupExpiredTokensAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while cleaning up expired refresh tokens.");
            }
        }
    }
}
