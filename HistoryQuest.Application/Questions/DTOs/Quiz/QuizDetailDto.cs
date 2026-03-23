namespace HistoryQuest.Application.Questions.DTOs.Quiz;

public class QuizDetailDto
{
    public Guid Id { get; set; }
    public string Title { get; set; } = null!;
    public List<QuizQuestionDto> QuizQuestions { get; set; } = new();
}

public class QuizQuestionDto
{
    public Guid QuestionId { get; set; }
    public int Order { get; set; }
    public string QuestionText { get; set; } = null!;
    public bool IsDeleted { get; set; }
}
