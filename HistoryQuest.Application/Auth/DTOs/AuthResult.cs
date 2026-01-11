namespace HistoryQuest.Application.Auth.DTOs;

public class AuthResult
{
    public Guid UserId { get; init;}
    public string UserName {get; init;}
    public string AccessToken {get; init;}
    public DateTime ExpiresAt { get; init; }
}