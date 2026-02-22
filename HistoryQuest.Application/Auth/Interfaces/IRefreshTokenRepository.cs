
using HistoryQuest.Domain.Entities;

namespace HistoryQuest.Application.Auth.Interfaces;

public interface IRefreshTokenRepository
{
    Task AddAsync(RefreshToken token);
}
