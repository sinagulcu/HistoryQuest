using HistoryQuest.Application.Questions.Interfaces;
using HistoryQuest.Domain.Exceptions;

namespace HistoryQuest.Application.Questions.UseCases.Quiz.Commands;

public class RestoreQuizCommand
{
    private readonly IQuizRepository _quizRepository;

    public RestoreQuizCommand(IQuizRepository quizRepository)
    {
        _quizRepository = quizRepository;
    }

    public async Task ExecuteAsync(Guid quizId, Guid teacherId)
    {
        var quiz = await _quizRepository.GetByIdAsync(quizId);
        if (quiz == null) throw new NotFoundException("Quiz not found.");
        if (quiz.CreatedByTeacherId != teacherId) throw new UnauthorizedException("Cannot restore this quiz.");

        quiz.Restore();
        await _quizRepository.SaveChangesAsync();
    }
}
