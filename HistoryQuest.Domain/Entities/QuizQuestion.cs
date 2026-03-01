

namespace HistoryQuest.Domain.Entities;

public class QuizQuestion
{
    public Guid QuizId { get; private set; }
    public Quiz Quiz { get; private set; } = null!;

    public Guid QuestionId { get; private set; }
    public Question Question { get; private set; } = null!;

    public int Order { get; private set; }
    
    protected QuizQuestion() { }

    public QuizQuestion(Guid quizId, Guid questionId, int order)
    {
        QuizId = quizId;
        QuestionId = questionId;
        Order = order;
    }
}
