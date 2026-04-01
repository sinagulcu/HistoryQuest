

using HistoryQuest.Application.Challenges.Interfaces;
using HistoryQuest.Domain.Exceptions;

namespace HistoryQuest.Application.Challenges.UseCases;

public class DeleteChallengeCommand
{
    private readonly IChallengeRepository _challengeRepository;

    public DeleteChallengeCommand(IChallengeRepository challengeRepository)
    {
        _challengeRepository = challengeRepository;
    }

    public async Task ExecuteAsync(
        Guid challengeId,
        Guid currentUserId,
        bool isAdmin,
        CancellationToken cancellationToken = default)
    {
        var challenge = await _challengeRepository.GetByIdAsync(challengeId, cancellationToken);
        if (challenge is null)
            throw new NotFoundException("Challenge not found.");
        if (!isAdmin && challenge.CreatedByTeacherId != currentUserId)
            throw new UnauthorizedException("You are not authorized to delete this challenge.");

        challenge.SoftDelete();
        await _challengeRepository.DeleteAsync(challenge, cancellationToken);
    }
}
