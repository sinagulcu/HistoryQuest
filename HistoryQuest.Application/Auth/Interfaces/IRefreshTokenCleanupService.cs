

namespace HistoryQuest.Application.Auth.Interfaces;

public interface IRefreshTokenCleanupService
{
    Task CleaupExpiredTokensAsync();
}
