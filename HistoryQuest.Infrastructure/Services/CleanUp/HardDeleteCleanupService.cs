using HistoryQuest.Infrastructure.Persistence;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace HistoryQuest.Infrastructure.Services.CleanUp;

public sealed class HardDeleteCleanupService : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly IOptionsMonitor<HardDeleteOptions> _options;
    private readonly ILogger<HardDeleteCleanupService> _logger;

    public HardDeleteCleanupService(
        IServiceScopeFactory scopeFactory,
        IOptionsMonitor<HardDeleteOptions> options,
        ILogger<HardDeleteCleanupService> logger)
    {
        _scopeFactory = scopeFactory;
        _options = options;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("HardDeleteCleanupService started.");

        while (!stoppingToken.IsCancellationRequested)
        {
            var opt = _options.CurrentValue;

            if (opt.Enabled)
            {
                try
                {
                    using var scope = _scopeFactory.CreateScope();
                    var db = scope.ServiceProvider.GetRequiredService<HistoryQuestDbContext>();
                    var policies = scope.ServiceProvider
                        .GetServices<IHardDeletePolicy>()
                        .OrderBy(x => x.Order)
                        .ToList();

                    var disabled = new HashSet<string>(opt.DisabledPolicies, StringComparer.OrdinalIgnoreCase);
                    var totalDeleted = 0;

                    foreach (var policy in policies)
                    {
                        if (disabled.Contains(policy.Key))
                            continue;

                        var retention = opt.RetentionDays.TryGetValue(policy.Key, out var days) ? days : 30;
                        var deleted = await policy.ExecuteAsync(db, DateTime.UtcNow, retention, stoppingToken);
                        totalDeleted += deleted;
                    }

                    if (totalDeleted > 0)
                    {
                        _logger.LogInformation("HardDelete completed. Total deleted: {TotalDeleted}", totalDeleted);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "HardDelete cycle failed.");
                }
            }

            await Task.Delay(TimeSpan.FromMinutes(Math.Max(opt.IntervalMinutes, 1)), stoppingToken);
        }
    }
}