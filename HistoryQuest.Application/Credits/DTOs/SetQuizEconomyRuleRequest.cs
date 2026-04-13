

namespace HistoryQuest.Application.Credits.DTOs;

public class SetQuizEconomyRuleRequest
{
    public long EntryCost { get; set; }
    public long RewardPool { get; set; }
    public int WrongPenaltyPerQuestion { get; set; }
    public bool IsActive { get; set; } = true;
}
