
using System.ComponentModel;

namespace HistoryQuest.Application.Questions.DTOs;

public class QuestionListItemDto
{
    public Guid Id { get; set; }
    public string Text { get; set; } = string.Empty;
    public string Difficulty { get; set; } = string.Empty;

    public Guid? CategoryId { get; set; }
    public string? CategoryName { get; set; }

    public Guid CreatedByTeacherId { get; set; }

    public string CreatedByTeacherFullName { get; set; } = string.Empty;
    public string CreatedByTeacherUserName { get; set; } = string.Empty;

    public string TextPreview { get; set; } = string.Empty;

    public bool IsDeleted { get; set; }
    public DateTime? DeletedAt { get; set; }

    public List<OptionForTeacherDto> Options { get; set; } = [];
}
