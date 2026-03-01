using HistoryQuest.Application.Auth.Interfaces;
using Microsoft.Extensions.Logging;

namespace HistoryQuest.Infrastructure.Services;

public class RefreshTokenCleanupService : IRefreshTokenCleanupService
{
    private readonly IRefreshTokenRepository _refreshTokenRepository;
    private readonly ILogger<RefreshTokenCleanupService> _logger;

    public RefreshTokenCleanupService(IRefreshTokenRepository refreshTokenRepository, ILogger<RefreshTokenCleanupService> logger)
    {
        _refreshTokenRepository = refreshTokenRepository;
        _logger = logger;
    }

    public async Task CleaupExpiredTokensAsync()
    {
        var expiredTokens = await _refreshTokenRepository
            .GetAllExpiredOrRevokedAsync();

        foreach (var token in expiredTokens)
        {
            await _refreshTokenRepository.DeleteAsync(token);
        }

        if(expiredTokens.Count > 0)
            _logger.LogInformation("Cleaned up {Count} expired or revoked refresh tokens.", expiredTokens.Count);
    }
}
