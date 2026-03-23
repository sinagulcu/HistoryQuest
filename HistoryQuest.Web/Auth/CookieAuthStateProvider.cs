using Microsoft.AspNetCore.Components.Authorization;
using System.Security.Claims;
using System.Text.Json;

namespace HistoryQuest.Web.Auth;

public class CookieAuthStateProvider : AuthenticationStateProvider
{
    private ClaimsPrincipal _currentUser = new(new ClaimsIdentity());

    public override Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        return Task.FromResult(new AuthenticationState(_currentUser));
    }

    public void NotifyUserLoggedIn(string token)
    {
        var claims = ParseClaimsFromJwt(token);
        var identity = new ClaimsIdentity(claims,"jwt");
        _currentUser = new ClaimsPrincipal(identity);
        NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
    }

    public void NotifyUserLoggedOut()
    {
        _currentUser = new ClaimsPrincipal(new ClaimsIdentity());
        NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
    }

    public string? GetRole()
    {
        return _currentUser.FindFirst(ClaimTypes.Role)?.Value;
    }
    public Guid? GetUserId()
    {
        var value = _currentUser.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        return value != null ? Guid.Parse(value) : null;
    }

    private static IEnumerable<Claim> ParseClaimsFromJwt(string jwt)
    {
        var payload = jwt.Split('.')[1];
        var jsonBytes = ParseBase64WithoutPadding(payload);
        var keyValuePairs = JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(jsonBytes)!;

        var claims = new List<Claim>();

        foreach (var keyValuePair in keyValuePairs)
        {
            if(keyValuePair.Key == "role" || keyValuePair.Key == "http://schemas.microsoft.com/ws/2008/06/identity/claims/role")
            {
                if (keyValuePair.Value.ValueKind == JsonValueKind.Array)
                    foreach (var role in keyValuePair.Value.EnumerateArray())
                        claims.Add(new Claim(ClaimTypes.Role, role.GetString()!));
                else
                    claims.Add(new Claim(ClaimTypes.Role, keyValuePair.Value.GetString()!));
            }
            else
            {
                claims.Add(new Claim(keyValuePair.Key, keyValuePair.Value.ToString()));
            }
        }
        return claims;
    }

    private static byte[] ParseBase64WithoutPadding(string base64)
    {
        switch (base64.Length % 4)
        {
            case 2: base64 += "=="; break;
            case 3: base64 += "="; break;
        }
        return Convert.FromBase64String(base64);
    }
}
