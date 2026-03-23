

using HistoryQuest.Application.Questions.Interfaces;
using HistoryQuest.Domain.Entities;
using HistoryQuest.Domain.Exceptions;
using HistoryQuest.Domain.Models;

namespace HistoryQuest.Application.Questions.UseCases.Quiz.Commands;

public class SubmitQuizCommandHandler
{
    private readonly IQuizRepository _quizRepository;
    private readonly IQuizAttemptRepository _attemptRepository;

    public SubmitQuizCommandHandler(
        IQuizRepository quizRepository,
        IQuizAttemptRepository attemptRepository)
    {
        _quizRepository = quizRepository;
        _attemptRepository = attemptRepository;
    }

    public async Task<SubmitQuizResponse> Handle(SubmitQuizCommand command)
    {
        var quiz = await _quizRepository.GetByIdAsync(command.QuizId);

        if (quiz is null)
            throw new NotFoundException("Quiz not found.");

        if (quiz.Status != QuizStatus.Published)
            throw new BusinessRuleException("Quiz not published");

        var optionMap = quiz.QuizQuestions
            .SelectMany(x => x.Question.Options)
            .ToDictionary(o => o.Id, o => o.IsCorrect);

        var attempt = QuizAttempt.Start(command.QuizId, command.StudentId, quiz.QuizQuestions.Count);

        var answers = command.Answers.Select(a => AttemptAnswer.Create(
            attempt.Id,
            a.QuestionId,
            a.SelectedOptionId,
            optionMap.TryGetValue(a.SelectedOptionId, out var isCorrect) && isCorrect
        )).ToList();

        attempt.Complete(answers);

        await _attemptRepository.AddAsync(attempt);
        await _attemptRepository.SaveChangesAsync();

        return new SubmitQuizResponse
        {
            AttemptId = attempt.Id,
            Score = attempt.Score,
            TotalQuestions = attempt.TotalQuestions,
        };

    }
}

public class SubmitQuizResponse
{
    public Guid AttemptId { get; set; }
    public int Score { get; set; }  
    public int TotalQuestions { get; set; }
}
