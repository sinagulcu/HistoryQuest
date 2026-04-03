

using HistoryQuest.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace HistoryQuest.Infrastructure.Services.CleanUp;

public sealed class CategoryHardDeletePolicy : IHardDeletePolicy
{
    public string Key => "Category";

    public int Order => 30;

    public async Task<int> ExecuteAsync(HistoryQuestDbContext db, DateTime utcNow, int retentionDays, CancellationToken ct)
    {
        var threshold = utcNow.AddDays(-retentionDays);

        var deletableCategoryIds = await db.Categories
            .Where(c => c.IsDeleted && c.DeletedAt != null && c.DeletedAt <= threshold)
            .Where(c => !db.Questions.Any(q => q.CategoryId == c.Id && !q.IsDeleted))
            .Where(c => !db.Quizzes.Any(q => q.CategoryId == c.Id && !q.IsDeleted))
            .Select(c => c.Id)
            .ToListAsync(ct);

        if (deletableCategoryIds.Count == 0)
            return 0;

        var deleted = await db.Categories
            .Where(c => deletableCategoryIds.Contains(c.Id))
            .ExecuteDeleteAsync(ct);

        return deleted;
    }
}
