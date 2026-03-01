
using HistoryQuest.Domain.Entities;
using HistoryQuest.Domain.Models;

namespace HistoryQuest.Application.Auth.Interfaces;

public interface IRoleRepository
{
    Task<Role?> GetByTypeAsync(UserRoleType roleType);
    Task AddAsync(Role role);
    Task<bool> ExistsAsync(UserRoleType roleType);
}
