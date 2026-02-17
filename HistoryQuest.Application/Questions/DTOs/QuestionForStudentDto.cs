
namespace HistoryQuest.Application.Questions.DTOs;

public class QuestionForStudentDto
{
    public Guid Id { get; set; }
    public string Text { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public string Difficulty { get; set; } = string.Empty;

    public List<OptionForStudentDto> Options { get; set; } = new();
}

public class OptionForStudentDto
{
    public Guid Id { get; set; }
    public string Text { get; set; } = string.Empty;
}
