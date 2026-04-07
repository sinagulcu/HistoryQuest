
using HistoryQuest.Application.Credits.DTOs;
using HistoryQuest.Application.Credits.Interfaces;

namespace HistoryQuest.Application.Credits.UseCases;

public class GetCreditTransactionsQuery
{
    private readonly ICreditTransactionRepository _transactionRepository;

    public GetCreditTransactionsQuery(ICreditTransactionRepository transactionRepository)
    {
        _transactionRepository = transactionRepository;
    }

    public async Task<List<CreditTransactionDto>> ExecuteAsync(Guid userId, int take = 50, CancellationToken cancellationToken = default)
    {
        var list = await _transactionRepository.GetByUserIdAsync(userId, take, cancellationToken);

        return list.Select(x => new CreditTransactionDto
        {
            Id = x.Id,
            UserId = x.UserId,
            Amount = x.Amount,
            BalanceAfter = x.BalanceAfter,
            Type = x.Type,
            Reason = x.Reason,
            ReferenceType = x.ReferenceType,
            ReferenceId = x.ReferenceId,
            CreatedAt = x.CreatedAt,
        }).ToList();
    }
}
