

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

        var quiz = await _quizRepository.GetByIdAsync(command.QuizId) ?? throw new NotFoundException("Quiz not found.");

        if (quiz.Status != QuizStatus.Published)
            throw new BusinessRuleException("Quiz not published");

        var attempt = await _attemptRepository.GetActiveAttemptAsync(command.QuizId, command.StudentId) 
            ?? throw new BusinessRuleException("Active quiz not found. Quiz must be started");

        if (attempt.IsCompleted)
            throw new BusinessRuleException("This quiz try completed allready.");

        var optionMap = quiz.QuizQuestions
            .SelectMany(x => x.Question.Options)
            .ToDictionary(o => o.Id, o => o.IsCorrect);

        var answers = command.Answers.Select(a => AttemptAnswer.Create(
            attempt.Id,
            a.QuestionId,
            a.SelectedOptionId,
            optionMap.TryGetValue(a.SelectedOptionId, out var isCorrect) && isCorrect
        )).ToList();

        attempt.Complete(answers);
        await _attemptRepository.SaveChangesAsync();

        var totalQuestionCount = attempt.TotalQuestions;
        var correctCount = answers.Count(x => x.IsCorrect);
        var wrongCount = Math.Max(totalQuestionCount - correctCount, 0);

        if (!attempt.IsSettled)
        {
            await _applyQuizSettlementCommand.ExecuteAsync(
                quizId: command.QuizId,
                studentId: command.StudentId,
                attemptId: attempt.Id,
                totalQuestionCount: totalQuestionCount,
                correctCount: correctCount,
                wrongCount: wrongCount
                );

            attempt.MarkSettled();
            await _attemptRepository.SaveChangesAsync();
        }

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
