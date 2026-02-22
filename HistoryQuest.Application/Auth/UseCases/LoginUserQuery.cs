
using HistoryQuest.Application.Auth.DTOs;
using HistoryQuest.Application.Auth.Interfaces;
using HistoryQuest.Domain.Entities;
using HistoryQuest.Domain.Exceptions;
using System.Security.Authentication;

namespace HistoryQuest.Application.Auth.UseCases;

public class LoginUserQuery
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly ITokenService _tokenService;
    private readonly IRefreshTokenRepository _refreshTokenRepository;

    public LoginUserQuery(IUserRepository userRepository, IPasswordHasher passwordHasher, ITokenService tokenService, IRefreshTokenRepository refreshTokenRepository)
    {
        _userRepository = userRepository;
        _passwordHasher = passwordHasher;
        _tokenService = tokenService;
        _refreshTokenRepository = refreshTokenRepository;
    }

    public async Task<AuthResult> ExecuteAsync(LoginRequest request)
    {
        User? user =
            await _userRepository.GetByUserNameAsync(request.UserNameOrEmail)
            ?? await _userRepository.GetByEmailAsync(request.UserNameOrEmail);

        if (user is null)
            throw new UnauthorizedException("Invalid username/email or password");

        var passwordValid = _passwordHasher.Verify(request.Password, user.PasswordHash);

        if (!passwordValid)
            throw new UnauthorizedException("Invalid username/email or password");

        var token = _tokenService.GenerateToken(user);
        var expiresAt = _tokenService.GetExpiration();

        var refreshTokenValue = _tokenService.GenerateRefreshToken();

        var refreshToken = new RefreshToken(
            refreshTokenValue,
            DateTime.UtcNow.AddDays(30),
            user.Id
            );

        await _refreshTokenRepository.AddAsync(refreshToken);

        return new AuthResult
        {
            UserId = user.Id,
            UserName = user.UserName,
            AccessToken = token,
            ExpiresAt = expiresAt,
            RefreshToken = refreshTokenValue
        };

    }

}
