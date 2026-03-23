using HistoryQuest.Application.Questions.Interfaces;
using HistoryQuest.Domain.Exceptions;

namespace HistoryQuest.Application.Questions.UseCases.Quiz.Commands;

public class PublishQuizCommand
{
    private readonly IQuizRepository _quizRepository;

    public PublishQuizCommand(IQuizRepository quizRepository)
    {
        _quizRepository = quizRepository;
    }

    public async Task ExecuteAsync(Guid quizId, Guid teacherId)
    {
        var quiz = await _quizRepository.GetByIdAsync(quizId);
        if (quiz == null) throw new NotFoundException("Quiz not found.");

        if (quiz.CreatedByTeacherId != teacherId) throw new UnauthorizedException("You are not authorized to publish this quiz.");

        quiz.Publish();
        await _quizRepository.SaveChangesAsync();
    }
}
