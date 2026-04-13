
using HistoryQuest.Application.Credits.DTOs;
using HistoryQuest.Application.Credits.Interfaces;
using HistoryQuest.Application.Questions.Interfaces;
using HistoryQuest.Domain.Entities;
using HistoryQuest.Domain.Exceptions;

namespace HistoryQuest.Application.Credits.UseCases;

public sealed class SetQuizEconomyRuleCommand
{
    private readonly IQuizRepository _quizRepository;
    private readonly IQuizEconomyRuleRepository _ruleRepository;

    public SetQuizEconomyRuleCommand(IQuizRepository quizRepository, IQuizEconomyRuleRepository ruleRepository)
    {
        _quizRepository = quizRepository;
        _ruleRepository = ruleRepository;
    }

    public async Task ExecuteAsync(Guid quizId, Guid currentUserId, bool isAdmin, SetQuizEconomyRuleRequest request, CancellationToken ct = default)
    {
        var quiz = await _quizRepository.GetByIdAsync(quizId);
        if (quiz is null) throw new NotFoundException("Quiz not found.");

        if (!isAdmin && quiz.CreatedByTeacherId != currentUserId)
            throw new UnauthorizedException("You are not modify this quiz");

        var existing = await _ruleRepository.GetByQuizIdAsync(quizId, ct);
        if(existing is null)
        {
            var rule = QuizEconomyRule.Create(
                quizId,
                request.EntryCost,
                request.RewardPool,
                request.WrongPenaltyPerQuestion);

            if (!request.IsActive)
                rule.Update(request.EntryCost, request.RewardPool, request.WrongPenaltyPerQuestion, false);

            await _ruleRepository.AddAsync(rule, ct);
            await _ruleRepository.SaveChangesAsync(ct);

            return;
        }

        existing.Update(request.EntryCost, request.RewardPool, request.WrongPenaltyPerQuestion, request.IsActive);
        await _ruleRepository.SaveChangesAsync(ct);
    }
}
