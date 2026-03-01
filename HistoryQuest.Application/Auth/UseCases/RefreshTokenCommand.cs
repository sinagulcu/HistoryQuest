

using HistoryQuest.Application.Auth.DTOs;
using HistoryQuest.Application.Auth.Interfaces;
using HistoryQuest.Domain.Entities;
using HistoryQuest.Domain.Exceptions;

namespace HistoryQuest.Application.Auth.UseCases;

public class RefreshTokenCommand
{
    private readonly IRefreshTokenRepository _refreshTokenRepository;
    private readonly ITokenService _tokenService;

    public RefreshTokenCommand(IRefreshTokenRepository refreshTokenRepository, ITokenService tokenService)
    {
        _refreshTokenRepository = refreshTokenRepository;
        _tokenService = tokenService;
    }

    public async Task<AuthResult> ExecuteAsync(RefreshRequest request)
    {
        var storedToken = await _refreshTokenRepository
            .GetByTokenAsync(request.RefreshToken);

        if (storedToken is null)
            throw new UnauthorizedException("Invalid refresh token.");

        if (storedToken.IsRevoked || storedToken.IsExpired())
            throw new UnauthorizedException("Refresh token is revoked or expired.");

        storedToken.Revoke();

        await _refreshTokenRepository.UpdateAsync(storedToken);

        var newAccessToken = _tokenService.GenerateToken(storedToken.User);
        var newRefreshTokenValue = _tokenService.GenerateRefreshToken();

        var newRefreshToken = new RefreshToken(
            newRefreshTokenValue,
            DateTime.UtcNow.AddDays(30),
            storedToken.UserId
         );

        await _refreshTokenRepository.AddAsync(newRefreshToken);

        return new AuthResult
        {
            UserId = storedToken.User.Id,
            UserName = storedToken.User.UserName,
            AccessToken = newAccessToken,
            RefreshToken = newRefreshTokenValue,
            ExpiresAt = _tokenService.GetExpiration()
        };

    }


}
