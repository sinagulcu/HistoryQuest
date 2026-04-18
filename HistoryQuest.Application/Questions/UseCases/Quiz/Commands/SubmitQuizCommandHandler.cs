

using HistoryQuest.Application.Credits.UseCases;
using HistoryQuest.Application.Questions.Interfaces;
using HistoryQuest.Domain.Entities;
using HistoryQuest.Domain.Exceptions;
using HistoryQuest.Domain.Models;

namespace HistoryQuest.Application.Questions.UseCases.Quiz.Commands;

public class SubmitQuizCommandHandler
{
    private readonly IQuizRepository _quizRepository;
    private readonly IQuizAttemptRepository _attemptRepository;
    private readonly ApplyQuizSettlementCommand _applyQuizSettlementCommand;

    public SubmitQuizCommandHandler(
        IQuizRepository quizRepository,
        IQuizAttemptRepository attemptRepository,
        ApplyQuizSettlementCommand applyQuizSettlementCommand)
    {
        _quizRepository = quizRepository;
        _attemptRepository = attemptRepository;
        _applyQuizSettlementCommand = applyQuizSettlementCommand;
    }

    public async Task<SubmitQuizResponse> Handle(SubmitQuizCommand command)
    {
        if (command.StudentId == Guid.Empty)
            throw new BusinessRuleException("StudentId is required.");

        var quiz = await _quizRepository.GetByIdAsync(command.QuizId)
            ?? throw new NotFoundException("Quiz not found.");

        if (quiz.Status != QuizStatus.Published)
            throw new BusinessRuleException("Quiz not published");

        var attempt = await _attemptRepository.GetActiveAttemptAsync(command.QuizId, command.StudentId)
            ?? throw new BusinessRuleException("Active quiz not found. Quiz must be started");

        var optionMap = quiz.QuizQuestions
            .SelectMany(x => x.Question.Options)
            .ToDictionary(o => o.Id, o => o.IsCorrect);

        var answeredQuestionIds = attempt.Answers.Select(a => a.QuestionId).ToHashSet();

        var incomingDistinct = command.Answers
            .Where(a => !answeredQuestionIds.Contains(a.QuestionId))
            .ToList();

        var newAnswers = incomingDistinct.Select(a => AttemptAnswer.Create(
            attempt.Id,
            a.QuestionId,
            a.SelectedOptionId,
            optionMap.TryGetValue(a.SelectedOptionId, out var isCorrect) && isCorrect
            )).ToList();

        await _attemptRepository.AddAnswersAsync(attempt.Id, newAnswers);

        var refreshed = await _attemptRepository.GetByIdAsync(attempt.Id)
            ?? throw new NotFoundException("Attempt not found.");

        var totalQuestionCount = refreshed.TotalQuestions;
        var answeredCount = refreshed.Answers.Count;
        var correctCount = refreshed.Answers.Count(a => a.IsCorrect);
        var wrongCount = Math.Max(answeredCount - correctCount, 0);

        if (answeredCount < totalQuestionCount)
        {
            return new SubmitQuizResponse
            {
                AttemptId = refreshed.Id,
                Score = 0,
                CreditDelta = 0,
                TotalQuestions = totalQuestionCount,
                AnsweredQuestions = answeredCount,
                IsCompleted = false,
            };
        }

        var completeAffected = await _attemptRepository.CompleteAttemptAsync(refreshed.Id, 0);
        if (completeAffected == 0)
            throw new BusinessRuleException("Quiz attempt already completed or not found.");

        var creditDelta = await _applyQuizSettlementCommand.ExecuteAsync(
            quizId: command.QuizId,
            studentId: command.StudentId,
            attemptId: refreshed.Id,
            totalQuestionCount: totalQuestionCount,
            correctCount: correctCount,
            wrongCount: totalQuestionCount - correctCount
        );

        await _attemptRepository.UpdateScoreAsync(refreshed.Id, creditDelta);
        await _attemptRepository.MarkSettledAsync(refreshed.Id);


        return new SubmitQuizResponse
        {
            AttemptId = refreshed.Id,
            Score = creditDelta,
            CreditDelta = creditDelta,
            TotalQuestions = totalQuestionCount,
            AnsweredQuestions = totalQuestionCount,
            IsCompleted = true
        };

    }
}

public class SubmitQuizResponse
{
    public Guid AttemptId { get; set; }
    public int Score { get; set; }
    public int CreditDelta { get; set; }
    public int TotalQuestions { get; set; }

    public int AnsweredQuestions { get; set; }
    public bool IsCompleted { get; set; }
}
