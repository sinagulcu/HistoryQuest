namespace HistoryQuest.Application.Questions.DTOs.Quiz;

public class QuizForTeacherDto
{
    public Guid Id { get; set; }
    public string Title { get; set; } = null!;
    public string? Description { get; set; }
    public string Status { get; set; } = null!;
    public int QuestionCount { get; set; }
    public DateTime CreatedAt { get; set; }
    public bool IsDeleted { get; set; }
    public DateTime? DeletedAt { get; set; }
    public Guid CreatedTeacherId { get; set; }
    public string? CreatedTeacherUserName { get; set; }
    public string? CreatedTeacherFullName { get; set; }
    public Guid? CategoryId { get; set; }
    public string? CategoryName { get; set; }
    public int TimeLimitMinutes { get; set; }

}
