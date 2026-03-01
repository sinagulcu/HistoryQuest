using HistoryQuest.Application.Auth.DTOs;
using HistoryQuest.Application.Auth.Interfaces;
using HistoryQuest.Application.Auth.UseCases;
using HistoryQuest.Infrastructure.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace HistoryQuest.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly RegisterUserCommand _registerUserCommand;
    private readonly LoginUserQuery _loginUserQuery;
    private readonly LogoutCommand _logoutCommand;
    private readonly RefreshTokenCommand _refreshTokenCommand;
    private readonly RefreshTokenCleanupService _refreshTokenCleanupService;
    private readonly IUserRepository _userRepository;
    private readonly ChangeUserRoleCommand _changeUserRoleCommand;

    public AuthController(
        RegisterUserCommand registerUserCommand,
        LoginUserQuery loginUserQuery,
        LogoutCommand logoutCommand,
        RefreshTokenCommand refreshTokenCommand,
        RefreshTokenCleanupService refreshTokenCleanupService,
        IUserRepository userRepository,
        ChangeUserRoleCommand changeUserRoleCommand)
    {
        _registerUserCommand = registerUserCommand;
        _loginUserQuery = loginUserQuery;
        _logoutCommand = logoutCommand;
        _userRepository = userRepository;
        _refreshTokenCleanupService = refreshTokenCleanupService;
        _refreshTokenCommand = refreshTokenCommand;
        _changeUserRoleCommand = changeUserRoleCommand;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterRequest request)
    {
        var result = await _registerUserCommand.ExecuteAsync(request);
        return Ok(result);
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginRequest request)
    {
        var result = await _loginUserQuery.ExecuteAsync(request);
        return Ok(result);
    }

    [HttpPost("logout")]
    public async Task<IActionResult> Logout(LogoutRequest request)
    {
        await _logoutCommand.ExecuteAsync(request);
        return NoContent();
    }

    [HttpPost("refresh-token")]
    public async Task<IActionResult> RefreshToken(RefreshRequest request)
    {
        var result = await _refreshTokenCommand.ExecuteAsync(request);
        return Ok(result);
    }

    [HttpPost("cleanup-refresh-tokens")]
    [Authorize(Roles = "Admin,Moderator")]
    public async Task<IActionResult> CleanupRefreshTokens()
    {
        await _refreshTokenCleanupService.CleaupExpiredTokensAsync();
        return Ok(new { message = "Expired/revoked refresh tokens cleaned up successfully." });
    }

    [HttpPost("change-role")]
    [Authorize(Roles = "Admin,Teacher")]
    public async Task<IActionResult> ChangeUserRole(ChangeUserRoleRequest request)
    {
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
        var currentUser = await _userRepository.GetByIdAsync(userId);

        await _changeUserRoleCommand.ExecuteAsync(request, currentUser);

        return Ok(new { message = "User role changed successfully." });

    }
}
