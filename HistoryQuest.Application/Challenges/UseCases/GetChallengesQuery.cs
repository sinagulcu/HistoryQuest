

using HistoryQuest.Application.Challenges.DTOs;
using HistoryQuest.Application.Challenges.Interfaces;

namespace HistoryQuest.Application.Challenges.UseCases;

public class GetChallengesQuery
{
    private readonly IChallengeRepository _challengeRepository;

    public GetChallengesQuery(IChallengeRepository challengeRepository)
    {
        _challengeRepository = challengeRepository;
    }

    public async Task<List<ChallengeDto>> ExecuteAsync(Guid currentUserId,
        bool isAdmin,
        bool includeDeleted = false,
        CancellationToken cancellationToken = default)
    {
        var list = await _challengeRepository.GetAllAsync(currentUserId, isAdmin, includeDeleted, cancellationToken);

        return list
            .OrderByDescending(x => x.ScheduledAtUtc)
            .Select(x => new ChallengeDto
            {
                Id = x.Id,
                Title = x.Title,
                QuestionId = x.QuestionId,
                CreatedByTeacherId = x.CreatedByTeacherId,
                ScheduledAtUtc = x.ScheduledAtUtc,
                AnswerWindowSeconds = x.AnswerWindowSeconds,
                VisibilityWindowSeconds = x.VisibilityWindowSeconds,
                MaxScore = x.MaxScore,
                Status = x.GetStatus(DateTime.UtcNow),
                IsDeleted = x.IsDeleted
            }).ToList();
    }
}
