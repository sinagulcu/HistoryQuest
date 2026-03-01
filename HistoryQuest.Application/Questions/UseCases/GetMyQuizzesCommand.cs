

using HistoryQuest.Application.Questions.Interfaces;
using HistoryQuest.Domain.Entities;

namespace HistoryQuest.Application.Questions.UseCases;

public class GetMyQuizzesCommand
{
    private readonly IQuizRepository _repository;

    public GetMyQuizzesCommand(IQuizRepository repository)
    {
        _repository = repository;
    }

    public async Task<List<Quiz>> ExecuteAsync(Guid teacherId, bool includeDeleted)
    {
        return await _repository.GetByTeacherIdAsync(teacherId, includeDeleted);
    }
}
