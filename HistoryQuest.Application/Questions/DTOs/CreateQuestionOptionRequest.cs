

namespace HistoryQuest.Application.Questions.DTOs;

public class CreateQuestionOptionRequest
{
    public string Text { get; init; } = null!;
    public bool IsCorrect { get; init; } 
}
