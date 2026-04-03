

using HistoryQuest.Domain.Models;

namespace HistoryQuest.Application.Questions.DTOs;

public class CreateQuestionRequest
{
    public string Text { get; set; } = string.Empty;
    public QuestionDifficulty Difficulty { get; set; }
    public QuestionType Type { get; set; }

    public Guid CategoryId { get; set; }
    public string? Explanation { get; set; }

    public List<CreateQuestionOptionRequest> Options { get; set; } = [];
}
