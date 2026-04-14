

using HistoryQuest.Domain.Entities;

namespace HistoryQuest.Application.Questions.Interfaces;

public interface IQuizAttemptRepository
{
    Task AddAsync(QuizAttempt attempt);
    Task<QuizAttempt?> GetByIdAsync(Guid attemptId, CancellationToken ct = default);
    Task<List<QuizAttempt>> GetByStudentIdAsync(Guid studentId);

    Task<QuizAttempt?> GetActiveAttemptAsync(Guid quizId, Guid studentId, CancellationToken ct = default);
    Task SaveChangesAsync();
}
