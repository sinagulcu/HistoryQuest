using HistoryQuest.Domain.Entities;
using HistoryQuest.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace HistoryQuest.Infrastructure.Services.CleanUp;

public sealed class QuestionHardDeletePolicy : IHardDeletePolicy
{
    public string Key => "Question";
    public int Order => 20;

    public async Task<int> ExecuteAsync(
        HistoryQuestDbContext db,
        DateTime utcNow,
        int retentionDays,
        CancellationToken ct)
    {
        var threshold = utcNow.AddDays(-retentionDays);

        var candidateQuestionIds = await db.Questions
            .Where(q => q.IsDeleted && q.DeletedAt != null && q.DeletedAt <= threshold)
            .Select(q => q.Id)
            .ToListAsync(ct);

        if (candidateQuestionIds.Count == 0)
            return 0;

        await db.QuizQuestions
            .Where(x => candidateQuestionIds.Contains(x.QuestionId))
            .ExecuteDeleteAsync(ct);

        await db.TimedChallenges
            .Where(c =>
                candidateQuestionIds.Contains(c.QuestionId) &&
                c.IsDeleted &&
                c.DeletedAt != null &&
                c.DeletedAt <= threshold)
            .ExecuteDeleteAsync(ct);

        var stillReferencedQuestionIds = await db.TimedChallenges
            .Where(c => candidateQuestionIds.Contains(c.QuestionId))
            .Select(c => c.QuestionId)
            .Distinct()
            .ToListAsync(ct);

        var finalQuestionIds = candidateQuestionIds
            .Except(stillReferencedQuestionIds)
            .ToList();

        if (finalQuestionIds.Count == 0)
            return 0;

        await db.Set<QuestionOption>()
            .Where(o => finalQuestionIds.Contains(o.Id))
            .ExecuteDeleteAsync(ct);

        var deleted = await db.Questions
            .Where(q => finalQuestionIds.Contains(q.Id))
            .ExecuteDeleteAsync(ct);

        return deleted;
    }
}