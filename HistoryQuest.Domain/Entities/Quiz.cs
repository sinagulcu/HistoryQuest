

using HistoryQuest.Domain.Exceptions;
using HistoryQuest.Domain.Models;

namespace HistoryQuest.Domain.Entities;

public class Quiz
{
    public Guid Id { get; private set; } = Guid.NewGuid();
    public string Title { get; private set; } = null!;
    public string? Description { get; private set; } = null;
    public Guid CreatedByTeacherId { get; private set; }
    public Guid CategoryId { get; set; }
    public string? CategoryName { get; set; }
    public int TimedLimitMinutes { get; set; }
    public string? CreatedTeacherUserName { get; set; }
    public string? CreatedTeacherFullName { get; set; }
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

    public static Quiz Create(string title, string? description, Guid teacherId)
    {
        return new Quiz(title, description, teacherId);
    }

    public void Update(string title, string? description)
    {
        EnsureEditable();

        Title = title;
        Description = description;
    }

    public void Publish()
    {
        if (!QuizQuestions.Any())
            throw new BusinessRuleException("Quiz must contain at least one question before publishing.");
        if(Status != QuizStatus.Draft)
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
