
using HistoryQuest.Domain.Entities;
using HistoryQuest.Domain.Enums;

namespace HistoryQuest.Application.Auth.Interfaces;

public interface IRoleRepository
{
    Task<Role?> GetByTypeAsync(UserRoleType roleType);
    Task AddAsync(Role role);
}
