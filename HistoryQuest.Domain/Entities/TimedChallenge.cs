

using HistoryQuest.Domain.Enums;
using HistoryQuest.Domain.Exceptions;

namespace HistoryQuest.Domain.Entities;

public class TimedChallenge
{
    public Guid Id { get; private set; } = Guid.NewGuid();

    public string Title { get; private set; } = string.Empty;
    public Guid QuestionId { get; private set; }
    public Guid CreatedByTeacherId { get; private set; }

    public DateTime ScheduledAtUtc { get; private set; }
    public int AnswerWindowSeconds { get; private set; }
    public int VisibilityWindowSeconds { get; private set; }
    public int MaxScore { get; private set; }

    public bool ShowCorrectAnswerOnWrong { get; private set; }
    public bool ShowExplanationOnWrong { get; private set; }
    public bool NotifyAllStudents { get; private set; }

    public DateTime CreatedAt { get; private set; }
    public DateTime? UpdatedAt { get; private set; }

    public bool IsDeleted { get; private set; }
    public DateTime? DeletedAt { get; private set; }

    private TimedChallenge() { }

    private TimedChallenge(
        string title,
        Guid questionId,
        Guid createdByTeacherId,
        DateTime scheduledAtUtc,
        int answerWindowSeconds,
        int visibilityWindowSeconds,
        int maxScore,
        bool showCorrectAnswerOnWrong,
        bool showExplanationOnWrong,
        bool notifyAllStudents)
    {
        Id = Guid.NewGuid();
        CreatedAt = DateTime.UtcNow;

        SetTitle(title);
        SetQuestion(questionId);
        SetCreator(createdByTeacherId);
        SetTiming(scheduledAtUtc, answerWindowSeconds, visibilityWindowSeconds);
        SetScoring(maxScore);

        ShowCorrectAnswerOnWrong = showCorrectAnswerOnWrong;
        ShowExplanationOnWrong = showExplanationOnWrong;
        NotifyAllStudents = notifyAllStudents;
    }

    public static TimedChallenge Create(
        string title,
        Guid questionId,
        Guid createdByTeacherId,
        DateTime scheduledAtUtc,
        int answerWindowSeconds,
        int visibilityWindowSeconds,
        int maxScore,
        bool showCorrectAnswerOnWrong,
        bool showExplanationOnWrong,
        bool notifyAllStudents)
    {
        return new TimedChallenge(
            title,
            questionId,
            createdByTeacherId,
            scheduledAtUtc,
            answerWindowSeconds,
            visibilityWindowSeconds,
            maxScore,
            showCorrectAnswerOnWrong,
            showExplanationOnWrong,
            notifyAllStudents);
    }

    public void Update(
        string title,
        Guid questionId,
        DateTime scheduledAtUtc,
        int answerWindowSeconds,
        int visibilityWindowSeconds,
        int maxScore,
        bool showCorrectAnswerOnWrong,
        bool showExplanationOnWrong,
        bool notifyAllStudents)
    {
        EnsureNotDeleted();

        SetTitle(title);
        SetQuestion(questionId);
        SetTiming(scheduledAtUtc, answerWindowSeconds, visibilityWindowSeconds);
        SetScoring(maxScore);
        
        ShowCorrectAnswerOnWrong = showCorrectAnswerOnWrong;
        ShowExplanationOnWrong = showExplanationOnWrong;
        NotifyAllStudents = notifyAllStudents;
        
        UpdatedAt = DateTime.UtcNow;
    }

    public ChallengeStatus GetStatus(DateTime utcNow)
    {
        var scoreDeadLine = ScheduledAtUtc.AddSeconds(AnswerWindowSeconds);
        var visibilityDeadline = ScheduledAtUtc.AddSeconds(VisibilityWindowSeconds);

        if(utcNow < ScheduledAtUtc)
            return ChallengeStatus.Scheduled;
        if(utcNow <= visibilityDeadline)
            return ChallengeStatus.Active;

        return ChallengeStatus.Expired;
    }

    public void SoftDelete()
    {
        EnsureNotDeleted();
     
        IsDeleted = true;
        DeletedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Restore()
    {
        if (!IsDeleted)
            throw new BusinessRuleException("Timed challenge is not deleted.");
        IsDeleted = false;
        DeletedAt = null;
        UpdatedAt = DateTime.UtcNow;
    }

    private void SetTitle(string title)
    {
        if (string.IsNullOrWhiteSpace(title))
            throw new BusinessRuleException("Title cannot be empty.");
        var normalized = title.Trim();

        if(normalized.Length < 3)
            throw new BusinessRuleException("Title must be at least 3 characters long.");
        
        Title = normalized;
    }

    private void SetQuestion(Guid questionId)
    {
        if (questionId == Guid.Empty)
            throw new UnauthorizedException("Question must be selected.");
        
        QuestionId = questionId;
    }

    private void SetCreator(Guid teacherId)
    {
        if (teacherId == Guid.Empty)
            throw new UnauthorizedException("Creator must be specified.");
        
        CreatedByTeacherId = teacherId;
    }

    private void SetTiming(DateTime scheduledAtUtc, int answerWindowSeconds, int visibilityWindowSeconds)
    {
        if (scheduledAtUtc.Kind != DateTimeKind.Utc)
            scheduledAtUtc = DateTime.SpecifyKind(scheduledAtUtc, DateTimeKind.Utc);

        if (answerWindowSeconds < 30)
            throw new BusinessRuleException("Scored time must be greater than 30 seconds.");
        if (visibilityWindowSeconds < answerWindowSeconds)
            throw new BusinessRuleException("Broadcast time must be longer than the rated time.");

        ScheduledAtUtc = scheduledAtUtc;
        AnswerWindowSeconds = answerWindowSeconds;
        VisibilityWindowSeconds = visibilityWindowSeconds;
    }

    private void SetScoring(int maxScore)
    {
        if (maxScore <= 0)
            throw new BusinessRuleException("Max score must be at least 0.");
        
        MaxScore = maxScore;
    }

    private void EnsureNotDeleted()
    {
        if(IsDeleted)
            throw new UnauthorizedException("Timed challenge is deleted.");
    }
}