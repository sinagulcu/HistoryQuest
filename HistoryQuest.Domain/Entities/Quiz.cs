

using HistoryQuest.Domain.Exceptions;
using HistoryQuest.Domain.Models;

namespace HistoryQuest.Domain.Entities;

public class Quiz
{
    public Guid Id { get; private set; } = Guid.NewGuid();
    public string Title { get; private set; } = null!;
    public string? Description { get; private set; } = null;
    public Guid CreatedByTeacherId { get; private set; }
    public Guid CategoryId { get; private set; }
    public int TimeLimitMinutes { get; private set; }

    public Category? Category { get; private set; }
    public User? CreatedByTeacher { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public QuizStatus Status { get; private set; } = QuizStatus.Draft;

    public int QuestionCount => QuizQuestions.Count;

    public bool IsDeleted { get; private set; }
    public DateTime? DeletedAt { get; private set; }

    public ICollection<QuizQuestion> QuizQuestions { get; private set; } = new List<QuizQuestion>();

    protected Quiz() { }

    private Quiz(string title, string? description, Guid teacherId)
    {
        Title = title;
        Description = description;
        CreatedByTeacherId = teacherId;
    }

    public static Quiz Create(string title, string? description, Guid teacherId,
        Guid categoryId, int timeLimitMinutes)
    {
        if (string.IsNullOrWhiteSpace(title))
            throw new BusinessRuleException("Quiz title cannot be empty.");
        if (string.IsNullOrWhiteSpace(description))
            throw new BusinessRuleException("Quiz description cannot be empty.");
        if (categoryId == Guid.Empty)
            throw new BusinessRuleException("Quiz must have a valid category.");
        if (timeLimitMinutes < 1)
            throw new BusinessRuleException("Time limit must be ar least 1 minute.");

        return new Quiz
        {
            Id = Guid.NewGuid(),
            Title = title.Trim(),
            Description = description.Trim(),
            CreatedByTeacherId = teacherId,
            CategoryId = categoryId,
            TimeLimitMinutes = timeLimitMinutes,
            Status = QuizStatus.Draft,
            IsDeleted = false
        };

    }

    public void Update(string title, string description, Guid categoryId, int timeLimitMinutes)
    {
        if (string.IsNullOrWhiteSpace(title))
            throw new BusinessRuleException("Quiz title cannot be empty.");

        if (string.IsNullOrWhiteSpace(description))
            throw new BusinessRuleException("Quiz description cannot be empty.");

        if (categoryId == Guid.Empty)
            throw new BusinessRuleException("Quiz must have a valid category.");
        if (timeLimitMinutes < 1)
            throw new BusinessRuleException("Time limit must be ar least 1 minute.");

        EnsureEditable();

        Title = title.Trim();
        Description = description.Trim();
        CategoryId = categoryId;
        TimeLimitMinutes = timeLimitMinutes;
    }

    public void Publish()
    {
        if (!QuizQuestions.Any())
            throw new BusinessRuleException("Quiz must contain at least one question before publishing.");
        if (Status != QuizStatus.Draft)
            throw new BusinessRuleException("Only quizzes in Draft status can be published.");

        Status = QuizStatus.Published;
    }

    public void Unpublish()
    {
        if (Status != QuizStatus.Published)
            throw new BusinessRuleException("Only quizzes in Published status can be unpublished.");
        Status = QuizStatus.Draft;
    }

    public void Archive()
    {
        Status = QuizStatus.Archived;
    }

    public void SoftDelete()
    {
        IsDeleted = true;
        DeletedAt = DateTime.UtcNow;
    }

    public void Restore()
    {
        IsDeleted = false;
        DeletedAt = null;
    }

    public void EnsureEditable()
    {
        if (Status != QuizStatus.Draft)
            throw new BusinessRuleException("Only quizzes in Draft status can be edited.");
    }
}
