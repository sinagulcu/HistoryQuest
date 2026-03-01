

using HistoryQuest.Domain.Exceptions;
using HistoryQuest.Domain.Models;

namespace HistoryQuest.Domain.Entities;

public class Quiz
{
    public Guid Id { get; private set; } = Guid.NewGuid();
    public string Title { get; private set; } = null!;
    public string? Description { get; private set; } = null;
    public Guid CreatedByTeacherId { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public QuizStatus Status { get; private set; } = QuizStatus.Draft;

    public int QuestionCount => QuizQuestions.Count;

    public bool IsDeleted { get; private set; }
    public DateTime? DeletedAt { get; private set; }

    public List<QuizQuestion> QuizQuestions { get; private set; } = [];

    protected Quiz() { }

    private Quiz(string title, string? description, Guid teacherId)
    {
        Title = title;
        Description = description;
        CreatedByTeacherId = teacherId;
    }

    public static Quiz Create(string title, string? description, Guid teacherId)
    {
        return new Quiz(title, description, teacherId);
    }

    public void Update(string title, string? description)
    {
        Title = title;
        Description = description;
    }

    public void Publish()
    {
        if (!QuizQuestions.Any())
            throw new BusinessRuleException("Quiz must contain at least one question before publishing.");

        Status = QuizStatus.Published;
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
}
