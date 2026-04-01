

using HistoryQuest.Domain.Models;

namespace HistoryQuest.Application.Challenges.DTOs;

public class ChallengeDto
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public Guid QuestionId { get; set; }
    public Guid CreatedByTeacherId { get; set; }

    public DateTime ScheduledAtUtc { get; set; }
    public int AnswerWindowSeconds { get; set; }
    public int VisibilityWindowSeconds { get; set; }
    public int MaxScore { get; set; }

    public ChallengeStatus Status { get; set; }
    public bool IsDeleted { get; set; }
}
