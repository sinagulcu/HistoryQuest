

using HistoryQuest.Application.Questions.Interfaces;
using HistoryQuest.Domain.Entities;
using System.Runtime.CompilerServices;

namespace HistoryQuest.Application.Questions.UseCases;

public class GetMyQuestionsCommand
{
    private readonly IQuestionRepository _repository;

    public GetMyQuestionsCommand(IQuestionRepository repository)
    {
        _repository = repository;
    }

    public async Task<List<Question>> ExecuteAsync(Guid teacherId, bool includeDeleted = false)
    {
        return await _repository.GetByTeacherIdAsync(teacherId, includeDeleted);
    }
}
