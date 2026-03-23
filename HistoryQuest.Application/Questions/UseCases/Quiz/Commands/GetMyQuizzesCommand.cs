using HistoryQuest.Application.Questions.Interfaces;

namespace HistoryQuest.Application.Questions.UseCases.Quiz.Commands;

public class GetMyQuizzesCommand
{
    private readonly IQuizRepository _repository;

    public GetMyQuizzesCommand(IQuizRepository repository)
    {
        _repository = repository;
    }

    public async Task<List<Domain.Entities.Quiz>> ExecuteAsync(Guid teacherId, bool includeDeleted)
    {
        return await _repository.GetByTeacherIdAsync(teacherId, includeDeleted);
    }
}
