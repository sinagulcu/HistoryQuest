
namespace HistoryQuest.Application.Questions.DTOs;

public class QuestionForTeacherDto
{
    public Guid Id { get; set; }
    public string Text { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public string Difficulty { get; set; } = string.Empty;
    public bool IsDeleted { get; set; }
    public DateTime? DeletedAt { get; set; }
    public int? DaysUntilHardDelete { get; set; }
    public List<OptionForTeacherDto> Options { get; set; } = new();

}

public class OptionForTeacherDto
{
    public Guid Id { get; set; }
    public string Text { get; set; } = string.Empty;
    public bool IsCorrect { get; set; }
}