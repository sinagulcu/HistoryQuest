
namespace HistoryQuest.Application.Credits.DTOs;

public class WalletSummaryDto
{
    public Guid UserId { get; set; }
    public long Balance { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}
