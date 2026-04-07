

using HistoryQuest.Domain.Entities;

namespace HistoryQuest.Application.Credits.Interfaces;

public interface ICreditTransactionRepository
{
    Task<bool> ExistsByIdempotencyKeyAsync(string idempotencyKey, CancellationToken cancellationToken = default);
    Task AddAsync(CreditTransaction transaction, CancellationToken cancellationToken = default);
        Task<List<CreditTransaction>> GetByUserIdAsync(Guid userId, int take = 50,CancellationToken cancellationToken = default);
}
