namespace HistoryQuest.Application.Auth.DTOs;

public class LoginRequest
{
    public string UserNameOrEmail { get; init; } = null!;
    public string Password { get; init; } = null!;
}