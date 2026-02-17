

namespace HistoryQuest.Application.Questions.DTOs;

public class CreateQuestionRequest
{
    public string Text { get; init; } = null!;
    public List<CreateQuestionOptionRequest> Options { get; init; } = [];
}
