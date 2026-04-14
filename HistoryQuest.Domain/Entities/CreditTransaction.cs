
using HistoryQuest.Domain.Exceptions;
using HistoryQuest.Domain.Models;

namespace HistoryQuest.Domain.Entities;

public class CreditTransaction : BaseEntity
{
    public Guid UserId { get; private set; }
    public long Amount { get; private set; }
    public long BalanceAfter { get; private set; }

    public CreditTransactionType Type { get; private set; }
    public string Reason { get; private set; } = string.Empty;

    public string? ReferenceType { get; private set; }
    public Guid? ReferenceId { get; private set; }

    public string? IdempotencyKey { get; private set; }
    public string? MetadataJson { get; private set; }

    protected CreditTransaction() { }

    private CreditTransaction(
        Guid userId,
        long amount,
        long balanceAfter,
        CreditTransactionType type,
        string reason,
        string? referenceType,
        Guid? referenceId,
        string? idempotencyKey,
        string? metadataJson)
    {
        if (userId == Guid.Empty)
            throw new BusinessRuleException("UserId is required.");
        if (amount == 0)
            throw new BusinessRuleException("Amount cannot be zero.");
        if (string.IsNullOrWhiteSpace(reason))
            throw new BusinessRuleException("Reason is required.");

        UserId = userId;
        Amount = amount;
        BalanceAfter = balanceAfter;
        Type = type;
        Reason = reason.Trim();
        ReferenceType = string.IsNullOrWhiteSpace(referenceType) ? null : referenceType.Trim();
        ReferenceId = referenceId;
        IdempotencyKey = string.IsNullOrWhiteSpace(idempotencyKey) ? null : idempotencyKey.Trim();
        MetadataJson = string.IsNullOrWhiteSpace(metadataJson) ? null : metadataJson.Trim();
    }

    public static CreditTransaction Create(
       Guid userId,
        long amount,
        long balanceAfter,
        CreditTransactionType type,
        string reason,
        string? referenceType = null,
        Guid? referenceId = null,
        string? idempotencyKey = null,
        string? metadataJson = null)
    {
        return new CreditTransaction(
            userId,
            amount,
            balanceAfter,
            type,
            reason,
            referenceType,
            referenceId,
            idempotencyKey,
            metadataJson
        );
    }
}