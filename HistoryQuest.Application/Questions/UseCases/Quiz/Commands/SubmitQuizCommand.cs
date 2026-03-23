
namespace HistoryQuest.Application.Questions.UseCases.Quiz.Commands;

public record SubmitQuizCommand(Guid QuizId, Guid StudentId, List<SubmitAnswerDto> Answers);

public class SubmitAnswerDto
{
    public Guid QuestionId { get; set; }
    public Guid SelectedOptionId { get; set; }
}