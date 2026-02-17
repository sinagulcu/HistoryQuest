

using HistoryQuest.Application.Questions.Interfaces;
using HistoryQuest.Domain.Entities;
using System.Runtime.CompilerServices;

namespace HistoryQuest.Application.Questions.UseCases;

public class GetMyQuestionsQuery
{
    private readonly IQuestionRepository _repository;

    public GetMyQuestionsQuery(IQuestionRepository repository)
    {
        _repository = repository;
    }

    public async Task<List<Question>> ExecuteAsync(Guid teacherId)
    {
        return await _repository.GetByTeacherIdAsync(teacherId);
    }
}
