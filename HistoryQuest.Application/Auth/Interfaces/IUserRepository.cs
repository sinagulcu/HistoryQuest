using HistoryQuest.Domain.Entities;

namespace HistoryQuest.Application.Auth.Interfaces;

public interface IUserRepository
{
    Task<User?> GetByUserNameAsync(string userName);
    Task<User?> GetByEmailAsync(string email);
    Task AddAsync(User user);
}
