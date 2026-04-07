


using HistoryQuest.Application.Credits.DTOs;
using HistoryQuest.Application.Credits.Interfaces;
using HistoryQuest.Domain.Entities;

namespace HistoryQuest.Application.Credits.UseCases;

public sealed class GetWalletSummaryQuery
{
    private readonly IWalletRepository _walletRepository;

    public GetWalletSummaryQuery(IWalletRepository walletRepository)
    {
        _walletRepository = walletRepository;
    }

    public async Task<WalletSummaryDto> ExecuteAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var wallet = await _walletRepository.GetByUserIdAsync(userId, cancellationToken);

        if(wallet is null)
        {
            wallet = Wallet.Create(userId, 0);
            await _walletRepository.AddAsync(wallet, cancellationToken);

            await _walletRepository.SaveChangesAsync(cancellationToken);
        }

        return new WalletSummaryDto
        {
            UserId = userId,
            Balance = wallet.Balance,
            CreatedAt = wallet.CreatedAt,
            UpdatedAt = wallet.UpdatedAt,
        };
    }
}
