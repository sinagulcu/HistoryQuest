
using HistoryQuest.Domain.Enums;

namespace HistoryQuest.Application.Questions.DTOs;

public class UpdateQuestionRequest
{
    public string Text { get; set; } = string.Empty;

    public QuestionDifficulty Difficulty { get; set; }

    public string? Explanation { get; set; }

    public List<UpdateOptionDto> Options { get; set; } = [];
}

public class UpdateOptionDto
{
    public Guid? Id { get; set; }
    public string Text { get; set; } = string.Empty;
    public bool IsCorrect { get; set; }

}
