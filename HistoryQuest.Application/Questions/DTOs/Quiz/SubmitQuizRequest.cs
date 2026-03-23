using HistoryQuest.Domain.Entities;

namespace HistoryQuest.Application.Questions.DTOs.Quiz;

public class SubmitQuizRequest
{
    public List<AnswerDto> Answers { get; set; }
}

public class AnswerDto
{
    public Guid QuestionId { get; set; }
    public Guid SelectedOptionId { get; set; }
}
