

using HistoryQuest.Application.Auth.Interfaces;
using BCrypt.Net;

namespace HistoryQuest.Infrastructure.Security;

public class PasswordHasher : IPasswordHasher
{
    public string Hash(string password) => BCrypt.Net.BCrypt.HashPassword(password);

    public bool Verify(string password, string passwordHash)
      => BCrypt.Net.BCrypt.Verify(password, passwordHash);
}
