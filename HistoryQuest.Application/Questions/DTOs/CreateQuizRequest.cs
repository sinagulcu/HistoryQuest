

namespace HistoryQuest.Application.Questions.DTOs;

public class CreateQuizRequest
{
    public string Title { get; set; } = null!;
    public string? Description { get; set; }
}
