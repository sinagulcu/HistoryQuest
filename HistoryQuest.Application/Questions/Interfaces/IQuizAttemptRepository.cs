

using HistoryQuest.Domain.Entities;

namespace HistoryQuest.Application.Questions.Interfaces;

public interface IQuizAttemptRepository
{
    Task AddAsync(QuizAttempt attempt);
    Task<QuizAttempt?> GetByIdAsync(Guid attemptId);
    Task<List<QuizAttempt>> GetByStudentIdAsync(Guid studentId);
    Task SaveChangesAsync();
}
