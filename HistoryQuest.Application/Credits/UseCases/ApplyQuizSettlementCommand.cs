

using HistoryQuest.Application.Credits.Interfaces;
using HistoryQuest.Domain.Exceptions;
using HistoryQuest.Domain.Models;

namespace HistoryQuest.Application.Credits.UseCases;

public sealed class ApplyQuizSettlementCommand
{
    private readonly IQuizEconomyRuleRepository _ruleRepository;
    private readonly ICreditLedgerService _creditLedgerService;
    private readonly ICreditTransactionRepository _creditTransactionRepository;
    private readonly IWalletRepository _walletRepository;

    public ApplyQuizSettlementCommand(IQuizEconomyRuleRepository ruleRepository, ICreditLedgerService creditLedgerService,
        ICreditTransactionRepository creditTransactionRepository, IWalletRepository walletRepository)
    {
        _ruleRepository = ruleRepository;
        _creditLedgerService = creditLedgerService;
        _creditTransactionRepository = creditTransactionRepository;
        _walletRepository = walletRepository;
    }

    public async Task<int> ExecuteAsync(
        Guid quizId,
        Guid studentId,
        Guid attemptId,
        int totalQuestionCount,
        int correctCount,
        int wrongCount,
        CancellationToken ct = default)
    {
        if (totalQuestionCount <= 0)
            throw new BusinessRuleException("Quiz question count must be greater than zero.");

        var rule = await _ruleRepository.GetByQuizIdAsync(quizId, ct);

        if (rule is null)
            throw new NotFoundException($"Quiz economy rule not found for quiz: {quizId}");

        if (!rule.IsActive)
            throw new BusinessRuleException("Quiz economy rule is not active");

        var rewardedCount = await _creditTransactionRepository.GetQuizRewardCountForUserAsync(studentId, quizId, ct);
        var canReceiveReward = rewardedCount < rule.MaxRewardedAttemptsPerUser;

        var perQuestionReward = rule.RewardPool / totalQuestionCount;
        var reward = canReceiveReward ? perQuestionReward * Math.Max(correctCount, 0) : 0L;
        var penalty = (long)rule.WrongPenaltyPerQuestion * Math.Max(wrongCount, 0);

        var rawNet = reward - penalty;
        var safeNet = rawNet;

        if (rawNet < 0)
        {
            var wallet = await _walletRepository.GetByUserIdAsync(studentId, ct);
            var currentBalance = wallet?.Balance ?? 0L;
            var maxDebit = Math.Min(Math.Abs(rawNet), currentBalance);
            safeNet = -maxDebit;
        }

        var net = (int)Math.Clamp(safeNet, int.MinValue, int.MaxValue);

        if (net == 0)
            return 0;

        var settlementKey = $"quiz-settlement:{studentId}:{attemptId}";

        try
        {
            await _creditLedgerService.ApplyAsync(
                userId: studentId,
                amount: net,
                type: net > 0 ? CreditTransactionType.QuizReward : CreditTransactionType.WrongPenalty,
                reason: canReceiveReward ? "Quiz sonucu kredi düşümü" : "Tekrar deneme: ödül hakkı doldu, cezalar uygulanır",
                referenceType: "QuizAttempt",
                referenceId: attemptId,
                idempotencyKey: settlementKey,
                metadataJson: $"{{\"quizId\":\"{quizId}\",\"correct\":{correctCount},\"wrong\":{wrongCount},\"total\":{totalQuestionCount},\"rewardAllowed\":{canReceiveReward.ToString().ToLower()}}}",
                cancellationToken: ct
            );
        }
        catch (BusinessRuleException ex) when (ex.Message.Contains("Insufficient balance.", StringComparison.OrdinalIgnoreCase))
        {
            return 0;
        }

        return net;
    }
}
