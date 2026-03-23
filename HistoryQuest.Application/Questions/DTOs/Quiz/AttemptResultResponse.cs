

using HistoryQuest.Application.Questions.UseCases.Quiz;

namespace HistoryQuest.Application.Questions.DTOs.Quiz;

public class AttemptResultResponse
{
    public int Score { get; set; }
    public int TotalQuestions { get; set; }
    public List<ResultQuestionDto> Questions { get; set; }
}
