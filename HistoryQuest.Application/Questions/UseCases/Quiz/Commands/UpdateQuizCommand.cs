

using HistoryQuest.Application.Questions.DTOs.Quiz;
using HistoryQuest.Application.Questions.Interfaces;
using HistoryQuest.Domain.Exceptions;

namespace HistoryQuest.Application.Questions.UseCases.Quiz.Commands;

public class UpdateQuizCommand
{
    private readonly IQuizRepository _quizRepository;

    public UpdateQuizCommand(IQuizRepository quizRepository)
    {
        _quizRepository = quizRepository;
    }

    public async Task ExecuteAsync(Guid quizId, Guid currentUserId, bool isAdmin, UpdateQuizRequest request)
    {
        var quiz = await _quizRepository.GetByIdAsync(quizId);
        if (quiz == null)
            throw new NotFoundException("Quiz not found.");

        if (!isAdmin && quiz.CreatedByTeacherId != currentUserId)
            throw new UnauthorizedException("You do not have permission to update this quiz.");

        quiz.Update(
            request.Title,
            request.Description,
            request.CategoryId,
            request.TimeLimitMinutes);

        await _quizRepository.SaveChangesAsync();
    }
}
