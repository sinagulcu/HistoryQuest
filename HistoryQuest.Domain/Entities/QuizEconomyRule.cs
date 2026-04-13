
using HistoryQuest.Domain.Exceptions;

namespace HistoryQuest.Domain.Entities;

public class QuizEconomyRule : BaseEntity
{
    public Guid QuizId { get; private set; }
    public long EntryCost { get; private set; }
    public long RewardPool { get; private set; }
    public int WrongPenaltyPerQuestion { get; private set; }

    public bool IsActive { get;private set; }

    protected QuizEconomyRule () { }

    private QuizEconomyRule(Guid quizId, long entryCost, long rewardPool, int wrongPenaltyPerQuestion)
    {
        if (quizId == Guid.Empty) throw new BusinessRuleException("Quiz must be selected.");
        if (entryCost < 0) throw new BusinessRuleException("Cost cannot be negative");
        if (rewardPool < 0) throw new BusinessRuleException("Reward cannot be negative");
        if (wrongPenaltyPerQuestion < 0) throw new BusinessRuleException("Wrong penalty cannot be negative");

        QuizId = quizId;
        EntryCost = entryCost;
        RewardPool = rewardPool;
        WrongPenaltyPerQuestion = wrongPenaltyPerQuestion;
        IsActive = true;
    }

    public static QuizEconomyRule Create(Guid quizId, long entryCost, long rewardPool, int wrongPenaltyPerQuestion)
        => new(quizId, entryCost, rewardPool, wrongPenaltyPerQuestion);

    public void Update(long entryCost, long rewardPool, int wrongPenaltyPerQuestion, bool isActive)
    {
        if (entryCost < 0) throw new BusinessRuleException("Cost cannot be negative");
        if (rewardPool < 0) throw new BusinessRuleException("Reward cannot be negative");
        if (wrongPenaltyPerQuestion < 0) throw new BusinessRuleException("Wrong penalty cannot be negative");

        EntryCost = entryCost;
        RewardPool = rewardPool;
        WrongPenaltyPerQuestion = wrongPenaltyPerQuestion;
        IsActive = isActive;
        UpdatedAt = DateTime.UtcNow;
    }


}
