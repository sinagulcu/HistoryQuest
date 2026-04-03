

namespace HistoryQuest.Application.Questions.DTOs;

public class QuestionDeleteUsageDto
{
    public Guid QuestionId { get; set; }
    public int ActiveQuizUsageCount { get; set; }
    public int ActiveChallengeUsageCount { get; set; }
    public bool IsUsedInQuiz => ActiveQuizUsageCount > 0;
    public bool IsUsedChallenge => ActiveChallengeUsageCount > 0;

    public bool CanDelete => !IsUsedInQuiz && !IsUsedChallenge;

    public string Message =>
        CanDelete
            ? "This question can be safely deleted."
            : $"This question cannot be deleted because it is currently used in {ActiveQuizUsageCount} active quiz(es) and {ActiveChallengeUsageCount} active challenge(s).";
}
