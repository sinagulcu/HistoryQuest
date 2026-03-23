

namespace HistoryQuest.Domain.Entities;

public class AttemptAnswer
{
    public Guid Id { get; private set; }
    public Guid AttemptId { get; private set; }
    public Guid QuestionId { get; private set; }
    public Guid SelectedOptionId { get; private set; }
    public bool IsCorrect { get; private set; }
    
    public static AttemptAnswer Create(
        Guid attemptId,
        Guid questionId,
        Guid selectedOptionId,
        bool isCorrect)
    {
        return new AttemptAnswer
        {
            Id = Guid.NewGuid(),
            AttemptId = attemptId,
            QuestionId = questionId,
            SelectedOptionId = selectedOptionId,
            IsCorrect = isCorrect
        };
    }
}
