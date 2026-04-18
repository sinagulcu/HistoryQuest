

using HistoryQuest.Application.Credits.Interfaces;
using HistoryQuest.Domain.Entities;
using HistoryQuest.Domain.Models;
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

    public async Task<bool> HasUserReceivedQuizRewardAsync(Guid userId, Guid quizId, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Set<CreditTransaction>()
            .Where(t => t.UserId == userId)
            .Where(t => t.Type == CreditTransactionType.QuizReward)
            .Where(t => t.ReferenceType == "QuizAttempt" && t.ReferenceId != null)
            .Join(
            _dbContext.QuizAttempts,
            t => t.ReferenceId!.Value,
            a => a.Id,
            (t, a) => new { t, a })
            .AnyAsync(x => x.a.QuizId == quizId, cancellationToken);
    }

    public async Task<int> GetQuizRewardCountForUserAsync(Guid userId, Guid quizId, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Set<CreditTransaction>()
            .Where(t => t.UserId == userId)
            .Where(t => t.Type == CreditTransactionType.QuizReward)
            .Where(t => t.ReferenceType == "QuizAttempt" && t.ReferenceId != null)
            .Join(
                    _dbContext.QuizAttempts,
                    t => t.ReferenceId!.Value,
                    a => a.Id,
                    (t, a) => new { t, a })
            .CountAsync(x => x.a.QuizId == quizId, cancellationToken);
    }
}
