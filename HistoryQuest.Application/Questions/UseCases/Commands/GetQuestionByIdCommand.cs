using HistoryQuest.Application.Questions.Interfaces;
using HistoryQuest.Domain.Entities;

namespace HistoryQuest.Application.Questions.UseCases.Commands;

public class GetQuestionByIdCommand
{
    private readonly IQuestionRepository _repository;

    public GetQuestionByIdCommand(IQuestionRepository repository)
    {
        _repository = repository;
    }

    public async Task<Question?> ExecuteAsync(Guid id)
    {
        return await _repository.GetByIdAsync(id);
    }
}
