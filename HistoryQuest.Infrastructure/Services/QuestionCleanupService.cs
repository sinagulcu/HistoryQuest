using HistoryQuest.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace HistoryQuest.Infrastructure.Services;

public class QuestionCleanupService : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    public QuestionCleanupService(IServiceScopeFactory scopeFactory)
    {
        _scopeFactory = scopeFactory;
    }
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            using var scope = _scopeFactory.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<HistoryQuestDbContext>();

            var threshold = DateTime.UtcNow.AddDays(-30);

            var questionsToDelete = await db.Questions
                .Where(q => q.IsDeleted && q.DeletedAt <= threshold)
                .ToListAsync();

            db.Questions.RemoveRange(questionsToDelete);
            await db.SaveChangesAsync();

            await Task.Delay(TimeSpan.FromHours(24), stoppingToken);
        }
    }
}
