

using HistoryQuest.Domain.Exceptions;

namespace HistoryQuest.Domain.Entities;

public class Wallet : BaseEntity
{
    public Guid UserId { get; private set; }
    public long Balance { get; private set; }

    public byte[] RowVersion { get; private set; } = Array.Empty<byte>();

    protected Wallet() { }

    private Wallet(Guid userId, long initialBalance)
    {
        if (userId == Guid.Empty)
            throw new BusinessRuleException("UserId is required.");

        if(initialBalance < 0)
            throw new BusinessRuleException("Initial balance cannot be negative.");

        UserId = userId;
        Balance = initialBalance;
    }

    public static Wallet Create(Guid userId, long initialBalance = 0) => new(userId, initialBalance);

    public void Credit(long amount)
    {
        if(amount <= 0)
            throw new BusinessRuleException("Credit amount must be positive");

        checked
        {
            Balance += amount;
        }

        UpdatedAt = DateTime.UtcNow;
    }

    public void Debit(long amount)
    {
        if (amount <= 0)
            throw new BusinessRuleException("Debit amount must be positive");
        if (Balance < amount)
            throw new BusinessRuleException("Insufficient balance.");
        checked
        {
            Balance -= amount;
        }
        UpdatedAt = DateTime.UtcNow;
    }
}
