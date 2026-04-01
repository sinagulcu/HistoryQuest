

using HistoryQuest.Domain.Entities;

namespace HistoryQuest.Application.Challenges.Interfaces;

public interface IChallengeRepository
{
    Task<List<TimedChallenge>> GetAllAsync(
        Guid currentUserId,
        bool isAdmin,
        bool includeDeleted = false,
        CancellationToken cancellationToken = default);

    Task<TimedChallenge?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    Task AddAsync(TimedChallenge challenge, CancellationToken cancellationToken = default);
    Task UpdateAsync(TimedChallenge challenge, CancellationToken cancellationToken = default);
    Task DeleteAsync(TimedChallenge challenge, CancellationToken cancellationToken = default);
}
