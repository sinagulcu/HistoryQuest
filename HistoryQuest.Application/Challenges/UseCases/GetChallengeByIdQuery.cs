

using HistoryQuest.Application.Challenges.DTOs;
using HistoryQuest.Application.Challenges.Interfaces;
using HistoryQuest.Domain.Exceptions;

namespace HistoryQuest.Application.Challenges.UseCases;

public class GetChallengeByIdQuery
{
    private readonly IChallengeRepository _challengeRepository;

    public GetChallengeByIdQuery(IChallengeRepository challengeRepository)
    {
        _challengeRepository = challengeRepository;
    }

    public async Task<ChallengeDetailDto?> ExecuteAsync(
        Guid challengeId,
        Guid currentUserId,
        bool isAdmin,
        CancellationToken cancellationToken = default)
    {
        var challenge = await _challengeRepository.GetByIdAsync(challengeId,cancellationToken);
        if (challenge is null)
            return null;

        if(!isAdmin && challenge.CreatedByTeacherId != currentUserId)
            throw new UnauthorizedException("You are not authorized to access this challenge.");


        return new ChallengeDetailDto
        {
            Id = challenge.Id,
            Title = challenge.Title,
            QuestionId = challenge.QuestionId,
            CreatedByTeacherId = challenge.CreatedByTeacherId,
            ScheduledAtUtc = challenge.ScheduledAtUtc,
            AnswerWindowSeconds = challenge.AnswerWindowSeconds,
            VisibilityWindowSeconds = challenge.VisibilityWindowSeconds,
            MaxScore = challenge.MaxScore,
            ShowCorrectAnswerOnWrong = challenge.ShowCorrectAnswerOnWrong,
            ShowExplanationOnWrong = challenge.ShowExplanationOnWrong,
            NotifyAllStudents = challenge.NotifyAllStudents,
            Status = challenge.GetStatus(DateTime.UtcNow),
            IsDeleted = challenge.IsDeleted,
            CreatedAt = challenge.CreatedAt,
            UpdatedAt = challenge.UpdatedAt
        };
    }
}
