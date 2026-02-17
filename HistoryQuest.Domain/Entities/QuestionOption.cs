

namespace HistoryQuest.Domain.Entities;

public class QuestionOption
{
    public Guid Id { get; private set; } = Guid.NewGuid();
    public string Text { get; private set; } = null!;
    public bool IsCorrect { get; private set; }
    protected QuestionOption() { }

    public QuestionOption(string text, bool isCorrect)
    {
        Text = text;
        IsCorrect = isCorrect;
    }

    public void Update(string text, bool isCorrect)
    {
        Text = text;
        IsCorrect = isCorrect;
    }
}
