

namespace HistoryQuest.Application.Questions.DTOs.Quiz;

public class ResultOptionDto
{
    public Guid OptionId { get; set; }
    public string Text { get; set; } = string.Empty;
    public bool IsCorrect { get; set; }
    public bool WasSelected { get; set; }
}
