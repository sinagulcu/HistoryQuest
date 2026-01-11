using HistoryQuest.Domain.Entities;

namespace HistoryQuest.Application.Auth.Interfaces;

public interface ITokenService
{
    string GenerateToken(User user);
    DateTime GetExpiration();
}
