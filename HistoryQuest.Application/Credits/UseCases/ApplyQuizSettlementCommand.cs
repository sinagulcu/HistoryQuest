

using HistoryQuest.Application.Credits.Interfaces;
using HistoryQuest.Domain.Exceptions;
using HistoryQuest.Domain.Models;

namespace HistoryQuest.Application.Credits.UseCases;

public sealed class ApplyQuizSettlementCommand
{
    private readonly IQuizEconomyRuleRepository _ruleRepository;
    private readonly ICreditLedgerService _creditLedgerService;
    private readonly ICreditTransactionRepository _creditTransactionRepository;

    public ApplyQuizSettlementCommand(IQuizEconomyRuleRepository ruleRepository, ICreditLedgerService creditLedgerService, ICreditTransactionRepository creditTransactionRepository)
    {
        _ruleRepository = ruleRepository;
        _creditLedgerService = creditLedgerService;
        _creditTransactionRepository = creditTransactionRepository;
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


        var settlementKey = $"quiz-settlement:{studentId}:{attemptId}";

        var perQuestionReward = rule.RewardPool / totalQuestionCount;
        var reward = perQuestionReward * Math.Max(correctCount, 0);
        var penalty = (long)rule.WrongPenaltyPerQuestion * Math.Max(wrongCount, 0);

        var alreadyRewardedForThisQuiz = await _creditTransactionRepository
            .HasUserReceivedQuizRewardAsync(studentId, quizId, ct);

        if (alreadyRewardedForThisQuiz)
        {
            reward = 0;
        }

        var rewardCount = await _creditTransactionRepository.GetQuizRewardCountForUserAsync(studentId, quizId, ct);
        var canReceiveReward = rewardCount < rule.MaxRewardedAttemptsPerUser;

        if (!canReceiveReward)
            reward = 0;

        var netLong = reward - penalty;

        var net = (int)Math.Clamp(netLong, int.MinValue, int.MaxValue);

        if (net != 0)
        {
            await _creditLedgerService.ApplyAsync(
                    userId: studentId,
                    amount: net,
                    type: net > 0 ? CreditTransactionType.QuizReward : CreditTransactionType.WrongPenalty,
                    reason: canReceiveReward
                        ? "Quiz sonucu kredi settlement"
                        : "Tekrar deneme: ödül hakkı dolu, ceza uygulanır",
                    referenceType: "QuizAttempt",
                    referenceId: attemptId,
                    idempotencyKey: settlementKey,
                    metadataJson: $"{{\"quizId\":\"{quizId}\",\"correct\":{correctCount},\"wrong\":" +
                    $"{wrongCount},\"repeat\":{alreadyRewardedForThisQuiz.ToString().ToLower()}}}",
                    cancellationToken: ct
                );
        }

        return net;
    }
}
