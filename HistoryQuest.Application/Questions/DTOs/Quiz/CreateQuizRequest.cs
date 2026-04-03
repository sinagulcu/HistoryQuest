namespace HistoryQuest.Application.Questions.DTOs.Quiz;

public class CreateQuizRequest
{
    public string Title { get; set; } = null!;
    public string? Description { get; set; }

    public Guid CategoryId { get; set; }
    public int TimeLimitMinutes { get; set; }
}
