namespace HistoryQuest.Web.Services.Interfaces;

public interface IAuthService
{
    Task<bool> LoginAsync(string userNameOrEmail, string password);
    Task LogoutAsync();
}
