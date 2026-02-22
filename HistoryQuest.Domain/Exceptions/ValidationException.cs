
namespace HistoryQuest.Domain.Exceptions;

public class ValidationException(string message) : DomainException(message)
{
    public override int StatusCode => 400;
}
