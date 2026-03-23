using HistoryQuest.Web.Auth;
using HistoryQuest.Web.Services.Interfaces;
using System.Text.Json;

namespace HistoryQuest.Web.Services;

public class AuthService : IAuthService
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly CookieAuthStateProvider _authStateProvider;

    public AuthService(IHttpClientFactory httpClientFactory, CookieAuthStateProvider authStateProvider)
    {
        _httpClientFactory = httpClientFactory;
        _authStateProvider = authStateProvider;
    }

    public async Task<bool> LoginAsync(string userNameOrEmail, string password)
    {
        var client = _httpClientFactory.CreateClient("API");

        var response = await client.PostAsJsonAsync("api/Auth/login", new
        {
            userNameOrEmail,
            password
        });

        if(!response.IsSuccessStatusCode)
            return false;

        var content = await response.Content.ReadAsStringAsync();
        var json = JsonSerializer.Deserialize<JsonElement>(content);

        var token = json.GetProperty("accessToken").GetString();

        if(token == null)
            return false;

        _authStateProvider.NotifyUserLoggedIn(token);
        return true;
    }

    public async Task LogoutAsync()
    {
        _authStateProvider.NotifyUserLoggedOut();
    }
}
