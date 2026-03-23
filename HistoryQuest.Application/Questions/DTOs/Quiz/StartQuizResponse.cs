namespace HistoryQuest.Application.Questions.DTOs.Quiz;

public class StartQuizResponse
{
    public Guid QuizId { get; set; }
    public string Title { get; set; }
    public List<QuestionDto> Questions { get; set; }
}
