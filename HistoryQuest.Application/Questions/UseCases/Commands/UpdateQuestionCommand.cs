using HistoryQuest.Application.Questions.DTOs;
using HistoryQuest.Application.Questions.Interfaces;
using HistoryQuest.Domain.Entities;

namespace HistoryQuest.Application.Questions.UseCases.Commands;

public class UpdateQuestionCommand
{
    private readonly IQuestionRepository _repository;

    public UpdateQuestionCommand(IQuestionRepository repository)
    {
        _repository = repository;
    }

    public async Task<bool> ExecuteAsync(
        Guid questionId,
        Guid currentUserId,
        bool isAdmin,
        UpdateQuestionRequest request)
    {
        var question = await _repository.GetByIdAsync(questionId);

        if (question == null)
            return false;

        if (question.CreatedByTeacherId != currentUserId && !isAdmin)
            throw new UnauthorizedAccessException("You cannot update this question.");
        var mappedOptions = request.Options
            .Select(o => new Question.UpdateQuestionOptionRequest(
                o.Id,
                o.Text,
                o.IsCorrect))
            .ToList();

        question.Update(
            request.Text,
            request.Difficulty,
            request.Explanation,
            mappedOptions

            );

        await _repository.SaveChangesAsync();

        return true;
    }
}
