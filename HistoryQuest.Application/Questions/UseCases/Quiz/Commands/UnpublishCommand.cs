using HistoryQuest.Application.Questions.Interfaces;
using HistoryQuest.Domain.Exceptions;

namespace HistoryQuest.Application.Questions.UseCases.Quiz.Commands;

public class UnpublishCommand
{
    private readonly IQuizRepository _quizRepository;

    public UnpublishCommand(IQuizRepository quizRepository)
    {
        _quizRepository = quizRepository;
    }

    public async Task ExecuteAsync(Guid quizId, Guid teacherId)
    {
        var quiz = await _quizRepository.GetByIdAsync(quizId);
        if (quiz == null) throw new NotFoundException("Quiz Not Found");

        if (quiz.CreatedByTeacherId != teacherId)
            throw new UnauthorizedException("You are not authorized to unpublish this quiz.");

        quiz.Unpublish();
        await _quizRepository.SaveChangesAsync();

    }
}
