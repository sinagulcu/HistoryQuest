namespace HistoryQuest.Application.Questions.DTOs.Quiz;

public class QuestionDto
{
    public Guid QuestionId { get; set; }
    public string Text  { get; set; }
    public List<OptionDto> Options { get; set; }
}
