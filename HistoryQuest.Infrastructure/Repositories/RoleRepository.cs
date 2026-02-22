
using HistoryQuest.Application.Auth.Interfaces;
using HistoryQuest.Domain.Entities;
using HistoryQuest.Domain.Enums;
using HistoryQuest.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace HistoryQuest.Infrastructure.Repositories;

public class RoleRepository : IRoleRepository
{
    private readonly HistoryQuestDbContext _context;

    public RoleRepository(HistoryQuestDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(Role role)
    {
        _context.Roles.Add(role);
        await _context.SaveChangesAsync();
    }

    public async Task<Role?> GetByTypeAsync(UserRoleType roleType)
    {
        return await _context.Roles
            .SingleOrDefaultAsync(r => r.RoleType == roleType);
    }
}
