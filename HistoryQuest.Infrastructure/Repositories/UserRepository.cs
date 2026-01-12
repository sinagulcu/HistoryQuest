
using HistoryQuest.Application.Auth.Interfaces;
using HistoryQuest.Domain.Entities;
using HistoryQuest.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace HistoryQuest.Infrastructure.Repositories;

public class UserRepository : IUserRepository
{
    private readonly HistoryQuestDbContext _context;

    public UserRepository(HistoryQuestDbContext context)
    {
        _context = context;
    }

    public async Task<User?> GetByUserNameAsync(string userName)
    {
        return await _context.Users
            .Include(x => x.Roles)
            .ThenInclude(x => x.Role)
            .FirstOrDefaultAsync(x => x.UserName == userName);
    }

    public async Task<User?> GetByEmailAsync(string email)
    {
        return await _context.Users
            .Include(x => x.Roles)
            .ThenInclude(x => x.Role)
            .FirstOrDefaultAsync(x => x.Email == email);
    }

    public async Task AddAsync(User user)
    {
        _context.Users.Add(user);
        await _context.SaveChangesAsync();
    }
}
