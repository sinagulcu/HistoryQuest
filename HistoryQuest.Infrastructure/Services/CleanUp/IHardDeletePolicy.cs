

using HistoryQuest.Infrastructure.Persistence;

namespace HistoryQuest.Infrastructure.Services.CleanUp;

public interface IHardDeletePolicy
{
    string Key { get; }
    int Order { get; }
    Task<int> ExecuteAsync(HistoryQuestDbContext db, DateTime utcNow, int retentionDays, CancellationToken ct);
}
