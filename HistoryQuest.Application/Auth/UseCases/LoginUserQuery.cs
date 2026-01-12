
using HistoryQuest.Application.Auth.DTOs;
using HistoryQuest.Application.Auth.Interfaces;
using HistoryQuest.Domain.Entities;

namespace HistoryQuest.Application.Auth.UseCases;

public class LoginUserQuery
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly ITokenService _tokenService;

    public LoginUserQuery(IUserRepository userRepository, IPasswordHasher passwordHasher, ITokenService tokenService)
    {
        _userRepository = userRepository;
        _passwordHasher = passwordHasher;
        _tokenService = tokenService;
    }

    public async Task<AuthResult> ExecuteAsync(LoginRequest request)
    {
        User? user =
            await _userRepository.GetByUserNameAsync(request.UserNameOrEmail)
            ?? await _userRepository.GetByEmailAsync(request.UserNameOrEmail);

        if (user is null)
            throw new Exception("Invalid username/email or password.");

        var passwordValid = _passwordHasher.Verify(request.Password, user.PasswordHash);

        if (!passwordValid)
            throw new Exception("Invalid username/email or password.");

        var token = _tokenService.GenerateToken(user);
        var expiresAt = _tokenService.GetExpiration();

        return new AuthResult
        {
            UserId = user.Id,
            UserName = user.UserName,
            AccessToken = token,
            ExpiresAt = expiresAt
        };

    }

}
