
using HistoryQuest.Application.Auth.DTOs;
using HistoryQuest.Application.Auth.Interfaces;
using HistoryQuest.Domain.Entities;
using HistoryQuest.Domain.Enums;

namespace HistoryQuest.Application.Auth.UseCases;

public class RegisterTeacherCommand
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordHasher _passwordHasher;

    public RegisterTeacherCommand(
        IUserRepository userRepository,
        IPasswordHasher passwordHasher)
    {
        _userRepository = userRepository;
        _passwordHasher = passwordHasher;
    }

    public async Task<AuthResult> ExecuteAsync(RegisterRequest request)
    {
        if (await _userRepository.GetByUserNameAsync(request.UserName) != null)
            throw new Exception("Username already exists.");

        if(await _userRepository.GetByEmailAsync(request.Email) != null)
            throw new Exception("Email already exists.");

        var passwordHash = _passwordHasher.Hash(request.Password);

        var user = new User(
            request.UserName,
            request.FirstName,
            request.LastName,
            request.Email,
            passwordHash
            );

        var teacherRole = new Role(UserRoleType.Teacher);

        user.AssignRole(teacherRole);

        await _userRepository.AddAsync(user);

        return new AuthResult
        {
            UserId = user.Id,
            UserName = user.UserName,
            AccessToken = string.Empty, 
            ExpiresAt = DateTime.UtcNow
        };
    }
}
