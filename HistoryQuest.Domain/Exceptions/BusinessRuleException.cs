
namespace HistoryQuest.Domain.Exceptions;

public class BusinessRuleException(string message) : DomainException(message)
{
    public override int StatusCode => 400;
}
