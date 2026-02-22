

using System.Xml.Serialization;

namespace HistoryQuest.Domain.Entities;

public class RefreshToken : BaseEntity
{
    public string Token { get; private set; }
    public DateTime ExpiryDate { get; private set; }
    public bool IsRevoked { get; private set; }
    public Guid UserId { get; private set; }
    public User User { get; private set; }

    protected RefreshToken() { }

    public RefreshToken(string token, DateTime expiryDate, Guid userId)
    {
        Token = token;
        ExpiryDate = expiryDate;
        UserId = userId;
        IsRevoked = false;
    }

    public void Revoke()
    {
        IsRevoked = true;
    }

    public bool IsExpired()
        => DateTime.UtcNow >= ExpiryDate;
}
