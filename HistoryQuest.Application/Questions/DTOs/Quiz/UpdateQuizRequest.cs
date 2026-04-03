

namespace HistoryQuest.Application.Questions.DTOs.Quiz;

public sealed class UpdateQuizRequest
{
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;

    public Guid CategoryId { get; set; }
    public int TimeLimitMinutes { get; set; }
}
