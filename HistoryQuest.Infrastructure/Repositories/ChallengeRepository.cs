

using HistoryQuest.Application.Challenges.Interfaces;
using HistoryQuest.Domain.Entities;
using HistoryQuest.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace HistoryQuest.Infrastructure.Repositories;

public class ChallengeRepository : IChallengeRepository
{
    private readonly HistoryQuestDbContext _dbContext;
    
    public ChallengeRepository(HistoryQuestDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<List<TimedChallenge>> GetAllAsync(
        Guid currentUserId,
        bool isAdmin,
        bool includeDeleted = false,
        CancellationToken cancellationToken = default)
    {
        var query = _dbContext.TimedChallenges.AsQueryable();

        if(!includeDeleted)
            query = query.Where(c => !c.IsDeleted);

        if(!isAdmin)
            query = query.Where(c => c.CreatedByTeacherId == currentUserId);

        return await query
            .OrderByDescending(c => c.ScheduledAtUtc)
            .ToListAsync(cancellationToken);
    }

    public async Task<TimedChallenge?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _dbContext.TimedChallenges
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
    }

    public async Task AddAsync(TimedChallenge challenge, CancellationToken cancellationToken = default)
    {
        await _dbContext.TimedChallenges.AddAsync(challenge, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(TimedChallenge challenge, CancellationToken cancellationToken = default)
    {
        _dbContext.TimedChallenges.Update(challenge);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(TimedChallenge challenge, CancellationToken cancellationToken = default)
    {
        _dbContext.TimedChallenges.Update(challenge);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }
}
