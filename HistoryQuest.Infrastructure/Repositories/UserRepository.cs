
using HistoryQuest.Application.Auth.Interfaces;
using HistoryQuest.Domain.Entities;
using HistoryQuest.Domain.Enums;
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
            .SingleOrDefaultAsync(x => x.UserName == userName && !x.IsDeleted);
    }

    public async Task<User?> GetByEmailAsync(string email)
    {
        return await _context.Users
            .Include(x => x.Roles)
            .ThenInclude(x => x.Role)
            .SingleOrDefaultAsync(x => x.Email == email && !x.IsDeleted);
    }

    public async Task<User?> GetByIdAsync(Guid id)
    {
        return await _context.Users
            .Include(u => u.Roles)
            .SingleOrDefaultAsync(u => u.Id == id && !u.IsDeleted);
    }

    public async Task AddAsync(User user)
    {
        _context.Users.Add(user);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(User user)
    {
        await _context.SaveChangesAsync();
    }

    public async Task<bool> AnyUserInRoleAsync(UserRoleType roleType)
    {
        return await _context.Users
            .AnyAsync(u => u.Roles.Any(r => r.Role.RoleType == roleType));
    }
}
