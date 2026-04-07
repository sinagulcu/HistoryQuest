

using HistoryQuest.Application.Credits.Interfaces;
using HistoryQuest.Domain.Entities;
using HistoryQuest.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace HistoryQuest.Infrastructure.Repositories;

public sealed class CreditTransactionRepository : ICreditTransactionRepository
{
    private HistoryQuestDbContext _dbContext;

    public CreditTransactionRepository(HistoryQuestDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    public Task<bool> ExistsByIdempotencyKeyAsync(string idempotencyKey, CancellationToken cancellationToken = default)
    {
        return _dbContext.Set<CreditTransaction>()
            .AnyAsync(x => x.IdempotencyKey == idempotencyKey, cancellationToken);
    }

    public async Task AddAsync(CreditTransaction transaction, CancellationToken cancellationToken = default)
    {
        await _dbContext.Set<CreditTransaction>()
            .AddAsync(transaction, cancellationToken);
    }

    public Task<List<CreditTransaction>> GetByUserIdAsync(Guid userId, int take = 50, CancellationToken cancellationToken = default)
    {
        return _dbContext.Set<CreditTransaction>()
            .Where(x => x.UserId == userId)
            .OrderByDescending(x => x.CreatedAt)
            .Take(Math.Max(1, Math.Min(take, 200)))
            .ToListAsync(cancellationToken);
    }
}
