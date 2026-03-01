
using HistoryQuest.Application.Auth.DTOs;
using HistoryQuest.Application.Auth.Interfaces;
using HistoryQuest.Domain.Exceptions;

namespace HistoryQuest.Application.Auth.UseCases;

public class LogoutCommand
{
    private readonly IRefreshTokenRepository _refreshTokenRepository;

    public LogoutCommand(IRefreshTokenRepository refreshTokenRepository)
    {
        _refreshTokenRepository = refreshTokenRepository;
    }

    public async Task ExecuteAsync(LogoutRequest request)
    {
        var token = await _refreshTokenRepository
            .GetByTokenAsync(request.RefreshToken);

        if (token is null)
            throw new UnauthorizedException("Invalid refresh token");

        if (token.IsRevoked)
            return;
        if (token.IsExpired())
            return;


        token.Revoke();
        await _refreshTokenRepository.UpdateAsync(token);
    }
}
