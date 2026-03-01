
using HistoryQuest.Domain.Entities;

namespace HistoryQuest.Application.Questions.Interfaces;

public interface IQuizRepository
{
    Task AddAsync(Quiz quiz);
    Task <List<Quiz>> GetByTeacherIdAsync(Guid teacherId, bool includeDeleted = false);
    Task SaveChangesAsync();

    Task<Quiz?> GetByIdAsync(Guid quizId);
}
