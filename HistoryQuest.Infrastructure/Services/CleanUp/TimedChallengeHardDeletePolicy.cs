

using HistoryQuest.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace HistoryQuest.Infrastructure.Services.CleanUp;

public sealed class TimedChallengeHardDeletePolicy : IHardDeletePolicy
{
    public string Key => "TimedChallenge";
    public int Order => 15;

    public async Task<int> ExecuteAsync(HistoryQuestDbContext db, DateTime utcNow, int retentionDays, CancellationToken ct)
    {
        var threshold = utcNow.AddDays(-retentionDays);

        var deleted = await db.TimedChallenges
            .Where(x => x.IsDeleted && x.DeletedAt != null && x.DeletedAt <= threshold)
            .ExecuteDeleteAsync(ct);

        return deleted;
    }
}