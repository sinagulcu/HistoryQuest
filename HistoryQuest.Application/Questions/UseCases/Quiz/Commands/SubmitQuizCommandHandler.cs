

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

        var answers = command.Answers.Select(a => AttemptAnswer.Create(
            attempt.Id,
            a.QuestionId,
            a.SelectedOptionId,
            optionMap.TryGetValue(a.SelectedOptionId, out var isCorrect) && isCorrect
        )).ToList();

        var score = answers.Count(a => a.IsCorrect);
        var totalQuestionCount = attempt.TotalQuestions;
        var correctCount = score;
        var wrongCount = Math.Max(totalQuestionCount - correctCount, 0);

        var completeAffected = await _attemptRepository.CompleteAttemptAsync(attempt.Id, score, answers);
        if (completeAffected == 0)
            throw new BusinessRuleException("Quiz attempt already completed or not found.");

        var creditDelta = await _applyQuizSettlementCommand.ExecuteAsync(
                                    quizId: command.QuizId,
                                    studentId: command.StudentId,
                                    attemptId: attempt.Id,
                                    totalQuestionCount: totalQuestionCount,
                                    correctCount: correctCount,
                                    wrongCount: wrongCount
                                );

        await _attemptRepository.UpdateScoreAsync(attempt.Id, creditDelta);
        await _attemptRepository.MarkSettledAsync(attempt.Id);


        return new SubmitQuizResponse
        {
            AttemptId = attempt.Id,
            Score = creditDelta,
            CreditDelta = creditDelta,
            TotalQuestions = attempt.TotalQuestions,
        };

    }
}

public class SubmitQuizResponse
{
    public Guid AttemptId { get; set; }
    public int Score { get; set; }
    public int CreditDelta { get; set; } 
    public int TotalQuestions { get; set; }
}
