

using HistoryQuest.Domain.Models;

namespace HistoryQuest.Domain.Entities;

public class QuizAttempt
{
    public Guid Id { get; private set; }
    public Guid QuizId { get; private set; }
    public Guid StudentId { get; private set; }
    public DateTime StartedAt { get; private set; }
    public DateTime? CompletedAt { get; private set; }
    public int Score { get; private set; }
    public int TotalQuestions { get; private set; }
    public AttemptStatus Status { get; private set; }

    public bool IsEntryFeeCharged { get; private set; }
    public DateTime? EntryFeeChargedAt { get; private set; }
    public bool IsSettled { get; private set; }
    public DateTime? SettledAt { get; private set; }
    public bool IsCompleted { get; private set; }

    public Quiz? Quiz { get; private set; }

    public IReadOnlyCollection<AttemptAnswer> Answers => _answers.AsReadOnly();
    private readonly List<AttemptAnswer> _answers = new();

    public static QuizAttempt Start(Guid quizId, Guid studentId, int totalQuestions)
    {
        return new QuizAttempt
        {
            Id = Guid.NewGuid(),
            QuizId = quizId,
            StudentId = studentId,
            StartedAt = DateTime.UtcNow,
            TotalQuestions = totalQuestions,
            Status = AttemptStatus.InProgress
        };
    }

    public void MarkEntryFeeCharged()
    {
        if (IsEntryFeeCharged) return;

        IsEntryFeeCharged = true;
        EntryFeeChargedAt = DateTime.UtcNow;
    }

    public void MarkSettled()
    {
        if (IsSettled) return;

        IsSettled = true;
        SettledAt = DateTime.UtcNow;
    }

    public void Complete(List<AttemptAnswer> answers)
    {
        _answers.AddRange(answers);
        Score = answers.Count(a => a.IsCorrect);
        IsCompleted = true;
        CompletedAt = DateTime.UtcNow;
        Status = AttemptStatus.Completed;
    }

}
