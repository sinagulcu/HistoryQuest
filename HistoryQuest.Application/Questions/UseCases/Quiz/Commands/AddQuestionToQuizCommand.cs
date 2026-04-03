using HistoryQuest.Application.Questions.Interfaces;
using HistoryQuest.Domain.Entities;
using HistoryQuest.Domain.Exceptions;

namespace HistoryQuest.Application.Questions.UseCases.Quiz.Commands;

public class AddQuestionToQuizCommand
{
    private readonly IQuizRepository _quizRepository;
    private readonly IQuestionRepository _questionRepository;

    public AddQuestionToQuizCommand(IQuizRepository quizRepository, IQuestionRepository questionRepository)
    {
        _quizRepository = quizRepository;
        _questionRepository = questionRepository;
    }

    public async Task ExecuteAsync(Guid quizId, Guid questionId, Guid teacherId)
    {
        var quiz = await _quizRepository.GetByIdAsync(quizId);
        if (quiz == null) throw new NotFoundException("Quiz not found.");

        if(quiz.CreatedByTeacherId != teacherId)
            throw new UnauthorizedException("You cannot modify this quiz.");

        quiz.EnsureEditable();

        var question = await _questionRepository.GetByIdAsync(questionId);
        if (question == null || question.IsDeleted)
            throw new NotFoundException("Question not found or deleted.");

        if (quiz.QuizQuestions.Any(q => q.QuestionId == questionId))
            throw new BusinessRuleException("Question already added to this quiz.");

        var order = quiz.QuizQuestions.Count + 1;

        quiz.QuizQuestions.Add(new QuizQuestion(quizId,questionId,order));
        await _quizRepository.SaveChangesAsync();
    }
}
