

using HistoryQuest.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace HistoryQuest.Infrastructure.Services.CleanUp;

public sealed class QuizHardDeletePolicy : IHardDeletePolicy
{
    public string Key => "Quiz";

    public int Order => 20;

    public async Task<int> ExecuteAsync(HistoryQuestDbContext db, DateTime utcNow, int retentionDays, CancellationToken ct)
    {
        var threshold = utcNow.AddDays(-retentionDays);

        var quizIds = await db.Quizzes
            .Where(q => q.IsDeleted && q.DeletedAt != null && q.DeletedAt <= threshold)
            .Select(q => q.Id)
            .ToListAsync(ct);

        if(quizIds.Count == 0)
            return 0;
        
        await db.QuizQuestions
            .Where(qq => quizIds.Contains(qq.QuizId))
            .ExecuteDeleteAsync(ct);

        await db.QuizAttempts
            .Where(a => quizIds
            .Contains(a.QuizId))
            .ExecuteDeleteAsync(ct);

        var deleted = await db.Quizzes
            .Where(q => quizIds.Contains(q.Id))
            .ExecuteDeleteAsync(ct);

        return deleted;
    }
}
