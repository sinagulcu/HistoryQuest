
using HistoryQuest.Application.Questions.DTOs;
using HistoryQuest.Domain.Entities;

namespace HistoryQuest.Application.Questions.Interfaces;

public interface IQuestionRepository
{
    Task AddAsync(Question question);
    Task<Question?> GetByIdAsync(Guid id);
    Task<List<Question>> GetAllQuestionsAsync(bool includedDeleted = false);
    Task<List<Question>> GetByTeacherIdAsync(Guid teacherId, bool includeDeleted = false);
    Task<List<QuestionListItemDto>> GetAllForAdminPanelAsync(bool included = false);
    Task<Question?> GetByIdIncludingDeletedAsync(Guid id);
    Task SaveChangesAsync();
}
