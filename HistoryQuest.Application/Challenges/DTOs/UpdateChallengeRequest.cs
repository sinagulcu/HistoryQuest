

namespace HistoryQuest.Application.Challenges.DTOs;

public class UpdateChallengeRequest
{
    public string Title { get; set; } = string.Empty;
    public Guid QuestionId { get; set; }

    public DateTime ScheduledAtUtc { get; set; }
    public int AnswerWindowSeconds { get; set; }
    public int VisibilityWindowSeconds { get; set; }
    public int MaxScore { get; set; }

    public bool ShowCorrectAnswerOnWrong { get; set; }
    public bool ShowExplanationOnWrong { get; set; }
    public bool NotifyAllStudents { get; set; }
}
