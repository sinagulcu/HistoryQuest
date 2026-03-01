
using HistoryQuest.Application.Auth.Interfaces;
using HistoryQuest.Domain.Entities;
using HistoryQuest.Domain.Models;
using HistoryQuest.Domain.Exceptions;
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
        var role = await _context.Roles
            .FirstOrDefaultAsync(r => r.RoleType == roleType);

        if (role == null)
            throw new NotFoundException($"Role {roleType} not found.");

        return role;
    }

    public async Task<bool> ExistsAsync(UserRoleType roleType)
    {
        return await _context.Roles
            .AnyAsync(r => r.RoleType == roleType);
    }
}
