

using HistoryQuest.Application.Users.DTOs;
using HistoryQuest.Application.Users.Interfaces;
using HistoryQuest.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace HistoryQuest.Infrastructure.Repositories;

public class UserReadRepository : IUserReadRepository
{
    private readonly HistoryQuestDbContext _dbContext;

    public UserReadRepository(HistoryQuestDbContext dbContext)
    {
        _dbContext = dbContext;
    }


    public async Task<List<UserListItemDto>> GetAllAsync(bool includeDeleted = false, CancellationToken cancellationToken = default)
    {
        var query = _dbContext.Users
            .AsNoTracking()
            .Include(u => u.Roles)
            .ThenInclude(ur => ur.Role)
            .AsQueryable();

        if (!includeDeleted) query = query.Where(u => !u.IsDeleted);

        var users = await query
            .OrderByDescending(u => u.CreatedAt)
            .ToListAsync(cancellationToken);

        return users.Select(u => new UserListItemDto
        {
            Id = u.Id,
            UserName = u.UserName,
            Email = u.Email,
            FirstName = u.FirstName,
            LastName = u.LastName,
            FullName = $"{u.FirstName} {u.LastName}".Trim(),
            Role = ResolvePrimaryRole(u),
            CreatedAt = u.CreatedAt
        }).ToList();
    }

    public async Task<int> GetCountAsync(bool includeDeleted = false, CancellationToken cancellationToken = default)
    {
        var query = _dbContext.Users.AsNoTracking().AsQueryable();

        if (!includeDeleted) query = query.Where(u => !u.IsDeleted);

        return await query.CountAsync(cancellationToken);
    }

    private static string ResolvePrimaryRole(Domain.Entities.User user)
    {
        var roleNames = user.Roles
            .Select(ur => ur.Role?.RoleType.ToString())
            .Where(r => !string.IsNullOrWhiteSpace(r))
            .ToList();

        if (roleNames.Contains("Admin")) return "Admin";
        if (roleNames.Contains("Teacher")) return "Teacher";
        if (roleNames.Contains("Student")) return "Student";

        return "Student";
    }
}
