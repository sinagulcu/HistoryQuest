

using HistoryQuest.Domain.Models;
using HistoryQuest.Domain.Exceptions;

namespace HistoryQuest.Domain.Entities;

public class Question
{
    public Guid Id { get; private set; } = Guid.NewGuid();
    public string Text { get; private set; } = null!;
    public QuestionDifficulty Difficulty { get; private set; }
    public QuestionType Type { get; private set; }
    public Guid CreatedByTeacherId { get; private set; }
    public string? Explanation { get; private set; }
    public List<QuestionOption> Options { get; private set; } = [];
    public bool IsDeleted { get; private set; }
    public DateTime? DeletedAt { get; private set; }

    protected Question(
        string text,
        QuestionDifficulty difficulty,
        QuestionType type,
        Guid createdByTeacherId,
        string? explanation,
        bool isDeleted = false)
    {
        Text = text;
        Difficulty = difficulty;
        Type = type;
        CreatedByTeacherId = createdByTeacherId;
        Explanation = explanation;
        IsDeleted = isDeleted;
        DeletedAt = null;
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
        List<UpdateQuestionOptionRequest> updatedOptions)
    {
        Text = text;
        Difficulty = difficulty;
        Explanation = explanation;
        foreach (var dto in updatedOptions)
        {
            if (dto.Id.HasValue)
            {
                var existing = Options.FirstOrDefault(o => o.Id == dto.Id.Value);

                if (existing == null)
                    throw new NotFoundException($"Option with ID {dto.Id.Value} not found.");

                existing.Update(dto.Text, dto.IsCorrect);
            }
            else
                Options.Add(new QuestionOption(dto.Text, dto.IsCorrect));

            var idsFromRequest = updatedOptions
                .Select(o => o.Id)
                .OfType<Guid>()
                .ToList();

            var toDelete = Options
                .Where(o => !idsFromRequest.Contains(o.Id))
                .ToList();

            foreach (var option in toDelete)
                Options.Remove(option);

            ValidateSingleCorrectOption();
        }
    }

    public void AddOption(string text, bool isCorrect)
    {
        if (Type == QuestionType.SingleChoice && Options.Any(o => o.IsCorrect) && isCorrect)
            throw new BusinessRuleException("Single choice question can have only one correct answer.");

        Options.Add(new QuestionOption(text, isCorrect));
    }

    private void ValidateSingleCorrectOption()
    {
        if (Options.Count(o => o.IsCorrect) != 1)
            throw new BusinessRuleException("Question must have exactly one correct option.");
    }

    public void DeleteByUser(Guid userId, bool isAdmin)
    {
        if(!isAdmin && CreatedByTeacherId != userId)
            throw new UnauthorizedException("Only the creator or an admin can delete this question.");

        IsDeleted = true;
        DeletedAt = DateTime.UtcNow;
    }

    public void Restore(Guid userId, bool isAdmin) 
    {
        if(!IsDeleted)
            throw new BusinessRuleException("Question is not deleted.");

        if(!isAdmin && CreatedByTeacherId != userId)
            throw new UnauthorizedException("Only the creator or an admin can restore this question.");

        IsDeleted = false;
        DeletedAt = null;
    }

    public record UpdateQuestionOptionRequest(
        Guid? Id,
        string Text,
        bool IsCorrect);
}
