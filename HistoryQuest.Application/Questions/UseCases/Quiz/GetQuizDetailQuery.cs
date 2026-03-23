using HistoryQuest.Application.Questions.Interfaces;
using HistoryQuest.Domain.Entities;
using System.Runtime.CompilerServices;

namespace HistoryQuest.Application.Questions.UseCases.Quiz;

public class GetQuizDetailQuery
{
    private readonly IQuizRepository _repository;

    public GetQuizDetailQuery(IQuizRepository repository)
    {
        _repository = repository;
    }

    public async Task<Domain.Entities.Quiz?> ExecuteAsync(Guid quizId)
    {
        return await _repository.GetByIdAsync(quizId);
    }
}

