using HistoryQuest.Application.Questions.Interfaces;
using HistoryQuest.Domain.Entities;

namespace HistoryQuest.Application.Questions.UseCases.Quiz.Commands;

public class CreateQuizCommand
{
    private readonly IQuizRepository _repository;

    public CreateQuizCommand(IQuizRepository repository)
    {
        _repository = repository;
    }

    public async Task<Guid> ExecuteAsync(string title, string? description, Guid teacherId)
    {
        var quiz = Domain.Entities.Quiz.Create(title, description, teacherId);

        await _repository.AddAsync(quiz);
        await _repository.SaveChangesAsync();

        return quiz.Id;
    }
}
