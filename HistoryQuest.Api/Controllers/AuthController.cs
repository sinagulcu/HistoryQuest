using HistoryQuest.Application.Auth.DTOs;
using HistoryQuest.Application.Auth.UseCases;
using Microsoft.AspNetCore.Mvc;

namespace HistoryQuest.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly RegisterUserCommand _registerUserCommand;
    private readonly RegisterTeacherCommand _registerTeacherCommand;
    private readonly LoginUserQuery _loginUserQuery;

    public AuthController(
        RegisterUserCommand registerUserCommand,
        LoginUserQuery loginUserQuery,
        RegisterTeacherCommand registerTeacherCommand)
    {
        _registerUserCommand = registerUserCommand;
        _loginUserQuery = loginUserQuery;
        _registerTeacherCommand = registerTeacherCommand;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterRequest request)
    {
        var result = await _registerUserCommand.ExecuteAsync(request);
        return Ok(result);
    }

    [HttpPost("register-teacher")]
    public async Task<IActionResult> RegisterTeacher(RegisterRequest request)
    {
        var result = await _registerTeacherCommand.ExecuteAsync(request);
        return Ok(result);
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login (LoginRequest request)
    {
        var result = await _loginUserQuery.ExecuteAsync(request);
        return Ok(result);
    }
}
