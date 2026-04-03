namespace HistoryQuest.Application.Questions.DTOs.Quiz;

public class QuizDetailDto
{
    public Guid Id { get; set; }
    public string Title { get; set; } = null!;
    public string? Description { get; set; }
    public Guid CategoryId { get; set; }
    public string? CategoryName { get; set; }
    public int TimedLimitMinutes { get; set; }
    public Guid CreatedTeacherId { get; set; }
    public string? CreatedTeacherUserName { get; set; }
    public string? CreatedTeacherFullName { get; set; }
    public List<QuizQuestionDto> QuizQuestions { get; set; } = new();
}

public class QuizQuestionDto
{
    public Guid QuestionId { get; set; }
    public int Order { get; set; }
    public string QuestionText { get; set; } = null!;
    public bool IsDeleted { get; set; }
}
