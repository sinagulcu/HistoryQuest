using HistoryQuest.Domain.Entities;
using HistoryQuest.Domain.Models;

namespace HistoryQuest.Application.Auth.Interfaces;

public interface IUserRepository
{
    Task<User?> GetByUserNameAsync(string userName);
    Task<User?> GetByEmailAsync(string email);
    Task<User?> GetByIdAsync(Guid id);
    Task UpdateAsync(User user);
    Task AddAsync(User user);
    Task<bool> AnyUserInRoleAsync(UserRoleType roleType);
}
