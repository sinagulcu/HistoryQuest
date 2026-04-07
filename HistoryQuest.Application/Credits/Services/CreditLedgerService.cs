
using HistoryQuest.Application.Credits.Interfaces;
using HistoryQuest.Domain.Entities;
using HistoryQuest.Domain.Models;

namespace HistoryQuest.Application.Credits.Services;

public class CreditLedgerService : ICreditLedgerService
{
    private readonly IWalletRepository _walletRepository;
    private readonly ICreditTransactionRepository _transactionRepository;

    public CreditLedgerService(IWalletRepository walletRepository, ICreditTransactionRepository transactionRepository)
    {
        _walletRepository = walletRepository;
        _transactionRepository = transactionRepository;
    }

    public async Task<long> ApplyAsync(
        Guid userId,
        long amount,
        CreditTransactionType type,
        string reason,
        string? referenceType = null,
        Guid? referenceId = null,
        string? idempotencyKey = null,
        string? metadataJson = null,
        CancellationToken cancellationToken = default)
    {
        if(!string.IsNullOrWhiteSpace(idempotencyKey))
        {
            var exists = await _transactionRepository.ExistsByIdempotencyKeyAsync(idempotencyKey, cancellationToken);
            if (exists)
            {
                var wallet = await _walletRepository.GetByUserIdAsync(userId, cancellationToken);
                return wallet?.Balance ?? 0;
            }
        }
        var walletEntity = await _walletRepository.GetByUserIdAsync(userId, cancellationToken);
        if(walletEntity is null)
        {
            walletEntity = Wallet.Create(userId, 0);
            await _walletRepository.AddAsync(walletEntity, cancellationToken);
        }

        if (amount > 0) walletEntity.Credit(amount);
        else walletEntity.Debit(Math.Abs(amount));

        var tx = CreditTransaction.Create(
            userId: userId,
            amount: amount,
            balanceAfter: walletEntity.Balance,
            type: type,
            reason: reason,
            referenceType: referenceType,
            referenceId: referenceId,
            idempotencyKey: idempotencyKey,
            metadataJson: metadataJson);

        await _transactionRepository.AddAsync(tx, cancellationToken);
        await _walletRepository.SaveChangesAsync(cancellationToken);

        return walletEntity.Balance;
    }

}
