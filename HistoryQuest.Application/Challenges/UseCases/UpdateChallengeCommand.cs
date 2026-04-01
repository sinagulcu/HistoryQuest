

using HistoryQuest.Application.Challenges.DTOs;
using HistoryQuest.Application.Challenges.Interfaces;
using HistoryQuest.Domain.Exceptions;

namespace HistoryQuest.Application.Challenges.UseCases;

public class UpdateChallengeCommand
{
    private readonly IChallengeRepository _challengeRepository;

    public UpdateChallengeCommand(IChallengeRepository challengeRepository)
    {
        _challengeRepository = challengeRepository;
    }

    public async Task ExecuteAsync(
        Guid challengeId,
        UpdateChallengeRequest request,
        Guid currentUserId,
        bool isAdmin,
        CancellationToken cancellationToken = default)
    {
        var challenge = await _challengeRepository.GetByIdAsync(challengeId, cancellationToken);
        if (challenge is null)
            throw new NotFoundException("Challenge not found.");
        if (challenge.CreatedByTeacherId != currentUserId)
            throw new UnauthorizedException("You are not authorized to update this challenge.");

        challenge.Update(
            request.Title,
            request.QuestionId,
            request.ScheduledAtUtc,
            request.AnswerWindowSeconds,
            request.VisibilityWindowSeconds,
            request.MaxScore,
            request.ShowCorrectAnswerOnWrong,
            request.ShowExplanationOnWrong,
            request.NotifyAllStudents);

        await _challengeRepository.UpdateAsync(challenge, cancellationToken);
    }
}
