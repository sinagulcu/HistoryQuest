using HistoryQuest.Application.Questions.DTOs.Quiz;
using HistoryQuest.Application.Questions.Interfaces;

namespace HistoryQuest.Application.Questions.UseCases.Quiz.Commands;

public class CreateQuizCommand
{
    private readonly IQuizRepository _repository;

    public CreateQuizCommand(IQuizRepository repository)
    {
        _repository = repository;
    }

    public async Task<Guid> ExecuteAsync(CreateQuizRequest request, Guid teacherId)
    {
        var quiz = Domain.Entities.Quiz.Create(
            request.Title,
            request.Description,
            teacherId,
            request.CategoryId,
            request.TimeLimitMinutes);

        await _repository.AddAsync(quiz);
        await _repository.SaveChangesAsync();

        return quiz.Id;
    }
}
