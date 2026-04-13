

using HistoryQuest.Application.Credits.Interfaces;
using HistoryQuest.Application.Questions.DTOs.Quiz;
using HistoryQuest.Application.Questions.Interfaces;
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

    public StartQuizQueryHandler(IQuizRepository quizRepository, IQuizEconomyRuleRepository quizEconomyRuleRepository, ICreditLedgerService creditLedgerService)
    {
        _quizRepository = quizRepository;
        _quizEconomyRuleRepository = quizEconomyRuleRepository;
        _creditLedgerService = creditLedgerService;
    }

    public async Task<StartQuizResponse> Handle(StartQuizQuery query, CancellationToken cancellationToken = default)
    {
        var quiz = await _quizRepository.GetByIdAsync(query.QuizId);

        if (quiz is null)
            throw new NotFoundException("Quiz not found.");

        if (quiz.Status != QuizStatus.Published)
            throw new BusinessRuleException("Quiz not published.");

        var rule = await _quizEconomyRuleRepository.GetByQuizIdAsync(query.QuizId, cancellationToken);
        if (rule is not null && rule.IsActive && rule.EntryCost > 0)
        {
            var key = $"quiz-entry:{query.QuizId}:{query.StudentId}:{DateTime.UtcNow:yyyyMMdd}";

            await _creditLedgerService.ApplyAsync(
        userId: query.StudentId,
        amount: -rule.EntryCost,
        type: CreditTransactionType.QuizEntry,
        reason: "Quiz giriş ücreti",
        referenceType: "Quiz",
        referenceId: query.QuizId,
        idempotencyKey: key,
        cancellationToken: cancellationToken);

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
