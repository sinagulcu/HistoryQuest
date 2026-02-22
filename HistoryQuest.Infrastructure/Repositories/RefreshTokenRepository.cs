

using HistoryQuest.Application.Auth.Interfaces;
using HistoryQuest.Domain.Entities;
using HistoryQuest.Infrastructure.Persistence;

namespace HistoryQuest.Infrastructure.Repositories;

public class RefreshTokenRepository : IRefreshTokenRepository
{
    private readonly HistoryQuestDbContext _context;

    public RefreshTokenRepository(HistoryQuestDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(RefreshToken token)
    {
        await _context.RefreshTokens.AddAsync(token);
        await _context.SaveChangesAsync();
    }
}
