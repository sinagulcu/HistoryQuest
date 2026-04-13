

using HistoryQuest.Application.Credits.Interfaces;
using HistoryQuest.Domain.Models;

namespace HistoryQuest.Application.Credits.UseCases;

public sealed class ApplyQuizSettlementCommand
{
    private readonly IQuizEconomyRuleRepository _ruleRepository;
    private readonly ICreditLedgerService _creditLedgerService;

    public ApplyQuizSettlementCommand(IQuizEconomyRuleRepository ruleRepository, ICreditLedgerService creditLedgerService)
    {
        _ruleRepository = ruleRepository;
        _creditLedgerService = creditLedgerService;
    }

    public async Task ExecuteAsync(
        Guid quizId,
        Guid studentId,
        Guid attemptId,
        int totalQuestionCount,
        int correctCount,
        int wrongCount,
        CancellationToken ct = default)
    {
        var rule = await _ruleRepository.GetByQuizIdAsync(quizId,ct);

        if (rule is null || !rule.IsActive)
            return;

        var settlementKey = $"quiz-settlement:{attemptId}";

        var perQuestionReward = totalQuestionCount > 0 ? rule.RewardPool / totalQuestionCount : 0;

        var reward = perQuestionReward * Math.Max(correctCount, 0);
        var penalty = (long)rule.WrongPenaltyPerQuestion * Math.Max(wrongCount, 0);

        var net = reward - penalty;

        if(net == 0)
        {
            return;
        }

        await _creditLedgerService.ApplyAsync(
            userId: studentId,
            amount: net,
            type: net > 0 ? CreditTransactionType.QuizReward : CreditTransactionType.WrongPenalty,
            reason: "Quiz sonucu kredi settlement",
            referenceType: "Quiz Attempt",
            referenceId: attemptId,
            idempotencyKey: settlementKey,
            metadataJson: $"{{\"quizId\":\"{quizId}\",\"correct\":{correctCount},\"wrong\":{wrongCount}}}",
            cancellationToken: ct);
    }
}
