using HistoryQuest.Application.Questions.Interfaces;
using HistoryQuest.Domain.Exceptions;

namespace HistoryQuest.Application.Questions.UseCases.Commands;

public class RemoveQuestionFromQuizCommand
{
    private readonly IQuizRepository _quizRepository;

    public RemoveQuestionFromQuizCommand(IQuizRepository quizRepository)
    {
        _quizRepository = quizRepository;
    }

    public async Task ExecuteAsync(Guid quizId, Guid questionId, Guid teacherId)
    {
        var quiz = await _quizRepository.GetByIdAsync(quizId);
        if (quiz == null) throw new NotFoundException("Quiz not found.");
        if (quiz.CreatedByTeacherId != teacherId)
            throw new UnauthorizedException("Cannot modify this quiz.");

        var qq = quiz.QuizQuestions.FirstOrDefault(q => q.QuestionId == questionId);
        if (qq == null) throw new NotFoundException("Question not part of this quiz.");

        quiz.QuizQuestions.Remove(qq);
        await _quizRepository.SaveChangesAsync();
    }

}
