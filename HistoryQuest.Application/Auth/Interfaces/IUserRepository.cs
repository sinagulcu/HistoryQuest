using HistoryQuest.Domain.Entities;

namespace HistoryQuest.Application.Auth.Interfaces;

public interface IUserRepository
{
    Task<User?> GetByUserNameAsync(string userName);
    Task<User?> GetByEmailAsync(string email);
    Task<User?> GetByIdAsync(Guid id);
    Task UpdateAsync(User user);
    Task AddAsync(User user);
}
