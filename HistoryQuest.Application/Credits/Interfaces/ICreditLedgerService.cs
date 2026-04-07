

using HistoryQuest.Domain.Models;

namespace HistoryQuest.Application.Credits.Interfaces;

public interface ICreditLedgerService
{
    Task<long> ApplyAsync(
        Guid userId,
        long amount,
        CreditTransactionType type,
        string reason,
        string? referenceType = null,
        Guid? referenceId = null,
        string? idempotencyKey = null,
        string? metadataJson = null,
        CancellationToken cancellationToken = default);
}
