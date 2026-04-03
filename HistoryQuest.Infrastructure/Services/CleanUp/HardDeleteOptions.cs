
namespace HistoryQuest.Infrastructure.Services.CleanUp;

public class HardDeleteOptions
{
    public bool Enabled { get; set; } = true;
    public int IntervalMinutes { get; set; } = 60;
    public Dictionary<string, int> RetentionDays { get; set; } = new();
    public List<string> DisabledPolicies { get; set; } = new();
}
