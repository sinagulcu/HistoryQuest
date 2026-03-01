
using HistoryQuest.Domain.Entities;

namespace HistoryQuest.Application.Auth.Interfaces;

public interface IRefreshTokenRepository
{
    Task AddAsync(RefreshToken token);
    Task<RefreshToken?> GetByTokenAsync(string token);
    Task UpdateAsync(RefreshToken token);
    Task<List<RefreshToken>> GetAllExpiredOrRevokedAsync();
    Task DeleteAsync(RefreshToken token);
}
