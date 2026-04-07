
using HistoryQuest.Domain.Models;

namespace HistoryQuest.Application.Credits.DTOs;

public class CreditTransactionDto
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public long Amount { get; set; }
    public long BalanceAfter { get; set; }
    public CreditTransactionType Type { get; set; }
    public string Reason { get; set; } = string.Empty;
    public string? ReferenceType { get; set; }
    public Guid? ReferenceId { get; set; }
    public DateTime CreatedAt { get; set; }
}
