

using HistoryQuest.Application.Questions.Interfaces;
using HistoryQuest.Domain.Entities;

namespace HistoryQuest.Application.Questions.UseCases;

public class GetQuestionByIdQuery
{
    private readonly IQuestionRepository _repository;

    public GetQuestionByIdQuery(IQuestionRepository repository)
    {
        _repository = repository;
    }

    public async Task<Question?> ExecuteAsync(Guid id)
    {
        return await _repository.GetByIdAsync(id);
    }
}
