
using HistoryQuest.Domain.Entities;

namespace HistoryQuest.Application.Questions.Interfaces;

public interface IQuestionRepository
{
    Task AddAsync(Question question);
    Task<Question?> GetByIdAsync(Guid id);
    Task<List<Question>> GetByTeacherIdAsync(Guid teacherId, bool includeDeleted = false);
    Task<Question?> GetByIdIncludingDeletedAsync(Guid id);
    Task SaveChangesAsync();
}
