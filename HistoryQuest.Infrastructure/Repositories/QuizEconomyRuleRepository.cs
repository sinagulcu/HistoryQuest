

using HistoryQuest.Application.Credits.Interfaces;
using HistoryQuest.Domain.Entities;
using HistoryQuest.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace HistoryQuest.Infrastructure.Repositories;

public sealed class QuizEconomyRuleRepository : IQuizEconomyRuleRepository
{
    private readonly HistoryQuestDbContext _db;

    public QuizEconomyRuleRepository(HistoryQuestDbContext db)
    {
        _db = db;
    }

    public async Task AddAsync(QuizEconomyRule rule, CancellationToken cancellationToken = default)
    {
        await _db.Set<QuizEconomyRule>().AddAsync(rule, cancellationToken);
    }

    public Task<QuizEconomyRule?> GetByQuizIdAsync(Guid quizId, CancellationToken cancellationToken = default)
    {
        return _db.Set<QuizEconomyRule>()
            .FirstOrDefaultAsync(x => x.QuizId == quizId, cancellationToken);
    }

    public Task SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return _db.SaveChangesAsync(cancellationToken);
    }
}
