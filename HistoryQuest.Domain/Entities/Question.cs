

using HistoryQuest.Domain.Enums;

namespace HistoryQuest.Domain.Entities;

public class Question
{
    public Guid Id { get; private set; } = Guid.NewGuid();

    public string Text { get; private set; } = null!;
    public QuestionDifficulty Difficulty { get; private set; }
    public QuestionType Type { get; private set; }

    public Guid CreatedByTeacherId { get; private set; }

    public string? Explanation { get; private set; }

    public ICollection<QuestionOption> Options { get; private set; } = [];

    protected Question(
        string text,
        QuestionDifficulty difficulty,
        QuestionType type,
        Guid createdByTeacherId,
        string? explanation)
    {
        Text = text;
        Difficulty = difficulty;
        Type = type;
        CreatedByTeacherId = createdByTeacherId;
        Explanation = explanation;
    }

    public static Question Create(
        string text,
        QuestionDifficulty difficulty,
        QuestionType type,
        Guid teacherId,
        string? explanation = null)
    {
        return new Question(text, difficulty, type, teacherId, explanation);
    }

    public void Update(
        string text,
        QuestionDifficulty difficulty,
        string? explanation,
        List<(Guid? Id, string Text, bool IsCorrect)> updatedOptions)
    {
        Text = text;
        Difficulty = difficulty;
        Explanation = explanation;

        var correctCount = updatedOptions.Count(o => o.IsCorrect);

        if (correctCount == 0)
            throw new InvalidOperationException("At least one option must be correct.");
        if (Type == QuestionType.SingleChoice && correctCount > 1)
            throw new InvalidOperationException("Single choice question can have only one correct answer.");


        var optionIds = updatedOptions.Where(o => o.Id.HasValue).Select(o => o.Id!.Value).ToList();


        var toRemove = Options.Where(o => !optionIds.Contains(o.Id)).ToList();
        foreach (var option in toRemove)
        {
            Options.Remove(option);
        }

        foreach (var dto in updatedOptions)
        {
            if (dto.Id.HasValue)
            {
                var existing = Options.FirstOrDefault(o => o.Id == dto.Id.Value);
                if (existing == null)
                    throw new InvalidOperationException("Option not found or does not belong to this question.");

                existing.Update(dto.Text, dto.IsCorrect);
            }
            else
            {
                Options.Add(new QuestionOption(dto.Text, dto.IsCorrect));
            }
        }
    }

    public void AddOption(string text, bool isCorrect)
    {
        if (Type == QuestionType.SingleChoice && Options.Any(o => o.IsCorrect) && isCorrect)
            throw new InvalidOperationException("Single choice question can have only one correct answer.");

        Options.Add(new QuestionOption(text, isCorrect));
    }
}
