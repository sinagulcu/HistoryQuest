

using HistoryQuest.Domain.Entities;

namespace HistoryQuest.Application.Credits.Interfaces;

public interface IQuizEconomyRuleRepository
{
    Task<QuizEconomyRule?> GetByQuizIdAsync(Guid quizId, CancellationToken cancellationToken = default);
    Task AddAsync(QuizEconomyRule rule, CancellationToken cancellationToken = default);
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}
