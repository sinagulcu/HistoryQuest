
using HistoryQuest.Application.Challenges.DTOs;
using HistoryQuest.Application.Challenges.Interfaces;
using HistoryQuest.Domain.Entities;

namespace HistoryQuest.Application.Challenges.UseCases;

public class CreateChallengeCommand
{
    private readonly IChallengeRepository _challengeRepository;

    public CreateChallengeCommand(IChallengeRepository challengeRepository)
    {
        _challengeRepository = challengeRepository;
    }

    public async Task<Guid> ExecuteAsync(
       CreateChallengeRequest request,
       Guid teacherId,
       CancellationToken cancellationToken = default)
    {
        var challenge = TimedChallenge.Create(
            request.Title,
            request.QuestionId,
            teacherId,
            request.ScheduledAtUtc,
            request.AnswerWindowSeconds,
            request.VisibilityWindowSeconds,
            request.MaxScore,
            request.ShowCorrectAnswerOnWrong,
            request.ShowExplanationOnWrong,
            request.NotifyAllStudents);

        await _challengeRepository.AddAsync(challenge, cancellationToken);
        return challenge.Id;
    }
}
