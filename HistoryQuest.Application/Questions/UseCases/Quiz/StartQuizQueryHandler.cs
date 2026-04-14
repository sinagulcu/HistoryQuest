

using HistoryQuest.Application.Credits.Interfaces;
using HistoryQuest.Application.Questions.DTOs.Quiz;
using HistoryQuest.Application.Questions.Interfaces;
using HistoryQuest.Domain.Entities;
using HistoryQuest.Domain.Exceptions;
using HistoryQuest.Domain.Models;
using Microsoft.Extensions.Options;
using System.Reflection;

namespace HistoryQuest.Application.Questions.UseCases.Quiz;

public class StartQuizQueryHandler
{
    private readonly IQuizRepository _quizRepository;
    private readonly IQuizEconomyRuleRepository _quizEconomyRuleRepository;
    private readonly ICreditLedgerService _creditLedgerService;
    private readonly IQuizAttemptRepository _attemptRepository;

    public StartQuizQueryHandler(IQuizRepository quizRepository, IQuizEconomyRuleRepository quizEconomyRuleRepository, ICreditLedgerService creditLedgerService, IQuizAttemptRepository attemptRepository)
    {
        _quizRepository = quizRepository;
        _quizEconomyRuleRepository = quizEconomyRuleRepository;
        _creditLedgerService = creditLedgerService;
        _attemptRepository = attemptRepository;
    }

    public async Task<StartQuizResponse> Handle(StartQuizQuery query, CancellationToken cancellationToken = default)
    {
        if (query.StudentId == Guid.Empty)
            throw new BusinessRuleException("StudentId is required.");

        var quiz = await _quizRepository.GetByIdAsync(query.QuizId) ?? throw new NotFoundException("Quiz not found.");

        if (quiz.Status != QuizStatus.Published)
            throw new BusinessRuleException("Quiz not published.");

        var attempt = await _attemptRepository.GetActiveAttemptAsync(query.QuizId, query.StudentId, cancellationToken);

        if(attempt is null)
        {
            attempt = QuizAttempt.Start(query.QuizId, query.StudentId, quiz.QuizQuestions.Count);
            await _attemptRepository.AddAsync(attempt);
            await _attemptRepository.SaveChangesAsync();
        }

        var rule = await _quizEconomyRuleRepository.GetByQuizIdAsync(query.QuizId, cancellationToken);
        if (rule is not null && rule.IsActive && rule.EntryCost > 0 && !attempt.IsEntryFeeCharged)
        {
            var entryKey = $"quiz-entry:{attempt.Id}";

            await _creditLedgerService.ApplyAsync(
                userId: query.StudentId,
                amount: -rule.EntryCost,
                type: CreditTransactionType.QuizEntry,
                reason: "Quiz giriş ücreti",
                referenceType: "QuizAttempt",
                referenceId: attempt.Id,
                idempotencyKey: entryKey,
                cancellationToken: cancellationToken);

            attempt.MarkEntryFeeCharged();
            await _attemptRepository.SaveChangesAsync();
        }

        return new StartQuizResponse
        {
            QuizId = quiz.Id,
            Title = quiz.Title,
            Questions = quiz.QuizQuestions
            .OrderBy(qq => qq.Order)
            .Select(qq => new QuestionDto
            {
                QuestionId = qq.Question.Id,
                Text = qq.Question.Text,
                Options = qq.Question.Options.Select(o => new OptionDto
                {
                    OptionId = o.Id,
                    Text = o.Text
                }).ToList()
            }).ToList()
        };
    }
}
