using HistoryQuest.Application.Questions.Interfaces;
using HistoryQuest.Domain.Exceptions;

namespace HistoryQuest.Application.Questions.UseCases.Quiz.Commands;

public class SoftDeleteQuizCommand
{
    private readonly IQuizRepository _quizRepository;

    public SoftDeleteQuizCommand(IQuizRepository quizRepository)
    {
        _quizRepository = quizRepository;
    }

    public async Task ExecuteAsync(Guid quizId, Guid teacherId)
    {
        var quiz = await _quizRepository.GetByIdAsync(quizId);
        if (quiz == null) throw new NotFoundException("Quiz Not Found.");
        if (quiz.CreatedByTeacherId != teacherId) throw new UnauthorizedException("Cannot delete this quiz.");

        quiz.SoftDelete();
        await _quizRepository.SaveChangesAsync();
    }
}
