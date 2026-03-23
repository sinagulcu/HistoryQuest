

namespace HistoryQuest.Application.Questions.DTOs.Quiz;

public class ResultQuestionDto
{
    public Guid QuestionId { get; set; }
    public string Text { get; set; }
    public string Explanation { get; set; }
    public List<ResultOptionDto> Options { get; set; }

}
