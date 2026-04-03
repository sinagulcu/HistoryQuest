

using HistoryQuest.Application.Users.DTOs;

namespace HistoryQuest.Application.Users.Interfaces;

public interface IUserReadRepository
{
    Task<List<UserListItemDto>> GetAllAsync(bool includeDeleted = false, CancellationToken cancellationToken = default);
    Task<int> GetCountAsync(bool includeDeleted = false, CancellationToken cancellationToken = default);
}
